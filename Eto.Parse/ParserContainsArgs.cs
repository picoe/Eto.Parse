using System;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Scanners;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse
{
	public class ParserContainsArgs : ParserChain
	{
		public Parser Parser { get; private set; }

		public ParserContainsArgs(Parser parser)
		{
			this.Parser = parser;
		}
	}
	
}
