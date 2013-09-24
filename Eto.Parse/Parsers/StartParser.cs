using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class StartParser : Parser
	{
		protected StartParser(StartParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
		}

		public StartParser()
		{
		}

		protected override int InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			if (pos <= 0)
				return 0;
			else
				return -1;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new StartParser(this, chain);
		}
	}
}
