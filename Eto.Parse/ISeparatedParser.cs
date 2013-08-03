using System;
using Eto.Parse;

namespace Eto.Parse.Parsers
{
	public interface ISeparatedParser
	{
		Parser Separator { get; set; }
	}
}
