using System;
using Eto.Parse.Scanners;

namespace Eto.Parse
{
	public class Grammar : NamedParser
	{
		/// <summary>
		/// Gets or sets a value indicating that the match events will be triggered after a successful match
		/// </summary>
		/// <value></value>
		public bool EnableMatchEvents { get; set; }

		public Parser Separator { get; set; }

		public bool CaseSensitive { get; set; }

		protected Grammar(Grammar other)
		{
			this.EnableMatchEvents = other.EnableMatchEvents;
			this.Separator = other.Separator != null ? other.Separator.Clone() : null;
			this.CaseSensitive = other.CaseSensitive;
		}

		public Grammar(string name = null, Parser rule = null)
			: base(name, rule)
		{
			CaseSensitive = true;
			EnableMatchEvents = true;
		}

		public Grammar(Parser rule)
			: this(null, rule)
		{
		}

		public NamedMatch Match(string value)
		{
			value.ThrowIfNull("value");
			return Match(new StringScanner(value));
		}

		public NamedMatch Match(IScanner scanner)
		{
			scanner.ThrowIfNull("scanner");
			var args = new ParseArgs(this, scanner);
			Parse(args);
			var topMatch = args.Top;

			if (topMatch.Success && EnableMatchEvents)
			{
				topMatch.PreMatch();
				topMatch.Match();
			}
			return topMatch;
		}

		public NamedMatchCollection Matches(string value)
		{
			value.ThrowIfNull("value");
			return Matches(new StringScanner(value));
		}

		public NamedMatchCollection Matches(IScanner scanner)
		{
			scanner.ThrowIfNull("scanner");
			var matches = new NamedMatchCollection();
			while (!scanner.IsEof)
			{
				var match = Match(scanner);
				if (match.Success)
					matches.Add(match);
				else
					scanner.Advance(1);
			}
			return matches;
		}
	}
}

