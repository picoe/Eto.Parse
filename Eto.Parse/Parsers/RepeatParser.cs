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
				return null;
			
			Scanner scanner = args.Scanner;
			int count = 0;
			ParseMatch match = null;

			// retrieve up to the maximum number
			while (count < Maximum)
			{
				ParseMatch childMatch = Inner.Parse(args);
				if (childMatch == null || childMatch.Empty)
				{
					if (count < Minimum)
					{
						args.Pop(false);
						return null;
					}
					break;
				}

				match = ParseMatch.Merge(match, childMatch);
				count++;
			}
			
			args.Pop(true);

			return match ?? args.EmptyMatch;
		}

		public override Parser Clone()
		{
			return new RepeatParser(this);
		}
	}
}
