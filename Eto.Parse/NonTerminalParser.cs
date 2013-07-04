using System;
using System.Collections.Generic;
using Eto.Parse.Scanners;

namespace Eto.Parse
{
	public class NonTerminalParser : UnaryParser
	{
		public string Id { get; set; }

		public NonTerminalParser()
		{
		}

		public NonTerminalParser(string id)
		{
			this.Id = id;
		}

		public NonTerminalParser(string id, Parser parser)
			: base(parser)
		{
			this.Id = id;
		}

		protected override string GetDescriptiveNameInternal(HashSet<Parser> parents)
		{
			return string.Format("{0} \"{1}\"", base.GetDescriptiveNameInternal(parents), this.Id);
		}

		public event Action<NonTerminalMatch> Matched;

		protected void OnMatched(NonTerminalMatch match)
		{
			if (Matched != null)
				Matched(match);
		}

		public event Action<NonTerminalMatch> PreMatch;

		protected void OnPreMatch(NonTerminalMatch match)
		{
			if (PreMatch != null)
				PreMatch(match);
		}

		internal void TriggerMatch(NonTerminalMatch match)
		{
			OnMatched(match);
		}

		internal void TriggerPreMatch(NonTerminalMatch match)
		{
			OnPreMatch(match);
		}

		public override IEnumerable<NonTerminalParser> Find(string parserId)
		{
			if (this.Id == parserId)
				yield return this;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var namedMatch = new NonTerminalMatch(this, args.Scanner);
			args.Push(this, namedMatch);
			var match = Inner.Parse(args);
			if (match != null)
			{
				namedMatch.Set(match);

				args.Pop(namedMatch, true);
				return namedMatch;
			}
			else
			{
				args.AddError(this);
				if (!args.Pop(namedMatch, false))
					return namedMatch;
				else
					return null;
			}
		}

		public NonTerminalMatch Match(string value, bool match = true)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			return Match(new StringScanner(value), match);
		}

		public NonTerminalMatch Match(IScanner scanner, bool match = true)
		{
			if (scanner == null)
				throw new ArgumentNullException("scanner");
			var args = new ParseArgs(scanner);
			var topMatch = (NonTerminalMatch)Parse(args);

			if (match)
			{
				topMatch.PreMatch();
				topMatch.Match();
			}
			return topMatch;
		}

	}
}
