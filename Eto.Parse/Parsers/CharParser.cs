using System;
using Eto.Parse.Testers;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class CharParser : NegatableParser
	{
		public CharTester Tester { get; set; }

		protected CharParser(CharParser other)
			: base(other)
		{
			Tester = other.Tester;
		}

		public CharParser()
		{
		}

		public CharParser(CharTester tester)
		{
			this.Tester = tester;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			if (scanner.IsEnd || Tester == null)
				return null;
	
			long offset = scanner.Offset;
			
			if (!scanner.Read())
				return null;

			bool matched = Tester.Test(scanner.Current);
			if (matched == Negative)
			{
				scanner.Offset = offset;
				return null;
			}
			
			return args.Match(scanner.Offset, 1);
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}

		public override Parser Clone()
		{
			return new CharParser(this);
		}
	}
}
