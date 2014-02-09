using System;
using System.Linq;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Xml
{
	public class XmlGrammar : Grammar
	{
		public XmlGrammar()
			: base("xml")
		{
			EnableMatchEvents = false;
			var comment = new GroupParser("<!--", "-->");
			var ws = Terminals.Repeat(Char.IsWhiteSpace, 1);
			var ows = Terminals.Repeat(Char.IsWhiteSpace, 0);
			var wsc = -(ws | comment);

			var name = Terminals.Repeat(new RepeatCharItem(Char.IsLetter, 1, 1), new RepeatCharItem(Char.IsLetterOrDigit, 0));
			var namedName = Terminals.Repeat(new RepeatCharItem(Char.IsLetter, 1, 1), new RepeatCharItem(Char.IsLetterOrDigit, 0)).WithName("name");

			var text = new UntilParser("<", 1).WithName("text");
			var attributeValue = new StringParser { QuoteCharacters = new [] { '"' }, Name = "value" };
			var attribute = (namedName & ows & "=" & ows & attributeValue);
			var attributes = (ws & (+attribute).SeparatedBy(ws).WithName("attributes")).Optional();

			var content = new RepeatParser { Separator = wsc };

			var startTag = "<" & namedName & attributes & ows;
			var endTag = "</" & name & ">";
			var obj = (startTag & ("/>" | (">" & wsc & content & wsc & endTag))).WithName("object");
			var cdata = ("<![CDATA[" & new UntilParser("]]>", 0, skip: true)).WithName("cdata");
			content.Inner = obj | text | cdata;

			var declaration = "<?" & name & attributes & ows & "?>";
			Inner = declaration & wsc & obj & wsc;
		}
	}
}

