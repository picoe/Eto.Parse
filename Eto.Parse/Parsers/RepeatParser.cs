using System;
using Eto.Parse;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class RepeatParser : UnaryParser, ISeparatedParser
	{
		Parser separator;
		bool skipUntilMatches;
		bool hasChildMatch;

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

		protected override void InnerInitialize(ParserInitializeArgs args)
		{
			if (Separator != null)
				Separator.Initialize(args);
			if (Until != null)
				Until.Initialize(args);
			separator = Separator ?? args.Grammar.Separator;
			skipUntilMatches = (Until != null && (Until.AddMatch || Until.Children.Any(r => r.AddMatch)));
			hasChildMatch = false;
			if (!AddMatch)
			{
				if (separator != null)
					hasChildMatch |= separator.AddMatch || separator.Children.Any(r => r.AddMatch);
				if (Inner != null)
					hasChildMatch |= Inner.AddMatch || Inner.Children.Any(r => r.AddMatch);
			}
			base.InnerInitialize(args);
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

					if (hasChildMatch)
						args.Push();

					if (separator != null && count > 0)
					{
						sepMatch = separator.Parse(args);
						if (sepMatch < 0)
						{
							if (hasChildMatch)
								args.PopFailed();
							break;
						}
					}

					var childMatch = Inner.Parse(args);
					if (childMatch > 0)
					{
						if (hasChildMatch)
							args.PopSuccess();
						length += childMatch + sepMatch;
						count++;
					}
					else
					{
						if (hasChildMatch)
							args.PopFailed();
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

		protected override void InnerReplace(ParserReplaceArgs args)
		{
			base.InnerReplace(args);
			Separator = args.Replace(Separator);
			Until = args.Replace(Until);
		}

		protected override IEnumerable<Parser> GetChildren()
		{
            var children = base.GetChildren();
            if (Separator != null)
                children = children.Concat(new[] { Separator });
            if (Until != null)
                children = children.Concat(new[] { Until });
            return children;
		}
	}
}
