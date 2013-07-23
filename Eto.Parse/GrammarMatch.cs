using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class GrammarMatch : NamedMatch
	{
		public int ErrorIndex { get; private set; }

		public IEnumerable<Parser> Errors { get; private set; }

		public GrammarMatch(Grammar grammar, Scanner scanner, int index, int length, NamedMatchCollection matches, int errorIndex, IEnumerable<Parser> errors)
			: base(grammar, scanner, index, length, matches)
		{
			this.Errors = errors;
			this.ErrorIndex = errorIndex;
		}

		public string GetErrorContext(int count, string indicator = ">>>")
		{
			var before = Scanner.SubString(Math.Max(0, ErrorIndex - count), (int)Math.Min(ErrorIndex, count));
			var after = Scanner.SubString(ErrorIndex, count);
			return before + indicator + after;
		}

		public string ErrorMessage
		{
			get {
				var context = GetErrorContext(10);
				var messages = string.Join("\n", Errors.Select(r => r.GetErrorMessage()));
				return string.Format("Index={0}, Context=\"{1}\"\nExpected:\n{2}", ErrorIndex, context, messages);
			}
		}
	}
}

