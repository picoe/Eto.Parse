using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class EndParser : Parser
	{
		protected EndParser(EndParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
		}

		public EndParser()
		{
		}

		protected override int InnerParse(ParseArgs args)
		{
			if (args.Scanner.IsEof)
				return 0;
			else
				return -1;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new EndParser(this, chain);
		}
	}
}
