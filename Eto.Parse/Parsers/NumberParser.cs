using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public class NumberParser : Parser
	{
		Func<string, object> getValue;

		public bool AllowSign { get; set; }

		public bool AllowDecimal { get; set; }

		public char DecimalSeparator { get; set; }

		public bool AllowExponent { get; set; }

		public Type ValueType { get; set; }

		public CultureInfo Culture { get; set; }

		protected NumberParser(NumberParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			AllowSign = other.AllowSign;
			AllowExponent = other.AllowExponent;
			DecimalSeparator = other.DecimalSeparator;
			ValueType = other.ValueType;
			Culture = CultureInfo.CurrentCulture;
		}

		public NumberParser()
		{
			DecimalSeparator = '.';
			ValueType = typeof(decimal);
			Culture = CultureInfo.CurrentCulture;
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (args.Push(this))
			{
				if (ValueType != null)
				{
					var style = NumberStyles.None;

					if (AllowSign)
						style |= NumberStyles.AllowLeadingSign;
					if (AllowDecimal)
						style |= NumberStyles.AllowDecimalPoint;
					if (AllowExponent)
						style |= NumberStyles.AllowExponent;

					var numberFormat = Culture.NumberFormat;

					if (typeof(decimal) == ValueType)
						getValue = text => decimal.Parse(text, style, numberFormat);
					else if (typeof(Int16) == ValueType)
						getValue = text => Int16.Parse(text, style, numberFormat);
					else if (typeof(Int32) == ValueType)
						getValue = text => Int32.Parse(text, style, numberFormat);
					else if (typeof(Int64) == ValueType)
						getValue = text => Int64.Parse(text, style, numberFormat);
					else if (typeof(UInt16) == ValueType)
						getValue = text => UInt16.Parse(text, style, numberFormat);
					else if (typeof(UInt32) == ValueType)
						getValue = text => UInt32.Parse(text, style, numberFormat);
					else if (typeof(UInt64) == ValueType)
						getValue = text => UInt64.Parse(text, style, numberFormat);
					else if (typeof(double) == ValueType)
						getValue = text => double.Parse(text, style, numberFormat);
					else if (typeof(float) == ValueType)
						getValue = text => float.Parse(text, style, numberFormat);
					else
					{
#if PCL
						var parameters = new [] { typeof(string), typeof(NumberStyles) };
						var parseMethod = ValueType.GetTypeInfo().DeclaredMethods.FirstOrDefault(r => r.Name == "Parse" && r.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameters));
#else
						var parseMethod = ValueType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(NumberStyles) }, null);
#endif
						getValue = text => parseMethod.Invoke(null, new object[] { text, style });
					}
				}
				args.Pop();
			}
		}

		public override object GetValue(string text)
		{
			return getValue(text);
		}

		protected override int InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			var len = 0;
			int ch;
			var pos = scanner.Position;
			if (AllowSign)
			{
				ch = scanner.ReadChar();
				if (ch == -1)
				{
					return -1;
				}
				if (ch == '-' || ch == '+')
				{
					len++;
					ch = scanner.ReadChar();
				}
			}
			else
				ch = scanner.ReadChar();

			bool foundNumber = false;
			bool hasDecimal = false;
			bool hasExponent = false;
			do
			{
				if (char.IsDigit((char)ch))
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
					scanner.Position = pos;
					return -1;
				}
				else
					break;
				len++;
				ch = scanner.ReadChar();
			}
			while (ch != -1);
			scanner.Position = pos + len;
			return len;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new NumberParser(this, args);
		}
	}
}

