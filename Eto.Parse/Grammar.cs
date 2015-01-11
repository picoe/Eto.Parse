using System;
using Eto.Parse.Scanners;
using System.Collections.Generic;
using System.Linq;
using Eto.Parse.Parsers;
using System.Diagnostics;

namespace Eto.Parse
{
	[Flags]
	public enum GrammarOptimizations
	{
		CharacterSetAlternations = 1 << 0
	}

	/// <summary>
	/// Defines the top level parser (a grammar) used to parse text
	/// </summary>
	public class Grammar : UnaryParser
	{
		bool initialized;

		/// <summary>
		/// Gets or sets a value indicating that the match events will be triggered after a successful match
		/// </summary>
		/// <value></value>
		public bool EnableMatchEvents { get; set; }

		/// <summary>
		/// Gets or sets the separator to use for <see cref="Parsers.RepeatParser"/> and <see cref="Parsers.SequenceParser"/> if not explicitly defined.
		/// </summary>
		/// <value>The separator to use inbetween repeats and items of a sequence</value>
		public Parser Separator { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this grammar is case sensitive or not
		/// </summary>
		/// <value><c>true</c> if case sensitive; otherwise, <c>false</c>.</value>
		public bool CaseSensitive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a partial match of the input scanner is allowed
		/// </summary>
		/// <value><c>true</c> to allow a successful match if partially matched; otherwise, <c>false</c> to indicate that the entire input must be consumed to match.</value>
		public bool AllowPartialMatch { get; set; }

		public bool Trace { get; set; }

		public GrammarOptimizations Optimizations { get;set; }

		/// <summary>
		/// Initializes a new copy of the <see cref="Eto.Parse.Grammar"/> class
		/// </summary>
		/// <param name="other">Other object to copy</param>
		/// <param name="args">Arguments for the copy</param>
		protected Grammar(Grammar other, ParserCloneArgs args)
		{
			this.EnableMatchEvents = other.EnableMatchEvents;
			this.Separator = args.Clone(other.Separator);
			this.CaseSensitive = other.CaseSensitive;
			this.Optimizations = other.Optimizations;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Parse.Grammar"/> class
		/// </summary>
		/// <param name="name">Name of the grammar</param>
		/// <param name="rule">Top level grammar rule</param>
		public Grammar(string name = null, Parser rule = null)
			: base(name, rule)
		{
			CaseSensitive = true;
			EnableMatchEvents = true;
			Optimizations = GrammarOptimizations.CharacterSetAlternations;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Parse.Grammar"/> class
		/// </summary>
		/// <param name="rule">Top level grammar rule</param>
		public Grammar(Parser rule)
			: this(null, rule)
		{
		}

		/// <summary>
		/// Initializes this instance for parsing
		/// </summary>
		/// <remarks>
		/// Initialization (usually) occurs only once, and should only be called after
		/// the grammar is fully defined. This will be called automatically the first
		/// time you call the <see cref="Match(string)"/> method.
		/// </remarks>
		public void Initialize()
		{
			if (Optimizations.HasFlag(GrammarOptimizations.CharacterSetAlternations))
			{
				// turns character sets, ranges, and single characters into a single parser
				foreach (var alt in Children().OfType<AlternativeParser>().Where(r => r.Items.Count > 2))
				{
					if (alt.Items.All(r => r.Name == null && (r is CharSetTerminal || r is CharRangeTerminal || r is SingleCharTerminal)))
					{
						var chars = new List<char>();
						var inverse = new List<char>();
						foreach (var item in alt.Items)
						{
							var singleChar = item as SingleCharTerminal;
							if (singleChar != null)
							{
								if (singleChar.Inverse)
									inverse.Add(singleChar.Character);
								else
									chars.Add(singleChar.Character);
								continue;
							}
							var charSet = item as CharSetTerminal;
							if (charSet != null)
							{
								if (charSet.Inverse)
									inverse.AddRange(charSet.Characters);
								else
									chars.AddRange(charSet.Characters);
								continue;
							}

							var charRange = item as CharRangeTerminal;
							if (charRange != null)
							{
								for (char i = charRange.Start; i < charRange.End; i++)
								{
									if (charRange.Inverse)
										inverse.Add(i);
									else
										chars.Add(i);
								}
								continue;
							}
						}
						//Debug.WriteLine("Optimizing characters normal:{0} inverse:{1}", chars.Count, inverse.Count);
						alt.Items.Clear();
						if (chars.Count > 0)
							alt.Items.Add(new CharSetTerminal(chars.ToArray()));
						if (inverse.Count > 0)
							alt.Items.Add(new CharSetTerminal(inverse.ToArray()) { Inverse = true });
					}
				}
			}

			Initialize(new ParserInitializeArgs(this));
			initialized = true;
		}

		protected override int InnerParse(ParseArgs args)
		{
			if (args.IsRoot)
			{
				var scanner = args.Scanner;
				var pos = scanner.Position;
				args.Push();
				var match = Inner.Parse(args);
				var matches = args.Pop();
				if (match >= 0 && !AllowPartialMatch && !scanner.IsEof)
				{
					scanner.Position = pos;
					match = -1;
				}

				var errorIndex = -1;
				var childErrorIndex = -1;
				IEnumerable<Parser> errors = null;
				if (match < 0 || match == args.ErrorIndex)
				{
					var errorList = new List<Parser>(args.Errors.Count);
					for (int i = 0; i < args.Errors.Count; i++)
					{
						var error = args.Errors[i];
						if (!errorList.Contains(error))
							errorList.Add(error);
					}
					errors = errorList;
					errorIndex = args.ErrorIndex;
					childErrorIndex = args.ChildErrorIndex;
				}

				args.Root = new GrammarMatch(this, scanner, pos, match, matches, errorIndex, childErrorIndex, errors);
				return match;
			}
			return base.InnerParse(args);
		}

		public GrammarMatch Match(string value)
		{
			//value.ThrowIfNull("value");
			return Match(new StringScanner(value));
		}

		public GrammarMatch Match(Scanner scanner)
		{
			//scanner.ThrowIfNull("scanner");
			var args = new ParseArgs(this, scanner);

			if (!initialized)
				Initialize();
			Parse(args);
			var root = args.Root;

			if (root.Success && EnableMatchEvents)
			{
				root.TriggerPreMatch();
				root.TriggerMatch();
			}
			return root;
		}

		public MatchCollection Matches(string value)
		{
			value.ThrowIfNull("value");
			return Matches(new StringScanner(value));
		}

		public MatchCollection Matches(Scanner scanner)
		{
			scanner.ThrowIfNull("scanner");
			var matches = new MatchCollection();
			var eof = scanner.IsEof;
			while (!eof)
			{
				var match = Match(scanner);
				if (match.Success)
				{
					matches.AddRange(match.Matches);
					eof = scanner.IsEof;
				}
				else
					eof = scanner.Advance(1) < 0;
			}
			return matches;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new Grammar(this, args);
		}
	}
}

