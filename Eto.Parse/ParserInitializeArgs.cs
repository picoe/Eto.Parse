using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParserInitializeArgs : ParserChain
	{
		List<Parser> recursionFixes;

		public Grammar Grammar { get; private set; }

		public List<Parser> RecursionFixes
		{
			get { return recursionFixes ?? (recursionFixes = new List<Parser>()); }
		}

		public ParserInitializeArgs(Grammar grammar)
		{
			this.Grammar = grammar;
		}
	}
}

