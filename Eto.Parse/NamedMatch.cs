using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class NamedMatch
	{
		NamedMatchCollection matches;

		public NamedMatchCollection Matches
		{
			get { return matches ?? (matches = new NamedMatchCollection()); }
		}

		public ParseError Error { get; set; }

		public virtual IEnumerable<NamedMatch> Find(string id, bool deep = false)
		{
			if (matches != null)
				return matches.Find(id, deep);
			else
				return Enumerable.Empty<NamedMatch>();
		}

		public virtual NamedMatch this [string id, bool deep = false]
		{
			get
			{
				if (matches != null)
					return matches[id, deep];
				else
					return new NamedMatch(null, null);
			}
		}

		public IScanner Scanner { get; private set; }

		public string Value
		{
			get { return Success ? Scanner.SubString(Index, Length) : null; }
		}

		public NamedParser Parser { get; private set; }

		public object Context { get; set; }

		public NamedMatch(NamedParser parser, IScanner scanner)
		{
			this.Parser = parser;
			this.Scanner = scanner;
			this.Index = -1;
			this.Length = -1;
		}

		public void PreMatch()
		{
			if (matches != null)
				matches.ForEach(r => r.PreMatch());
			Parser.TriggerPreMatch(this);
		}

		public void Match()
		{
			if (matches != null)
				matches.ForEach(r => r.Match());
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

		public int Index { get; private set; }

		public int Length { get; private set; }

		public int End
		{
			get { return (Length > 0) ? Index + Length - 1 : Index; }
		}

		public bool Success
		{
			get { return Length >= 0; }
		}

		public bool Empty
		{
			get { return Length == 0; }
		}

		public static bool operator true(NamedMatch match)
		{
			return match.Success;
		}

		public static bool operator false(NamedMatch match)
		{
			return !match.Success;
		}
	}

	public class NamedMatchCollection : List<NamedMatch>
	{
		public virtual IEnumerable<NamedMatch> Find(string id, bool deep = false)
		{
			var matches = this.Where(r => r.Parser.Name == id);
			if (deep && !matches.Any())
				return matches.Concat(this.SelectMany(r => r.Find(id, deep)));
			else
				return matches;
		}

		public virtual NamedMatch this [string id, bool deep = false]
		{
			get { return Find(id, deep).FirstOrDefault() ?? new NamedMatch(null, null); }
		}
	}
}
