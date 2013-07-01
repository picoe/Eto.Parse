using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : Scanner
	{
		long offset = -1;
		string value;
		
		public override long Offset
		{
			get { return offset; }
			set { offset = value; }
		}

		public override bool IsEnd
		{
			get { return offset == value.Length; }
		}
		
		public override char Current
		{
			get { return value[(int)offset]; }
		}
		
		public StringScanner(string value)
		{
			this.value = value;
		}
		
		public override bool Read()
		{
			if (IsEnd) return false;
			offset++;
			return !IsEnd;
		}

		public override String SubString(long offset, int length)
		{
			return value.Substring((int)offset, length);
		}
		
	}
}
