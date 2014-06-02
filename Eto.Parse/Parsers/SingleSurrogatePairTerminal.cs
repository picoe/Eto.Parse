using System;

namespace Eto.Parse.Parsers
{
    public class SingleSurrogatePairTerminal : SurrogatePairTerminal
    {
        private const int MinCodePoint = 0x10000;
        private const int MaxCodePoint = 0x10FFFF;

        private readonly char _lowSurrogate;
        private readonly char _highSurrogate;

        public SingleSurrogatePairTerminal(int codePoint)
        {
            if (codePoint < MinCodePoint || codePoint > MaxCodePoint)
            {
                throw new ArgumentOutOfRangeException("codePoint", "Invalid UTF code point");
            }

            var unicodeString = char.ConvertFromUtf32(codePoint);

            _highSurrogate = unicodeString[0];
            _lowSurrogate = unicodeString[1];
        }

        protected SingleSurrogatePairTerminal(SingleSurrogatePairTerminal other, ParserCloneArgs args) 
            : base(other, args)
        {
        }

        public override Parser Clone(ParserCloneArgs args)
        {
            return new SingleSurrogatePairTerminal(this, args);
        }

        protected override bool TestLowSurrogate(int lowSurrogate)
        {
            return lowSurrogate == _lowSurrogate;
        }

        protected override bool TestHighSurrogate(int highSurrogate)
        {
            return highSurrogate == _highSurrogate;
        }
    }
}