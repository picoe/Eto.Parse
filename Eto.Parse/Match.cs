using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class Match
	{
		MatchCollection matches;
		static Match empty;

		public static Match EmptyMatch
		{
			get { return empty ?? (empty = new Match(null, null, -1, -1, new MatchCollection())); }
		}

		public MatchCollection Matches
		{
			get { return matches ?? (matches = new MatchCollection()); }
		}

		public IEnumerable<Match> Find(string id, bool deep = false)
		{
			if (matches != null)
				return matches.Find(id, deep);
			else
				return Enumerable.Empty<Match>();
		}

		public Match this [string id, bool deep = false]
		{
			get
			{
				if (matches != null)
					return matches[id, deep];
				else
					return Match.EmptyMatch;
			}
		}

		public Scanner Scanner { get; private set; }

		public string StringValue { get { return GetValue<string>(); } }
		public int Int32Value { get { return GetValue<int>(); } }
		public Int64 Int64Value { get { return GetValue<Int64>(); } }
		public double DoubleValue { get { return GetValue<double>(); } }
		public decimal DecimalValue { get { return GetValue<decimal>(); } }

		public T GetValue<T>()
		{
			return Success ? Parser.GetValue<T>(this) : default(T);
		}

		public string Text { get { return Success ? Scanner.SubString(Index, Length) : null; } }

		public string Name { get { return this.Parser != null ? this.Parser.Name : null; } }

		public Parser Parser { get; private set; }

		public object Tag { get; set; }

		internal Match(Parser parser, Scanner scanner, int index, int length, MatchCollection matches)
		{
			this.Parser = parser;
			this.Scanner = scanner;
			this.Index = index;
			this.Length = length;
			this.matches = matches;
		}

		internal void TriggerPreMatch()
		{
			if (matches != null)
				matches.ForEach(r => r.TriggerPreMatch());
			Parser.TriggerPreMatch(this);
		}

		internal void TriggerMatch()
		{
			if (matches != null)
				matches.ForEach(r => r.TriggerMatch());
			Parser.TriggerMatch(this);
		}

		public override string ToString()
		{
			return Text ?? string.Empty;
		}

		public int Index { get; private set; }

		public int Length { get; private set; }

		public bool Success { get { return Length >= 0; } }

		public bool Empty { get { return Length == 0; } }

		public static bool operator true(Match match)
		{
			return match.Success;
		}

		public static bool operator false(Match match)
		{
			return !match.Success;
		}
	}

	public class MatchCollection : List<Match>
	{
		public MatchCollection()
			: base(4)
		{
		}

		public IEnumerable<Match> Find(string id, bool deep = false)
		{
			var matches = this.Where(r => r.Name == id);
			if (deep && !matches.Any())
				return matches.Concat(this.SelectMany(r => r.Find(id, deep)));
			else
				return matches;
		}

		public Match this [string id, bool deep = false]
		{
			get { return Find(id, deep).FirstOrDefault() ?? Match.EmptyMatch; }
		}
	}
}
