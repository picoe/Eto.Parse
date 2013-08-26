using System;
using System.Text;

namespace Eto.Parse.Samples.Markdown
{
	static class Extensions
	{
		public static void AppendUnixLine(this StringBuilder builder)
		{
			builder.Append('\n');
		}

		public static void AppendUnixLine(this StringBuilder builder, string line)
		{
			builder.Append(line);
			builder.Append('\n');
		}
	}
}

