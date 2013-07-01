using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class NamedParser : UnaryParser
	{
		public string Id { get; set; }

		public NamedParser()
		{
		}

		public NamedParser(string id)
		{
			this.Id = id;
		}

		public NamedParser(string id, Parser parser)
			: base(parser)
		{
			this.Id = id;
		}

		public event Action<NamedMatch> Matched;

		protected void OnMatched(NamedMatch match)
		{
			if (Matched != null)
				Matched(match);
		}

		public event Action<NamedMatch> PreMatch;

		protected void OnPreMatch(NamedMatch match)
		{
			if (PreMatch != null)
				PreMatch(match);
		}

		internal void TriggerMatch(NamedMatch match)
		{
			OnMatched(match);
		}

		internal void TriggerPreMatch(NamedMatch match)
		{
			OnPreMatch(match);
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			if (this.Id == parserId)
				yield return this;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var namedMatch = new NamedMatch(this, args.Scanner);
			if (!args.Push(this, namedMatch.Matches))
				return null;
			var match = Inner.Parse(args);
			args.Pop();
			if (match != null)
			{
				namedMatch.Set(match);
				args.Matches.Add(namedMatch);
			}
			return match;
		}
	}
}
