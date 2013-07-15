using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class NamedMatch
	{
		NamedMatchCollection matches;
		static NamedMatch empty;

		public static NamedMatch EmptyMatch
		{
			get { return empty ?? (empty = new NamedMatch(null, null, -1, -1, new NamedMatchCollection())); }
		}

		public NamedMatchCollection Matches
		{
			get { return matches ?? (matches = new NamedMatchCollection()); }
		}

		public IEnumerable<NamedMatch> Find(string id, bool deep = false)
		{
			if (matches != null)
				return matches.Find(id, deep);
			else
				return Enumerable.Empty<NamedMatch>();
		}

		public NamedMatch this [string id, bool deep = false]
		{
			get
			{
				if (matches != null)
					return matches[id, deep];
				else
					return NamedMatch.EmptyMatch;
			}
		}

		public Scanner Scanner { get; private set; }

		public string Value
		{
			get { return Success ? Scanner.SubString(Index, Length) : null; }
		}

		public string Name { get; private set; }

		public NamedParser Parser { get; private set; }

		public object Tag { get; set; }

		public NamedMatch(NamedParser parser, Scanner scanner, int index, int length, NamedMatchCollection matches)
		{
			this.Parser = parser;
			if (parser != null)
				this.Name = parser.Name;
			this.Scanner = scanner;
			this.Index = index;
			this.Length = length;
			this.matches = matches;
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
		public IEnumerable<NamedMatch> Find(string id, bool deep = false)
		{
			var matches = this.Where(r => r.Name == id);
			if (deep && !matches.Any())
				return matches.Concat(this.SelectMany(r => r.Find(id, deep)));
			else
				return matches;
		}

		public NamedMatch this [string id, bool deep = false]
		{
			get { return Find(id, deep).FirstOrDefault() ?? NamedMatch.EmptyMatch; }
		}
	}
}
