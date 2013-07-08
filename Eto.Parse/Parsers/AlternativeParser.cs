using System;
using Eto.Parse;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class AlternativeParser : ListParser
	{
		public static Parser ExcludeNull(params Parser[] parsers)
		{
			return ExcludeNull((IEnumerable<Parser>)parsers);
		}

		public static Parser ExcludeNull(IEnumerable<Parser> parsers)
		{
			var p = parsers.Where(r => r != null).ToArray();
			if (p.Length == 1)
				return p[0];
			else
				return new AlternativeParser(p);
		}

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
			if (args.IsRecursive(this))
				return args.NoMatch;
			for (int i = 0; i < Items.Count; i++)
			{
				args.Push(this);
				var parser = Items[i];
				var match = parser != null ? parser.Parse(args) : args.EmptyMatch;
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
