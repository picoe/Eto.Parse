using System;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Samples
{
	public class JsonGrammar : Grammar
	{
		public JsonGrammar()
			: base("json")
		{
			// terminals
			var jstring = new GroupParser("\"");
			var jnumber = new NumberParser { AllowExponent = true, AllowSign = true, AllowDecimal = true };
			var comma = Terminals.String(",");

			// nonterminals (things we're interested in getting back)
			var jobject = new NamedParser("Object"); 
			var jarray = new NamedParser("Array");
			var jvalue = new NamedParser("Value");
			var jprop = new NamedParser("Property"); 

			var ws = -(Terminals.Set(" \n\r"));

			// rules
			var jobjectBr = "{" & ~jobject & "}";
			var jarrayBr = "[" & ~jarray & "]";

			jvalue.Inner = jstring | jnumber | jobjectBr | jarrayBr | "true" | "false" | "null";
			jobject.Inner = jprop & -(comma & jprop);
			jprop.Inner = jstring & ":" & jvalue;
			jarray.Inner = jvalue & -(comma & jvalue);

			// separate sequence and repeating parsers by whitespace
			jvalue.SeparateChildrenBy(ws);

			// allow whitespace before and after
			this.Inner = ws & jvalue & ws;

			// turn off character errors, named parser errors are granular enough
			SetError<CharParser>(false);
		}
	}
}

