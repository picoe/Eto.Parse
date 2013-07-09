using System;
using System.Collections.Generic;
using Eto.Parse.Scanners;

namespace Eto.Parse
{
	public class NamedParser : UnaryParser
	{
		public string Name { get; set; }

		protected NamedParser(NamedParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			this.Name = other.Name;
		}

		public NamedParser()
		{
			this.Name = Guid.NewGuid().ToString();
		}

		public NamedParser(string name)
		{
			this.Name = name ?? Guid.NewGuid().ToString();
		}

		public NamedParser(string name, Parser parser)
			: base(parser)
		{
			this.Name = name ?? Guid.NewGuid().ToString();
		}

		public override string DescriptiveName
		{
			get { return string.Format("{0} \"{1}\"", base.DescriptiveName, this.Name); }
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
			var matches = args.Push(this, true);
			var match = base.InnerParse(args);
			if (match.Success)
			{
				var namedMatch = new NamedMatch(this, args.Scanner, match.Index, match.Length, matches);
				args.Pop(namedMatch, true);
				return match;
			}
			else
			{
				args.Pop(false);
				return args.NoMatch;
			}
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new NamedParser(this, chain);
		}
	}
}
