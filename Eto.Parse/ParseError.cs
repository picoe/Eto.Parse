using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public struct ParseErrorMessage
	{
		Parser parser;
		public Parser Parser { get { return parser; } }

		public string Message { get { return Parser.GetErrorMessage(); } }

		public ParseErrorMessage(Parser parser)
		{
			this.parser = parser;
		}
	}

	public class ParseError
	{
		List<ParseErrorMessage> errors;

		public IScanner Scanner { get; private set; }

		public IEnumerable<ParseErrorMessage> Errors { get { return errors; } }

		public IEnumerable<string> Messages
		{
			get { return Errors.Select(r => r.Message); }
		}

		public long Index { get; set; }

		public ParseError(IScanner scanner, long index)
		{
			Scanner = scanner;
			Reset(index);
		}

		public void AddError(Parser parser)
		{
			errors.Add(new ParseErrorMessage(parser));
		}

		public void Reset(long index)
		{
			errors = new List<ParseErrorMessage>(10);
			Index = index;
		}

		public string GetStringBefore(int count)
		{
			return Scanner.SubString(Math.Max(0, Index - count), (int)Math.Min(Index, count));
		}
		public string GetStringAfter(int count)
		{
			return Scanner.SubString(Index, count);
		}

		public override string ToString()
		{
			var context = GetStringBefore(10) + ">>>" + GetStringAfter(10);
			var messages = string.Join("\n", Messages);
			return string.Format("Index={0}, Context=\"{1}\", Messages={2}", Index, context, messages);
		}
	}
}
