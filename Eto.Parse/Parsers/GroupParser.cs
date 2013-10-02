using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class GroupParser : Parser
	{
		Parser line;
		Parser start;
		Parser end;
		SequenceParser lineSeq;
		SequenceParser blockSeq;
		Parser groupParser;

		public Parser Start
		{
			get { return start; }
			set {
				start = value;
				SetBlock();
				SetInner();
			}
		}

		public Parser End
		{
			get { return end; }
			set
			{
				end = value;
				SetBlock();
				SetInner();
			}
		}

		public Parser Line {
			get { return line; }
			set {
				line = value;
				SetLine();
				SetInner();
			}
		}

		void SetBlock()
		{
			if (start != null && end != null)
				blockSeq = start & new RepeatParser().Until(end, true);
			else
				blockSeq = null;
		}

		void SetLine()
		{
			if (line != null)
				lineSeq = line & new RepeatParser().Until(Terminals.Eol);
			else
				lineSeq = null;
		}

		void SetInner()
		{
			if (lineSeq != null && blockSeq != null)
				groupParser = blockSeq | lineSeq;
			else if (lineSeq != null)
				groupParser = lineSeq;
			else if (blockSeq != null)
				groupParser = blockSeq;
			else
				groupParser = null;
		}

		protected GroupParser(GroupParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			this.line = chain.Clone(other.line);
			this.start = chain.Clone(other.start);
			this.end = chain.Clone(other.end);
			SetLine();
			SetBlock();
			SetInner();
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
			this.start = start;
			this.end = end;
			this.line = line;
			SetLine();
			SetBlock();
			SetInner();
		}

		protected override int InnerParse(ParseArgs args)
		{
			if (groupParser != null)
				return groupParser.Parse(args);
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
				if (line != null)
					line.Initialize(args);
				if (start != null)
					start.Initialize(args);
				if (end != null)
					end.Initialize(args);
				args.Pop();
			}
		}

		public override IEnumerable<Parser> Children(ParserChildrenArgs args)
		{
			var items = new Parser[] { start, end, line }.Where(r => r != null);
			var childItems = items.SelectMany(r => r.Children(args));
			return items.Concat(childItems);
		}

	}
}