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

		public override IEnumerable<NamedParser> Find(ParserFindArgs args)
		{
			if (args.Push(this)) 
			{
				var ret = Items.Where(r => r != null).SelectMany(r => r.Find(args)).ToArray();
				args.Pop(this);
				return ret;
			}
			return Enumerable.Empty<NamedParser>();
		}

		public void InitializeItems(ParserInitializeArgs args)
		{
			foreach (var item in Items)
			{
				if (item != null)
					item.Initialize(args);
			}
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

