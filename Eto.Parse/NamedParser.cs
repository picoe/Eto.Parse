using System;
using System.Collections.Generic;
using Eto.Parse.Scanners;

namespace Eto.Parse
{
	public class NamedParser : UnaryParser
	{
		public string Name { get; set; }

		public NamedParser()
		{
		}

		public NamedParser(string name)
		{
			this.Name = name;
		}

		public NamedParser(string name, Parser parser)
			: base(parser)
		{
			this.Name = name;
		}

		protected override string GetDescriptiveNameInternal(HashSet<Parser> parents)
		{
			return string.Format("{0} \"{1}\"", base.GetDescriptiveNameInternal(parents), this.Name);
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
			if (this.Name == parserId)
				yield return this;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var namedMatch = new NamedMatch(this, args.Scanner);
			args.Push(this, namedMatch);
			var match = Inner.Parse(args);
			if (match.Success)
			{
				namedMatch.Set(match);

				args.Pop(namedMatch, true);
				return match;
			}
			else
			{
				args.Pop(namedMatch, false);
				return args.NoMatch;
			}
		}
	}
}
