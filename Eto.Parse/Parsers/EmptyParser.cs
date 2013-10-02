
namespace Eto.Parse.Parsers
{
	public class EmptyParser : Parser
	{
		protected EmptyParser(EmptyParser other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public EmptyParser()
		{
		}

		protected override int InnerParse(ParseArgs args)
		{
			return 0;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new EmptyParser(this, args);
		}
	}
}

