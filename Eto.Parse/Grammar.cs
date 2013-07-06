using System;
using Eto.Parse.Scanners;

namespace Eto.Parse
{
	public class Grammar : NamedParser
	{
		public bool EnableEvents { get; set; }

		public string Name { get; set; }

		public Grammar(string name = null, Parser rule = null)
			: base(name)
		{
			Inner = rule;
		}

		public Grammar(Parser rule)
		{
			Inner = rule;
		}

		public NamedMatch Match(string value)
		{
			value.ThrowIfNull("value");
			return Match(new StringScanner(value));
		}

		public NamedMatch Match(IScanner scanner)
		{
			scanner.ThrowIfNull("scanner");
			var args = new ParseArgs(scanner);
			Parse(args);
			var topMatch = args.Top;

			if (EnableEvents)
			{
				topMatch.PreMatch();
				topMatch.Match();
			}
			return topMatch;
		}
	}
}

