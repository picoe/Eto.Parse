using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : Scanner
	{
		int length;
		string value;

		public override bool IsEof
		{
			get { return Position >= length; }
		}
		
		public override char Current
		{
			get { return value[Position]; }
		}

		public string Value
		{
			get { return value; }
		}

		public StringScanner(string value)
		{
			this.value = value;
			this.length = value.Length;
		}

		public StringScanner(string value, int index, int length)
		{
			this.value = value;
			this.Position = index;
			this.length = length;
		}

		public override bool ReadChar(out char ch)
		{
			if (Position < length)
			{
				ch = value[Position];
				Position ++;
				return true;
			}
			ch = default(char);
			return false;
		}

		public override int Advance(int length)
		{
			var start = Position;
			var newPos = start + length;
			if (newPos > this.length)
				return -1;
			Position = newPos;
			return start;
		}

		public override bool ReadString(string matchString, bool caseSensitive)
		{
			var index = this.Position;
			var end = index + matchString.Length;
			if (end <= this.length)
			{
				if (caseSensitive)
				{
					for (int i = 0; i < matchString.Length; i++)
					{
						if (value[index++] != matchString[i])
							return false;
					}
				}
				else
				{
					for (int i = 0; i < matchString.Length; i++)
					{
						if (char.ToLowerInvariant(value[index++]) != char.ToLowerInvariant(matchString[i]))
							return false;
					}
				}
				Position = end;
				return true;
			}
			return false;
		}

		public override string SubString(int offset, int length)
		{
			if (offset >= this.length)
				return null;
			length = Math.Min(offset + length, this.length) - offset;
			return value.Substring(offset, length);
		}
	}
}
