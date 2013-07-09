using System;
using Eto.Parse.Testers;

namespace Eto.Parse.Writers.Code
{
	public class RangeTesterWriter : TesterWriter<RangeTester>
	{
		public override void WriteContents(TextParserWriterArgs args, RangeTester tester, string name)
		{
			base.WriteContents(args, tester, name);
			args.Output.WriteLine("{0}.Start = (char)0x{1:x};", name, (int)tester.Start);
			args.Output.WriteLine("{0}.End = (char)0x{1:x};", name, (int)tester.End);
		}
	}
}