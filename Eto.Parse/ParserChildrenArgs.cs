using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParserChildrenArgs : ParserChain
	{
		Dictionary<Parser, List<Parser>> children = new Dictionary<Parser, List<Parser>>();
		public Dictionary<Parser, List<Parser>> Children { get { return children; } }
	}
}
