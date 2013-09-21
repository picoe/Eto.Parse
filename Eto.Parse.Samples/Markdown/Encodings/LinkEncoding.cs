using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using System.Net;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class LinkEncoding : AlternativeParser, IMarkdownReplacement
	{
		public LinkEncoding()
		{
			Name = "link";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var inner = grammar.Encoding.Replacements();
			var name = ~Terminals.Literal("!") & "[" & (+(inner | Terminals.Set("]").Inverse())).WithName("name") & "]";
			var reference = ~(Terms.sporht | Terms.eol) & "[" & (-Terminals.Set("]\n\r").Inverse()).WithName("ref") & "]";
			var title = new StringParser { Name = "title", BeginQuoteCharacters = "\"'(".ToCharArray(), EndQuoteCharacters = "\"')".ToCharArray() };
			var enclosedLink = Terminals.Literal("\\").Not() & "<" & (Terminals.Set(">\r\n ").Inverse().Except("\\>").Repeat(1)).WithName("link") & ">";
			var openLink = (+("\\)" | Terminals.Set(") \t\r\n").Inverse())).WithName("link");
			var link = "(" & Terms.ows & (enclosedLink | openLink | new EmptyParser().WithName("link")) & ~((Terms.ws | (Terms.ows & Terms.eol & Terms.ows)) & title) & Terms.ows & ")";

			Add(enclosedLink, name & ~(reference | link) );
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			Match link; 
			string linkValue, nameValue;
			var count = match.Matches.Count;
			if (count == 1)
			{
				link = match.Matches[0];
				linkValue = nameValue = link.StringValue;
			}
			else
			{
				nameValue = match.Matches[0].Text;
				link = match.Matches[1];
				linkValue = link.StringValue;
			}
			MarkdownReference reference;
			string url;
			string title;
			if (link.Name == "ref" || link.Name == "name")
			{
				if (string.IsNullOrEmpty(linkValue))
					linkValue = nameValue;
				if (args.References.TryGetValue(linkValue, out reference))
				{
					url = reference.Url;
					title = reference.Title;
				}
				else
					url = title = null;
			}
			else
			{
				url = linkValue;
				if (count > 2)
					title = match.Matches[2].StringValue;
				else
					title = null;
			}
			if (url != null)
			{
				var isImage = match.Scanner.Substring(match.Index, 1) == "!";
				if (isImage)
					args.Output.Append("<img src=\"");
				else
					args.Output.Append("<a href=\"");
				args.Output.Append(MarkdownEncoding.EncodeAttribute(url));
				args.Output.Append("\"");
				if (isImage)
				{
					var alt = nameValue;
					if (!string.IsNullOrEmpty(alt))
					{
						args.Output.Append(" alt=\"");
						args.Output.Append(MarkdownEncoding.EncodeAttribute(alt));
						args.Output.Append("\"");
					}
				}
				if (!string.IsNullOrEmpty(title))
				{
					args.Output.Append(" title=\"");
					args.Output.Append(MarkdownEncoding.EncodeAttribute(title));
					args.Output.Append("\"");
				}
				if (isImage)
					args.Output.Append(" />");
				else
				{
					args.Output.Append(">");
					args.Encoding.Transform(args, nameValue);
					args.Output.Append("</a>");
				}
			}
			else
			{
				args.Output.Append(match.Text);
			}
		}
	}
	
}