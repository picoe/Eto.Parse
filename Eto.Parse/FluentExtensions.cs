using System;
using System.Linq;
using Eto.Parse.Parsers;
using Eto.Parse.Testers;

namespace Eto.Parse
{
	public static class FluentParserExtensions
	{
		public static Parser Then(this Parser parser, params Parser[] parsers)
		{
			var sequence = parser as SequenceParser;
			if (sequence == null || !sequence.Reusable)
				sequence = new SequenceParser(parser) { Reusable = true };
			sequence.Items.AddRange(parsers);
			return sequence;
		}

		public static NegatableParser Inverse(this NegatableParser parser)
		{
			parser.Negative = !parser.Negative;
			return parser;
		}

		public static Parser Not(this Parser parser)
		{
			return new NotParser(parser);
		}

		public static Parser Not(this Parser parser, Parser inner)
		{
			return new SequenceParser(parser, new NotParser(inner));
		}

		public static Parser Repeat(this Parser parser, int minimum = 1, int maximum = Int32.MaxValue)
		{
			return new RepeatParser(parser, minimum, maximum);
		}

		public static Parser Optional(this Parser parser)
		{
			return new OptionalParser(parser);
		}

		public static Parser Or(this Parser left, Parser right)
		{
			var alternative = left as AlternativeParser;
			if (alternative != null && alternative.Reusable)
			{
				alternative.Items.Add(right);
				return alternative;
			}
			alternative = right as AlternativeParser;
			if (alternative != null && alternative.Reusable)
			{
				alternative.Items.Insert(0, left);
				return alternative;
			}
			return new AlternativeParser(left, right) { Reusable = true };
		}

		public static Parser Named(this Parser parser, string id, Action<NamedMatch> matched = null, Action<NamedMatch> preMatch = null)
		{
			var namedParser = new NamedParser(id ?? Guid.NewGuid().ToString(), parser);
			if (matched != null)
				namedParser.Matched += match => {
					matched(match);
				};
			if (preMatch != null)
				namedParser.PreMatch += match => {
					preMatch(match);
				};
			return namedParser;
		}

		public static CharParser Include(this CharParser parser, CharParser include)
		{
			return new CharParser(new IncludeTester(parser.Tester, include.Tester));
		}

		public static CharParser Include(this CharParser parser, params char[] include)
		{
			return new CharParser(new IncludeTester(parser.Tester, new CharSetTester(include)));
		}

		public static CharParser Exclude(this CharParser include, CharParser exclude)
		{
			return new CharParser(new ExcludeTester(include.Tester, exclude.Tester));
		}

		public static CharParser Exclude(this CharParser include, params char[] exclude)
		{
			return new CharParser(new ExcludeTester(include.Tester, new CharSetTester(exclude)));
		}
	}
}

