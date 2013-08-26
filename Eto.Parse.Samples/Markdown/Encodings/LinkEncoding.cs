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
			var name = ~Terminals.Literal("!") & "[" & (+Terminals.Set(']').Inverse().Except(Terms.eol)).WithName("name") & "]";
			var reference = ~(Terms.sporht) & ~Terms.eol & "[" & (-Terminals.Set(']').Inverse().Except(Terms.eol)).WithName("ref") & "]";
			var title = new StringParser { Name = "title", BeginQuoteCharacters = "\"'(".ToCharArray(), EndQuoteCharacters = "\"')".ToCharArray() };
			var enclosedLink = new StringParser { Name = "link", BeginQuoteCharacters = "<".ToCharArray(), EndQuoteCharacters = ">".ToCharArray() };
			var openLink = (+Terminals.Set(") \t").Inverse().Except(Terms.eol)).WithName("link");
			var link = "(" & (enclosedLink | openLink) & ~((Terms.ws | (Terms.ows & Terms.eol & Terms.ows)) & title) & ")";

			Add(enclosedLink, name & ~(reference | link) );
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			Match link, name; 
			var count = match.Matches.Count;
			if (count == 1)
			{
				link = name = match.Matches[0];
			}
			else
			{
				name = match.Matches[0];
				link = match.Matches[1];
			}
			MarkdownReference reference;
			string url;
			string title;
			if (link.Name == "ref" || link.Name == "name")
			{
				var linkValue = link.StringValue;
				if (string.IsNullOrEmpty(linkValue))
					linkValue = name.StringValue;
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
				url = link.StringValue;
				if (count > 2)
					title = match.Matches[2].StringValue;
				else
					title = null;
			}
			if (url != null)
			{
				var isImage = match.Scanner.SubString(match.Index, 1) == "!";
				if (isImage)
					args.Output.Append("<img src=\"");
				else
					args.Output.Append("<a href=\"");
				args.Output.Append(MarkdownEncoding.EncodeAttribute(url));
				args.Output.Append("\"");
				if (isImage)
				{
					var alt = name.StringValue;
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
					args.Encoding.Replace(args, name.StringValue);
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