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
	public class BnfParser : NamedParser
	{
		Dictionary<string, NamedParser> parserLookup = new Dictionary<string, NamedParser>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, NamedParser> baseLookup = new Dictionary<string, NamedParser>(StringComparer.InvariantCultureIgnoreCase);
		Parser sws = Terminals.SingleLineWhiteSpace.Repeat(0);
		Parser ws = Terminals.WhiteSpace.Repeat(0);
		Parser sq = Terminals.Set('\'');
		Parser dq = Terminals.Set('"');
		StringParser ruleSeparator = new StringParser("::=");

		protected string RuleSeparator { get { return ruleSeparator.Value; } set { ruleSeparator.Value = value; } }

		protected AlternativeParser RuleParser { get; private set; }

		protected AlternativeParser TermParser { get; private set; }

		protected AlternativeParser Expresssions { get; private set; }

		protected SequenceParser RuleNameParser { get; private set; }

		public Dictionary<string, NamedParser> Rules { get { return parserLookup; } protected set { parserLookup = value; } }

		public BnfParser(bool enhanced = true)
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

			var literal = (
				(sq & (+!sq).Named("value").Optional() & sq)
				| (dq & (+!dq).Named("value").Optional() & dq)
			).Named("parser", m => m.Context = new StringParser(m["value"].Value));

			RuleNameParser = "<" & Terminals.Set('>').Inverse().Repeat().Named("name") & ">";

			RuleParser = new AlternativeParser(); // defined later 

			TermParser = new AlternativeParser();
			TermParser.Items.Add(literal);
			TermParser.Items.Add(RuleNameParser.Named("parser", m => {
				NamedParser parser;
				var name = m["name"].Value;
				if (!parserLookup.TryGetValue(name, out parser) && !baseLookup.TryGetValue(name, out parser))
					parser = Terminals.LetterOrDigit.Repeat().Named(name);
				m.Context = parser;
			}));
			if (enhanced)
			{
				TermParser.Items.Add('(' & sws & RuleParser & sws & ')');
				TermParser.Items.Add(('{' & sws & RuleParser & sws & '}').Named("parser", m => {
					m.Context = new RepeatParser((Parser)m["parser"].Context);
				}));
				TermParser.Items.Add(('[' & sws & RuleParser & sws & ']').Named("parser", m => {
					m.Context = new OptionalParser((Parser)m["parser"].Context);
				}));
			}


			var list = new SequenceParser(TermParser.Named("term"), -(sws.Named("ws") & TermParser.Named("term")))
			.Named("parser", m => {
				if (m.Matches.Count > 1)
				{
					var parser = new SequenceParser();
					foreach (var child in m.Matches)
					{
						if (child.Parser.Name == "ws")
							parser.Items.Add(sws);
						else if (child.Parser.Name == "term")
							parser.Items.Add((Parser)child["parser"].Context);
					}
					m.Context = parser;
				}
				else
				{
					m.Context = m["parser"].Context;
				}
			});

			RuleParser.Items.Add((list.Named("list") & ws & '|' & sws & RuleParser.Named("expression"))
			                     .Named("parser", m => {
				// collapse alternatives to one alternative parser
				var parser = (Parser)m["expression"]["parser"].Context;
				var alt = parser as AlternativeParser ?? new AlternativeParser(parser);
				alt.Items.Insert(0, (Parser)m["list"]["parser"].Context);
				m.Context = alt;
			}));
			RuleParser.Items.Add(list);

			var rule = (~lineEnd & sws & RuleNameParser.Named("ruleName") & ws & ruleSeparator & sws & RuleParser & lineEnd)
					.Named("parser", 
			         matched: m => {
				var parser = (NamedParser)m.Context;
				parser.Inner = (Parser)m["parser"].Context;
				m.Context = parser;
			},
			         preMatch: m => {
				var parser = new NamedParser(m["ruleName"]["name"].Value);
				m.Context = parser;
				parserLookup[parser.Name] = parser;
			});
			Expresssions = new AlternativeParser();
			Expresssions.Items.Add(rule);

			this.Inner = (Expresssions & this) | Expresssions;
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

		public Dictionary<string, NamedParser> Build(string bnf)
		{
			var match = this.Match(new StringScanner(bnf));

			if (match.Error != null)
			{
				throw new FormatException(string.Format("Error parsing bnf starting at: \n{0}", bnf.Substring((int)match.Error.Index)));
			}
			return parserLookup;
		}

		public NamedParser Build(string bnf, string startParserName)
		{
			NamedParser parser;
			if (!Build(bnf).TryGetValue(startParserName, out parser))
				throw new ArgumentException("the topParser specified is not found in this bnf");
			return parser;
		}

		public string ToCode(string bnf, string startParserName, bool includeWrapperClass = true)
		{
			using (var writer = new StringWriter())
			{
				ToCode(bnf, startParserName, writer);
				return writer.ToString();
			}
		}

		public void ToCode(string bnf, string startParserName, TextWriter writer, bool includeWrapperClass = true)
		{
			var parser = Build(bnf, startParserName);
			var iw = new IndentedTextWriter(writer, "    ");

			if (includeWrapperClass)
			{
				iw.WriteLine("public static class GeneratedParser");
				iw.WriteLine("{");
				iw.Indent ++;
			}
			iw.WriteLine("/* Date Created: {0}, Source BNF:", DateTime.Now);
			iw.Indent ++;
			foreach (var line in bnf.Split('\n'))
				iw.WriteLine(line);
			iw.Indent --;
			iw.WriteLine("*/");
			if (includeWrapperClass)
			{
				iw.WriteLine("public static Eto.Parse.Parser GetParser()");
				iw.WriteLine("{");
				iw.Indent ++;
			}
			WriteCode(parser, iw);
			if (includeWrapperClass)
			{
				iw.WriteLine("return {0};", Writers.Code.NamedWriter.GetIdentifier(startParserName));
				iw.Indent --;
				iw.WriteLine("}");
				iw.Indent --;
				iw.WriteLine("}");
			}
		}

		protected virtual void WriteCode(Parser parser, IndentedTextWriter writer)
		{
			var parserWriter = new CodeParserWriter();
			parserWriter.Write(parser, writer);
		}
	}
}

