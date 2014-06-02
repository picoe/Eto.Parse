namespace Eto.Parse.Parsers
{
    public abstract class SurrogatePairTerminal : Parser, IInverseParser
    {
        protected SurrogatePairTerminal()
        {
        }

        protected SurrogatePairTerminal(SurrogatePairTerminal other, ParserCloneArgs args) 
            : base(other, args)
        {
            Inverse = other.Inverse;
        }

        public bool Inverse { get; set; }

        protected override int InnerParse(ParseArgs args)
        {
            var scanner = args.Scanner;

            var highSurrogate = scanner.ReadChar();
            if (highSurrogate > 0 && char.IsHighSurrogate((char)highSurrogate) 
                && TestHighSurrogate(highSurrogate) != Inverse)
            {
                var lowSurrogate = scanner.ReadChar();
                if (lowSurrogate > 0 && char.IsLowSurrogate((char)lowSurrogate)
                    && TestLowSurrogate(lowSurrogate) != Inverse)
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