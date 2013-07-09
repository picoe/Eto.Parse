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
			if (tester.IncludeInverse)
				args.Output.WriteLine("{0}.IncludeInverse = {1};", name, tester.IncludeInverse.ToString().ToLowerInvariant());
			if (tester.ExcludeInverse)
				args.Output.WriteLine("{0}.ExcludeInverse = {1};", name, tester.ExcludeInverse.ToString().ToLowerInvariant());
		}
	}
}

