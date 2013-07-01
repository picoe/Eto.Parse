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
			if (args.Scanner.IsEnd) return args.EmptyMatch;

			long offset = args.Offset;
			
			if (!args.Scanner.Read()) return args.EmptyMatch;
			
			args.Offset = offset;
			return args.NoMatch;
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}

		public override Parser Clone()
		{
			return new EndParser(this);
		}
	}
}
