using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public class UnaryParser : Parser
	{
		public Parser Inner { get; set; }

		protected UnaryParser(UnaryParser other, ParserCloneArgs args)
			: base(other, args)
		{
			Inner = args.Clone(other.Inner);
		}

		public override string DescriptiveName
		{
			get
			{
				if (Inner != null)
					return string.Format("{0}: {1}", base.DescriptiveName, Inner.DescriptiveName);
				return base.DescriptiveName;
			}
		}

		public UnaryParser()
		{
		}

		public UnaryParser(string name)
		{
			this.Name = name;
		}

		public UnaryParser(string name, Parser inner)
		{
			this.Name = name;
			this.Inner = inner;
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

		public override IEnumerable<Parser> Find(ParserFindArgs args)
		{
			if (Inner != null && args.Push(this))
			{
				var ret = Inner.Find(args);
				args.Pop(this);
				return base.Find(args).Concat(ret);
			}
			return base.Find(args);
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (Inner != null)
				return Inner.Parse(args);
			else
				return args.EmptyMatch;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new UnaryParser(this, args);
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

		public override IEnumerable<Parser> Children(ParserChildrenArgs args)
		{
			if (Inner != null && args.Push(this))
			{
				var ret = new Parser[] { Inner }.Concat(Inner.Children(args).ToArray());
				args.Pop(this);
				return ret;
			}
			return Enumerable.Empty<Parser>();
		}

		public override object GetValue(Match match)
		{
			if (Inner != null)
				return Inner.GetValue(match);
			return base.GetValue(match);
		}
	}
}
