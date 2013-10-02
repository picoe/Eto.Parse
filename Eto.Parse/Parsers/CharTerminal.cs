using System;

namespace Eto.Parse.Parsers
{
	public abstract class CharTerminal : Parser, IInverseParser
	{
		bool caseSensitive;

		public bool Inverse { get; set; }

		public bool? CaseSensitive { get; set; }

		public override string DescriptiveName
		{
			get { return string.Format("Char: {0}", CharName); }
		}

		protected abstract string CharName { get; }

		protected CharTerminal(CharTerminal other, ParserCloneArgs chain)
			: base(other, chain)
		{
			this.Inverse = other.Inverse;
			this.CaseSensitive = other.CaseSensitive;
		}

		protected CharTerminal()
		{
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			caseSensitive = CaseSensitive ?? args.Grammar.CaseSensitive;
		}

		protected abstract bool Test(char ch);

		protected bool TestCaseSensitive { get { return caseSensitive; } }

		protected override int InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			int ch = scanner.ReadChar();
			if (ch != -1)
			{
				if (Test((char)ch) != Inverse)
				{
					return 1;
				}
				scanner.Position--;
			}
			return -1;
		}
	}
}
