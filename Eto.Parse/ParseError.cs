using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public struct ParseErrorMessage
	{
		Parser parser;
		ParseError error;
		public Parser Parser { get { return parser; } }

		public string Message { get { return Parser.GetErrorMessage(); } }

		public ParseError Error { get { return error; } }

		public ParseErrorMessage(Parser parser, ParseError error)
		{
			this.parser = parser;
			this.error = error;
		}
	}

	public class ParseError
	{
		List<ParseErrorMessage> errors;

		public IScanner Scanner { get; private set; }

		public IEnumerable<ParseErrorMessage> Errors { get { return errors; } }

		public ParseError Parent { get; private set; }

		public IEnumerable<string> Messages
		{
			get { return Errors.Select(r => r.Message); }
		}

		public int Index { get; set; }

		public ParseError LastError
		{
			get
			{
				if (errors.Count > 0)
				{
					var children = errors.Select(r => r.Error).Where(r => r != null);
					if (children.Any())
					{
						return children.Select(r => r.LastError).MaxBy(r => r.Index);
					}
				}
				return this;
			}
		}

		public ParseError(IScanner scanner, int index)
		{
			Scanner = scanner;
			Reset(index);
		}

		public void AddError(Parser parser, ParseError error)
		{
			if (error != null && error.Parent == null)
			{
				error.Parent = this;
				errors.Add(new ParseErrorMessage(parser, error));
			}
			else
				errors.Add(new ParseErrorMessage(parser, null));
		}

		public void Reset(int index)
		{
			errors = new List<ParseErrorMessage>();
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
			var messages = string.Join("\n", Messages.Where(r => r != null));
			return string.Format("Index={0}, Context=\"{1}\", Expected one of:\n{2}", Index, context, messages);
		}
	}
}
