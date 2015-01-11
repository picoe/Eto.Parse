using System;
using Eto.Parse.Parsers;
using Eto.Parse.Scanners;
using System.Collections.Generic;
using System.Linq;
using Eto.Parse.Writers;
using System.IO;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Eto.Parse.Grammars
{
	/// <summary>
	/// Styles for the <see cref="EbnfGrammar"/>
	/// </summary>
	[Flags]
	public enum EbnfStyle
	{
		/// <summary>
		/// ISO format as defined here: http://en.wikipedia.org/wiki/Extended_Backus-Naur_Form
		/// </summary>
		Iso14977 = SquareBracketAsOptional | BracketComments | NumericCardinality | SemicolonTerminator | CommaSeparator | WhitespaceSeparator,

		/// <summary>
		/// W3C format as defined for the XML spec here: http://www.w3.org/TR/REC-xml/#sec-notation
		/// </summary>
		W3c = CharacterSets | CardinalityFlags | DoubleColonEquals,

		/// <summary>
		/// Enables comments using round brackets (* *), otherwise comments use C style /* */
		/// </summary>
		BracketComments = 1 << 0,

		/// <summary>
		/// Enables using square brackets for optional sequences.  E.g. [ (rules) ]. Mutually exclusive with <see cref="CharacterSets"/> option.
		/// </summary>
		SquareBracketAsOptional = 1 << 1,

		/// <summary>
		/// Enables using character sets and ranges in square brackets. E.g. [1-9ABCDEF], with a carat for inverse: [^1-9ABCDEF]
		/// </summary>
		CharacterSets = 1 << 2,

		/// <summary>
		/// Enables numeric cardinality to prefix a term. E.g. 10 * (myTerm)
		/// </summary>
		NumericCardinality = 1 << 3,

		/// <summary>
		/// Enables cardinality flags *+? after the rule, E.g. myFirstTerm* mySecondTerm+ myThirdTerm?.
		/// </summary>
		/// <remarks>
		/// * = zero or more
		/// + = one or more
		/// ? = zero or one, only enabled when <see cref="SquareBracketAsOptional"/> is not specified.
		/// </remarks>
		CardinalityFlags = 1 << 4,

		/// <summary>
		/// Require a semicolon to terminate each rule, otherwise only whitespace is required. E.g. myTerm = Term1 , Term2;
		/// </summary>
		SemicolonTerminator = 1 << 5,

		/// <summary>
		/// Separate each term by a comma. If not specified, a term may be more than one word.
		/// </summary>
		CommaSeparator = 1 << 6,

		/// <summary>
		/// Use a double colon for the rule equals. If not specified, no colon is required.  E.g. myTerm ::= Term1  vs. myTerm = Term1
		/// </summary>
		DoubleColonEquals = 1 << 7,

		/// <summary>
		/// Use Terminals.WhiteSpace as a default separator between each non-terminal. Ignored when <see cref="UseWhitespaceRule"/> is specified and grammar defines the whitespace terminal.
		/// </summary>
		WhitespaceSeparator = 1 << 8,

		/// <summary>
		/// Use the rule named 'comment' for whitespace in the definition. Ignored when <see cref="UseWhitespaceRule"/> is specified and grammar defines the whitespace terminal.
		/// </summary>
		UseCommentRuleWithSeparator = 1 << 9,

		/// <summary>
		/// Use the rule named 'whitespace' as the separator between each non-terminal. Overrides <see cref="WhitespaceSeparator"/> and <see cref="UseCommentRuleWithSeparator"/>.
		/// </summary>
		UseWhitespaceRule = 1 << 10
	}

	/// <summary>
	/// Grammar to build a parser grammar using Extended Backus-Naur Form
	/// </summary>
	/// <remarks>
	/// This has a few extensions to allow for explicit or implicit whitespace:
	/// <code>
	/// 	terminal sequence := 'a', 'b', 'c', 'd'; (* no whitespace inbetween *)
	/// 	rule sequence = 'a', 'b', 'c', 'd'; (* allows for whitespace *)
	/// </code>
	/// 
	/// You can also reference parsers from <see cref="EbnfGrammar.SpecialParsers"/> which by default includes the list
	/// of terminals from <see cref="Terminals"/>
	/// <code>
	/// 	ws := ? Terminals.Whitespace ?;
	/// </code>
	///
	/// This grammar is not thread-safe.
	/// </remarks>
	public class EbnfGrammar : Grammar
	{
		Dictionary<string, Parser> parserLookup;
		Dictionary<string, Parser> specialLookup = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase);
		string startParserName;
		Parser separator;

		public EbnfStyle Style { get; private set; }

		public bool DefineCommonNonTerminals { get; set; }

		public IDictionary<string, Parser> SpecialParsers { get { return specialLookup; } }

		void GenerateSeparator()
		{
			separator = null;
			Parser whitespaceRule;
			if (Style.HasFlag(EbnfStyle.UseWhitespaceRule) && parserLookup.TryGetValue("whitespace", out whitespaceRule))
				separator = whitespaceRule;
			else
			{
				if (Style.HasFlag(EbnfStyle.WhitespaceSeparator))
					separator = Terminals.WhiteSpace;
				Parser comment;
				if (Style.HasFlag(EbnfStyle.UseCommentRuleWithSeparator) && parserLookup.TryGetValue("comment", out comment))
					separator = separator != null ? (separator | comment) : comment;
				if (separator != null)
					separator = -separator;
			}
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

		public EbnfGrammar(EbnfStyle style)
			: base("ebnf")
		{
			Style = style;
			DefineCommonNonTerminals = true;
			GenerateSpecialSequences();

			// terminals
			var comment = style.HasFlag(EbnfStyle.BracketComments) ? new GroupParser("(*", "*)") : new GroupParser("/*", "*/");
			var ows = -(Terminals.WhiteSpace | comment);
			var rws = +(Terminals.WhiteSpace | comment);
			var hex_character = ("#x" & +Terminals.HexDigit);
			var character = (("\\" & Terminals.AnyChar) | hex_character | Terminals.AnyChar.Except("]")).WithName("character");
			var character_range = (character & "-" & character).WithName("character range");
			var character_set = ("[" & ~(Parser)"^" & +(character_range | character) & "]").WithName("character set");
			var terminal_string = new StringParser { QuoteCharacters = new [] { '\"', '\'', '’' }, Name = "terminal string" };
			var special_sequence = ("?" & (+Terminals.AnyChar).Until("?").WithName("name") & "?").WithName("special sequence");
			var meta_identifier_terminal = Terminals.Letter & -(Terminals.LetterOrDigit | '_');
			var integer = new NumberParser().WithName("integer");

			// nonterminals
			var definition_list = new RepeatParser(0).WithName("definition list");
			var single_definition = new RepeatParser(1).WithName("single definition");
			var term = new SequenceParser().WithName("term");
			var primary = new AlternativeParser().WithName("primary");
			var exception = new UnaryParser("exception");
			var factor = new SequenceParser().WithName("factor");
			var meta_identifier = new RepeatParser(1).WithName("meta identifier");
			var syntax_rule = new SequenceParser().WithName("syntax rule");
			var rule_equals = new AlternativeParser().WithName("equals");
			Parser meta_reference = meta_identifier;

			Parser grouped_sequence = ("(" & ows & definition_list & ows & ")").WithName("grouped sequence");

			if (style.HasFlag(EbnfStyle.SquareBracketAsOptional))
			{
				primary.Add(("[" & ows & definition_list & ows & "]").WithName("optional sequence"));
			}

			if (!style.HasFlag(EbnfStyle.CardinalityFlags))
			{
				var repeated_sequence = ("{" & ows & definition_list & ows & "}").WithName("repeated sequence");
				primary.Add(repeated_sequence);
			}

			// rules
			meta_identifier.Inner = meta_identifier_terminal;
			meta_identifier.Separator = +(Terminals.SingleLineWhiteSpace);
			if (!style.HasFlag(EbnfStyle.CommaSeparator))
			{
				// w3c identifiers must be a single word
				meta_identifier.Maximum = 1;
				meta_reference = meta_reference.NotFollowedBy(ows & rule_equals);
			}
			primary.Add(grouped_sequence, meta_reference, terminal_string, special_sequence);
			if (style.HasFlag(EbnfStyle.CharacterSets) && !style.HasFlag(EbnfStyle.SquareBracketAsOptional))
			{
				// w3c supports character sets
				primary.Add(hex_character.Named("hex character"));
				primary.Add(character_set);
			}
			if (style.HasFlag(EbnfStyle.NumericCardinality))
				factor.Add(~(integer & ows & "*" & ows));
			factor.Add(primary);
			if (style.HasFlag(EbnfStyle.CardinalityFlags))
			{
				// w3c defines cardinality at the end of a factor
				var flags = style.HasFlag(EbnfStyle.SquareBracketAsOptional) ? "*+" : "?*+";
				factor.Add(~(ows & Terminals.Set(flags).WithName("cardinality"))); 
			}
			term.Add(factor, ~(ows & "-" & ows & exception));
			exception.Inner = term;
			single_definition.Inner = term;
			single_definition.Separator = style.HasFlag(EbnfStyle.CommaSeparator) ? (Parser)(ows & "," & ows) : ows;
			definition_list.Inner = single_definition;
			definition_list.Separator = ows & "|" & ows;
			rule_equals.Add(style.HasFlag(EbnfStyle.DoubleColonEquals) ? "::=" : "=", ":=");
			syntax_rule.Add(meta_identifier, ows, rule_equals, ows, definition_list);
			if (style.HasFlag(EbnfStyle.SemicolonTerminator))
				syntax_rule.Add(ows, ";"); // iso rules are terminated by a semicolon

			var syntax_rules = +syntax_rule;
			syntax_rules.Separator = style.HasFlag(EbnfStyle.SemicolonTerminator) ? ows : rws;

			Inner = ows & syntax_rules & ows;

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
				var isTerminal = m["equals"].Text == ":=";
				var parser = m.Tag as UnaryParser;
				var inner = DefinitionList(m["definition list"], isTerminal);
				if (separator != null && name == startParserName)
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
			var definitions = match.Find("single definition").ToList();
			if (definitions.Count == 1)
				return SingleDefinition(definitions[0], isTerminal);
			if (definitions.Count == 0)
				return null;
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
			else if (factor == null)
				throw new Exception("woo");
			else
				return factor;
		}

		Parser Factor(Match match, bool isTerminal)
		{
			var primary = Primary(match["primary"], isTerminal);
			var cardinality = match["cardinality"];
			if (cardinality)
			{
				switch (cardinality.Text)
				{
					case "?":
						primary = new OptionalParser(primary);
						break;
					case "*":
						primary = new RepeatParser(primary, 0);
						break;
					case "+":
						primary = new RepeatParser(primary, 1);
						break;
					default:
						throw new FormatException(string.Format("Cardinality '{0}' is unknown", cardinality.Text));
						break;
				}
			}
			var integer = match["integer"];
			if (integer)
				return new RepeatParser(primary, Int32.Parse(integer.Text));
			else
				return primary;
		}

		Parser Primary(Match match, bool isTerminal)
		{
			var child = match.Matches.FirstOrDefault(r => r.Name != null);
			if (child == null)
				throw new FormatException(string.Format("Primary must have a child. Text: '{0}'", match.Text));
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
					return new LiteralTerminal(child.StringValue);
				case "hex character":
					return new SingleCharTerminal((char)int.Parse(child.Text.Substring(2), NumberStyles.HexNumber));
				case "character set":
					return CharacterSet(child);
				case "special sequence":
					var name = child["name"].Text.Trim();
					if (specialLookup.TryGetValue(name, out parser))
						return parser;
					else
						return null;
				default:
					throw new FormatException(string.Format("Could not parse child with name '{0}'", child.Name));
			}
		}

		char? Character(Match match)
		{
			var text = match.Text;
			if (text.StartsWith("#x", StringComparison.OrdinalIgnoreCase))
			{
				var val = int.Parse(text.Substring(2), NumberStyles.HexNumber);
				if (val <= 0xFFFF && val >= 0)
					return (char)val;
				else
					return null;
			}
			if (text.Length == 2 && text[0] == '\\')
			{
				return text[1];
			}
			if (text.Length != 1)
				throw new FormatException(string.Format("Character should only match one character '{0}'", text));
			return text[0];
		}

		Parser CharacterSet(Match match)
		{
			var alt = new AlternativeParser();
			var inverse = match.Text.StartsWith("[^", StringComparison.Ordinal);
			var characters = new List<char>();
			foreach (var child in match.Matches.Where(r => r.Name != null))
			{
				switch (child.Name)
				{
					case "character range":
						var first = Character(child.Matches.First(r => r.Name == "character"));
						var last = Character(child.Matches.Last(r => r.Name == "character"));
						if (first != null && last != null)
							alt.Add(new CharRangeTerminal(first.Value, last.Value) { Inverse = inverse });
						break;
					case "character":
						var character = Character(child);
						if (character != null)
							characters.Add(character.Value);
						break;
					default:
						throw new FormatException(string.Format("Invalid character set child for text '{0}'", child.Text));
				}
			}
			if (characters.Count > 0)
			{
				alt.Add(new CharSetTerminal(characters.ToArray()) { Inverse = inverse });
			}
			if (alt.Items.Count > 1)
				return alt;
			if (alt.Items.Count > 0)
				return alt.Items[0];
			return new UnaryParser();
			//throw new FormatException(string.Format("Character set has no characters '{0}'", match.Text));
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

