using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Eto.Parse.Parsers
{
	public class NumberParser : Parser
	{
		MethodInfo parseMethod;

		public bool AllowSign { get; set; }

		public bool AllowDecimal { get; set; }

		public char DecimalSeparator { get; set; }

		public bool AllowExponent { get; set; }

		public Type ValueType { get; set; }

		protected NumberParser(NumberParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			AllowSign = other.AllowSign;
			AllowExponent = other.AllowExponent;
			DecimalSeparator = other.DecimalSeparator;
			ValueType = other.ValueType;
		}

		public NumberParser()
		{
			DecimalSeparator = '.';
			ValueType = typeof(decimal);
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (ValueType != null)
				parseMethod = ValueType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(NumberStyles) }, null);
		}

		public override object GetValue(Match match)
		{
			var style = NumberStyles.None;

			if (AllowSign)
				style |= NumberStyles.AllowLeadingSign;
			if (AllowDecimal)
				style |= NumberStyles.AllowDecimalPoint;
			if (AllowExponent)
				style |= NumberStyles.AllowExponent;

			return parseMethod.Invoke(null, new object[] { match.Text, style });
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			var len = 0;
			char ch;
			bool hasNext;
			var pos = scanner.Position;
			if (AllowSign)
			{
				hasNext = scanner.ReadChar(out ch);
				if (!hasNext)
				{
					scanner.SetPosition(pos);
					return args.NoMatch;
				}
				if (ch == '-' || ch == '+')
				{
					len++;
					hasNext = scanner.ReadChar(out ch);
				}
			}
			else
				hasNext = scanner.ReadChar(out ch);

			bool foundNumber = false;
			bool hasDecimal = false;
			bool hasExponent = false;
			while (hasNext)
			{
				if (char.IsDigit(ch))
				{
					foundNumber = true;
				}
				else if (AllowDecimal && !hasDecimal && ch == DecimalSeparator)
				{
					hasDecimal = true;
				}
				else if (hasExponent && (ch == '+' || ch == '-'))
				{
				}
				else if (AllowExponent && !hasExponent && (ch == 'E' || ch == 'e'))
				{
					hasExponent = true;
					hasDecimal = true; // no decimals after exponent
				}
				else if (!foundNumber)
				{
					scanner.SetPosition(pos);
					return args.NoMatch;
				}
				else
					break;
				len++;
				hasNext = scanner.ReadChar(out ch);
			}
			scanner.SetPosition(pos + len);
			return new ParseMatch(pos, len);
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new NumberParser(this, chain);
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}
	}
}

