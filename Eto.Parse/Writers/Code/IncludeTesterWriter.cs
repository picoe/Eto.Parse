using System;
using Eto.Parse.Testers;

namespace Eto.Parse.Writers.Code
{
	public class IncludeTesterWriter : TesterWriter<IncludeTester>
	{
		public override void WriteContents(TextParserWriterArgs args, IncludeTester tester, string name)
		{
			base.WriteContents(args, tester, name);
			args.Output.WriteLine("{0}.First = {1};", name, args.Write(tester.First));
			args.Output.WriteLine("{0}.Second = {1};", name, args.Write(tester.Second));
			args.Output.WriteLine("{0}.FirstNegative = {1};", name, tester.FirstNegative.ToString().ToLowerInvariant());
			args.Output.WriteLine("{0}.SecondNegative = {1};", name, tester.SecondNegative.ToString().ToLowerInvariant());
		}
	}
}

