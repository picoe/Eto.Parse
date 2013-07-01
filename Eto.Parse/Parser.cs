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
			var matches = args.Pop(top.Success);

			var containerMatch = new ContainerMatch(scanner, top.Index, top.Length, matches);
			containerMatch.Error = top.Success ? null : args.Error;
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
			if (match == null)
				throw new InvalidOperationException("Parser returned a null match");
			if (match.Success)
				OnSucceeded(match);
			else if (args.Error == null || match.Index > args.Error.Index)
			{
				args.Error = match;
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
