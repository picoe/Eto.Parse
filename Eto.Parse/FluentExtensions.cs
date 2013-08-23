using System;
using System.Linq;
using Eto.Parse.Parsers;

namespace Eto.Parse
{
	/// <summary>
	/// Extensions for fluent API to define a grammar
	/// </summary>
	public static class FluentParserExtensions
	{
		public static SequenceParser Then(this Parser parser, params Parser[] parsers)
		{
			var sequence = parser as SequenceParser;
			if (sequence == null || !sequence.Reusable)
				sequence = new SequenceParser(parser) { Reusable = true };
			sequence.Items.AddRange(parsers);
			return sequence;
		}

		public static T SeparatedBy<T>(this T parser, Parser separator)
			where T: ISeparatedParser
		{
			parser.Separator = separator;
			return parser;
		}

		public static T SeparateChildrenBy<T>(this T parser, Parser separator, bool overrideExisting = true)
			where T: Parser
		{
			foreach (var item in parser.Children().OfType<ISeparatedParser>())
			{
				if (overrideExisting || item.Separator == null)
					item.Separator = separator;
			}
			return parser;
		}

		public static T Inverse<T>(this T parser)
			where T: Parser, IInverseParser
		{
			parser.Inverse = !parser.Inverse;
			return parser;
		}

		public static LookAheadParser Not(this Parser parser)
		{
			return new LookAheadParser(parser) { Inverse = true };
		}

		public static LookAheadParser NonCaptured(this Parser parser)
		{
			return new LookAheadParser(parser);
		}

		public static SequenceParser NotFollowedBy(this Parser parser, Parser inner)
		{
			return parser & new LookAheadParser(inner) { Inverse = true };
		}

		public static SequenceParser FollowedBy(this Parser parser, Parser lookAhead)
		{
			return parser & new LookAheadParser(lookAhead);
		}

		public static ExceptParser Except(this Parser parser, Parser exclude)
		{
			return new ExceptParser(parser, exclude);
		}

		public static RepeatParser Repeat(this Parser parser, int minimum = 1, int maximum = Int32.MaxValue)
		{
			return new RepeatParser(parser, minimum, maximum);
		}

		public static RepeatParser Until(this RepeatParser parser, Parser until)
		{
			parser.Until = until;
			return parser;
		}

		public static RepeatParser Until(this RepeatParser parser, Parser until, bool captureUntil)
		{
			parser.Until = until;
			parser.CaptureUntil = captureUntil;
			return parser;
		}

		public static OptionalParser Optional(this Parser parser)
		{
			return new OptionalParser(parser);
		}

		public static AlternativeParser Or(this Parser left, Parser right)
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

		public static Parser Named(this Parser parser, string name)
		{
			var unary = new UnaryParser(parser);
			unary.Name = name ?? Guid.NewGuid().ToString();
			return unary;
		}

		public static Parser WithName(this Parser parser, string name)
		{
			parser.Name = name;
			return parser;
		}

		public static T Separate<T>(this T parser)
			where T: Parser
		{
			parser.Reusable = false;
			return parser;
		}
	}
}

