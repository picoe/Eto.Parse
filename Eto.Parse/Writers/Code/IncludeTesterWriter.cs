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
			args.Output.WriteLine("{0}.FirstInverse = {1};", name, tester.FirstInverse.ToString().ToLowerInvariant());
			args.Output.WriteLine("{0}.SecondInverse = {1};", name, tester.SecondInverse.ToString().ToLowerInvariant());
		}
	}
}

