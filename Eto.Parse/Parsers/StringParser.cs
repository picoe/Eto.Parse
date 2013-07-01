using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class StringParser : Parser
	{
		public string Value { get; set; }

		protected StringParser(StringParser other)
			: base(other)
		{
			Value = other.Value;
		}

		public StringParser()
		{
		}

		public StringParser(string value)
		{
			this.Value = value;
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}
		
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (Value == null)
				return args.EmptyMatch;
			Scanner scanner = args.Scanner;
			long offset = scanner.Offset;
			long start = -1;
			foreach (char ch in Value)
			{
				if (!scanner.Read()) return scanner.NoMatch(offset);
				
				if (scanner.Current != ch) return scanner.NoMatch(offset);

				if (start == -1) start = scanner.Offset;
			}
			return args.Match(start, (int)(scanner.Offset - start + 1));
		}

		public override Parser Clone()
		{
			return new StringParser(this);
		}
	}
}
