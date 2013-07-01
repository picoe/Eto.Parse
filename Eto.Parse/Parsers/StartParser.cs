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
			if (args.Scanner.Offset == -1)
				return args.EmptyMatch;
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
