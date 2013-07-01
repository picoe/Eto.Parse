using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class NamedMatch : ContainerMatch
	{
		public NamedParser Parser { get; private set; }

		public object Context { get; set; }

		public NamedMatch(NamedParser parser, Scanner scanner)
			: base(scanner, -1, -1)
		{
			this.Parser = parser;
		}

		public override void PreMatch()
		{
			base.PreMatch();
			Parser.TriggerPreMatch(this);
		}

		public override void Match()
		{
			base.Match();
			Parser.TriggerMatch(this);
		}

		internal void Set(ParseMatch inner)
		{
			this.Index = inner.Index;
			this.Length = inner.Length;
		}
	}

	public class NamedMatchCollection : List<NamedMatch>
	{
		public virtual IEnumerable<NamedMatch> Find(string id, bool deep = false)
		{
			var matches = this.Where(r => r.Parser.Id == id);
			if (deep | !matches.Any())
				return matches.Concat(this.SelectMany(r => r.Find(id)));
			else
				return matches;
		}

		public virtual NamedMatch this[string id]
		{
			get { return Find(id).FirstOrDefault() ?? new NamedMatch(null, null); }
		}
	}
}
