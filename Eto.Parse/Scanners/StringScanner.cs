using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : IScanner
	{
		int offset;
		int length;
		string value;

		public int Position
		{
			get { return offset; }
			set { offset = value; }
		}

		public bool IsEof
		{
			get { return offset >= length; }
		}
		
		public char Current
		{
			get { return value[offset]; }
		}

		public StringScanner(string value)
		{
			this.value = value;
			this.length = value.Length;
		}
		
		public bool ReadChar(out char ch)
		{
			if (offset < length)
			{
				ch = value[offset];
				offset++;
				return true;
			}
			ch = (char)0;
			return false;
		}

		public bool ReadChar(out char ch, out int pos)
		{
			pos = offset;
			if (offset < length)
			{
				ch = value[offset];
				offset++;
				return true;
			}
			ch = (char)0;
			return false;
		}

		public int Advance(int length)
		{
			var start = offset;
			if (offset + length > this.length)
				return -1;
			offset += length;
			return start;
			//offset = Math.Min(offset + length, this.length);
			//return offset + length > this.length ? -1 : start;
		}

		public bool ReadString(string matchString, bool caseSensitive, out int pos)
		{
			var index = pos = this.Position;
			if (index + matchString.Length <= value.Length)
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
				offset += matchString.Length;
				return true;
			}
			return false;
		}

		public string SubString(int offset, int length)
		{
			if (offset >= value.Length)
				return null;
			return value.Substring(offset, length);
		}
		
	}
}
