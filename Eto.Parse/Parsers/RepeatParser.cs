using System;
using Eto.Parse;

namespace Eto.Parse.Parsers
{
	public class RepeatParser : UnaryParser
	{
		public int Minimum { get; set; }
		public int Maximum { get; set; }

		public Parser Until { get; set; }

		protected RepeatParser(RepeatParser other)
			: base(other)
		{
			Minimum = other.Minimum;
			Maximum = other.Maximum;
			if (other.Until != null)
				Until = other.Until.Clone();
		}

		public RepeatParser()
		{
			Maximum = Int32.MaxValue;
		}

		public RepeatParser(Parser inner, int minimum = 1, int maximum = Int32.MaxValue, Parser until = null)
		: base(inner)
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
			this.Until = until;
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
				if (Until != null && count >= Minimum)
				{
					var offset = scanner.Offset;
					var stopMatch = Until.Parse(args);
					if (stopMatch != null) {
						scanner.Offset = offset;
						break;
					}
				}
				if (Inner != null)
				{
					var childMatch = Inner.Parse(args);
					if (childMatch == null || childMatch.Empty)
					{
						break;
					}
					match = ParseMatch.Merge(match, childMatch);
				}
				else
				{
					if (scanner.IsEnd)
						break;
					var childMatch = new ParseMatch(scanner, scanner.Offset, 1);
					scanner.Offset++;
					match = ParseMatch.Merge(match, childMatch);
				}

				count++;
			}
			
			if (count < Minimum)
			{
				args.Pop(false);
				return null;
			}

			args.Pop(true);

			return match ?? args.EmptyMatch;
		}

		public override Parser Clone()
		{
			return new RepeatParser(this);
		}

		public static RepeatParser operator -(RepeatParser repeat, Parser until)
		{
			repeat.Until = until;
			return repeat;
		}
	}
}
