using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class EndParser : Parser
	{
		protected EndParser(EndParser other)
			: base(other)
		{
		}

		public EndParser()
		{
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (args.Scanner.IsEnd)
				return args.EmptyMatch;
			else
				return null;
		}

		public override IEnumerable<NonTerminalParser> Find(string parserId)
		{
			yield break;
		}

		public override Parser Clone()
		{
			return new EndParser(this);
		}
	}
}
