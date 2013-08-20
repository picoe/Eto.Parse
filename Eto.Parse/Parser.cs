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
		bool? addError;

		/// <summary>
		/// Gets or sets the name of the match added to the match result tree
		/// </summary>
		/// <remarks>
		/// When you set this property, it affects the match result tree returned from the <see cref="Grammar.Match"/>
		/// method. Each parser that is named will get a node entry in the match tree if it has succesfully matched
		/// on the input string.  This allows you to
		/// 
		/// If this is set to <c>null</c>, this parser will not add a node to the match tree, but any named
		/// children will still add to the match tree (if any).
		/// 
		/// If you set the name, the parser will automatically set <see cref="Parser.AddError"/> to <c>true</c>
		/// to give back information when this parser does not match, unless AddError has already been set
		/// to something else explicitly.
		/// </remarks>
		/// <value>The name to give the match in the match result tree</value>
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				if (addError == null && name != null)
					addError = true;
			}
		}

		public static Parser DefaultSeparator { get; set; }

		/// <summary>
		/// Gets or sets a value indicating that this parser should add to the errors list when not matched
		/// </summary>
		/// <value><c>true</c> to add errors; otherwise, <c>false</c>.</value>
		public bool AddError
		{
			get { return addError ?? false; }
			set { addError = value; }
		}

		internal bool Reusable { get; set; }

		/// <summary>
		/// Gets an enumeration of all child parsers of this instance
		/// </summary>
		public IEnumerable<Parser> Children()
		{
			return Children(new ParserChain());
		}

		/// <summary>
		/// Gets an enumeration of all child parsers of this instance
		/// </summary>
		/// <remarks>
		/// Implementors of parsers should implement this, and call <see cref="ParserChain.Push"/> and <see cref="ParserChain.Pop"/>
		/// before calling the Children method of contained parsers.
		/// </remarks>
		/// <param name="args">Arguments to get the children</param>
		public abstract IEnumerable<Parser> Children(ParserChain args);

		/// <summary>
		/// Gets the error message to display for this parser
		/// </summary>
		/// <remarks>
		/// By default, this will use the DescriptiveName
		/// </remarks>
		/// <returns>The error message to display when not matched</returns>
		public virtual string GetErrorMessage()
		{
			return DescriptiveName;
		}

		public virtual string DescriptiveName
		{
			get
			{
				if (this.Name != null)
					return this.Name;
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
			//var trace = args.Grammar.Trace;
			//if (trace)
			//	Trace.WriteLine(string.Format("{0}, {1}", args.Scanner.Position, this.DescriptiveName));

			if (Name == null)
			{
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

				args.PopFailed();
				if (AddError)
					args.AddError(this);
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

		public virtual object GetValue(Match match)
		{
			return match.Text;
		}
	}
}
