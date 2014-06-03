using System;

namespace Eto.Parse.Parsers
{
    /// <summary>
    /// Base parser for matching UTF-32 surrogate pair characters
    /// </summary>
    public abstract class SurrogatePairTerminal : Parser, IInverseParser
    {
        protected const int MinCodePoint = 0x10000;
        protected const int MaxCodePoint = 0x10FFFF;

        protected SurrogatePairTerminal()
        {
        }

        protected SurrogatePairTerminal(SurrogatePairTerminal other, ParserCloneArgs args) 
            : base(other, args)
        {
            Inverse = other.Inverse;
        }

        public bool Inverse { get; set; }

        public override string DescriptiveName
        {
            get { return string.Format("UTF code point {0}", CharName); }
        }

        protected virtual string CharName
        {
            get { return string.Empty; }
        }

        protected override int InnerParse(ParseArgs args)
        {
            int codePoint;

            if (!args.Scanner.IsEof && TryGetCodePoint(args.Scanner, out codePoint))
            {
                if (Test(codePoint) != Inverse)
                {
                    return 2;
                }
            }

            return -1;
        }

        public override object GetValue(string text)
        {
            return char.ConvertToUtf32(text, 0);
        }

        protected void AssertValidSurrogatePair(int codePoint)
        {
            if (codePoint < MinCodePoint || codePoint > MaxCodePoint)
            {
                throw new ArgumentOutOfRangeException("codePoint", string.Format("Invalid UTF code point: '{0}'", codePoint));
            }
        }

        protected abstract bool Test(int codePoint);

        private static bool TryGetCodePoint(Scanner scanner, out int codePoint)
        {
            codePoint = 0;
            var validCodePoint = false;

            var highSurrogate = scanner.ReadChar();
            if (highSurrogate > 0 && char.IsHighSurrogate((char) highSurrogate))
            {
                var lowSurrogate = scanner.ReadChar();
                if (lowSurrogate > 0 && char.IsLowSurrogate((char) lowSurrogate))
                {
                    codePoint = char.ConvertToUtf32((char) highSurrogate, (char) lowSurrogate);
                    validCodePoint = true;
                }
                else
                {
                    scanner.Position -= 2;
                }
            }
            else
            {
                scanner.Position--;
            }

            return validCodePoint;
        }
    }
}