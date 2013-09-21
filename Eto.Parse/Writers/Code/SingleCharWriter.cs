using System;
using Eto.Parse.Parsers;
using System.Text;
using System.Linq;

namespace Eto.Parse.Writers.Code
{
	public class SingleCharWriter : InverseWriter<SingleCharTerminal>
	{
		public override void WriteContents(TextParserWriterArgs args, SingleCharTerminal tester, string name)
		{
			base.WriteContents(args, tester, name);
			args.Output.WriteLine("{0}.Character = (char)0x{1:x}; // {2}", name, (int)tester.Character, tester.Character);
		}
	}
}

