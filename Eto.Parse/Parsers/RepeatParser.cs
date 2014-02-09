using System;
using Eto.Parse;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class RepeatParser : UnaryParser, ISeparatedParser
	{
		Parser separator;
		bool skipUntilMatches;

		public Parser Separator { get; set; }

		public int Minimum { get; set; }

		public int Maximum { get; set; }

		public Parser Until { get; set; }

		public bool SkipUntil { get; set; }

		public bool CaptureUntil { get; set; }

		protected RepeatParser(RepeatParser other, ParserCloneArgs args)
			: base(other, args)
		{
			Minimum = other.Minimum;
			Maximum = other.Maximum;
			Until = args.Clone(other.Until);
			SkipUntil = other.SkipUntil;
			CaptureUntil = other.CaptureUntil;
			Separator = args.Clone(other.Separator);
		}

		public RepeatParser()
		{
			Maximum = Int32.MaxValue;
			Separator = DefaultSeparator;
		}

		public RepeatParser(int minimum, int maximum = Int32.MaxValue)
		{
			Minimum = minimum;
			Maximum = maximum;
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

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (args.Push(this))
			{
				if (Separator != null)
					Separator.Initialize(args);
				if (Until != null)
					Until.Initialize(args);
				separator = Separator ?? args.Grammar.Separator;
				skipUntilMatches = (Until != null && (Until.Name != null || Until.Children().Any(r => r.Name != null)));
				args.Pop();
			}
		}

		protected override int InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			int count = 0;
			var pos = scanner.Position;
			int length = 0;

			// retrieve up to the maximum number
			var sepMatch = 0;
			if (Inner != null)
			{
				while (count < Maximum)
				{
					int curPos = scanner.Position;
					if (Until != null && count >= Minimum)
					{
						int stopMatch;
						if (skipUntilMatches)
						{
							args.Push();
							stopMatch = Until.Parse(args);
							args.PopFailed();
						}
						else
						{
							stopMatch = Until.Parse(args);
						}
						if (stopMatch >= 0)
						{
							if (CaptureUntil)
								length += stopMatch;
							else if (!SkipUntil)
								scanner.Position = curPos;
							return length;
						}
					}

					if (separator != null && count > 0)
					{
						sepMatch = separator.Parse(args);
						if (sepMatch < 0)
							break;
					}

					var childMatch = Inner.Parse(args);
					if (childMatch > 0)
					{
						length += childMatch + sepMatch;
						count++;
					}
					else
					{
						if (sepMatch > 0)
							scanner.Position = curPos;
						break;
					}

				}
			}
			else
			{
				while (count < Maximum)
				{
					int curPos = scanner.Position;
					if (Until != null && count >= Minimum)
					{
						int stopMatch;
						if (skipUntilMatches)
						{
							args.Push();
							stopMatch = Until.Parse(args);
							args.PopFailed();
						}
						else
						{
							stopMatch = Until.Parse(args);
						}
						if (stopMatch >= 0)
						{
							if (CaptureUntil)
								length += stopMatch;
							else if (!SkipUntil)
								scanner.Position = curPos;
							return length;
						}
					}

					if (separator != null && count > 0)
					{
						sepMatch = separator.Parse(args);
						if (sepMatch < 0)
							break;
					}

					var ofs = scanner.Advance(1);
					if (ofs >= 0)
					{
						length += sepMatch;
						length++;
						count++;
					}
					else
					{
						if (sepMatch > 0)
							scanner.Position = curPos;
						break;
					}
				}
			}

			if (count < Minimum)
			{
				scanner.Position = pos;
				return -1;
			}

			return length;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new RepeatParser(this, args);
		}

		public static RepeatParser operator ^(RepeatParser repeat, Parser until)
		{
			repeat.Until = until;
			return repeat;
		}
	}
}
