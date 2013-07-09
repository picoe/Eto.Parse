using System;
using Eto.Parse.Testers;
using System.Text;
using System.Linq;

namespace Eto.Parse.Writers.Code
{
	public class CharSetTesterWriter : TesterWriter<CharSetTester>
	{
		public override void WriteContents(TextParserWriterArgs args, CharSetTester tester, string name)
		{
			base.WriteContents(args, tester, name);
			args.Output.WriteLine("{0}.Characters = new char[] {{ {1} }}; // {2}", 
			                      name, 
			                      string.Join(", ", tester.Characters.Select(r => string.Format("(char)0x{0:x}", (int)r))),
			                      new string(tester.Characters));
		}
	}
}

