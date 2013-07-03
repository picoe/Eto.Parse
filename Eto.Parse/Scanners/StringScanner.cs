using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : IScanner
	{
		long offset = 0;
		string value;
		
		public long Position
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

		public string SubString(long offset, int length)
		{
			return value.Substring((int)offset, length);
		}
		
	}
}
