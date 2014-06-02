namespace Eto.Parse.Parsers
{
    public abstract class SurrogatePairTerminal : Parser
    {
        protected SurrogatePairTerminal()
        {
        }

        protected SurrogatePairTerminal(Parser other, ParserCloneArgs args) 
            : base(other, args)
        {
        }

        protected override int InnerParse(ParseArgs args)
        {
            var scanner = args.Scanner;

            var highSurrogate = scanner.ReadChar();
            if (highSurrogate > 0 && char.IsHighSurrogate((char)highSurrogate) 
                && TestHighSurrogate(highSurrogate))
            {
                var lowSurrogate = scanner.ReadChar();
                if (lowSurrogate > 0 && char.IsLowSurrogate((char)lowSurrogate) 
                    && TestLowSurrogate(lowSurrogate))
                {
                    return 2;
                }

                scanner.Position -= 2;
            }

            return -1;
        }

        protected abstract bool TestLowSurrogate(int lowSurrogate);

        protected abstract bool TestHighSurrogate(int highSurrogate);
    }
}