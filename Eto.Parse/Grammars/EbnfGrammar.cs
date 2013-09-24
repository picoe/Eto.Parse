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
	/// <summary>
	/// Grammar to build a parser grammar using Extended Backus-Naur Form
	/// </summary>
	/// <remarks>
	/// See http://en.wikipedia.org/wiki/Extended_Backus-Naur_Form
	/// 
	/// This grammar is not thread-safe.
	/// </remarks>
	public class EbnfGrammar : Grammar
	{
		Dictionary<string, Parser> parserLookup;
		Dictionary<string, Parser> specialLookup = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase);
		string startParserName;
		Parser ws = -(Terminals.WhiteSpace);
		Parser cws = -(("(*" & (-Terminals.AnyChar).Until("*)") & "*)") | Terminals.WhiteSpace);
		Parser separator;

		public bool DefineCommonNonTerminals { get; set; }

		public IDictionary<string, Parser> SpecialParsers { get { return specialLookup; } }

		void GenerateSeparator()
		{
			Parser comment;
			if (parserLookup.TryGetValue("comment", out comment))
				separator = (-(Terminals.WhiteSpace | comment)).Named("separator");
			else
				separator = (-(Terminals.WhiteSpace)).Named("separator");
		}

		void GenerateSpecialSequences()
		{
			// special sequences for each terminal
			foreach (var property in typeof(Terminals).GetProperties())
			{
				if (typeof(Parser).IsAssignableFrom(property.PropertyType))
				{
					var parser = property.GetValue(null, null) as Parser;
					var name = "Terminals." + property.Name;
					specialLookup[name] = parser;
				}
			}
		}

		public EbnfGrammar()
			: base("ebnf")
		{
			DefineCommonNonTerminals = true;
			GenerateSpecialSequences();

			// terminals
			var terminal_string = ("'" & (+Terminals.AnyChar).Until("'").Named("value") & "'")
				| ("\"" & (+Terminals.AnyChar).Until("\"").Named("value") & "\"")
				| ("’" & (+Terminals.AnyChar).Until("’").Named("value") & "’");

			var special_sequence = ("?" & (+Terminals.AnyChar).Until("?").Named("name") & "?").Named("special sequence");

			var meta_identifier_terminal = Terminals.Letter & -(Terminals.LetterOrDigit | '_');
			var integer = new NumberParser();

			var old = Parser.DefaultSeparator;
			Parser.DefaultSeparator = cws;

			// nonterminals
			var definition_list = new UnaryParser("definition list");
			var single_definition = new UnaryParser("single definition");
			var term = new UnaryParser("term");
			var primary = new UnaryParser("primary");
			var exception = new UnaryParser("exception");
			var factor = new UnaryParser("factor");
			var meta_identifier = new UnaryParser("meta identifier");
			var syntax_rule = new UnaryParser("syntax rule");
			var rule_equals = new UnaryParser("equals");

			var optional_sequence = ("[" & definition_list & "]").Named("optional sequence");
			var repeated_sequence = ("{" & definition_list & "}").Named("repeated sequence");
			var grouped_sequence = ("(" & definition_list & ")").Named("grouped sequence");

			// rules
			meta_identifier.Inner = (+meta_identifier_terminal).SeparatedBy(ws);
			primary.Inner = optional_sequence | repeated_sequence
				| special_sequence | grouped_sequence
				| meta_identifier | terminal_string.Named("terminal string") | null;
			factor.Inner = ~(integer.Named("integer") & "*") & primary;
			term.Inner = factor & ~("-" & exception);
			exception.Inner = term;
			single_definition.Inner = term & -("," & term);
			definition_list.Inner = single_definition & -("|" & single_definition);
			rule_equals.Inner = (Parser)"=" | ":=";
			syntax_rule.Inner = meta_identifier & rule_equals & definition_list & ";";

			this.Inner = cws & +syntax_rule & cws;

			Parser.DefaultSeparator = old;

			AttachEvents();
		}

		protected override void OnPreMatch(Match match)
		{
			base.OnPreMatch(match);
			GenerateSeparator();
		}

		void AttachEvents()
		{
			var syntax_rule = this["syntax rule"];
			syntax_rule.Matched += m => {
				var name = m["meta identifier"].Text;
				var isTerminal = m["equals"].Text == "==";
				var parser = m.Tag as UnaryParser;
				var inner = DefinitionList(m["definition list"], isTerminal);
				if (name == startParserName)
					parser.Inner = separator & inner & separator;
				else
					parser.Inner = inner;
			};
			syntax_rule.PreMatch += m => {
				var name = m["meta identifier"].Text;
				var parser = (name == startParserName) ? new Grammar(name) : new UnaryParser(name);
				m.Tag = parserLookup[name] = parser;
			};
		}

		Parser DefinitionList(Match match, bool isTerminal)
		{
			var definitions = match.Find("single definition").ToArray();
			if (definitions.Length == 1)
				return SingleDefinition(definitions[0], isTerminal);
			else
				return new AlternativeParser(definitions.Select(r => SingleDefinition(r, isTerminal)));
		}

		Parser SingleDefinition(Match match, bool isTerminal)
		{
			var terms = match.Find("term").ToArray();
			if (terms.Length == 1)
				return Term(terms[0], isTerminal);
			else
			{
				var sequence = new SequenceParser(terms.Select(r => Term(r, isTerminal)));
				if (!isTerminal)
					sequence.Separator = separator;
				return sequence;
			}
		}

		Parser Term(Match match, bool isTerminal)
		{
			var factor = Factor(match["factor"], isTerminal);
			var exception = match["exception"];
			if (exception)
				return new ExceptParser(factor, Term(exception["term"], isTerminal));
			else
				return factor;
		}

		Parser Factor(Match match, bool isTerminal)
		{
			var primary = Primary(match["primary"], isTerminal);
			var integer = match["integer"];
			if (integer)
				return new RepeatParser(primary, Int32.Parse(integer.Text));
			else
				return primary;
		}

		Parser Primary(Match match, bool isTerminal)
		{
			var child = match.Matches.FirstOrDefault();
			if (child == null)
				return null;
			Parser parser;
			switch (child.Name)
			{
				case "grouped sequence":
					return new UnaryParser(DefinitionList(child["definition list"], isTerminal));
				case "optional sequence":
					return new OptionalParser(DefinitionList(child["definition list"], isTerminal));
				case "repeated sequence":
					var repeat = new RepeatParser(DefinitionList(child["definition list"], isTerminal), 0);
					if (!isTerminal)
						repeat.Separator = separator;
					return repeat;
				case "meta identifier":
					if (!parserLookup.TryGetValue(child.Text, out parser))
					{
						parser = parserLookup[child.Text] = Terminals.LetterOrDigit.Repeat().Named(child.Text);
					}
					return parser;
				case "terminal string":
					return new LiteralTerminal(child["value"].Text);
				case "special sequence":
					var name = child["name"].Text.Trim();
					if (specialLookup.TryGetValue(name, out parser))
						return parser;
					else
						return null;
				default:
					return null;
			}
		}

		protected override int InnerParse(ParseArgs args)
		{
			parserLookup = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase);
			if (DefineCommonNonTerminals)
			{
				parserLookup["letter or digit"] = Terminals.LetterOrDigit;
				parserLookup["letter"] = Terminals.Letter;
				parserLookup["decimal digit"] = Terminals.Digit;
				parserLookup["character"] = Terminals.AnyChar;
			}

			return base.InnerParse(args);
		}

		public Grammar Build(string bnf, string startParserName)
		{
			Parser parser;
			this.startParserName = startParserName;
			var match = this.Match(new StringScanner(bnf));

			if (!match.Success)
			{
				throw new FormatException(string.Format("Error parsing ebnf: \n{0}", match.ErrorMessage));
			}
			if (!parserLookup.TryGetValue(startParserName, out parser))
				throw new ArgumentException("the topParser specified is not found in this ebnf");
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

			iw.WriteLine("/* Date Created: {0}, Source EBNF:", DateTime.Now);
			iw.Indent++;
			foreach (var line in bnf.Split('\n'))
				iw.WriteLine(line);
			iw.Indent--;
			iw.WriteLine("*/");

			var parserWriter = new CodeParserWriter
			{
				ClassName = className
			};
			parserWriter.Write(parser, writer);
		}
	}
}

