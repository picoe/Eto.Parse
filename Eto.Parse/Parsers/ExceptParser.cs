using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class ExceptParser : UnaryParser
	{
		public Parser Except { get; set; }

		protected ExceptParser(ExceptParser other, ParserCloneArgs args)
			: base(other, args)
		{
			Except = args.Clone(other.Except);
		}

		public ExceptParser()
		{
		}

		public ExceptParser(Parser inner, Parser except)
		{
			this.Inner = inner;
			this.Except = except;
		}

		protected override int InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			var match = Except.Parse(args);
			if (match < 0)
				return base.InnerParse(args);
			args.Scanner.Position = pos;
			return -1;
		}

		protected override void InnerInitialize(ParserInitializeArgs args)
		{
			if (Except != null)
			{
				Except.Initialize(args);
			}
			base.InnerInitialize(args);
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new ExceptParser(this, args);
		}

		protected override void InnerReplace(ParserReplaceArgs args)
		{
			base.InnerReplace(args);
			Except = args.Replace(Except);
		}

		protected override IEnumerable<Parser> GetChildren()
		{
			return new [] { Except }.Where(r => r != null).Concat(base.GetChildren());
		}
	}
}

