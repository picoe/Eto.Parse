using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse
{
	public interface IParserWriter
	{
		string WriteParser(ParserWriterArgs args, Parser parser);
		string WriteTester(ParserWriterArgs args, ICharTester tester);
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

		public interface ITesterWriterHandler
		{
			string Write(T_Args args, ICharTester tester);
		}

		public class ParserDictionary : Dictionary<Type, IParserWriterHandler> { }
		public class TesterDictionary : Dictionary<Type, ITesterWriterHandler> { }

		public ParserDictionary ParserWriters { get; private set; }
		public TesterDictionary TesterWriters { get; private set; }

		public ParserWriter(ParserDictionary writers = null, TesterDictionary testers = null)
		{
			ParserWriters = writers ?? new ParserDictionary();
			TesterWriters = testers ?? new TesterDictionary();
		}

		public virtual string WriteTester(T_Args args, ICharTester tester)
		{
			if (tester == null)
				throw new ArgumentNullException("tester");
			var type = tester.GetType();
			ITesterWriterHandler handler;
			while (type != null)
			{
				if (TesterWriters.TryGetValue(type, out handler))
					return handler.Write(args, tester);
				type = type.BaseType;
			}
			if (TesterWriters.TryGetValue(typeof(ICharTester), out handler))
				return handler.Write(args, tester);

			return null;
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

		string IParserWriter.WriteTester(ParserWriterArgs args, ICharTester parser)
		{
			return WriteTester((T_Args)args, parser);
		}

		string IParserWriter.WriteParser(ParserWriterArgs args, Parser parser)
		{
			return WriteParser((T_Args)args, parser);
		}
	}
}

