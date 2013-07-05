using System;
using Eto.Parse;

namespace Eto.Parse.Parsers
{
	public class RepeatParser : UnaryParser
	{
		public Parser Separator { get; set; }

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
			Separator = DefaultSeparator;
		}

		public RepeatParser()
		{
			Maximum = Int32.MaxValue;
			Separator = DefaultSeparator;
		}

		public RepeatParser(Parser inner, int minimum = 1, int maximum = Int32.MaxValue, Parser until = null)
		: base(inner)
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
			this.Until = until;
			Separator = DefaultSeparator;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (args.IsRecursive(this))
				return args.NoMatch;
			
			IScanner scanner = args.Scanner;
			int count = 0;
			ParseMatch match = args.NoMatch;
			var pos = scanner.Position;

			// retrieve up to the maximum number
			while (count < Maximum)
			{
				if (Until != null && count >= Minimum)
				{
					var offset = scanner.Position;
					var stopMatch = Until.Parse(args);
					if (stopMatch.Success) {
						scanner.Position = offset;
						break;
					}
				}

				if (Inner != null)
				{
					ParseMatch sepMatch = args.NoMatch;
					if (count > 0 && Separator != null)
					{
						sepMatch = Separator.Parse(args);
						if (!sepMatch.Success)
							break;
					}

					var childMatch = Inner.Parse(args);
					if (childMatch.FailedOrEmpty)
					{
						if (sepMatch.Success)
							scanner.Position = sepMatch.Index;
						break;
					}
					if (sepMatch.Success)
						match = ParseMatch.Merge(match, sepMatch);
					match = ParseMatch.Merge(match, childMatch);
				}
				else
				{
					if (scanner.IsEnd)
						break;
					var childMatch = new ParseMatch(scanner.Position, 1);
					scanner.Position++;
					match = ParseMatch.Merge(match, childMatch);
				}

				count++;
			}
			
			if (count < Minimum)
			{
				scanner.Position = pos;
				return args.NoMatch;
			}

			if (match.Success)
				return match;
			else
				return args.EmptyMatch;
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
