using System;
using System.Diagnostics;

namespace Eto.Parse
{
	
	public class ParserReplaceArgs : ParserChain
	{
		public Parser SearchParser { get; private set; }
		public Parser ReplaceParser { get; private set; }

		public Parser Replace(Parser parser)
		{
			if (parser == null)
				return null;
			if (ReferenceEquals(parser, SearchParser))
			{
				Debug.WriteLine("Replacing {0} with {1}", SearchParser, ReplaceParser);
				return ReplaceParser;
			}
			parser.Replace(this);
			return parser;
		}

		public ParserReplaceArgs(Parser searchParser, Parser replaceParser)
		{
			this.SearchParser = searchParser;
			this.ReplaceParser = replaceParser;
		}
	}
}
