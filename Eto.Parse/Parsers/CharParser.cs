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

		public static CharParser operator +(CharParser parser, CharParser include)
		{
			return new CharParser(new IncludeTester(parser.Tester, include.Tester)) { Reusable = true };
		}

		public static CharParser operator +(CharParser parser, char[] chars)
		{
			return new CharParser(new IncludeTester(parser.Tester, new CharSetTester(chars))) { Reusable = true };
		}

		public static CharParser operator +(CharParser parser, char ch)
		{
			return new CharParser(new IncludeTester(parser.Tester, new CharSetTester(ch))) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, CharParser exclude)
		{
			return new CharParser(new ExcludeTester(include.Tester, exclude.Tester)) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, char[] chars)
		{
			return new CharParser(new ExcludeTester(include.Tester, new CharSetTester(chars))) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, char ch)
		{
			return new CharParser(new ExcludeTester(include.Tester, new CharSetTester(ch))) { Reusable = true };
		}
	}
}
