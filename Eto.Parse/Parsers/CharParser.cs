using System;
using Eto.Parse.Testers;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class CharParser : Parser, IInverseParser
	{
		public bool Inverse { get; set; }

		public ICharTester Tester { get; set; }

		protected CharParser(CharParser other)
			: base(other)
		{
			Tester = other.Tester;
		}

		protected override string GetDescriptiveNameInternal(HashSet<Parser> parents)
		{
			var tester = Tester != null ? Tester.GetType().Name : null;
			return string.Format("{0}, Tester: {1}", base.GetDescriptiveNameInternal(parents), tester);
		}

		public CharParser()
		{
		}

		public CharParser(ICharTester tester)
		{
			this.Tester = tester;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			if (scanner.IsEnd || Tester == null)
				return args.NoMatch;
	
			bool matched = Tester.Test(scanner.Peek);
			if (matched == Inverse)
				return args.NoMatch;

			var offset = scanner.Position;
			scanner.Read();

			return args.Match(offset, 1);
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
			return new CharParser(new IncludeTester(parser.Tester, parser.Inverse, include.Tester, include.Inverse)) { Reusable = true };
		}

		public static CharParser operator +(CharParser parser, char[] chars)
		{
			return new CharParser(new IncludeTester(parser.Tester, parser.Inverse, new CharSetTester(chars), false)) { Reusable = true };
		}

		public static CharParser operator +(CharParser parser, char ch)
		{
			return new CharParser(new IncludeTester(parser.Tester, parser.Inverse, new CharSetTester(ch), false)) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, CharParser exclude)
		{
			return new CharParser(new ExcludeTester(include.Tester, include.Inverse, exclude.Tester, exclude.Inverse)) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, char[] chars)
		{
			return new CharParser(new ExcludeTester(include.Tester, include.Inverse, new CharSetTester(chars), false)) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, char ch)
		{
			return new CharParser(new ExcludeTester(include.Tester, include.Inverse, new CharSetTester(ch), false)) { Reusable = true };
		}
	}
}
