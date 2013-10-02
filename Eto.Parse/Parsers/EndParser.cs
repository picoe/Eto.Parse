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
			return !args.Scanner.IsEof ? -1 : 0;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new EndParser(this, args);
		}
	}
}
