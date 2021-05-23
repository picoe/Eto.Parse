using System;
using Eto.Parse.Scanners;
using System.Collections.Generic;
using System.Linq;
using Eto.Parse.Parsers;
using System.Diagnostics;

namespace Eto.Parse
{
	/// <summary>
	/// Flags to specify optimizations to apply to a grammar when initialized for the first match
	/// </summary>
	/// <remarks>
	/// These may increase startup time for the first match, but usually increase performance on subsequent
	/// matches with the same grammar instance.
	/// </remarks>
	[Flags]
	public enum GrammarOptimizations
	{
		/// <summary>
		/// No optimizations
		/// </summary>
		None = 0,

		/// <summary>
		/// Optimize alternations with only characters into a single character set parser
		/// </summary>
		CharacterSetAlternations = 1 << 0,

		/// <summary>
		/// Replace uncaptured unary parsers with their inner parser
		/// </summary>
		TrimUnnamedUnaryParsers = 1 << 1,

		/// <summary>
		/// Replace sequences and alternations with a single child
		/// </summary>
		TrimSingleItemSequencesOrAlterations = 1 << 2,

		/// <summary>
		/// Fix recursive grammars by replacing them with a repeating parser
		/// </summary>
		FixRecursiveGrammars = 1 << 3,

		/// <summary>
		/// All optimizations turned on
		/// </summary>
		All = CharacterSetAlternations | TrimUnnamedUnaryParsers | TrimSingleItemSequencesOrAlterations | FixRecursiveGrammars
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
		
		/// <summary>
		/// Sets the maximum character set range when <see cref="GrammarOptimizations.CharacterSetAlternations"/> is enabled.
		/// </summary>
		/// <value></value>
		public int MaxCharacterSetRangeOptimization { get; set; } = 100;

		public GrammarOptimizations Optimizations { get; set; }

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
			Optimizations = GrammarOptimizations.All;
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
				OptimizeCharacterSets();
			}

			if (Optimizations.HasFlag(GrammarOptimizations.TrimUnnamedUnaryParsers))
			{
				OptimizeUnmatchedUnaryParsers();
			}

			if (Optimizations.HasFlag(GrammarOptimizations.FixRecursiveGrammars))
			{
				FixRecursiveGrammars();
			}

			if (Optimizations.HasFlag(GrammarOptimizations.TrimSingleItemSequencesOrAlterations))
			{
				OptimizeSingleItemParsers();
			}

			//BuildChildren(new ParserChildrenArgs());
			Initialize(new ParserInitializeArgs(this));
			initialized = true;
		}

		void FixRecursiveGrammars()
		{
			var empty = new EmptyParser();
            var alternates = Children.OfType<AlternativeParser>();
			var first = new List<Parser>();
			var second = new List<Parser>();
			foreach (var alt in alternates.Distinct().ToList())
			{
				first.Clear();
				second.Clear();
                Parser separator = null;
                for (int i = 0; i < alt.Items.Count; i++)
				{
                    Parser item = alt.Items[i];
                    if (item != null && item.IsLeftRecursive(alt))
					{
                        var seqs = item.Scan(filter: p => {
                            if (ReferenceEquals(p, alt))
                                return false;
                            if (p is SequenceParser seq && seq.Items.Count > 0 && seq.Items[0].IsLeftRecursive(alt))
                            {
                                seq.Items[0] = empty;
                                return true;
                            }

                            return false;
                        }).OfType<SequenceParser>();
						foreach (var seq in seqs)
						{
                            separator = seq.Separator;
                            seq.Items.RemoveAt(0);
						}
						if (!item.IsLeftRecursive(alt))
							second.Add(item);
						else
							Debug.WriteLine(string.Format("Warning: Item in alternate is still recursive {0}", item.DescriptiveName));
					}
					else
					{
						first.Add(item);
					}
				}

				if (second.Count > 0)
				{
					Debug.WriteLine(string.Format("Fixing recursion in alternate: {0}", alt.DescriptiveName));
					alt.Items.Clear();
					var secondParser = second.Count > 1 ? new AlternativeParser(second) : second[0];
					if (first.Count > 0)
					{
						var firstParser = first.Count > 1 ? new AlternativeParser(first) : first[0];
                        var repeat = new RepeatParser(secondParser, 0) { Separator = separator };

                        if (first.Count == 1 && first[0] == null)
						{
							alt.Items.Add(repeat);
						}
						else
							alt.Items.Add(new SequenceParser(firstParser, repeat) { Separator = separator });
					}
					else
						alt.Items.Add(new RepeatParser(secondParser, 1) { Separator = separator });
				}
			}
		}

		void OptimizeSingleItemParsers()
		{
			var parsers = from r in Children.OfType<ListParser>()
			              where
			                  r.Items.Count == 1
			                  && (r is SequenceParser || r is AlternativeParser)
			              select r;
			foreach (var parser in parsers.ToList())
			{
				var replacement = parser.Items[0];
				if (parser.Name != null)
				{
					replacement = new UnaryParser { Inner = replacement, Name = parser.Name };
				}
				Replace(new ParserReplaceArgs(parser, replacement));
			}
		}

		void OptimizeUnmatchedUnaryParsers()
		{
			var parsers = from r in Children.OfType<UnaryParser>()
			              where !r.AddMatch && !r.AddError && r.Inner != null && r.GetType() == typeof(UnaryParser)
			              select r;
			foreach (var unary in parsers.ToList())
			{
				Replace(new ParserReplaceArgs(unary, unary.Inner));
			}
		}

		void OptimizeCharacterSets()
		{
			// turns character sets, ranges, and single characters into a single parser
			var parsers = Children.OfType<AlternativeParser>().Where(r => r.Items.Count > 2);
			foreach (var alt in parsers.ToList())
			{
				if (alt.Items.All(r => r != null && r.Name == null &&
				    (r is CharSetTerminal
				    || r is CharRangeTerminal
				    || r is SingleCharTerminal
				    || (r is LiteralTerminal && ((LiteralTerminal)r).Value.Length == 1)
				    )
				    ))
				{
					var chars = new List<char>();
					var inverse = new List<char>();
					var additionalParsers = new List<Parser>();
                    for (int i1 = 0; i1 < alt.Items.Count; i1++)
					{
                        Parser item = alt.Items[i1];
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
							if (charRange.End - charRange.Start > MaxCharacterSetRangeOptimization)
							{
								additionalParsers.Add(charRange);
							}
							else
							{
								for (char i = charRange.Start; i <= charRange.End; i++)
								{
									if (charRange.Inverse)
										inverse.Add(i);
									else
										chars.Add(i);
								}
							}
							continue;
						}
						var literal = item as LiteralTerminal;
						if (literal != null)
						{
							chars.Add(literal.Value[0]);
							continue;
						}
					}
					//Debug.WriteLine("Optimizing characters normal:{0} inverse:{1}", chars.Count, inverse.Count);
					alt.Items.Clear();
					if (chars.Count > 0)
						alt.Items.Add(new CharSetTerminal(chars.ToArray()));
					if (inverse.Count > 0)
						alt.Items.Add(new CharSetTerminal(inverse.ToArray())
						{
							Inverse = true
						});
					if (additionalParsers.Count > 0)
						alt.Items.AddRange(additionalParsers);
				}
			}
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

		/// <summary>
		/// Sets the parsers with the specified names as terminals, e.g. no match is generated
		/// </summary>
		/// <remarks>
		/// Terminals are usually single characters that are not interesting by themselves.
		/// Most grammars do not have a way to define a terminal vs. non-terminal, so one could use this
		/// method to improve performance on BNF/EBNF grammars on terminals so that match entries are not
		/// created for each character in the results.
		/// 
		/// This basically sets <see cref="Parser.AddError"/> and <see cref="Parser.AddMatch"/> to false
		/// for each matched parser.
		/// </remarks>
		/// <param name="terminalNames">Array of terminal names</param>
		public void SetTerminals(params string[] terminalNames)
		{
			var children = Children.Where(r => Array.IndexOf(terminalNames, r.Name) >= 0).ToList();
			foreach (var child in children)
			{
				child.AddError = false;
				child.AddMatch = false;
			}
		}
	}
}

