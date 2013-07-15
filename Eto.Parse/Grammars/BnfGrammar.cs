using System;
using Eto.Parse.Parsers;
using Eto.Parse.Scanners;
using System.Collections.Generic;
using System.Linq;
using Eto.Parse.Writers;
using System.IO;
using System.CodeDom.Compiler;

namespace Eto.Parse.Grammars
{
	public class BnfGrammar : Grammar
	{
		Dictionary<string, NamedParser> parserLookup = new Dictionary<string, NamedParser>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, NamedParser> baseLookup = new Dictionary<string, NamedParser>(StringComparer.InvariantCultureIgnoreCase);
		Parser sws = Terminals.SingleLineWhiteSpace.Repeat(0);
		Parser ws = Terminals.WhiteSpace.Repeat(0);
		Parser sq = Terminals.Set('\'');
		Parser dq = Terminals.Set('"');
		LiteralParser ruleSeparator = new LiteralParser("::=");
		string startParserName;
		NamedParser rule;
		NamedParser listRepeat;
		NamedParser list;
		NamedParser repeatRule;
		NamedParser optionalRule;
		NamedParser literal;
		NamedParser ruleName;


		protected string RuleSeparator { get { return ruleSeparator.Value; } set { ruleSeparator.Value = value; } }

		protected AlternativeParser RuleParser { get; private set; }

		protected AlternativeParser TermParser { get; private set; }

		protected AlternativeParser Expresssions { get; private set; }

		protected SequenceParser RuleNameParser { get; private set; }

		public Dictionary<string, NamedParser> Rules { get { return parserLookup; } protected set { parserLookup = value; } }

		public BnfGrammar(bool enhanced = true)
			: base("bnf")
		{
			foreach (var property in typeof(Terminals).GetProperties())
			{
				if (typeof(Parser).IsAssignableFrom(property.PropertyType))
				{
					var parser = property.GetValue(null, null) as Parser;
					baseLookup[property.Name] = parser.Named(property.Name);
				}
			}

			var lineEnd = LineEnd();

			literal = (
				(sq & (+!sq).Named("value").Optional() & sq)
				| (dq & (+!dq).Named("value").Optional() & dq)
			).Named("parser");


			RuleNameParser = "<" & Terminals.Set('>').Inverse().Repeat().Named("name") & ">";

			RuleParser = new AlternativeParser(); // defined later 

			TermParser = literal | (ruleName = RuleNameParser.Named("parser"));
			if (enhanced)
			{
				TermParser.Items.Add('(' & sws & RuleParser & sws & ')');
				TermParser.Items.Add(repeatRule = ('{' & sws & RuleParser & sws & '}').Named("parser"));
				TermParser.Items.Add(optionalRule = ('[' & sws & RuleParser & sws & ']').Named("parser"));
			}

			list = new SequenceParser(TermParser.Named("term"), -(sws.Named("ws") & TermParser.Named("term"))).Named("parser");

			listRepeat = (list.Named("list") & ws & '|' & sws & RuleParser.Named("expression")).Named("parser");
			RuleParser.Items.Add(listRepeat);
			RuleParser.Items.Add(list);

			rule = (~lineEnd & sws & RuleNameParser.Named("ruleName") & ws & ruleSeparator & sws & RuleParser & lineEnd).Named("parser");
			Expresssions = new AlternativeParser();
			Expresssions.Items.Add(rule);

			this.Inner = (Expresssions & this) | Expresssions;

			AttachEvents();
		}

		void AttachEvents()
		{
			ruleName.Matched += m => {
				NamedParser parser;
				var name = m["name"].Value;
				if (!parserLookup.TryGetValue(name, out parser) && !baseLookup.TryGetValue(name, out parser))
					parser = Terminals.LetterOrDigit.Repeat().Named(name);
				m.Tag = parser;
			};

			literal.Matched += m => m.Tag = new LiteralParser(m["value"].Value);
			optionalRule.Matched += m => m.Tag = new OptionalParser((Parser)m["parser"].Tag);
			repeatRule.Matched += m => m.Tag = new RepeatParser((Parser)m["parser"].Tag, 1);
			list.Matched += m => {
				if (m.Matches.Count > 1)
				{
					var parser = new SequenceParser();
					foreach (var child in m.Matches)
					{
						if (child.Parser.Name == "ws")
							parser.Items.Add(sws);
						else if (child.Parser.Name == "term")
							parser.Items.Add((Parser)child["parser"].Tag);
					}
					m.Tag = parser;
				}
				else
				{
					m.Tag = m["parser"].Tag;
				}
			};

			listRepeat.Matched += m => {
				// collapse alternatives to one alternative parser
				var parser = (Parser)m["expression"]["parser"].Tag;
				var alt = parser as AlternativeParser ?? new AlternativeParser(parser);
				alt.Items.Insert(0, (Parser)m["list"]["parser"].Tag);
				m.Tag = alt;
			};

			rule.Matched += m => {
				var parser = (NamedParser)m.Tag;
				parser.Inner = (Parser)m["parser"].Tag;
				m.Tag = parser;
			};
			rule.PreMatch += m => {
				var name = m["ruleName"]["name"].Value;
				NamedParser parser;
				if (name == startParserName)
					parser = new Grammar(name);
				else
					parser = new NamedParser(name);
				m.Tag = parser;
				parserLookup[parser.Name] = parser;
			};
		}

		Parser LineEnd()
		{
			var lineEnd = new AlternativeParser();
			lineEnd.Items.Add(lineEnd & lineEnd);
			lineEnd.Items.Add(sws & Terminals.Eol);
			return lineEnd;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			parserLookup = new Dictionary<string, NamedParser>();
			return base.InnerParse(args);
		}

		public Grammar Build(string bnf, string startParserName)
		{
			this.startParserName = startParserName;
			NamedParser parser;
			var match = this.Match(new StringScanner(bnf));

			if (!match.Success)
			{
				throw new FormatException(string.Format("Error parsing bnf: \n{0}", match.ErrorMessage));
			}
			if (!parserLookup.TryGetValue(startParserName, out parser))
				throw new ArgumentException("the topParser specified is not found in this bnf");
			return parser as Grammar;
		}

		public string ToCode(string bnf, string startParserName, string className = "GeneratedGrammar")
		{
			using (var writer = new StringWriter())
			{
				ToCode(bnf, startParserName, writer, className);
				return writer.ToString();
			}
		}

		public void ToCode(string bnf, string startParserName, TextWriter writer, string className = "GeneratedGrammar")
		{
			var parser = Build(bnf, startParserName);
			var iw = new IndentedTextWriter(writer, "    ");

			iw.WriteLine("/* Date Created: {0}, Source BNF:", DateTime.Now);
			iw.Indent ++;
			foreach (var line in bnf.Split('\n'))
				iw.WriteLine(line);
			iw.Indent --;
			iw.WriteLine("*/");

			var parserWriter = new CodeParserWriter
			{
				ClassName = className
			};
			parserWriter.Write(parser, writer);
		}
	}
}

