using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public abstract class ListParser : Parser
	{
		public List<Parser> Items { get; private set; }

		protected ListParser(ListParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Items = new List<Parser>(other.Items.Select(r => chain.Clone(r)));
		}

		public ListParser()
		{
			Items = new List<Parser>();
		}

		public ListParser(IEnumerable<Parser> sequence)
		{
			Items = sequence.ToList();
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			return Items.SelectMany(r => r.Find(parserId));
		}

		public override bool Contains(ParserContainsArgs args)
		{
			if (base.Contains(args))
				return true;
			if (args.Push(this))
			{
				foreach (var item in Items)
				{
					if (item != null && item.Contains(args))
					{
						args.Pop(this);
						return true;
					}
				}
				args.Pop(this);
			}
			return false;
		}
	}
}

