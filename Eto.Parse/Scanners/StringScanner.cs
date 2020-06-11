using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : Scanner
	{
		readonly int end;
		readonly int start;
		readonly string value;

		public override bool IsEof
		{
			get { return Position >= end; }
		}

		public string Value
		{
			get { return value; }
		}

		public string GetContext(int count)
		{
			return value.Substring(Position, Math.Min(value.Length - Position, count));
		}

		public StringScanner(string value)
		{
			this.value = value;
			this.start = 0;
			this.end = value.Length;
		}

		public StringScanner(string value, int index, int length)
		{
			this.value = value;
			this.Position = index;
			this.start = index;
			this.end = index + length;
		}

		public override int ReadChar()
		{
			var pos = Position;
			if (pos < end)
			{
				Position = pos + 1;
				return value[pos];
			}
			return -1;
		}

		public override int Peek()
		{
			var pos = Position;
			return pos < end ? value[pos] : -1;
		}

		public override int Advance(int length)
		{
			var pos = Position;
			var newPos = pos + length;
			if (newPos <= end)
			{
				Position = newPos;
				return pos;
			}
			return -1;
		}

		public override bool ReadString(string matchString, bool caseSensitive)
		{
			var index = Position;
			var endstring = index + matchString.Length;
			if (endstring <= end)
			{
				if (caseSensitive)
				{
					for (int i = 0; i < matchString.Length; i++)
					{
						if (value[index++] != matchString[i])
							return false;
					}
					Position = endstring;
					return true;
				}
				else
				{
					for (int i = 0; i < matchString.Length; i++)
					{
						if (char.ToLowerInvariant(value[index++]) != char.ToLowerInvariant(matchString[i]))
							return false;
					}
					Position = endstring;
					return true;
				}
			}
			return false;
		}

		public override string Substring(int index, int length)
		{
			if (index < end)
			{
				length = Math.Min(index + length, end) - index;
				return value.Substring(index, length);
			}
			return null;
		}

		public override int LineAtIndex(int index)
		{
			int lineCount = 0;
			var max = Math.Min(end, index);
			for (int i = start; i < max; i++)
			{
				if (value[i] == '\n')
					lineCount++;
			}
			return lineCount + 1;
		}
	}
}
