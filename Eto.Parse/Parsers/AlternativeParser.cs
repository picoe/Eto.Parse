using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class AlternativeParser : ListParser
	{
		protected AlternativeParser(AlternativeParser other)
			: base(other)
		{
		}

		public AlternativeParser()
		{
		}

		public AlternativeParser(IEnumerable<Parser> sequence)
				: base(sequence)
		{
		}

		public AlternativeParser(params Parser[] sequence)
				: base(sequence)
		{
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (!args.Push(this))
					return args.NoMatch;
				var parser = Items[i];
				var match = parser.Parse(args);
				if (match.Success)
				{
					args.Pop(true);
					return match;
				}
				args.Pop(false);
			}
			return args.NoMatch;
		}

		public override Parser Clone()
		{
			return new AlternativeParser(this);
		}
	}
}
