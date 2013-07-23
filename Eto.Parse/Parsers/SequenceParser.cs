using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class SequenceParser : ListParser
	{
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

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (Items.Count == 0)
				throw new InvalidOperationException("There are no items in this sequence");

			var pos = args.Scanner.Position;
			var separator = Separator ?? args.Grammar.Separator;
			var match = new ParseMatch(pos, 0);
			for (int i = 0; i < Items.Count; i++)
			{
				var sepMatch = args.NoMatch;
				if (i > 0 && separator != null)
				{
					sepMatch = separator.Parse(args);
					if (!sepMatch.Success)
					{
						args.Scanner.SetPosition(pos);
						return sepMatch;
					}
				}

				var parser = Items[i];
				var childMatch = parser.Parse(args);
				if (!childMatch.Success)
				{
					args.Scanner.SetPosition(pos);
					return childMatch;
				}
				if (sepMatch.Success && !childMatch.Empty)
					match.Length += sepMatch.Length;

				match.Length += childMatch.Length;
			}

			return match;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new SequenceParser(this, chain);
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
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
				args.Pop(this);
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
					args.Pop(this);
					return true;
				}
				args.Pop(this);
			}
			return false;
		}
	}
}
