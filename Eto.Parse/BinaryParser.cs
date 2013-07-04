using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public abstract class BinaryParser : Parser
	{

		public Parser First { get; set; }

		public Parser Second { get; set; }

		public BinaryParser()
		{
		}

		public BinaryParser(Parser first, Parser second)
		{
			this.First = first;
			this.Second = second;
		}

		public override IEnumerable<NonTerminalParser> Find(string parserId)
		{
			if (First != null && Second != null)
				return First.Find(parserId).Concat(Second.Find(parserId));
			else if (First != null)
				return First.Find(parserId);
			else if (Second != null)
				return Second.Find(parserId);
			else
				return Enumerable.Empty<NonTerminalParser>();

		}
	}
}
