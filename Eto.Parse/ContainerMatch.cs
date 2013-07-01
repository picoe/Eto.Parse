using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ContainerMatch : ParseMatch
	{
		public NamedMatchCollection Matches { get; private set; }

		public ParseMatch Error { get; set; }

		public ContainerMatch(Scanner scanner, long offset, int length, NamedMatchCollection matches = null)
			: base(scanner, offset, length)
		{
			this.Matches = matches ?? new NamedMatchCollection();
		}

		public virtual IEnumerable<NamedMatch> Find(string id, bool deep = false)
		{
			return Matches.Find(id, deep);
		}

		public virtual NamedMatch this[string id]
		{
			get { return Matches[id]; }
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
