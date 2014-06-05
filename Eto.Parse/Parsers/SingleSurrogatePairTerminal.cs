namespace Eto.Parse.Parsers
{
    /// <summary>
    /// Parser, which matches a single UTF-32 surrogate pair character
    /// </summary>
    public sealed class SingleSurrogatePairTerminal : SurrogatePairTerminal
    {
        private readonly int _codePoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSurrogatePairTerminal"/> class.
        /// </summary>
        /// <param name="codePoint">The UTF-32 code point to match.</param>
        public SingleSurrogatePairTerminal(int codePoint)
        {
            AssertValidSurrogatePair(codePoint);
            _codePoint = codePoint;
        }

        private SingleSurrogatePairTerminal(SingleSurrogatePairTerminal other, ParserCloneArgs args) 
            : base(other, args)
        {
        }

        public override Parser Clone(ParserCloneArgs args)
        {
            return new SingleSurrogatePairTerminal(this, args);
        }

        protected override string CharName
        {
            get { return string.Format("0x{0:x2}", _codePoint); }
        }

        protected override bool Test(int codePoint)
        {
            return codePoint == _codePoint;
        }
    }
}