using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class Match
	{
		MatchCollection matches;
		string name;
		readonly int index;
		readonly int length;
		readonly Parser parser;
		readonly Scanner scanner;
		internal static readonly Match EmptyMatch = new Match(null, null, null, -1, -1, new MatchCollection());

		public MatchCollection Matches
		{
			get { return matches ?? (matches = new MatchCollection()); }
		}

		public bool HasMatches
		{
			get { return matches != null && matches.Count > 0; }
		}

		public Scanner Scanner { get { return scanner; } }

		public object Value { get { return Success ? parser.GetValue(this) : null; } }

		public string StringValue { get { return Convert.ToString(Value); } }

		public string Text { get { return Success ? scanner.Substring(index, length) : null; } }

		public string Name { get { return name ?? (name = Parser.Name); } }

		public Parser Parser { get { return parser; } }

		public object Tag { get; set; }

		public int Index { get { return index; } }

		public int Length { get { return length; } }

		public bool Success { get { return length >= 0; } }

		public bool Empty { get { return length == 0; } }

		internal Match(string name, Parser parser, Scanner scanner, int index, int length, MatchCollection matches)
		{
			this.name = name;
			this.parser = parser;
			this.scanner = scanner;
			this.index = index;
			this.length = length;
			this.matches = matches;
		}

		internal Match(Parser parser, Scanner scanner, int index, int length, MatchCollection matches)
		{
			this.parser = parser;
			this.scanner = scanner;
			this.index = index;
			this.length = length;
			this.matches = matches;
		}

		internal Match(Parser parser, Scanner scanner, int index, int length)
		{
			this.parser = parser;
			this.scanner = scanner;
			this.index = index;
			this.length = length;
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

		public MatchCollection(IEnumerable<Match> collection)
			: base(collection)
		{
		}

		public IEnumerable<Match> Find(string id, bool deep = false)
		{
			bool found = false;
			for (int i = 0; i < Count; i++)
			{
				var item = this[i];
				if (item.Name == id)
				{
					yield return item;
					found = true;
				}
			}
			if (deep && !found)
			{
				for (int i = 0; i < Count; i++)
				{
					var item = this[i];
					foreach (var child in item.Find(id, deep))
					{
						yield return child;
					}
				}
			}
		}

		public Match this [string id, bool deep = false]
		{
			get { return Find(id, deep).FirstOrDefault() ?? Match.EmptyMatch; }
		}
	}
}
