using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public abstract class ListParser : Parser
	{
		public List<Parser> Items { get; private set; }

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

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			return Items.SelectMany(r => r.Find(parserId));
		}
	}
}

