using System;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class GroupParser : Parser
	{
		Parser line;
		Parser start;
		Parser end;
		SequenceParser lineSeq;
		SequenceParser blockSeq;
		Parser group;

		public Parser Start
		{
			get { return start; }
			set {
				start = value;
				SetBlock();
			}
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}

		public Parser End { get; set; }

		public Parser Line {
			get { return line; }
			set {
				line = value;
				if (line != null)
					lineSeq = line & +(Terminals.AnyChar - Terminals.Eol) & Terminals.Eol;
				else
					lineSeq = null;
				SetInner();
			}
		}

		void SetBlock()
		{
			if (start != null && end != null)
				blockSeq = start & (+Terminals.AnyChar).Until(end) & end;
			else
				blockSeq = null;
			SetInner();
		}

		void SetInner()
		{
			if (lineSeq != null && blockSeq != null)
				group = lineSeq | blockSeq;
			else if (lineSeq != null)
				group = lineSeq;
			else if (blockSeq != null)
				group = blockSeq;
			else
				group = null;
		}

		protected GroupParser(GroupParser other)
		{
			this.line = other.line != null ? other.line.Clone() : null;
			this.start = other.start != null ? other.start.Clone() : null;
			this.end = other.end != null ? other.end.Clone() : null;
			SetBlock();
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
			SetBlock();
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (group != null)
				return group.Parse(args);
			return args.NoMatch;
		}

		public override Parser Clone()
		{
			return new GroupParser(this);
		}
	}
}

