using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eto.Parse
{
	public class ParserWriterArgs
	{
		readonly Dictionary<Type, int> names = new Dictionary<Type, int>();
		readonly HashSet<string> namedParsers = new HashSet<string>();
		readonly Dictionary<object, string> objectNames = new Dictionary<object, string>();

		public Stack<Parser> Parsers { get; private set; }

		public virtual int Level { get; set; }

		public IParserWriter Writer { get; internal set; }

		public ParserWriterArgs()
		{
			Parsers = new Stack<Parser>();
		}

		public void Push(Parser parser)
		{
			Parsers.Push(parser);
			Level += 1;
		}

		public void Pop()
		{
			Parsers.Pop();
			Level -= 1;
		}

		public string Write(Parser parser)
		{
			return Writer.WriteParser(this, parser);
		}

		public string GenerateName(Parser parser, string name = null)
		{
			string cachedName;
			if (!objectNames.TryGetValue(parser, out cachedName))
			{
				if (name != null)
				{
					cachedName = name;
					var count = 1;
					while (objectNames.Values.Contains(cachedName))
					{
						cachedName = name + count++;
					}
				}
				else
					cachedName = GenerateName(parser.GetType());
				objectNames[parser] = cachedName;
			}
			return cachedName;
		}

		public string GenerateName(Type type)
		{
			int val;
			if (!names.TryGetValue(type, out val))
				val = 0;
			val++;

			names[type] = val;
			var name = type.Name;
			if (name.EndsWith("Parser", StringComparison.Ordinal))
				name = name.Substring(0, name.Length - 6);
			return name.ToLowerInvariant() + val;
		}

		public bool IsDefined(string name)
		{
			if (!namedParsers.Contains(name))
			{
				namedParsers.Add(name);
				return false;
			}
			return true;
		}
	}
}
