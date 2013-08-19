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
			EnableMatchEvents = false;

			// terminals
			var jstring = new StringParser { AllowEscapeCharacters = true, Name = "string" };
			var jnumber = new NumberParser { AllowExponent = true, AllowSign = true, AllowDecimal = true, Name = "number" };
			var jboolean = new BooleanTerminal { Name = "bool", TrueValues = new string[] { "true" }, FalseValues = new string[] { "false" } };
			var jname = new StringParser { AllowEscapeCharacters = true, Name = "name" };
			var comma = Terminals.Literal(",");
			var ws = -Terminals.WhiteSpace;

			// nonterminals (things we're interested in getting back)
			var jobject = new SequenceParser { Name = "object" }; 
			var jarray = new SequenceParser { Name = "array" };
			var jprop = new SequenceParser { Name = "property" };

			// rules
			var jvalue = jstring | jnumber | jobject | jarray | jboolean | "null";
			jobject.Add("{", (-jprop).SeparatedBy(ws & comma & ws), "}");
			jprop.Add(jname, ":", jvalue);
			jarray.Add("[", (-jvalue).SeparatedBy(ws & comma & ws), "]");

			// separate sequence and repeating parsers by whitespace
			jvalue.SeparateChildrenBy(ws, false);

			// allow whitespace before and after the initial object or array
			this.Inner = ws & (jobject | jarray) & ws;
		}
	}
}

