using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class EolTerminal : Parser
	{
		protected EolTerminal(EolTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public EolTerminal()
		{
		}

		public override string DescriptiveName
		{
			get { return "EOL"; }
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			var pos = scanner.Position;
			int ch = scanner.ReadChar();
			if (ch != -1)
			{
				if (ch == '\n')
					return new ParseMatch(pos, 1);
				if (ch == '\r')
				{
					ch = scanner.ReadChar();
					if (ch == -1)
						return new ParseMatch(pos, 1);
					if (ch == '\n')
						return new ParseMatch(pos, 2);
					scanner.Position = pos + 1;
					return new ParseMatch(pos, 1);
				}
			}

			scanner.Position = pos;
			return ParseMatch.None;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new EolTerminal(this, args);
		}
	}
}
