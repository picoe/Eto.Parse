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

		protected UnaryParser(UnaryParser other)
			: base(other)
		{
			if (other != null)
				Inner = other.Clone();
		}

		public UnaryParser()
		{
		}

		public UnaryParser(Parser inner)
		{
			this.Inner = inner;
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
			if (Inner == null)
				return args.EmptyMatch;
			else
				return Inner.Parse(args);
		}

		public override Parser Clone()
		{
			return new UnaryParser(this);
		}
	}
}
