using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : IScanner
	{
		int offset;
		string value;
		int line;
		
		public int Position
		{
			get { return offset; }
			set { offset = value; }
		}

		public bool IsEnd
		{
			get { return offset == value.Length; }
		}
		
		public char Peek
		{
			get { return value[(int)offset]; }
		}

		public StringScanner(string value)
		{
			this.value = value;
		}
		
		public void Read()
		{
			if (offset < value.Length)
				offset++;
		}

		public string SubString(int offset, int length)
		{
			if (offset >= value.Length)
				return null;
			return value.Substring(offset, length);
		}
		
	}
}
