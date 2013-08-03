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

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (Inner != null && args.Push(this))
			{
				Inner.Initialize(args);
				args.Pop(this);
			}
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

		public override IEnumerable<NamedParser> Find(ParserFindArgs args)
		{
			if (Inner != null && args.Push(this))
			{
				var ret = Inner.Find(args);
				args.Pop(this);
				return ret;
			}
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

		public override bool IsLeftRecursive(ParserContainsArgs args)
		{
			if (base.IsLeftRecursive(args))
				return true;
			if (Inner != null)
			{
				if (args.Push(this))
				{
					var ret = Inner.IsLeftRecursive(args);
					args.Pop(this);
					return ret;
				}
				return false;
			}
			else
				return false;
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			if (Inner != null && args.Push(this))
			{
				var ret = new Parser[] { Inner }.Concat(Inner.Children(args).ToArray());
				args.Pop(this);
				return ret;
			}
			return Enumerable.Empty<Parser>();
		}
	}
}
