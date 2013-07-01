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
			if (!args.Push(this)) return args.NoMatch;
			
			ParseMatch match = null;
			foreach (Parser parser in Items)
			{
				ParseMatch match2 = parser.Parse(args);
				if (!match2.Success) { args.Pop(false); return args.NoMatch; }
				
				match = ParseMatch.Merge(match, match2);
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
