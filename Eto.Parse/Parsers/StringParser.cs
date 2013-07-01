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
			var val = Value;
			if (val == null)
				return args.EmptyMatch;
			Scanner scanner = args.Scanner;
			long offset = scanner.Offset;
			for (int i = 0; i < val.Length; i++)
			{
				if (!scanner.Read() || scanner.Current != val[i]) 
					return scanner.NoMatch(offset);
			}
			return args.Match(offset + 1, val.Length);
		}

		public override Parser Clone()
		{
			return new StringParser(this);
		}
	}
}
