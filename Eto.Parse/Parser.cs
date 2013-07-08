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

		public virtual string GetErrorMessage()
		{
			return DescriptiveName;
		}

		public virtual string DescriptiveName
		{
			get {
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

		protected Parser(Parser other)
		{
			Context = other.Context;
		}

		public ParseMatch Parse(ParseArgs args)
		{
			if (args.Trace)
				Console.WriteLine(this.DescriptiveName);
			var match = InnerParse(args);
			if (!match.Success)
			{
				if (args.Trace)
					Console.WriteLine("FAILED: {0}", this.DescriptiveName);
				//args.AddError(this);
			}

			return match;
		}

		public virtual IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}

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
