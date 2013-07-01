using System;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Scanners;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse
{
	public abstract partial class Parser : ICloneable
	{
		internal bool Reusable { get; set; }

		public object Context { get; set; }

		public event Action<ParseMatch> Succeeded;

		protected virtual void OnSucceeded(ParseMatch parseMatch)
		{
			if (Succeeded != null)
				Succeeded(parseMatch);
		}

		public Parser()
		{
		}

		protected Parser(Parser other)
		{
			Context = other.Context;
		}

		public ContainerMatch Match(string value, bool match = true)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			return Match(new StringScanner(value), match);
		}

		public ContainerMatch Match(Scanner scanner, bool match = true)
		{
			if (scanner == null)
				throw new ArgumentNullException("scanner");
			var args = new ParseArgs(scanner);
			args.Push((Parser)null);
			var top = Parse(args);
			ContainerMatch containerMatch;
			if (top != null)
			{
				var matches = args.Pop(top.Success);
				containerMatch = new ContainerMatch(scanner, top.Index, top.Length, matches);
			}
			else {
				containerMatch = new ContainerMatch(scanner, -1, -1);
				containerMatch.Error = args.Error;
			}

			if (match)
			{
				containerMatch.PreMatch();
				containerMatch.Match();
			}
			return containerMatch;
		}

		public ParseMatch Parse(ParseArgs args)
		{
			var match = InnerParse(args);
			if (match != null)
				OnSucceeded(match);
			else if (args.Error == null || args.Scanner.Offset > args.Error.Index)
			{
				args.Error = new ParseMatch(args.Scanner, args.Scanner.Offset, -1);
			}

			return match;
		}

		public abstract IEnumerable<NamedParser> Find(string parserId);

		public NamedParser this [string parserId]
		{
			get { return Find(parserId).FirstOrDefault(); }
		}

		protected abstract ParseMatch InnerParse(ParseArgs args);

		public abstract Parser Clone();

		object ICloneable.Clone()
		{
			return Clone();
		}
	}

	public class ParserCollection : List<Parser>
	{
		
	}
}
