namespace Eto.Parse.Parsers
{
    public class AnySurrogatePairTerminal : Parser
    {
        public AnySurrogatePairTerminal()
        {
        }

        protected AnySurrogatePairTerminal(AnySurrogatePairTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

        protected override int InnerParse(ParseArgs args)
        {
            var scanner = args.Scanner;

            var highSurrogate = scanner.ReadChar();
            if (highSurrogate > 0 && char.IsHighSurrogate((char)highSurrogate))
            {
                var lowSurrogate = scanner.ReadChar();
                if (lowSurrogate > 0 && char.IsLowSurrogate((char)lowSurrogate))
                {
                    return 2;
                }

                scanner.Position -= 2;
            }

            return -1;
        }

        public override Parser Clone(ParserCloneArgs args)
        {
            return new AnySurrogatePairTerminal(this, args);
        }
    }
}