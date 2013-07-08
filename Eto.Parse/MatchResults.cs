using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class MatchResults
	{
		public NamedMatch Root { get; private set; }

		public List<ParseErrorMessage> Errors { get; private set; }

		public MatchResults()
		{
		}
	}
}

