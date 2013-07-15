using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples
{
	public class JsonGrammar : Grammar
	{
		public JsonGrammar()
			: base("json")
		{
			var jstring = new GroupParser("\"");
			var jnumber = new NumberParser { AllowExponent = true, AllowSign = true };
			var comma = Terminals.String(",");

			//Nonterminals
			var jobject = new NamedParser("Object"); 
			var jobjectBr = new UnaryParser();
			var jarray = new NamedParser("Array");
			var jarrayBr = new UnaryParser();
			var jvalue = new NamedParser("Value");
			var jprop = new NamedParser("Property"); 

			var ws = -(Terminals.Set(" \n\r"));
			Parser.DefaultSeparator = ws;

			//Rules
			jvalue.Inner = jstring | jnumber | jobjectBr | jarrayBr | "true" | "false" | "null";
			jobjectBr.Inner = "{" & ~jobject & "}";
			jobject.Inner = jprop & -(comma & jprop);
			jprop.Inner = jstring & ":" & jvalue;
			jarrayBr.Inner = "[" & ~jarray & "]";
			jarray.Inner = jvalue & -(comma & jvalue);

			this.Inner = ws & jvalue;
			Parser.DefaultSeparator = null;
		}
	}
}

