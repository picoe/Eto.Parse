using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public class UnaryParser : Parser
	{
		public Parser Inner { get; set; }

		/*protected override string GetDescriptiveNameInternal(HashSet<Parser> parents)
		{
			return string.Format("{0}, Inner: {1}", base.GetDescriptiveNameInternal(parents), Inner != null ? Inner.GetDescriptiveName(parents): null);
		}*/

		protected UnaryParser(UnaryParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Inner = chain.Clone(other.Inner);
		}

		public UnaryParser()
		{
		}

		public UnaryParser(Parser inner)
		{
			this.Inner = inner;
		}

		public override bool Contains(ParserContainsArgs args)
		{
			if (base.Contains(args))
				return true;
			if (Inner != null && args.Push(this))
			{
				var ret = Inner.Contains(args);
				args.Pop(this);
				return ret;
			}
			return false;
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			if (Inner != null)
				return Inner.Find(parserId);
			else
				return Enumerable.Empty<NamedParser>();
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (Inner != null)
				return Inner.Parse(args);
			else
				return args.EmptyMatch;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new UnaryParser(this, chain);
		}
	}
}
