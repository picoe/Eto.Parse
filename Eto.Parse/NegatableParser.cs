using System;

namespace Eto.Parse
{
	public abstract class NegatableParser : Parser
	{
		public bool Negative { get; set; }

		protected NegatableParser(NegatableParser other)
		{
			Negative = other.Negative;
		}

		public NegatableParser()
		{
		}

		public static NegatableParser operator !(NegatableParser parser)
		{
			parser.Negative = !parser.Negative;
			return parser;
		}
	}
}
