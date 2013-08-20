using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{
	public class ParserWriter<T> : TextParserWriter.IParserWriterHandler
		where T: Parser
	{
		public virtual string GetName(ParserWriterArgs args, T parser)
		{
			var type = parser.GetType();
			var name = type.Name;
			if (name.EndsWith("Parser"))
				name = name.Substring(0, name.LastIndexOf("Parser"));
			if (args.Parsers.Contains(parser))
				name += "(recursive)";
			return name;
		}

		public virtual void WriteObject(TextParserWriterArgs args, T parser, string name)
		{
			args.Output.WriteLine(name);
		}

		public virtual void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
		}

		string TextParserWriter.IParserWriterHandler.Write(TextParserWriterArgs args, Parser parser)
		{
			var name = GetName(args, (T)parser);
			WriteObject(args, (T)parser, name);
			if (!args.Parsers.Contains(parser))
			{
				args.Push(parser);
				WriteContents(args, (T)parser, name);
				args.Pop();
			}
			return name;
		}
	}
	
}
