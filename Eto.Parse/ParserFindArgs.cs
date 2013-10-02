using System;

namespace Eto.Parse
{
	public class ParserFindArgs : ParserChain
	{
		public string ParserId { get; set; }

		public ParserFindArgs(string parserId)
		{
			this.ParserId = parserId;
		}
	}
}
