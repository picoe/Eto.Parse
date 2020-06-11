using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Parse.Parsers
{
	public class SequenceParser : ListParser, ISeparatedParser
	{
		Parser separator;
		public Parser Separator { get; set; }

		protected SequenceParser(SequenceParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Separator = chain.Clone(other.Separator);
		}

		public SequenceParser()
		{
			Separator = DefaultSeparator;
		}

		public SequenceParser(IEnumerable<Parser> sequence)
			: base(sequence)
		{
			Separator = DefaultSeparator;
		}

		public SequenceParser(params Parser[] sequence)
			: base(sequence)
		{
			Separator = DefaultSeparator;
		}

		public override string GetErrorMessage(ParserErrorArgs args)
		{
			if (args.Detailed && args.Push(this))
			{
				var sb = new StringBuilder();
                for (int i = 0; i < Items.Count; i++)
				{
                    Parser item = Items[i];
                    if (sb.Length > 0)
						sb.Append(", ");
					sb.Append(item != null ? item.GetErrorMessage(args) : "null");
				}
				sb.Insert(0, "(");
				sb.Append(")");
				return sb.ToString();
			}
			return DescriptiveName;
		}

		protected override int InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			var length = 0;
			var count = Items.Count;
			if (separator != null)
			{
				var parser = Items[0];
				var childMatch = parser.Parse(args);
				if (childMatch < 0)
				{
					return childMatch;
				}

				length += childMatch;
				for (int i = 1; i < count; i++)
				{
					var sepMatch = separator.Parse(args);
					if (sepMatch >= 0)
					{
						parser = Items[i];
						childMatch = parser.Parse(args);
						if (childMatch > 0)
						{
							length += childMatch + sepMatch;
							continue;
						}
						else if (childMatch == 0)
						{
							continue;
						}
					}
					// failed
					args.Scanner.Position = pos;
					return -1;
				}
				return length;
			}
			for (int i = 0; i < count; i++)
			{
				var parser = Items[i];
				var childMatch = parser.Parse(args);
				if (childMatch >= 0)
				{
					length += childMatch;
					continue;
				}
				args.Scanner.Position = pos;
				return -1;
			}
			return length;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new SequenceParser(this, args);
		}

		protected override void InnerInitialize(ParserInitializeArgs args)
		{
			separator = Separator ?? args.Grammar.Separator;
			if (Items.Count == 0)
				throw new InvalidOperationException(string.Format("There are no items in this sequence {0}", DescriptiveName));

			if (Separator != null)
				Separator.Initialize(args);

			base.InnerInitialize(args);
		}

		public override bool IsLeftRecursive(ParserContainsArgs args)
		{
			if (base.IsLeftRecursive(args))
				return true;
			if (args.Push(this))
			{
				var item = Items[0];
				if (item != null && item.IsLeftRecursive(args)) {
					args.Pop();
					return true;
				}
				args.Pop();
			}
			return false;
		}

		protected override void InnerReplace(ParserReplaceArgs args)
		{
			base.InnerReplace(args);
			Separator = args.Replace(Separator);
		}

		protected override IEnumerable<Parser> GetChildren()
		{
            var children = base.GetChildren();
            if (Separator != null)
                children = children.Concat(new[] { Separator });
            return children;
		}

	}
}
