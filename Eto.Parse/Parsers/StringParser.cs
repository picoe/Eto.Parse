using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class StringParser : Parser
	{
		public char[] QuoteCharacters { get; set; }

		public bool AllowEscapeCharacters { get; set; }

		public bool AllowDoubleQuote { get; set; }


		protected StringParser(StringParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			this.QuoteCharacters = other.QuoteCharacters != null ? (char[])other.QuoteCharacters.Clone() : null;
			this.AllowDoubleQuote = other.AllowDoubleQuote;
			this.AllowEscapeCharacters = other.AllowEscapeCharacters;
		}

		public StringParser()
		{
			QuoteCharacters = "\"\'".ToCharArray();
			AddError = true;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			if (this.QuoteCharacters != null)
			{
				var pos = scanner.Position;

				char ch;
				if (scanner.ReadChar(out ch) && QuoteCharacters.Contains(ch))
				{

				}

				scanner.SetPosition(pos);
				return args.EmptyMatch;

			}
			else
			{
				int length = 0;
				int pos = scanner.Position;
				char ch;
				while (scanner.ReadChar(out ch) && !char.IsWhiteSpace(ch))
				{
					length++;
				}

				if (length > 0)
					return new ParseMatch(pos, length);
				else {
					scanner.SetPosition(pos);
					return args.NoMatch;
				}
			}
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new StringParser(this, chain);
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}
	}
}

