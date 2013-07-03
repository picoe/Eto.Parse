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
			IScanner scanner = args.Scanner;
			long offset = scanner.Position;
			for (int i = 0; i < val.Length; i++)
			{
				if (scanner.IsEnd || scanner.Peek != val[i])
				{
					scanner.Position = offset;
					return null;
				}
				scanner.Read();
			}
			return args.Match(offset, val.Length);
		}

		public override Parser Clone()
		{
			return new StringParser(this);
		}
	}
}
