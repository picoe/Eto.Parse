using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : Scanner
	{
		int end;
		string value;

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
			this.end = value.Length;
		}

		public StringScanner(string value, int index, int length)
		{
			this.value = value;
			this.Position = index;
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
			if (pos < end)
			{
				return value[pos];
			}
			return -1;
		}

		public override int Advance(int length)
		{
			var start = Position;
			var newPos = start + length;
			if (newPos <= end)
			{
				Position = newPos;
				return start;
			}
			return -1;
		}

		public override bool ReadString(string matchString, bool caseSensitive)
		{
			var index = this.Position;
			var end = index + matchString.Length;
			if (end <= this.end)
			{
				if (caseSensitive)
				{
					for (int i = 0; i < matchString.Length; i++)
					{
						if (value[index++] != matchString[i])
							return false;
					}
					Position = end;
					return true;
				}
				else
				{
					for (int i = 0; i < matchString.Length; i++)
					{
						if (char.ToLowerInvariant(value[index++]) != char.ToLowerInvariant(matchString[i]))
							return false;
					}
					Position = end;
					return true;
				}
			}
			return false;
		}

		public override string Substring(int offset, int length)
		{
			if (offset < end)
			{
				length = Math.Min(offset + length, end) - offset;
				return value.Substring(offset, length);
			}
			return null;
		}
	}
}
