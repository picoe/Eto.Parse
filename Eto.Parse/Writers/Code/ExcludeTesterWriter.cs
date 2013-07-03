using System;
using Eto.Parse.Testers;

namespace Eto.Parse.Writers.Code
{
	public class ExcludeTesterWriter : TesterWriter<ExcludeTester>
	{
		public override void WriteContents(TextParserWriterArgs args, ExcludeTester tester, string name)
		{
			base.WriteContents(args, tester, name);
			args.Output.WriteLine("{0}.Include = {1};", name, args.Write(tester.Include));
			args.Output.WriteLine("{0}.Exclude = {1};", name, args.Write(tester.Exclude));
			args.Output.WriteLine("{0}.IncludeNegative = {1};", name, tester.IncludeNegative.ToString().ToLowerInvariant());
			args.Output.WriteLine("{0}.ExcludeNegative = {1};", name, tester.ExcludeNegative.ToString().ToLowerInvariant());
		}
	}
}

