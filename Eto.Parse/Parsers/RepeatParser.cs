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
				skipUntilMatches = !CaptureUntil || (Until != null && Until.Children().Any(r => r.Name != null));
				args.Pop(this);
			}
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			int count = 0;
			var match = new ParseMatch(scanner.Position, 0);

			// retrieve up to the maximum number
			var sepMatch = ParseMatch.None;
			if (Inner != null)
			{
				while (count < Maximum)
				{
					if (Until != null && count >= Minimum)
					{
						ParseMatch stopMatch;
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
						if (stopMatch.Success)
						{
							if (CaptureUntil)
								match.Length += stopMatch.Length;
							else if (!SkipUntil)
								scanner.Position = stopMatch.Index;
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
					if (childMatch.Length > 0)
					{
						if (sepMatch.Success)
							match.Length += sepMatch.Length;
						match.Length += childMatch.Length;
						count++;
					}
					else
					{
						if (sepMatch.Success)
							scanner.Position = sepMatch.Index;
						break;
					}

				}
			}
			else
			{
				while (count < Maximum)
				{
					if (Until != null && count >= Minimum)
					{
						ParseMatch stopMatch;
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
						if (stopMatch.Success)
						{
							if (CaptureUntil)
								match.Length += stopMatch.Length;
							else if (!SkipUntil)
								scanner.Position = stopMatch.Index;
							return match;
						}
					}

					if (separator != null && count > 0)
					{
						sepMatch = separator.Parse(args);
						if (!sepMatch.Success)
							break;
					}

					var ofs = scanner.Advance(1);
					if (ofs >= 0)
					{
						if (sepMatch.Success)
							match.Length += sepMatch.Length;
						match.Length ++;
						count++;
					}
					else
					{
						if (sepMatch.Success)
							scanner.Position = sepMatch.Index;
						break;
					}
				}
			}

			if (count < Minimum)
			{
				scanner.Position = match.Index;
				return ParseMatch.None;
			}

			return match;
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
