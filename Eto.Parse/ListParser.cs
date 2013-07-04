using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public abstract class ListParser : Parser
	{
		public List<Parser> Items { get; private set; }

		protected override string GetDescriptiveNameInternal(HashSet<Parser> parents)
		{
			return null; // show only the contained items in the list, not the list itself
		}

		protected ListParser (ListParser other)
		{
			Items = new List<Parser>(other.Items.Select(r => r.Clone()));
		}

		public ListParser()
		{
			Items = new List<Parser>();
		}

		public ListParser(IEnumerable<Parser> sequence)
		{
			Items = sequence.ToList();
		}

		public override IEnumerable<NonTerminalParser> Find(string parserId)
		{
			return Items.SelectMany(r => r.Find(parserId));
		}
	}
}

