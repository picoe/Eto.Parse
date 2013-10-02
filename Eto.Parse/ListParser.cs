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
			Items = new List<Parser>(other.Items.Select(chain.Clone));
		}

		protected ListParser()
		{
			Items = new List<Parser>();
		}

		protected ListParser(IEnumerable<Parser> sequence)
		{
			Items = sequence.ToList();
		}

		public override IEnumerable<Parser> Find(ParserFindArgs args)
		{
			if (args.Push(this)) 
			{
				foreach (var item in base.Find(args))
				{
					yield return item;
				}
				foreach (var item in Items)
				{
					if (item != null)
					{
						foreach (var  child in item.Find(args))
							yield return child;
					}
				}
				//ret = ret.Concat(Items.Where(r => r != null).SelectMany(r => r.Find(args)).ToArray());
				args.Pop();
			}
			//return ret;
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
						args.Pop();
						return true;
					}
				}
				args.Pop();
			}
			return false;
		}

		public override IEnumerable<Parser> Children(ParserChildrenArgs args)
		{
			if (args.Push(this))
			{
				var items = Items.Where(r => r != null);
				foreach (var item in items)
				{
					yield return item;
					foreach (var child in item.Children(args))
					{
						yield return child;
					}
				}
				args.Pop();
			}
		}

		public void Add(params Parser[] parsers)
		{
			Items.AddRange(parsers);
		}
	}
}

