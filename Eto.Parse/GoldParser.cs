using System;
using System.Linq;
using System.Collections.Generic;
using Eto.Parse.Parsers;
using Eto.Parse.Testers;
using System.IO;
using System.CodeDom.Compiler;
using Eto.Parse.Writers;

namespace Eto.Parse
{
	public class GoldDefinition
	{
		public Dictionary<string, string> Properties { get; private set; }

		public Dictionary<string, CharParser> Sets { get; private set; }

		public Dictionary<string, Parser> Terminals { get; private set; }

		public Dictionary<string, NamedParser> Rules { get; private set; }

		public Parser Whitespace { get { return Terminals["Whitespace"]; } }

		public NamedParser StartSymbol { get; set; }

		public GoldDefinition()
		{
			Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			Sets = new Dictionary<string, CharParser>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ "HT", Parse.Terminals.Set(0x09) },
				{ "LF", Parse.Terminals.Set(0x10) },
				{ "VT", Parse.Terminals.Set(0x11) },
				{ "FF", Parse.Terminals.Set(0x12) },
				{ "CR", Parse.Terminals.Set(0x13) },
				{ "Space", Parse.Terminals.Set(0x20) },
				{ "NBSP", Parse.Terminals.Set(0xA0) },
				{ "LS", Parse.Terminals.Set(0x2028) },
				{ "PS", Parse.Terminals.Set(0x2029) },

				{ "Number", Parse.Terminals.Range(0x30, 0x39) },
				{ "Digit", Parse.Terminals.Range(0x30, 0x39) },
				{ "Letter", Parse.Terminals.Range(0x41, 0x58) + Parse.Terminals.Range(0x61, 0x78) },
				{ "AlphaNumeric", Parse.Terminals.Range(0x30, 0x39) + Parse.Terminals.Range(0x41, 0x5A) + Parse.Terminals.Range(0x61, 0x7A) },
				{ "Printable", Parse.Terminals.Range(0x20, 0x7E) + Parse.Terminals.Set(0xA0) },
				{ "Letter Extended", Parse.Terminals.Range(0xC0, 0xD6) + Parse.Terminals.Range(0xD8, 0xF6) + Parse.Terminals.Range(0xF8, 0xFF) },
				{ "Printable Extended", Parse.Terminals.Range(0xA1, 0xFF) },
				{ "Whitespace", Parse.Terminals.Range(0x09, 0x0D) + Parse.Terminals.Range(0x20, 0xA0) },
			};
			Terminals = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ "Whitespace", +Sets["Whitespace"] }
			};
			Rules = new Dictionary<string, NamedParser>(StringComparer.InvariantCultureIgnoreCase);
		}
	}

	public class GoldParser : SequenceParser
	{
		GoldDefinition definition;
		NamedParser parameter;
		NamedParser ruleDecl;
		NamedParser handle;
		NamedParser symbol;
		NamedParser terminalDecl;
		NamedParser setDecl;
		Parser whitespace;
		NamedParser regExpItem;
		NamedParser regExp;

		public GoldParser()
		{
			var oldSeparator = Parser.DefaultSeparator;
			// Special Terminals

			var parameterCh = Terminals.Printable - Terminals.Set("\"'");
			var nonterminalCh = Terminals.LetterOrDigit + Terminals.Set("_-. ");
			var terminalCh = Terminals.LetterOrDigit + Terminals.Set("_-.");
			var literalCh = Terminals.Printable - Terminals.Set('\'');
			var setLiteralCh = Terminals.Printable - Terminals.Set("[]'");
			var setNameCh = Terminals.Printable - Terminals.Set("{}");

			var parameterName = ('"' & (+parameterCh).Named("value") & '"').Separate();
			var nonterminal = ('<' & (+nonterminalCh).Named("value") & '>').Separate();
			var terminal = ((+terminalCh).Named("terminal") | ('\'' & (-literalCh).Named("literal") & '\'')).Separate();
			var setLiteral = ('[' & +(setLiteralCh.Named("ch") | '\'' & (-literalCh).Named("ch") & '\'') & ']').Named("setLiteral");
			var setName = ('{' & (+setNameCh).Named("value") & '}').Named("setName");

			// Line-Based Grammar Declarations

			var comments = ~(("!*" & (-Terminals.AnyChar).Until("*!") & "*!") | ('!' & -!Terminals.Eol));
			whitespace = -Terminals.SingleLineWhiteSpace & comments & -Terminals.SingleLineWhiteSpace;
			this.Separator = Parser.DefaultSeparator = whitespace;
			var newline = Terminals.Eol;
			var nlOpt = -newline;
			var nl = +newline;

			// Parameter Definition

			var parameterItem = parameterName | terminal | setLiteral | setName | nonterminal;

			var parameterItems = +parameterItem;

			var parameterBody = parameterItems & -(nlOpt & '|' & parameterItems);

			parameter = (parameterName.Named("name") & nlOpt & '=' & parameterBody.Named("body") & nl).Named("parameter");

			// Set Definition

			var setItem = setLiteral | setName;

			var setExp = new NamedParser("setExp");
			setExp.Inner = (setItem & nlOpt & '+' & setExp).Named("add")
				| (setItem & nlOpt & '-' & setExp).Named("sub")
				| setItem;

			setDecl = (setName & nlOpt & '=' & setExp & nl).Named("setDecl");

			//  Terminal Definition

			var regExp2 = new SequenceParser();

			var kleeneOpt = (~((Parser)'+' | '?' | '*')).Named("kleene");

			regExpItem = ((setLiteral & kleeneOpt)
				| (setName & kleeneOpt)
				| (terminal.Named("terminal") & kleeneOpt)
				| ('(' & regExp2.Named("regExp2") & ')' & kleeneOpt)).Named("regExpItem");

			var regExpSeq = (+regExpItem).Named("regExpSeq");

			regExp2.Items.Add(regExpSeq);
			regExp2.Items.Add(-('|' & regExpSeq));

			regExp = (regExpSeq & -(nlOpt & '|' & regExpSeq)).Named("regExp");

			var terminalName = terminal & -(terminal);

			terminalDecl = (terminalName.Named("name") & nlOpt & '=' & regExp & nl).Named("terminalDecl");

			// Rule Definition
			symbol = (terminal.Named("terminal") | nonterminal.Named("nonterminal")).Named("symbol");

			handle = (-symbol).Named("handle");
			var handles = handle & -(nlOpt & '|' & handle);
			ruleDecl = (nonterminal.Named("name") & nlOpt & "::=" & handles & nl).Named("ruleDecl");

			// Rules

			var definitionDecl = parameter | setDecl | terminalDecl | ruleDecl;

			var content = -definitionDecl;

			this.Items.Add(nlOpt);
			this.Items.Add(content);
			this.Items.Add(nlOpt);
			this.Items.Add(Terminals.End);

			Parser.DefaultSeparator = oldSeparator;
			AttachEvents();
		}

		void AttachEvents()
		{
			// attach logic to parsers
			parameter.Matched += m => {
				var name = m["name"]["value"].Value;
				definition.Properties[name] = m["body"].Value;
			};

			ruleDecl.Matched += m => {
				var name = m["name"]["value"].Value;
				var parser = Alternative(m, "handle", r => Sequence(r, "symbol", cm => Symbol(cm)));
				definition.Rules[name].Inner = parser;
			};
			ruleDecl.PreMatch += m => {
				var name = m["name"]["value"].Value;
				var parser = new NamedParser(name);
				definition.Rules.Add(parser.Id, parser);
			};

			terminalDecl.Matched += m => {
				var parser = m.Context as NamedParser;
				parser.Inner = Sequence(m, "regExp", r => RegExp(r));
			};

			terminalDecl.PreMatch += m => {
				var name = m["name"].Value;
				var parser = new NamedParser(name);
				definition.Terminals[parser.Id] = parser;
				m.Context = parser;
			};

			setDecl.Matched += m => {
				var parser = m.Context as CharParser;
				parser.Tester = SetMatch(m["setExp"]).Tester;
			};

			setDecl.PreMatch += m => {
				var parser = new CharParser();
				definition.Sets[m["setName"]["value"].Value] = parser;
				m.Context = parser;
			};
		}

		Parser Alternative(NamedMatch m, string innerName, Func<NamedMatch, Parser> inner)
		{
			var parsers = m.Find(innerName).Select(r => inner(r));
			if (parsers.Count() > 1)
				return new AlternativeParser(parsers);
			else
				return parsers.FirstOrDefault();
		}

		Parser Sequence(NamedMatch m, string innerName, Func<NamedMatch, Parser> inner)
		{
			var parsers = m.Find(innerName).Select(r => inner(r));
			if (parsers.Count() > 1)
				return new SequenceParser(parsers) { Separator = definition.Terminals["Whitespace"] };
			else
				return parsers.FirstOrDefault();
		}

		Parser Symbol(NamedMatch m)
		{
			var child = m["nonterminal"];
			if (child.Success)
			{
				var name = child["value"].Value;
				NamedParser parser;
				if (!definition.Rules.TryGetValue(name, out parser))
					throw new FormatException(string.Format("Nonterminal '{0}' not found", name));

				return parser;
			}
			child = m["terminal"];
			if (child)
				return Terminal(child);
			throw new FormatException("Invalid symbol");
		}

		Parser Terminal(NamedMatch m)
		{
			if (!m.Success)
				return null;
			var l = m["literal"];
			if (l.Success)
				return new StringParser(l.Value);

			var t = m["terminal"];
			if (t.Success)
				return definition.Terminals[t.Value];

			throw new FormatException("Invalid terminal");
		}

		Parser RegExp(NamedMatch m)
		{
			return Alternative(m, "regExpSeq", r => Sequence(r, "regExpItem", cm => RegExpItem(cm)));
		}

		Parser RegExpItem(NamedMatch m)
		{
			if (!m.Success)
				return null;
			return RegExp(m["regExp2"]) ?? SetLiteralOrName(m, false) ?? Terminal(m["terminal"]);
		}

		CharParser SetLiteralOrName(NamedMatch m, bool error = true)
		{
			var literal = m["setLiteral"];
			if (literal.Success)
				return Terminals.Set(literal.Find("ch").Select(r => r.Value.Length > 0 ? r.Value[0] : '\'').ToArray());
			var name = m["setName"]["value"];
			if (name.Success)
			{
				CharParser parser;
				if (definition.Sets.TryGetValue(name.Value, out parser))
					return parser;
			}
			if (error)
				throw new FormatException("Literal or set name missing");
			return null;
		}

		CharParser SetMatch(NamedMatch m)
		{
			var addMatch = m["add"];
			if (addMatch)
				return SetLiteralOrName(addMatch) + SetMatch(addMatch["setExp"]);
			var subMatch = m["sub"];
			if (subMatch)
				return SetLiteralOrName(subMatch) - SetMatch(subMatch["setExp"]);
			return SetLiteralOrName(m);
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			definition = new GoldDefinition();
			return base.InnerParse(args);
		}

		public new GoldDefinition Build(string grammar)
		{
			var match = Match(grammar);
			if (!match.Success)
				throw new FormatException(string.Format("Error parsing gold grammar: {0}", match.Error));
			string val;
			if (definition.Properties.TryGetValue("Start Symbol", out val))
			{
				var name = val.TrimStart('<').TrimEnd('>');
				definition.StartSymbol = definition.Rules[name];
			}
			return definition;
		}

		public string ToCode(string grammar, bool includeWrapperClass = true)
		{
			using (var writer = new StringWriter())
			{
				ToCode(grammar, writer);
				return writer.ToString();
			}
		}

		public void ToCode(string grammar, TextWriter writer, bool includeWrapperClass = true)
		{
			var definition = Build(grammar);
			var iw = new IndentedTextWriter(writer, "    ");

			if (includeWrapperClass)
			{
				iw.WriteLine("public static class GeneratedParser");
				iw.WriteLine("{");
				iw.Indent ++;
			}
			iw.WriteLine("/* Date Created: {0}, Source:", DateTime.Now);
			iw.Indent ++;
			foreach (var line in grammar.Split('\n'))
				iw.WriteLine(line);
			iw.Indent --;
			iw.WriteLine("*/");
			if (includeWrapperClass)
			{
				iw.WriteLine("public static Eto.Parse.Parser GetParser()");
				iw.WriteLine("{");
				iw.Indent ++;
			}
			var parserWriter = new CodeParserWriter();
			parserWriter.Write(definition.StartSymbol, iw);
			if (includeWrapperClass)
			{
				iw.WriteLine("return {0};", Writers.Code.NamedWriter.GetIdentifier(definition.StartSymbol.Id));
				iw.Indent --;
				iw.WriteLine("}");
				iw.Indent --;
				iw.WriteLine("}");
			}
		}

	}
}

