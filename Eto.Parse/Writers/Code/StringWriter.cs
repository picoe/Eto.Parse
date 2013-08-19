using System;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Writers.Code
{
	public class StringWriter : ParserWriter<StringParser>
	{
		public override void WriteContents(TextParserWriterArgs args, StringParser parser, string name)
		{
			base.WriteContents(args, parser, name);

			string quoteChars = null;
			if (parser.QuoteCharacters == null)
				quoteChars = "null";
			else if (parser.QuoteCharacters.Length != 2 || parser.QuoteCharacters[0] != '\"' || parser.QuoteCharacters[0] != '\'')
				quoteChars = string.Format("new char[] {{ {0} }}", string.Join(", ", parser.QuoteCharacters.Select(r => string.Format("(char)0x{0:x}", (int)r))));
			if (quoteChars != null)
				args.Output.WriteLine("{0}.QuoteCharacters = {1};", name, quoteChars);

			if (parser.AllowEscapeCharacters)
				args.Output.WriteLine("{0}.AllowEscapeCharacters = {1};", name, parser.AllowEscapeCharacters.ToString().ToLower());

			if (parser.AllowDoubleQuote)
				args.Output.WriteLine("{0}.AllowDoubleQuote = {1};", name, parser.AllowDoubleQuote.ToString().ToLower());

			if (parser.AllowNonQuoted)
				args.Output.WriteLine("{0}.AllowNonQuoted = {1};", name, parser.AllowNonQuoted.ToString().ToLower());

			if (parser.NonQuotedLetter == null || parser.NonQuotedLetter is CharTerminal)
				args.Output.WriteLine("{0}.NonQuotedLetter = {1};", name, args.Write(parser.NonQuotedLetter));
		}
	}
}

