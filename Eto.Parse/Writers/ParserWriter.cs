using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers
{
	public interface IParserWriter
	{
		string WriteParser(ParserWriterArgs args, Parser parser);
	}

	public class ParserWriter<T_Args> : IParserWriter
		where T_Args: ParserWriterArgs
	{
		public IDictionary<Type, IParserWriterHandler<T_Args>> Writers { get; private set; }

		public ParserWriter(IDictionary<Type, IParserWriterHandler<T_Args>> writers)
		{
			Writers = writers;
		}

		public ParserWriter()
		{
			Writers = new Dictionary<Type, IParserWriterHandler<T_Args>>();
		}

		public virtual string WriteParser(T_Args args, Parser parser)
		{
			var type = parser.GetType();
			while (type != null)
			{
				IParserWriterHandler<T_Args> handler;
				if (Writers.TryGetValue(type, out handler))
				{
					return handler.Write(args, parser);
					break;
				}
				type = type.BaseType;
			}
			return null;
		}

		string IParserWriter.WriteParser(ParserWriterArgs args, Parser parser)
		{
			return WriteParser((T_Args)args, parser);
		}
	}
}

