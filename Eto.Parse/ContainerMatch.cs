using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ContainerMatch : ParseMatch
	{
		public NonTerminalMatchCollection Matches { get; private set; }

		public ParseError Error { get; set; }

		public ContainerMatch(long offset, int length, NonTerminalMatchCollection matches = null)
			: base(offset, length)
		{
			this.Matches = matches ?? new NonTerminalMatchCollection();
		}

		public virtual IEnumerable<NonTerminalMatch> Find(string id, bool deep = false)
		{
			return Matches.Find(id, deep);
		}

		public virtual NonTerminalMatch this[string id, bool deep = false]
		{
			get { return Matches[id, deep]; }
		}

		public virtual void PreMatch()
		{
			Matches.ForEach(r => r.PreMatch());
		}

		public virtual void Match()
		{
			Matches.ForEach(r => r.Match());
		}
	}
	
}
