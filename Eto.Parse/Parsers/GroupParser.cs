using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class GroupParser : Parser
	{
		static Parser eol = Terminals.Eol;

		public Parser Start { get; set; }

		public Parser End { get; set; }

		public Parser Line { get; set; }

		protected GroupParser(GroupParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			this.Line = chain.Clone(other.Line);
			this.Start = chain.Clone(other.Start);
			this.End = chain.Clone(other.End);
		}

		public GroupParser()
		{
		}

		public GroupParser(Parser startEnd)
			: this(startEnd, startEnd, null)
		{
		}

		public GroupParser(Parser start, Parser end, Parser line = null)
		{
			this.Start = start;
			this.End = end;
			this.Line = line;
		}

		protected override int InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			var pos = scanner.Position;
			int match = -1;
			if (Start != null)
			{
				match = Start.Parse(args);
			}
			if (match < 0)
			{
				if (Line == null)
					return -1;
				match = Line.Parse(args);
				if (match < 0)
					return -1;
				for (;;)
				{
					match = eol.Parse(args);
					if (match >= 0)
						break;
					if (scanner.Advance(1) < 0)
						break;
				}
				if (match < 0)
					return -1;
				scanner.Advance(-match);
				return scanner.Position - pos + 1;
			}
			for (;;)
			{
				match = End.Parse(args);
				if (match >= 0)
					break;
				if (scanner.Advance(1) < 0)
					break;
			}

			if (match >= 0)
				return scanner.Position - pos + 1;

			return -1;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new GroupParser(this, args);
		}
		
		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (args.Push(this))
			{
				if (Line != null)
					Line.Initialize(args);
				if (Start != null)
					Start.Initialize(args);
				if (End != null)
					End.Initialize(args);
				args.Pop();
			}
		}

		public override IEnumerable<Parser> Children(ParserChildrenArgs args)
		{
			var items = new Parser[] { Start, End, Line }.Where(r => r != null);
			var childItems = items.SelectMany(r => r.Children(args));
			return items.Concat(childItems);
		}

		protected override void InnerReplace(ParserReplaceArgs args)
		{
			base.InnerReplace(args);
			Start = args.Replace(Start);
			Line = args.Replace(Line);
			End = args.Replace(End);
		}

	}
}