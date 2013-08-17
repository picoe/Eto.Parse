using System;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Scanners;
using Eto.Parse.Parsers;
using System.Linq;
using System.Diagnostics;

namespace Eto.Parse
{
	public abstract partial class Parser : ICloneable
	{
		string name;

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				if (name != null)
					AddError = true;
			}
		}

		public static Parser DefaultSeparator { get; set; }

		public bool AddError { get; set; }

		internal bool Reusable { get; set; }

		public IEnumerable<Parser> Children()
		{
			return Children(new ParserChain());
		}

		public abstract IEnumerable<Parser> Children(ParserChain args);

		public virtual string GetErrorMessage()
		{
			return DescriptiveName;
		}

		public virtual string DescriptiveName
		{
			get
			{
				var type = GetType();
				var name = type.Name;
				if (type.Assembly == typeof(Parser).Assembly && name.EndsWith("Parser"))
					name = name.Substring(0, name.LastIndexOf("Parser"));
				return name;
			}
		}

		public event Action<Match> Matched;

		protected virtual void OnMatched(Match match)
		{
			if (Matched != null)
				Matched(match);
		}

		public event Action<Match> PreMatch;

		protected virtual void OnPreMatch(Match match)
		{
			if (PreMatch != null)
				PreMatch(match);
		}

		internal void TriggerMatch(Match match)
		{
			OnMatched(match);
		}

		internal void TriggerPreMatch(Match match)
		{
			OnPreMatch(match);
		}

		public Parser()
		{
		}

		protected Parser(Parser other, ParserCloneArgs args)
		{
			AddError = other.AddError;
			args.Add(other, this);
		}

		public ParseMatch Parse(ParseArgs args)
		{
			if (Name == null)
			{
				//var trace = args.Grammar.Trace;
				//if (trace)
				//	Trace.WriteLine(string.Format("{0}, {1}", args.Scanner.Position, this.DescriptiveName));
				var match = InnerParse(args);
				if (match.Success)
				{
					//if (trace)
					//	Trace.WriteLine(string.Format("SUCCESS: {0}, {1}", args.Scanner.Position, this.DescriptiveName));
					return match;
				}

				//if (trace)
				//	Trace.WriteLine(string.Format("FAILED: {0}", this.DescriptiveName));
				if (AddError)
					args.AddError(this);
				return match;
			}
			else
			{
				args.Push();
				var match = InnerParse(args);
				if (match.Success)
				{
					args.PopMatch(this, match);
					return match;
				}

				if (AddError)
					args.AddError(this);
				args.PopFailed();
				return match;
			}
		}

		public virtual void Initialize(ParserInitializeArgs args)
		{
		}

		public bool Contains(Parser parser)
		{
			return Contains(new ParserContainsArgs(parser));
		}

		public virtual bool Contains(ParserContainsArgs args)
		{
			return args.Parser == this;
		}

		public bool IsLeftRecursive(Parser parser)
		{
			return IsLeftRecursive(new ParserContainsArgs(parser));
		}

		public virtual bool IsLeftRecursive(ParserContainsArgs args)
		{
			return object.ReferenceEquals(args.Parser, this);
		}

		public IEnumerable<Parser> Find(string parserId)
		{
			return Find(new ParserFindArgs(parserId));
		}

		public virtual IEnumerable<Parser> Find(ParserFindArgs args)
		{
			if (string.Equals(this.Name, args.ParserId, StringComparison.Ordinal))
				yield return this;
		}

		public Parser this [string parserId]
		{
			get { return Find(parserId).FirstOrDefault(); }
		}

		protected abstract ParseMatch InnerParse(ParseArgs args);

		public Parser Clone()
		{
			return Clone(new ParserCloneArgs());
		}

		public abstract Parser Clone(ParserCloneArgs args);

		object ICloneable.Clone()
		{
			return Clone();
		}

		public void SetError<T>(bool addError)
			where T: Parser
		{
			foreach (var item in Children().OfType<T>())
				item.AddError = addError;
		}

		public virtual T GetValue<T>(Match match)
		{
			return (T)Convert.ChangeType(match.Text, typeof(T));
		}
	}
}
