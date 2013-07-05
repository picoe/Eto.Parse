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
			return GetDescriptiveName();
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

		public ParseMatch Parse(ParseArgs args)
		{
			var match = InnerParse(args);
			if (match.Success)
				OnSucceeded(match);
			else
			{
				//args.AddError(this);
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
