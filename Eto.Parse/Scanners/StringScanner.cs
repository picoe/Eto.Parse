using System;

namespace Eto.Parse.Scanners
{
	public class StringScanner : Scanner
	{
		long offset = 0;
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
		
		public override char Peek
		{
			get { return value[(int)offset]; }
		}

		public StringScanner(string value)
		{
			this.value = value;
		}
		
		public override void Read()
		{
			if (offset < value.Length)
				offset++;
		}

		public override string SubString(long offset, int length)
		{
			return value.Substring((int)offset, length);
		}
		
	}
}
