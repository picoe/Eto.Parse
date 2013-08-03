using System;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class EmptyParser : Parser
	{
		protected EmptyParser(EmptyParser other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public EmptyParser()
		{
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return args.EmptyMatch;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new EmptyParser(this, args);
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}
	}
}

