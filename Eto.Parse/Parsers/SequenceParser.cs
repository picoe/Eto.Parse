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
			if (!args.Push(this))
				return null;
			if (Items.Count == 0)
				throw new InvalidOperationException("There are no items in this sequence");
			ParseMatch match = null;
			for (int i = 0; i < Items.Count; i++)
			{
				ParseMatch sepMatch = null;
				if (i > 0 && Separator != null)
				{
					sepMatch = Separator.Parse(args);
					if (sepMatch == null)
					{
						args.Pop(false);
						return null;
					}
				}

				var parser = Items[i];
				var childMatch = parser.Parse(args);
				if (childMatch == null)
				{
					args.Pop(false);
					return null;
				}
				if (sepMatch != null && !childMatch.Empty)
					match = ParseMatch.Merge(match, sepMatch);

				match = ParseMatch.Merge(match, childMatch);

			}
			args.Pop(true);

			return match;
		}

		public override Parser Clone()
		{
			return new SequenceParser(this);
		}
	}
}
