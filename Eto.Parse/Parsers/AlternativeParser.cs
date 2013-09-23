using System;
using Eto.Parse;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class AlternativeParser : ListParser
	{
		public static Parser ExcludeNull(params Parser[] parsers)
		{
			return ExcludeNull((IEnumerable<Parser>)parsers);
		}

		public static Parser ExcludeNull(IEnumerable<Parser> parsers)
		{
			var p = parsers.Where(r => r != null).ToArray();
			if (p.Length == 1)
				return p[0];
			else
				return new AlternativeParser(p);
		}

		protected AlternativeParser(AlternativeParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
		}

		public AlternativeParser()
		{
		}

		public AlternativeParser(IEnumerable<Parser> sequence)
				: base(sequence)
		{
		}

		public AlternativeParser(params Parser[] sequence)
				: base(sequence)
		{
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var count = Items.Count;
			if (HasNamedChildren)
			{
				args.Push();
				for (int i = 0; i < count; i++)
				{
					var parser = Items[i];
					if (parser != null)
					{
						var match = parser.Parse(args);
						if (!match.Success)
						{
							args.ClearMatches();
						}
						else
						{
							args.PopSuccess();
							return match;
						}
					}
					else
					{
						args.PopFailed();
						return args.EmptyMatch;
					}
				}
				args.PopFailed();
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					var parser = Items[i];
					if (parser != null)
					{
						var match = parser.Parse(args);
						if (match.Success)
							return match;
					}
					else
					{
						return args.EmptyMatch;
					}
				}
			}
			return ParseMatch.None;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new AlternativeParser(this, chain);
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (args.Push(this))
			{
				var first = new List<Parser>();
				var second = new List<Parser>();
				foreach (var item in Items)
				{
					if (item != null && item.IsLeftRecursive(new ParserContainsArgs(this)))
					{
						second.Add(item);
						args.RecursionFixes.Add(this);
						item.Initialize(args);
						args.RecursionFixes.Remove(this);
					}
					else
					{
						first.Add(item);
						if (item != null)
							item.Initialize(args);
					}
				}
				if (second.Count > 0)
				{
					this.Items.Clear();
					var secondParser = second.Count > 1 ? new AlternativeParser(second) : second[0];
					if (first.Count > 0)
					{
						var firstParser = first.Count > 1 ? new AlternativeParser(first) : first[0];
						if (first.Count == 1 && first[0] == null)
						{
							this.Items.Add(-secondParser);
						}
						else
							this.Items.Add(new SequenceParser(firstParser, -secondParser));
					}
					else
						this.Items.Add(+secondParser);
				}
				args.Pop();
			}
		}

		public override bool IsLeftRecursive(ParserContainsArgs args)
		{
			if (base.IsLeftRecursive(args))
				return true;
			if (args.Push(this))
			{
				foreach (var item in Items)
				{
					if (item != null && item.IsLeftRecursive(args))
					{
						args.Pop();
						return true;
					}
				}
				args.Pop();
			}
			return false;
		}
	}
}
