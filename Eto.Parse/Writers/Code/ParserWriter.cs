using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{
	public class ParserWriter<T> : TextParserWriter.IParserWriterHandler
		where T: Parser
	{
		protected virtual bool WriteNewObject { get { return true; } }

		public virtual string GetName(TextParserWriterArgs args, T parser)
		{
			return args.GenerateName(parser);
		}

		public virtual void WriteObject(TextParserWriterArgs args, T parser, string name)
		{
			var type = parser.GetType();
			if (WriteNewObject)
				args.Output.WriteLine("var {0} = new {1}.{2}();", name, type.Namespace, type.Name);
		}

		public virtual void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
		}

		string TextParserWriter.IParserWriterHandler.Write(TextParserWriterArgs args, Parser parser)
		{
			var name = GetName(args, (T)parser);
			if (args.IsDefined(name))
				return name;
			if (!args.Parsers.Contains(parser))
			{
				WriteObject(args, (T)parser, name);
				args.Push(parser);
				WriteContents(args, (T)parser, name);
				args.Pop();
			}
			return name;
		}
	}
	
}
