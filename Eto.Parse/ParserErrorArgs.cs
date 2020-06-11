using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eto.Parse
{
	public class ParserErrorArgs : ParserChain
	{
		public bool Detailed { get; private set; }

		public ParserErrorArgs(bool detailed)
		{
			Detailed = detailed;
		}
	}
	
}
