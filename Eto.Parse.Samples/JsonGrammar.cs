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
			var jstring = new StringParser { AllowEscapeCharacters = true, Name = "string" };
			var jnumber = new NumberParser { AllowExponent = true, AllowSign = true, AllowDecimal = true, Name = "number" };
			var comma = Terminals.Literal(",");

			// nonterminals (things we're interested in getting back)
			var jobject = new SequenceParser { Name = "object" }; 
			var jarray = new SequenceParser { Name = "array" };
			var jprop = new SequenceParser { Name = "property" };
			var ws = -Terminals.WhiteSpace;

			// rules
			var jobjectBr = "{" & ~jobject & "}";
			var jarrayBr = "[" & ~jarray & "]";

			var jvalue = jstring | jobjectBr | jarrayBr | jnumber | "true" | "false" | "null";
			jobject.Add(jprop, -(comma & jprop));
			jprop.Add(new StringParser { AllowEscapeCharacters = true, Name = "name" }, ":", jvalue);
			jarray.Add(jvalue, -(comma & jvalue));

			// separate sequence and repeating parsers by whitespace
			jvalue.SeparateChildrenBy(ws);

			// allow whitespace before and after
			this.Inner = ws & jvalue & ws;
		}
	}
}

