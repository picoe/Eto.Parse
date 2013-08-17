using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse
{
	public interface IParserWriter
	{
		string WriteParser(ParserWriterArgs args, Parser parser);
	}

	public class ParserWriter : ParserWriter<ParserWriterArgs>
	{
		public ParserWriter(ParserDictionary writers)
			: base(writers)
		{
		}

		public ParserWriter()
		{
		}
	}

	public class ParserWriter<T_Args> : IParserWriter
		where T_Args: ParserWriterArgs
	{
		public interface IParserWriterHandler
		{
			string Write(T_Args args, Parser parser);
		}

		public class ParserDictionary : Dictionary<Type, IParserWriterHandler> { }

		public ParserDictionary ParserWriters { get; private set; }

		public ParserWriter(ParserDictionary writers = null)
		{
			ParserWriters = writers ?? new ParserDictionary();
		}

		public virtual string WriteParser(T_Args args, Parser parser)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");
			var type = parser.GetType();
			while (type != null)
			{
				IParserWriterHandler handler;
				if (ParserWriters.TryGetValue(type, out handler))
					return handler.Write(args, parser);
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

