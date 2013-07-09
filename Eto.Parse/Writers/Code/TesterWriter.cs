using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{
	public class TesterWriter<T> : TextParserWriter.ITesterWriterHandler
		where T: ICharTester
	{
		public virtual string GetName(TextParserWriterArgs args, T tester)
		{
			return args.GenerateName(tester);
		}

		public virtual void WriteObject(TextParserWriterArgs args, T tester, string name)
		{
			var type = tester.GetType();
			args.Output.WriteLine("var {0} = new {1}.{2}();", name, type.Namespace, type.Name);
		}

		public virtual void WriteContents(TextParserWriterArgs args, T tester, string name)
		{
		}

		string TextParserWriter.ITesterWriterHandler.Write(TextParserWriterArgs args, ICharTester tester)
		{
			var name = GetName(args, (T)tester);
			if (args.IsDefined(name))
				return name;
			WriteObject(args, (T)tester, name);
			WriteContents(args, (T)tester, name);
			return name;
		}
	}
}
