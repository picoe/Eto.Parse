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
			var matches = Matches.Where(r => r.Parser.Id == id);
			if (deep | !matches.Any())
				return matches.Concat(Matches.SelectMany(r => r.Find(id)));
			else
				return matches;
		}

		public virtual NamedMatch this[string id]
		{
			get { return Find(id).FirstOrDefault() ?? new NamedMatch(null, this.Scanner); }
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
