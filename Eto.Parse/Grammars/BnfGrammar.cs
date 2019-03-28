using System;
using Eto.Parse.Parsers;
using Eto.Parse.Scanners;
using System.Collections.Generic;
using System.Linq;
using Eto.Parse.Writers;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;

namespace Eto.Parse.Grammars
{
	[Flags]
	public enum BnfStyle
	{
		/// <summary>
		/// No extended styles
		/// </summary>
		None = 0,

		/// <summary>
		/// Enable ebnf-style comments using (* *)
		/// </summary>
		Comments = 1 << 0,

		/// <summary>
		/// Define common terminals from <see cref="Terminals"/>
		/// </summary>
		CommonTerminals = 1 << 1,

		/// <summary>
		/// Enable ebnf-style cardinality using { } (one or more), [ ] (optional)
		/// </summary>
		Cardinality = 1 << 2,

		/// <summary>
		/// All extended styles
		/// </summary>
		All = Comments | CommonTerminals | Cardinality
	}


	/// <summary>
	/// Grammar to build a parser grammar using Backus-Naur Form
	/// </summary>
	/// <remarks>
	/// See https://en.wikipedia.org/wiki/Backus-Naur_Form.
	/// 
	/// This implements certain enhancements, such as grouping using brackets, repeating using curly braces, and
	/// optional values using square brackets.
	/// 
	/// This grammar is not thread-safe.
	/// </remarks>
	public class BnfGrammar : Grammar
	{
		Dictionary<string, Parser> parserLookup = new Dictionary<string, Parser>(StringComparer.OrdinalIgnoreCase);
		readonly Dictionary<string, Parser> baseLookup = new Dictionary<string, Parser>(StringComparer.OrdinalIgnoreCase);
		readonly Parser sws = Terminals.SingleLineWhiteSpace.Repeat(0);
		readonly Parser sq = Terminals.Set('\'');
		readonly Parser dq = Terminals.Set('"');
		readonly LiteralTerminal ruleSeparator = new LiteralTerminal("::=");
		string startParserName;
		Grammar startGrammar;
		readonly Parser rule;
		readonly Parser listRepeat;
		readonly Parser list;
		readonly Parser repeatRule;
		readonly Parser optionalRule;
		readonly Parser literal;
		readonly Parser ruleName;

		/// <summary>
		/// Gets or sets the separator for rules, which is usually '::=' for BNF
		/// </summary>
		/// <value>The rule separator</value>
		protected string RuleSeparator { get { return ruleSeparator.Value; } set { ruleSeparator.Value = value; } }

		/// <summary>
		/// Gets the rule parser for each of the rule types
		/// </summary>
		/// <value>The rule parser.</value>
		protected AlternativeParser RuleParser { get; private set; }

		/// <summary>
		/// Gets the term parser for each term, such as optional, repeating, grouping, etc
		/// </summary>
		/// <value>The term parser.</value>
		protected AlternativeParser TermParser { get; private set; }

		/// <summary>
		/// Gets the expresssions parser to support different expressions. By default, only one expression
		/// is defined: RuleNameParser &amp; RuleSeparator &amp; RuleParser &amp; EOL
		/// </summary>
		/// <value>The expresssions this parser supports</value>
		protected AlternativeParser Expresssions { get; private set; }

		/// <summary>
		/// Gets the rule name parser, by default is a string surrounded by angle brackets
		/// </summary>
		/// <value>The rule name parser.</value>
		protected SequenceParser RuleNameParser { get; private set; }

		/// <summary>
		/// Gets the parsed rules
		/// </summary>
		/// <value>The rules.</value>
		public Dictionary<string, Parser> Rules { get { return parserLookup; } protected set { parserLookup = value; } }

		public BnfGrammar(BnfStyle style = BnfStyle.All)
			: base("bnf")
		{
			Parser ows = Terminals.WhiteSpace.Repeat(0);
			Parser rws = Terminals.WhiteSpace.Repeat(1);

			if (style.HasFlag(BnfStyle.CommonTerminals))
			{
				foreach (var terminal in Terminals.GetTerminals())
				{
					baseLookup[terminal.Item1] = terminal.Item2;
				}
			}

			if (style.HasFlag(BnfStyle.Comments))
			{
				// allow ebnf comments
				var comment = new GroupParser("(*", "*)");
				ows = (Terminals.WhiteSpace | comment).Repeat(0);
				rws = (Terminals.WhiteSpace | comment).Repeat(1);
			}

			literal = (
				(sq & (+!sq).WithName("value").Optional() & sq)
				| (dq & (+!dq).WithName("value").Optional() & dq)
				| (+Terminals.Set(" \n\r<[{(|)}]>").Inverse()).WithName("value")
			).WithName("parser");


			RuleNameParser = "<" & Terminals.Set("<>\n\r").Inverse().Repeat().WithName("name") & ">";

			RuleParser = new AlternativeParser(); // defined later 

			TermParser = new AlternativeParser();
			TermParser.Name = "term";
			TermParser.Add((ruleName = RuleNameParser.Named("parser")).NotFollowedBy(ows & ruleSeparator));
			if (style.HasFlag(BnfStyle.Cardinality))
			{
				TermParser.Items.Add('(' & ows & RuleParser & ows & ')');
				TermParser.Items.Add(repeatRule = ('{' & ows & RuleParser & ows & '}').WithName("parser"));
				TermParser.Items.Add(optionalRule = ('[' & ows & RuleParser & ows & ']').WithName("parser"));
			}
			TermParser.Items.Add(literal);

			var rep = -(~((+Terminals.WhiteSpace).WithName("ws")) & TermParser);
			rep.Name = "rep";
			rep.AddMatch = false;
			list = (TermParser & rep).WithName("parser");

			listRepeat = (list.Named("list") & ows & '|' & ~(ows & RuleParser.Named("expression"))).WithName("parser");
			RuleParser.Items.Add(listRepeat);
			RuleParser.Items.Add(list);
			RuleParser.Items.Add((ows & RuleNameParser & ows & ruleSeparator).Not() & Terminals.WhiteSpace.Inverse().Repeat().WithName("value").Named("parser"));

			rule = (RuleNameParser.Named("ruleName") & ows & ruleSeparator & ows & RuleParser).WithName("parser");
			Expresssions = new AlternativeParser();
			Expresssions.Items.Add(rule);

			this.Inner = ows & (+Expresssions).SeparatedBy(rws) & ows;

			AttachEvents();
		}

		void AttachEvents()
		{
			ruleName.Matched += m => {
				Parser parser;
				var name = m["name"].Text;
				if (!parserLookup.TryGetValue(name, out parser) && !baseLookup.TryGetValue(name, out parser))
				{
					parser = Terminals.LetterOrDigit.Repeat();
					parser.Name = name;
				}
				m.Tag = parser;
			};

			literal.Matched += m => m.Tag = new LiteralTerminal(m["value"].Text);
			if (optionalRule != null)
				optionalRule.Matched += m => m.Tag = new OptionalParser((Parser)m["parser"].Tag);
			if (repeatRule != null)
				repeatRule.Matched += m => m.Tag = new RepeatParser((Parser)m["parser"].Tag, 0) { Separator = sws };
			list.Matched += m => {
				if (m.Matches.Count > 1)
				{
					var parser = new SequenceParser();
					foreach (var child in m.Matches)
					{
						if (child.Parser.Name == "ws")
							parser.Items.Add(sws);
						else if (child.Parser.Name == "term")
						{
							var childParser = (Parser)child["parser"].Tag;
							if (childParser == null)
								throw new Exception("woo");
							parser.Items.Add(childParser);
						}
					}
					m.Tag = parser;
				}
				else
				{
					m.Tag = m["term"]["parser"].Tag;
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
				var parser = (UnaryParser)m.Tag;
				parser.Inner = (Parser)m["parser"].Tag;
				m.Tag = parser;
			};
			rule.PreMatch += m => {
				var name = m["ruleName"]["name"].Text;
				Parser parser;
				if (name == startParserName)
					parser = startGrammar ?? new Grammar(name);
				else

					parser = new UnaryParser(name);
				m.Tag = parser;
				parserLookup[name] = parser;
			};
		}

		protected override int InnerParse(ParseArgs args)
		{
			parserLookup = new Dictionary<string, Parser>();
			return base.InnerParse(args);
		}

		public Grammar Build(string bnf, string startParserName, Grammar grammar = null)
		{
			this.startParserName = startParserName;
			this.startGrammar = grammar;
			Parser parser;
			var match = Match(new StringScanner(bnf));

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

