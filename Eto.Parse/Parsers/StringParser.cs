using System;

namespace Eto.Parse.Parsers
{
	public class StringParser : Parser
	{
		public char[] QuoteCharacters { get; set; }

		public bool AllowEscapeCharacters { get; set; }

		public bool AllowDoubleQuote { get; set; }


		protected StringParser(StringParser other)
		{
			this.QuoteCharacters = other.QuoteCharacters != null ? (char[])other.QuoteCharacters.Clone() : null;
			this.AllowDoubleQuote = other.AllowDoubleQuote;
			this.AllowEscapeCharacters = other.AllowEscapeCharacters;
		}

		public StringParser()
		{
			QuoteCharacters = "\"\'".ToCharArray();
			AllowDoubleQuote = true;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			if (this.QuoteCharacters != null)
			{
				return args.EmptyMatch;

			}
			else
			{
				int length = 0;
				int pos;
				char ch;
				while (scanner.ReadChar(out ch, out pos) && !char.IsWhiteSpace(ch))
				{
					length++;
				}

				if (length > 0)
					return new ParseMatch(pos, length);
				else {
					scanner.Position = pos;
					return args.NoMatch;
				}
			}
		}

		public override Parser Clone()
		{
			return new StringParser(this);
		}
	}
}

