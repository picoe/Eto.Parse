using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples
{
	public class JsonGrammar : NamedParser
	{
		public JsonGrammar()
			: base("json")
		{
			Parser jstring = ("\"" & (-Terminals.AnyChar).Until("\"") & "\"").Separate();
			Parser jnumber = (~Terminals.Set("+-") & +Terminals.Digit & ~(Terminals.Set('.') & +Terminals.Digit)).Separate();
			//var jbool = "true" | "false";
			var comma = Terminals.Set(',');

			//Nonterminals
			var jobject = new NamedParser("Object"); 
			var jobjectBr = new NamedParser("ObjectBr");
			var jarray = new NamedParser("Array"); 
			var jarrayBr = new NamedParser("ArrayBr");
			var jvalue = new NamedParser("Value");
			var jprop = new NamedParser("Property"); 

			var ws = -(Terminals.WhiteSpace + Terminals.Eol);
			Parser.DefaultSeparator = ws;

			//Rules
			jvalue.Inner = jstring | jnumber | jobjectBr | jarrayBr | "true" | "false" | "null";
			jobjectBr.Inner = "{" & ~jobject & "}";
			jobject.Inner = jprop & -(comma & jprop);
			jprop.Inner = jstring & ":" & jvalue;
			jarrayBr.Inner = "[" & ~jarray & "]";
			jarray.Inner = jvalue & -(comma & jvalue);

			this.Inner = ws & jvalue & ws;
			Parser.DefaultSeparator = null;
		}
	}
}

