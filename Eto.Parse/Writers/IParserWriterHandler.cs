using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers
{
	public interface IParserWriterHandler<T>
		where T: ParserWriterArgs
	{
		string Write(T args, Parser parser);
	}
}
