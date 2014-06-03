namespace Eto.Parse.Parsers
{
    /// <summary>
    /// Parser, which matches a range of UTF-32 surrogate pair characters
    /// </summary>
    public class SurrogatePairRangeTerminal : SurrogatePairTerminal
    {
        private readonly int _min;
        private readonly int _max;

        /// <summary>
        /// Initializes a new instance of the <see cref="SurrogatePairRangeTerminal"/> class.
        /// </summary>
        /// <param name="min">The minimum UTF code point.</param>
        /// <param name="max">The maximum UTF code point.</param>
        public SurrogatePairRangeTerminal(int min, int max)
        {
            AssertValidSurrogatePair(min);
            AssertValidSurrogatePair(max);

            _min = min;
            _max = max;
        }

        protected SurrogatePairRangeTerminal(SurrogatePairRangeTerminal other, ParserCloneArgs args)
            : base(other, args)
        {
        }

        public override Parser Clone(ParserCloneArgs args)
        {
            return new SurrogatePairRangeTerminal(this, args);
        }

        protected override bool Test(int codePoint)
        {
            return codePoint >= _min && codePoint <= _max;
        }
    }
}