using System;
using Eto.Parse;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class UntilParser : UnaryParser
	{
		bool skipMatches;
		public int Minimum { get; set; }

		public int Maximum { get; set; }

		public bool Skip { get; set; }

		public bool Capture { get; set; }


		protected UntilParser(UntilParser other, ParserCloneArgs args)
			: base(other, args)
		{
			Minimum = other.Minimum;
			Maximum = other.Maximum;
			Skip = other.Skip;
			Capture = other.Capture;
		}

		public UntilParser()
		{
			Maximum = Int32.MaxValue;
		}

		public UntilParser(int minimum, int maximum = Int32.MaxValue)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public UntilParser(Parser inner, int minimum, int maximum = Int32.MaxValue, bool skip = false, bool capture = false)
			: base(null, inner)
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
			this.Capture = capture;
			this.Skip = skip;
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (args.Push(this))
			{
				skipMatches = this.Children().Any(r => r.Name != null);

				args.Pop();
			}
		}

		protected override int InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			int count = 0;
			var pos = scanner.Position;
			int length = 0;

			while (count < Maximum)
			{
				int curPos = scanner.Position;
				int stopMatch;
				if (skipMatches)
				{
					args.Push();
					stopMatch = Inner.Parse(args);
					args.PopFailed();
				}
				else
				{
					stopMatch = Inner.Parse(args);
				}
				if (stopMatch >= 0)
				{
					if (Capture)
						length += stopMatch;
					else if (!Skip)
						scanner.Position = curPos;
					break;
				}

				var ofs = scanner.Advance(1);
				if (ofs >= 0)
				{
					length++;
					count++;
				}
				else
				{
					scanner.Position = curPos;
					break;
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
			return new UntilParser(this, args);
		}
	}
}
