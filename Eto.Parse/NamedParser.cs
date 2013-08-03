using System;
using System.Collections.Generic;
using Eto.Parse.Scanners;
using System.Linq;

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
			: this(Guid.NewGuid().ToString())
		{
		}

		public NamedParser(string name)
			: this(name, null)
		{
		}

		public NamedParser(string name, Parser parser)
			: base(parser)
		{
			this.Name = name ?? Guid.NewGuid().ToString();
			this.AddError = true;
		}

		public override string DescriptiveName
		{
			get { return string.Format("{0} \"{1}\"", base.DescriptiveName, this.Name); }
		}

		public event Action<NamedMatch> Matched;

		protected virtual void OnMatched(NamedMatch match)
		{
			if (Matched != null)
				Matched(match);
		}

		public event Action<NamedMatch> PreMatch;

		protected virtual void OnPreMatch(NamedMatch match)
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

		public override IEnumerable<NamedParser> Find(ParserFindArgs args)
		{
			if (args.Push(this))
			{
				IEnumerable<NamedParser> ret;
				if (string.Equals(this.Name, args.ParserId, StringComparison.InvariantCultureIgnoreCase))
					ret = new NamedParser[] { this };
				else if (Inner != null)
					ret = Inner.Find(args);
				else
					ret = Enumerable.Empty<NamedParser>();
				args.Pop(this);
				return ret;
			}
			return Enumerable.Empty<NamedParser>();
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			args.Push(this);
			var match = base.InnerParse(args);
			if (match.Success)
			{
				args.PopMatch(this, match);
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
