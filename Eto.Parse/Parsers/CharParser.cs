using System;
using Eto.Parse.Testers;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class CharParser : Parser, IInverseParser
	{
		public bool Inverse { get; set; }

		public ICharTester Tester { get; set; }

		protected CharParser(CharParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Tester = other.Tester;
		}

		public override string DescriptiveName
		{
			get
			{
				var tester = Tester != null ? Tester.GetType().Name : null;
				return string.Format("{0}, Tester: {1}", base.DescriptiveName, tester);
			}
		}

		public CharParser()
		{
			AddError = true;
		}

		public CharParser(ICharTester tester)
			: this()
		{
			this.Tester = tester;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			char ch;
			var pos = scanner.Position;
			if (scanner.ReadChar(out ch))
			{
				if (Tester == null)
					return new ParseMatch(pos, 1);

				bool matched = Tester.Test(ch, args.Grammar.CaseSensitive);
				if (matched != Inverse)
					return new ParseMatch(pos, 1);
			}
			scanner.SetPosition(pos);
			return args.NoMatch;
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new CharParser(this, chain);
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
