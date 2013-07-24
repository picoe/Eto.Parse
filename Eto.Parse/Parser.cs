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
		public static Parser DefaultSeparator { get; set; }

		public bool AddError { get; set; }

		internal bool Reusable { get; set; }

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

		public IEnumerable<NamedParser> Find(string parserId)
		{
			return Find(new ParserFindArgs(parserId));
		}

		public virtual IEnumerable<NamedParser> Find(ParserFindArgs args)
		{
			yield break;
		}

		public NamedParser this [string parserId]
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
	}
}
