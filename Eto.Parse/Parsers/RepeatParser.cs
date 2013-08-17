using System;
using Eto.Parse;

namespace Eto.Parse.Parsers
{
	public class RepeatParser : UnaryParser, ISeparatedParser
	{
		public Parser Separator { get; set; }

		public int Minimum { get; set; }
		public int Maximum { get; set; }

		public Parser Until { get; set; }

		protected RepeatParser(RepeatParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Minimum = other.Minimum;
			Maximum = other.Maximum;
			Until = chain.Clone(other.Until);
			Separator = chain.Clone(other.Separator);
		}

		public RepeatParser()
		{
			Maximum = Int32.MaxValue;
			Separator = DefaultSeparator;
		}

		public RepeatParser(Parser inner, int minimum, int maximum = Int32.MaxValue, Parser until = null)
			: base(null, inner)
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
			this.Until = until;
			Separator = DefaultSeparator;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			int count = 0;
			var pos = scanner.Position;
			var match = new ParseMatch(pos, 0);

			var separator = Separator ?? args.Grammar.Separator;
			// retrieve up to the maximum number
			var sepMatch = args.NoMatch;
			if (Inner != null)
			{
				while (count < Maximum)
				{
					if (Until != null && count >= Minimum)
					{
						var stopMatch = Until.Parse(args);
						if (stopMatch.Success)
						{
							scanner.SetPosition(stopMatch.Index);
							return match;
						}
					}

					if (separator != null && count > 0)
					{
						sepMatch = separator.Parse(args);
						if (!sepMatch.Success)
							break;
					}

					var childMatch = Inner.Parse(args);
					if (childMatch.FailedOrEmpty)
					{
						if (sepMatch.Success)
							scanner.SetPosition(sepMatch.Index);
						break;
					}
					if (sepMatch.Success)
						match.Length += sepMatch.Length;
					match.Length += childMatch.Length;

					count++;
				}
			}
			else
			{
				while (count < Maximum)
				{
					if (Until != null && count >= Minimum)
					{
						var stopMatch = Until.Parse(args);
						if (stopMatch.Success)
						{
							scanner.SetPosition(stopMatch.Index);
							return match;
						}
					}

					var ofs = scanner.Advance(1);
					if (ofs == -1)
						break;
					match.Length += 1;
					count++;
				}
			}

			if (count < Minimum)
			{
				scanner.SetPosition(pos);
				return args.NoMatch;
			}

			return match;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new RepeatParser(this, chain);
		}

		public static RepeatParser operator -(RepeatParser repeat, Parser until)
		{
			repeat.Until = until;
			return repeat;
		}
	}
}
