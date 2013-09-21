using System;
using System.Collections.Generic;

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
			if (args.Push(this))
			{
				caseSensitive = CaseSensitive ?? args.Grammar.CaseSensitive;
				args.Pop(this);
			}
		}

		protected abstract bool Test(char ch, bool caseSensitive);

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			char ch;
			var pos = scanner.Position;
			if (!scanner.ReadChar(out ch) || Test(ch, caseSensitive) == Inverse)
			{
				scanner.Position = pos;
				return ParseMatch.None;
			}
			return new ParseMatch(pos, 1);
		}
	}
}
