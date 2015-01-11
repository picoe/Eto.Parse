using System;
using System.Linq;
using Eto.Parse.Parsers;

namespace Eto.Parse.Writers.Code
{
	public class TagWriter : ParserWriter<TagParser>
	{
		public override void WriteContents(TextParserWriterArgs args, TagParser parser, string name)
		{
			base.WriteContents(args, parser, name);
			if (parser.AllowWithDifferentPosition)
				args.Output.WriteLine("{0}.AllowWithDifferentPosition = {1};", name, parser.AllowWithDifferentPosition.ToString().ToLowerInvariant());
			if (!string.IsNullOrEmpty(parser.AddTag))
				args.Output.WriteLine("{0}.AddTag = \"{1}\";", name, parser.AddTag);
			if (!string.IsNullOrEmpty(parser.ExcludeTag))
				args.Output.WriteLine("{0}.ExcludeTag = \"{1}\";", name, parser.ExcludeTag);
			if (!string.IsNullOrEmpty(parser.IncludeTag))
				args.Output.WriteLine("{0}.IncludeTag = \"{1}\";", name, parser.IncludeTag);
		}
	}
}

