using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParserCloneArgs
	{
		readonly Dictionary<Parser, Parser> clones = new Dictionary<Parser, Parser>();

		public Parser Clone(Parser parser)
		{
			if (parser == null)
				return null;
			Parser newParser;
			if (!clones.TryGetValue(parser, out newParser))
			{
				newParser = parser.Clone(this);
				clones[parser] = newParser;
			}
			return newParser;
		}

		public void Add(Parser parser, Parser newParser)
		{
			clones.Add(parser, newParser);
		}

		internal ParserCloneArgs()
		{
		}
	}
	
}
