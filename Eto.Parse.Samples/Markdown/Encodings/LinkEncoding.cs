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
			var name = "[" & (+Terminals.Set(']').Inverse().Except(Terms.eol)).WithName("name") & "]";
			var reference = ~(Terms.sporht) & "[" & (+Terminals.Set(']').Inverse().Except(Terms.eol)).WithName("ref") & "]";
			var title = new StringParser { Name = "title", BeginQuoteCharacters = "\"'(".ToCharArray(), EndQuoteCharacters = "\"')".ToCharArray() };
			var enclosedLink = new StringParser { Name = "link", BeginQuoteCharacters = "<".ToCharArray(), EndQuoteCharacters = ">".ToCharArray() };
			var openLink = (+Terminals.Set(") \t").Inverse().Except(Terms.eol)).WithName("link");
			var link = "(" & (enclosedLink | openLink) & ~(Terms.ws & title) & ")";

			Add(enclosedLink, name & (reference | (link & ~(Terms.ows & ~(Terms.eol & Terms.ows) & title))));
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
				link = name = match.Matches[0];
			else
			{
				name = match.Matches[0];
				link = match.Matches[1];
			}
			MarkdownReference reference;
			string url;
			string title;
			if (link.Name == "ref")
			{
				if (args.References.TryGetValue(link.Text, out reference))
				{
					url = reference.Url;
					title = reference.Title;
				}
				else
					url = title = null;
			}
			else
			{
				url = link.Text;
				if (count > 2)
					title = match.Matches[2].StringValue;
				else
					title = null;
			}
			if (url != null)
			{
				args.Output.Append("<a href=\"");
				args.Output.Append(EscapeEncoding.Encode(MarkdownEncoding.HtmlAttributeEncode(url)));
				args.Output.Append("\"");
				if (!string.IsNullOrEmpty(title))
				{
					args.Output.Append(" title=\"");
					args.Output.Append(EscapeEncoding.Encode(MarkdownEncoding.HtmlAttributeEncode(title)));
					args.Output.Append("\"");
				}
				args.Output.Append(">");
				args.Encoding.Replace(name.Text, args);
				args.Output.Append("</a>");
			}
		}
	}
	
}