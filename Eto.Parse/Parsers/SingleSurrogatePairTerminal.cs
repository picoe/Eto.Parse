namespace Eto.Parse.Parsers
{
    public class SingleSurrogatePairTerminal : SurrogatePairTerminal
    {
        private readonly int _codePoint;

        public SingleSurrogatePairTerminal(int codePoint)
        {
            AssertValidSurrogatePair(codePoint);
            _codePoint = codePoint;
        }

        protected SingleSurrogatePairTerminal(SingleSurrogatePairTerminal other, ParserCloneArgs args) 
            : base(other, args)
        {
        }

        public override Parser Clone(ParserCloneArgs args)
        {
            return new SingleSurrogatePairTerminal(this, args);
        }

        protected override bool Test(int codePoint)
        {
            return codePoint == _codePoint;
        }
    }
}