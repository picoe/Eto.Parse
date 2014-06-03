namespace Eto.Parse.Parsers
{
    /// <summary>
    /// Parser, which matches any UTF-32 surrogate pair character
    /// </summary>
    public sealed class AnySurrogatePairTerminal : SurrogatePairTerminal
    {
        public AnySurrogatePairTerminal()
        {
        }

        private AnySurrogatePairTerminal(AnySurrogatePairTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

        public override Parser Clone(ParserCloneArgs args)
        {
            return new AnySurrogatePairTerminal(this, args);
        }

        protected override bool Test(int codePoint)
        {
            return true;
        }
    }
}