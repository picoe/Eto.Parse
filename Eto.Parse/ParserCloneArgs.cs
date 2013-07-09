using System;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Scanners;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse
{

	public class ParserCloneArgs
	{
		Dictionary<Parser, Parser> clones;

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
			clones = new Dictionary<Parser, Parser>();
		}
	}
	
}
