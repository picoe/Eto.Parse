namespace Eto.Parse.Parsers
{
    public class SurrogatePairRangeTerminal : SurrogatePairTerminal
    {
        private readonly int _min;
        private readonly int _max;

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