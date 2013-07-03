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
		public static Parser DefaultSeparator { get; set; }

		internal bool Reusable { get; set; }

		public object Context { get; set; }

		public event Action<ParseMatch> Succeeded;

		public virtual string GetErrorMessage()
		{
			return string.Format("Expected {0}", GetDescriptiveName());
		}

		public string GetDescriptiveName(HashSet<Parser> parents = null)
		{
			parents = new HashSet<Parser>();
			if (!parents.Contains(this))
			{
				parents.Add(this);
				var name = GetDescriptiveNameInternal(parents);
				parents.Remove(this);
				return name;
			}
			return "(recursive)";
		}

		protected virtual string GetDescriptiveNameInternal(HashSet<Parser> parents)
		{
			var type = GetType();
			var name = type.Name;
			if (type.Assembly == typeof(Parser).Assembly && name.EndsWith("Parser"))
				name = name.Substring(0, name.LastIndexOf("Parser"));
			return name;
		}

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

		public ContainerMatch Match(IScanner scanner, bool match = true)
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
			else
			{
				var nodePosition = args.NodePosition;
				if (args.Error == null)
					args.Error = new ParseError(args.Scanner, nodePosition);
				else if (nodePosition > args.Error.Index)
					args.Error.Reset(nodePosition);
				if (nodePosition == args.Error.Index)
					args.Error.AddError(this);
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
