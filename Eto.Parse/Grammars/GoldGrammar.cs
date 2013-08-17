using System;
using System.Linq;
using System.Collections.Generic;
using Eto.Parse.Parsers;
using System.IO;
using System.CodeDom.Compiler;
using Eto.Parse.Writers;

namespace Eto.Parse.Grammars
{
	public class GoldGrammar : Grammar
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

		public GoldGrammar()
			: base("gold")
		{
			var oldSeparator = Parser.DefaultSeparator;
			// Special Terminals

			var parameterCh = Terminals.Printable - Terminals.Set("\"'");
			var nonterminalCh = Terminals.LetterOrDigit | Terminals.Set("_-. ");
			var terminalCh = Terminals.LetterOrDigit | Terminals.Set("_-.");
			var literalCh = Terminals.Printable - Terminals.Set('\'');
			var setLiteralCh = Terminals.Printable - Terminals.Set("[]'");
			var setNameCh = Terminals.Printable - Terminals.Set("{}");

			var parameterName = ('"' & (+parameterCh).Named("value") & '"').Separate();
			var nonterminal = ('<' & (+nonterminalCh).Named("value") & '>').Separate();
			var terminal = ((+terminalCh).Named("terminal") | ('\'' & (-literalCh).Named("literal") & '\'')).Separate();
			var setLiteral = ('[' & +(setLiteralCh.Named("ch") | '\'' & (-literalCh).Named("ch") & '\'') & ']').Named("setLiteral");
			var setName = ('{' & (+setNameCh).Named("value") & '}').Named("setName");

			// Line-Based Grammar Declarations

			var comments = new GroupParser("!*", "*!", "!");
			whitespace = -(Terminals.SingleLineWhiteSpace | comments);
			Parser.DefaultSeparator = whitespace;
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
			setExp.Inner = (setExp & nlOpt & '+' & setItem).Named("add")
				| (setExp & nlOpt & '-' & setItem).Named("sub")
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

			var terminalName = +terminal;

			terminalDecl = (terminalName.Named("name") & nlOpt & '=' & regExp & nl).Named("terminalDecl");

			// Rule Definition
			symbol = (terminal.Named("terminal") | nonterminal.Named("nonterminal")).Named("symbol");

			handle = (-symbol).Named("handle");
			var handles = handle & -(nlOpt & '|' & handle);
			ruleDecl = (nonterminal.Named("name") & nlOpt & "::=" & handles & nl).Named("ruleDecl");

			// Rules

			var definitionDecl = parameter | setDecl | terminalDecl | ruleDecl;

			var content = -definitionDecl;

			this.Inner = nlOpt & content;

			Parser.DefaultSeparator = oldSeparator;
			AttachEvents();
		}

		void AttachEvents()
		{
			// attach logic to parsers
			parameter.PreMatch += m => {
				var name = m["name"]["value"].Value;
				definition.Properties[name] = m["body"].Value;
			};

			ruleDecl.Matched += m => {
				var name = m["name"]["value"].Value;
				bool addWhitespace = name == definition.GrammarName;
				var parser = Alternative(m, "handle", r => Sequence(r, "symbol", cm => Symbol(cm), addWhitespace));
				definition.Rules[name].Inner = parser;
			};
			ruleDecl.PreMatch += m => {
				var name = m["name"]["value"].Value;
				NamedParser parser;
				if (name == definition.GrammarName)
					parser = new Grammar(name);
				else
					parser = new NamedParser(name);
				definition.Rules.Add(parser.Name, parser);
			};

			terminalDecl.Matched += m => {
				var inner = Sequence(m, "regExp", r => RegExp(r));
				var parser = m.Tag as NamedParser;
				if (parser != null)
					parser.Inner = inner;
				var groupParser = m.Tag as GroupParser;
				var name = m["name"].Value;
				if (groupParser != null)
				{
					if (name.EndsWith(" Start"))
						groupParser.Start = inner;
					else if (name.EndsWith(" End"))
						groupParser.End = inner;
					else if (name.EndsWith(" Line"))
						groupParser.Line = inner;
					var count = name.EndsWith(" Start") ? 6 : name.EndsWith(" Line") ? 5 : name.EndsWith(" End") ? 4 : 0;
					name = name.Substring(0, name.Length - count);
				}

				if (name.Equals("Comment", StringComparison.OrdinalIgnoreCase)
				    || name.Equals("Whitespace", StringComparison.OrdinalIgnoreCase)
				    )
				{
					definition.ClearSeparator();
				}
			};

			terminalDecl.PreMatch += m => {
				var name = m["name"].Value;
				if (name.EndsWith(" Start") || name.EndsWith(" End") || name.EndsWith(" Line"))
				{
					Parser parser;
					var count = name.EndsWith(" Start") ? 6 : name.EndsWith(" Line") ? 5 : name.EndsWith(" End") ? 4 : 0;
					name = name.Substring(0, name.Length - count);
					if (definition.Terminals.TryGetValue(name, out parser))
					{
						parser = parser as GroupParser ?? new GroupParser();
					}
					else
						parser = new GroupParser();
					m.Tag = definition.Terminals[name] = parser;
				}
				else
					m.Tag = definition.Terminals[name] = new NamedParser(name);
			};

			setDecl.PreMatch += m => {
				var parser = SetMatch(m["setExp"]);
				definition.Sets[m["setName"]["value"].Value] = parser;
				m.Tag = parser;
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

		Parser Sequence(NamedMatch m, string innerName, Func<NamedMatch, Parser> inner, bool addWhitespace = false)
		{
			var parsers = m.Find(innerName).Select(r => inner(r));
			if (addWhitespace || parsers.Count() > 1)
			{
				var sep = definition.Separator;
				var seq = new SequenceParser(parsers) { Separator = sep };
				if (addWhitespace)
				{
					seq.Items.Insert(0, sep);
					seq.Items.Add(sep);
				}
				return seq;
			}
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
				return new LiteralTerminal(l.Value.Length > 0 ? l.Value : "'");

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
			var item = RegExp(m["regExp2"]) ?? SetLiteralOrName(m, false) ?? Terminal(m["terminal"]);
			var kleene = m["kleene"];
			switch (kleene.Value)
			{
				case "+":
					return new RepeatParser(item, 1);
				case "*":
					return new RepeatParser(item, 0);
				case "?":
					return new OptionalParser(item);
				default:
					return item;
			}
		}

		Parser SetLiteralOrName(NamedMatch m, bool error = true)
		{
			var literal = m.Name == "setLiteral" ? m : m["setLiteral"];
			if (literal.Success)
				return Terminals.Set(literal.Find("ch").Select(r => r.Value.Length > 0 ? r.Value[0] : '\'').ToArray());
			var name = m.Name == "setName" ? m["value"] : m["setName"]["value"];
			if (name.Success)
			{
				Parser parser;
				if (definition.Sets.TryGetValue(name.Value, out parser))
					return parser;
			}
			if (error)
				throw new FormatException("Literal or set name missing");
			return null;
		}

		Parser SetMatch(NamedMatch m)
		{
			Parser parser = null;
			foreach (var child in m.Matches)
			{
				if (parser == null)
					parser = SetLiteralOrName(child);
				else if (child.Name == "add")
					parser |= SetLiteralOrName(child);
				else if (child.Name == "sub")
					parser -= SetLiteralOrName(child);
			}
			return parser;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			definition = new GoldDefinition();
			return base.InnerParse(args);
		}

		public GoldDefinition Build(string grammar)
		{
			var match = Match(grammar);
			if (!match.Success)
				throw new FormatException(string.Format("Error parsing gold grammar: {0}", match.ErrorMessage));
			return definition;
		}

		public string ToCode(string grammar, string className = "GeneratedGrammar")
		{
			using (var writer = new StringWriter())
			{
				ToCode(grammar, writer, className);
				return writer.ToString();
			}
		}

		public void ToCode(string grammar, TextWriter writer, string className = "GeneratedGrammar")
		{
			var definition = Build(grammar);
			definition.Grammar.Initialize();
			var iw = new IndentedTextWriter(writer, "    ");

			iw.WriteLine("/* Date Created: {0}, Source:", DateTime.Now);
			iw.Indent ++;
			foreach (var line in grammar.Split('\n'))
				iw.WriteLine(line);
			iw.Indent --;
			iw.WriteLine("*/");

			var parserWriter = new CodeParserWriter
			{
				ClassName = className
			};
			parserWriter.Write(definition.Grammar, writer);
		}
	}
}

