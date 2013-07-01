using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class SequenceParser : ListParser
	{
		protected SequenceParser(SequenceParser other)
			: base(other)
		{
		}

		public SequenceParser()
		{
		}

		public SequenceParser(IEnumerable<Parser> sequence)
			: base(sequence)
		{
		}

		public SequenceParser(params Parser[] sequence)
			: base(sequence)
		{
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (!args.Push(this))
				return args.NoMatch;
			if (Items.Count == 0)
				throw new InvalidOperationException("There are no items in this sequence");
			ParseMatch match = null;
			for (int i = 0; i < Items.Count; i++)
			{
				var parser = Items[i];
				var childMatch = parser.Parse(args);
				if (!childMatch.Success)
				{
					args.Pop(false);
					return args.NoMatch;
				}
				
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
