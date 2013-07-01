using System;
using Eto.Parse;

namespace Eto.Parse.Parsers
{
	public class RepeatParser : UnaryParser
	{
		public int Minimum { get; set; }
		public int Maximum { get; set; }

		protected RepeatParser(RepeatParser other)
			: base(other)
		{
			Minimum = other.Minimum;
			Maximum = other.Maximum;
		}

		public RepeatParser()
		{
			Maximum = Int32.MaxValue;
		}

		public RepeatParser(Parser inner, int minimum = 1, int maximum = Int32.MaxValue)
		: base(inner)
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (!args.Push(this))
				return args.NoMatch;
			
			Scanner scanner = args.Scanner;
			int count = 0;
			ParseMatch match = null;
			while (count < Minimum)
			{
				var pos = scanner.Offset;
				ParseMatch match2 = Inner.Parse(args);

				if (!match2.Success || scanner.IsEnd || scanner.Offset == pos)
				{
					args.Pop(false);
					return args.NoMatch;
				}
				
				match = ParseMatch.Merge(match, match2);
				count++;
			}
			
			// retrieve up to the maximum number
			while (count < Maximum)
			{
				var pos = scanner.Offset;
				ParseMatch match2 = Inner.Parse(args);
				if (!match2.Success || scanner.IsEnd || scanner.Offset == pos)
				{
					if (match == null)
						match = match2;
					break;
				}

				match = ParseMatch.Merge(match, match2);
				count++;
			}
			
			args.Pop(true);

			if (!match.Success && count >= Minimum && count <= Maximum)
				return args.EmptyMatch;

			return match;
		}

		public override Parser Clone()
		{
			return new RepeatParser(this);
		}
	}
}
