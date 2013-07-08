using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class StartParser : Parser
	{
		protected StartParser(StartParser other)
			: base(other)
		{
		}

		public StartParser()
		{
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			if (pos <= 0)
				return new ParseMatch(pos, 0);
			else
				return args.NoMatch;
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}

		public override Parser Clone()
		{
			return new StartParser(this);
		}
	}
}
