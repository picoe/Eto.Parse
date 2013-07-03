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
					return null;
				var parser = Items[i];
				var match = parser != null ? parser.Parse(args) : args.EmptyMatch;
				if (match != null)
				{
					args.Pop(true);
					return match;
				}
				args.Pop(false);
			}
			return null;
		}

		public override Parser Clone()
		{
			return new AlternativeParser(this);
		}
	}
}
