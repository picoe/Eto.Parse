using System;
using System.Collections.Generic;
using System.Linq;

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

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			separator = Separator ?? args.Grammar.Separator;
			if (Items.Count == 0)
				throw new InvalidOperationException("There are no items in this sequence");
			if (args.Push(this))
			{
				var leftItem = Items[0];
				if (leftItem != null)
				{
					foreach (var parent in args.RecursionFixes)
					{
						if (leftItem.IsLeftRecursive(parent))
						{
							Items[0] = new EmptyParser();
							break;
						}
					}
				}
				foreach (var item in Items.Where(r => r != null))
				{
					item.Initialize(args);
				}

				if (Separator != null)
					Separator.Initialize(args);

				args.Pop();
			}
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
	}
}
