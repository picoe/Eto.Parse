using System;
using Eto.Parse.Parsers;

namespace Eto.Parse
{
	partial class Parser
	{
		public static Parser operator !(Parser parser)
		{
			var inverse = parser as IInverseParser;
			if (inverse != null)
			{
				if (!parser.Reusable)
				{
					parser = parser.Clone();
					inverse = parser as IInverseParser;
				}
				inverse.Inverse = !inverse.Inverse;
				return parser;
			}
			return new LookAheadParser(parser) { Reusable = true };
		}

		public static RepeatParser operator +(Parser parser)
		{
			return new RepeatParser(parser, 1) { Reusable = true };
		}

		public static RepeatParser operator -(Parser parser)
		{
			return new RepeatParser(parser, 0) { Reusable = true };
		}

		public static AlternativeParser operator |(Parser left, Parser right)
		{
			var alternative = left as AlternativeParser;
			if (alternative != null && alternative.Reusable)
			{
				alternative.Items.Add(right);
				return alternative;
			}
			/*alternative = right as AlternativeParser;
			if (alternative != null && alternative.Reusable)
			{
				alternative.Items.Insert(0, left);
				return alternative;
			}*/

			return new AlternativeParser(left, right) { Reusable = true };
		}

		public static OptionalParser operator ~(Parser parser)
		{
			return new OptionalParser(parser) { Reusable = true };
		}

		public static implicit operator Parser(char ch)
		{
			var parser = Terminals.Set(ch);
			parser.Reusable = true;
			return parser;
		}

		public static implicit operator Parser(char[] chars)
		{
			var parser = Terminals.Set(chars);
			parser.Reusable = true;
			return parser;
		}

		public static implicit operator Parser(string literalString)
		{
			return new LiteralTerminal(literalString) { Reusable = true };
		}

		public static SequenceParser operator &(Parser left, Parser right)
		{
			var sequence = left as SequenceParser;
			if (sequence != null && sequence.Reusable)
			{
				sequence.Items.Add(right);
				return sequence;
			}
			/*sequence = right as SequenceParser;
			if (sequence != null && sequence.Reusable)
			{
				sequence.Items.Insert(0, left);
				return sequence;
			}*/
			return new SequenceParser(left, right) { Reusable = true };
		}

		public static ExceptParser operator -(Parser include, Parser exclude)
		{
			return new ExceptParser(include, exclude);
		}

		public static RepeatParser operator *(Parser parser, int repetitions)
		{
			return new RepeatParser(parser, repetitions, repetitions);
		}
	}
}

