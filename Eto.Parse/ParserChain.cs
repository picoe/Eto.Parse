using System;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Scanners;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse
{
	public class ParserChain
	{
		List<Parser> parents;

		public IEnumerable<Parser> Parents { get { return parents; } }

		public bool Push(Parser parser)
		{
			if (!parents.Contains(parser))
			{
				parents.Add(parser);
				return true;
			}
			return false;
		}

		public void Pop(Parser parser)
		{
			parents.Remove(parser);
		}

		internal ParserChain()
		{
			parents = new List<Parser>();
		}
	}
	
}
