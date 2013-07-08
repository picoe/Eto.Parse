using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class SequenceParser : ListParser
	{
		public Parser Separator { get; set; }

		protected SequenceParser(SequenceParser other)
			: base(other)
		{
			if (other.Separator != null)
				Separator = other.Separator.Clone();
		}

		public SequenceParser()
		{
			Separator = DefaultSeparator;
		}

		public SequenceParser(IEnumerable<Parser> sequence)
			: base(sequence)
		{
			Separator = DefaultSeparator;
		}

		public SequenceParser(params Parser[] sequence)
			: base(sequence)
		{
			Separator = DefaultSeparator;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (Items.Count == 0)
				throw new InvalidOperationException("There are no items in this sequence");
			var pos = args.Scanner.Position;
			var separator = Separator ?? args.Grammar.Separator;
			ParseMatch match = new ParseMatch(pos, 0);
			for (int i = 0; i < Items.Count; i++)
			{
				ParseMatch sepMatch = args.NoMatch;
				if (i > 0 && separator != null)
				{
					sepMatch = separator.Parse(args);
					if (!sepMatch.Success)
					{
						args.Scanner.Position = pos;
						return sepMatch;
					}
				}

				var parser = Items[i];
				var childMatch = parser.Parse(args);
				if (!childMatch.Success)
				{
					args.Scanner.Position = pos;
					return childMatch;
				}
				if (sepMatch.Success && !childMatch.Empty)
					match.Length += sepMatch.Length;

				match.Length += childMatch.Length;

			}

			return match;
		}

		public override Parser Clone()
		{
			return new SequenceParser(this);
		}
	}
}
