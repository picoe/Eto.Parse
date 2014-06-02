namespace Eto.Parse.Parsers
{
    public class SurrogatePairTerminal : Parser
    {
        private readonly char _lowSurrogate;
        private readonly char _highSurrogate;

        public SurrogatePairTerminal(int codePoint)
        {
            var unicodeString = char.ConvertFromUtf32(codePoint);

            _highSurrogate = unicodeString[0];
            _lowSurrogate = unicodeString[1];
        }

        protected SurrogatePairTerminal(Parser other, ParserCloneArgs args) 
            : base(other, args)
        {
        }

        protected override int InnerParse(ParseArgs args)
        {
            var scanner = args.Scanner;

            var highSurrogate = scanner.ReadChar();
            if (highSurrogate > 0 && char.IsHighSurrogate((char)highSurrogate) && highSurrogate == _highSurrogate)
            {
                var lowSurrogate = scanner.ReadChar();
                if (lowSurrogate > 0 && char.IsLowSurrogate((char)lowSurrogate) && lowSurrogate == _lowSurrogate)
                {
                    return 2;
                }

                scanner.Position -= 2;
            }

            return -1;
        }

        public override Parser Clone(ParserCloneArgs args)
        {
            return new SurrogatePairTerminal(this, args);
        }
    }
}