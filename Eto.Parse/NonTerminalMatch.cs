using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class NonTerminalMatch : ContainerMatch
	{
		public IScanner Scanner { get; private set; }
		public string Value
		{
			get { return Success ? Scanner.SubString(Index, Length) : null; }
		}

		public NonTerminalParser Parser { get; private set; }

		public object Context { get; set; }

		public NonTerminalMatch(NonTerminalParser parser, IScanner scanner)
			: base( -1, -1)
		{
			this.Parser = parser;
			this.Scanner = scanner;
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

		public override string ToString()
		{
			return Value ?? string.Empty;
		}
	}

	public class NonTerminalMatchCollection : List<NonTerminalMatch>
	{
		public virtual IEnumerable<NonTerminalMatch> Find(string id, bool deep = false)
		{
			var matches = this.Where(r => r.Parser.Id == id);
			if (deep && !matches.Any())
				return matches.Concat(this.SelectMany(r => r.Find(id, deep)));
			else
				return matches;
		}

		public virtual NonTerminalMatch this[string id, bool deep = false]
		{
			get { return Find(id, deep).FirstOrDefault() ?? new NonTerminalMatch(null, null); }
		}
	}
}
