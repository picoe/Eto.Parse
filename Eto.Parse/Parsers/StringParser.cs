using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace Eto.Parse.Parsers
{
	public class StringParser : Parser
	{
		string quoteCharString;
		char[] quoteCharacters;

		public char[] QuoteCharacters
		{
			get { return quoteCharacters; }
			set
			{
				quoteCharacters = value;
				quoteCharString = value != null ? new string(value) : null;
			}
		}

		public bool AllowEscapeCharacters { get; set; }

		public bool AllowDoubleQuote { get; set; }

		public bool AllowNonQuoted { get; set; }

		public Parser NonQuotedLetter { get; set; }

		public bool AllowQuoted
		{
			get { return quoteCharString != null; }
		}

		public override string DescriptiveName
		{
			get { return AllowQuoted ? "Quoted String" : "String"; }
		}

		public string GetValue(NamedMatch match)
		{
			var val = match.Value;
			if (val.Length == 0)
				return string.Empty;
			// process quotes
			if (AllowQuoted)
			{
				var quoteIndex = quoteCharString.IndexOf(val[0]);
				if (quoteIndex >= 0)
				{
					var quoteChar = quoteCharString[quoteIndex];
					if (quoteIndex >= 0 && val.Length >= 2 && val[val.Length - 1] == quoteChar)
					{
						val = val.Substring(1, val.Length - 2);
					}
					if (AllowDoubleQuote)
					{
						val = val.Replace(quoteChar.ToString() + quoteChar, quoteChar.ToString());
					}
				}
			}
			// process escapes using string format with no parameters
			if (AllowEscapeCharacters)
			{
				val = GetEscapedString(val);
			}
			return val;
		}

		static string GetEscapedString(string source)
		{
			var sb = new StringBuilder(source.Length);
			int pos = 0;
			while (pos < source.Length)
			{
				char c = source[pos];
				if (c == '\\')
				{
					pos++;
					if (pos >= source.Length)
						throw new ArgumentException("Missing escape sequence");
					switch (source[pos])
					{
						case '\'':
							c = '\'';
							break;
						case '\"':
							c = '\"';
							break;
						case '\\':
							c = '\\';
							break;
						case '0':
							c = '\0';
							break;
						case 'a':
							c = '\a';
							break;
						case 'b':
							c = '\b';
							break;
						case 'f':
							c = '\f';
							break;
						case 'n':
							c = '\n';
							break;
						case 'r':
							c = '\r';
							break;
						case 't':
							c = '\t';
							break;
						case 'v':
							c = '\v';
							break;
						case 'x':
							var hex = new StringBuilder(4);
							pos++;
							if (pos >= source.Length)
								throw new ArgumentException("Missing escape sequence");
							for (int i = 0; i < 4; i++)
							{
								c = source[pos];
								if (!(char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
									break;
								hex.Append(c);
								pos++;
								if (pos > source.Length)
									break;
							}
							if (hex.Length == 0)
								throw new ArgumentException("Unrecognized escape sequence");
							c = (char)Int32.Parse(hex.ToString(), NumberStyles.HexNumber);
							pos--;
							break;
						case 'u':
							pos++;
							if (pos + 3 >= source.Length)
								throw new ArgumentException("Unrecognized escape sequence");
							try
							{
								uint charValue = UInt32.Parse(source.Substring(pos, 4), NumberStyles.HexNumber);
								c = (char)charValue;
								pos += 3;
							}
							catch (SystemException)
							{
								throw new ArgumentException("Unrecognized escape sequence");
							}
							break;
						case 'U':
							pos++;
							if (pos + 7 >= source.Length)
								throw new ArgumentException("Unrecognized escape sequence");
							try
							{
								uint charValue = UInt32.Parse(source.Substring(pos, 8), NumberStyles.HexNumber);
								if (charValue > 0xffff)
									throw new ArgumentException("Unrecognized escape sequence");
								c = (char)charValue;
								pos += 7;
							}
							catch (SystemException)
							{
								throw new ArgumentException("Unrecognized escape sequence");
							}
							break;
						default:
							throw new ArgumentException("Unrecognized escape sequence");
					}
				}
				pos++;
				sb.Append(c);
			}
			return sb.ToString();
		}

		protected StringParser(StringParser other, ParserCloneArgs args)
			: base(other, args)
		{
			this.QuoteCharacters = other.QuoteCharacters != null ? (char[])other.QuoteCharacters.Clone() : null;
			this.AllowDoubleQuote = other.AllowDoubleQuote;
			this.AllowEscapeCharacters = other.AllowEscapeCharacters;
			this.AllowNonQuoted = other.AllowNonQuoted;
			this.NonQuotedLetter = args.Clone(other.NonQuotedLetter);
		}

		public StringParser()
		{
			NonQuotedLetter = Terminals.LetterOrDigit;
			QuoteCharacters = "\"\'".ToCharArray();
			AddError = true;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			int length = 0;
			var scanner = args.Scanner;
			var pos = scanner.Position;
			char ch;

			if (AllowQuoted)
			{
				if (!scanner.ReadChar(out ch))
					return args.EmptyMatch;

				if (quoteCharacters.Contains(ch))
				{
					char quote = ch;
					bool isEscape = false;
					for (;;)
					{
						if (!scanner.ReadChar(out ch))
							break;
						length++;
						if (AllowEscapeCharacters && ch == '\\')
							isEscape = true;
						else if (!isEscape)
						{
							if (ch == quote)
							{
								if (!AllowDoubleQuote || scanner.IsEof || scanner.Current != quote)
								{
									return new ParseMatch(pos, length + 1);
								}
								else
									isEscape = true;
							}
						}
						else
							isEscape = false;
					}

					scanner.SetPosition(pos);
					return args.EmptyMatch;
				}
				else
					scanner.SetPosition(pos);
			}

			if (AllowNonQuoted)
			{
				for (;;)
				{
					var m = NonQuotedLetter.Parse(args);
					if (!m.Success || m.Length == 0)
						break;
					length += m.Length;
				}
				if (length > 0)
					return new ParseMatch(pos, length);
				else
					return args.EmptyMatch;
			}

			return args.EmptyMatch;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new StringParser(this, chain);
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}
	}
}

