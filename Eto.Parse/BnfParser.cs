using System;
using Eto.Parse.Parsers;
using Eto.Parse.Scanners;
using System.Collections.Generic;
using System.Linq;
using Eto.Parse.Writers;
using System.IO;
using System.CodeDom.Compiler;

namespace Eto.Parse
{
	public class BnfParser : SequenceParser
	{
		Dictionary<string, Parser> parserLookup = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, Parser> baseLookup = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase);
		Parser sws = Terminals.SingleLineWhiteSpace.Repeat(0);
		Parser ws = Terminals.WhiteSpace.Repeat(0);
		Parser sq = Terminals.Set('\'');
		Parser dq = Terminals.Set('"');

		public BnfParser()
		{
			foreach (var property in typeof(Terminals).GetProperties())
			{
				if (typeof(Parser).IsAssignableFrom(property.PropertyType))
				{
					var parser = property.GetValue(null, null) as Parser;
					baseLookup[property.Name] = parser;
				}
			}

			var lineEnd = LineEnd();

			var literal = (
				(sq & (+!sq).Named("value").Optional() & sq)
				| (dq & (+!dq).Named("value").Optional() & dq)
			).Named("parser", m => m.Context = new StringParser(m["value"].Value));

			Parser ruleName = "<" & Terminals.Set('>').Inverse().Repeat().Named("name") & ">";

			var expression = new AlternativeParser(); // defined later 
			var term = new AlternativeParser();
			term.Items.Add(literal);
			term.Items.Add(ruleName.Named("parser", m => {
				Parser parser;
				var name = m["name"].Value;
				if (!parserLookup.TryGetValue(name, out parser) && !baseLookup.TryGetValue(name, out parser))
					parser = Terminals.LetterOrDigit.Repeat().Named(name);
				m.Context = parser;
			}));
			term.Items.Add('(' & sws & expression & sws & ')');
			term.Items.Add(('{' & sws & expression & sws & '}').Named("parser", m => {
				m.Context = new RepeatParser((Parser)m["parser"].Context);
			}));
			term.Items.Add(('[' & sws & expression & sws & ']').Named("parser", m => {
				m.Context = new OptionalParser((Parser)m["parser"].Context);
			}));


			var list = new SequenceParser(term.Named("term"), -(sws.Named("ws") & term.Named("term")))
			.Named("parser", m => {
				if (m.Matches.Count > 1)
				{
					var parser = new SequenceParser();
					foreach (var child in m.Matches)
					{
						if (child.Parser.Id == "ws")
							parser.Items.Add(sws);
						else if (child.Parser.Id == "term")
							parser.Items.Add((Parser)child["parser"].Context);
					}
					m.Context = parser;
				}
				else
				{
					m.Context = m["parser"].Context;
				}
			});

			expression.Items.Add((list.Named("list") & ws & '|' & sws & expression.Named("expression"))
			                     .Named("parser", m => {
				// collapse alternatives to one alternative parser
				var parser = (Parser)m["expression"]["parser"].Context;
				var alt = parser as AlternativeParser ?? new AlternativeParser(parser);
				alt.Items.Insert(0, (Parser)m["list"]["parser"].Context);
				m.Context = alt;
			}));
			expression.Items.Add(list);

			var rule = (~lineEnd & sws & ruleName.Named("ruleName") & ws & "::=" & sws & expression & lineEnd)
					.Named("parser", 
			         matched: m => {
				var parser = (NamedParser)m.Context;
				parser.Inner = (Parser)m["parser"].Context;
				m.Context = parser;
			},
			         preMatch: m => {
				var parser = new NamedParser(m["ruleName"]["name"].Value);
				m.Context = parser;
				parserLookup[parser.Id] = parser;
			});

			this.Items.Add((rule & this) | rule);
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
			parserLookup = new Dictionary<string, Parser>();
			return base.InnerParse(args);
		}

		public Dictionary<string, Parser> Build(string bnf)
		{
			var match = this.Match(new StringScanner(bnf));

			if (match.Error != null)
			{
				throw new FormatException(string.Format("Error parsing bnf starting at: \n{0}", bnf.Substring((int)match.Error.Offset)));
			}
			return parserLookup;
		}

		public Parser Build(string bnf, string startParserName)
		{
			Parser parser;
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
			var parserWriter = new CodeParserWriter();
			parserWriter.Write(parser, iw);
			if (includeWrapperClass)
			{
				iw.WriteLine("return {0};", Writers.Code.NamedWriter.GetIdentifier(startParserName));
				iw.Indent --;
				iw.WriteLine("}");
				iw.Indent --;
				iw.WriteLine("}");
			}
		}
	}
}

