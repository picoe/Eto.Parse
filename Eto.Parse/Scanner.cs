using System;

namespace Eto.Parse
{
	public abstract class Scanner
	{
		public int Position { get; set; }

		[Obsolete("Use Position property instead")]
		public void SetPosition(int position)
		{
			Position = position;
		}

		public abstract int Advance(int length);

		public abstract bool IsEof { get; }

		public abstract char Current { get; }

		public abstract bool ReadString(string matchString, bool caseSensitive);

		public abstract bool ReadChar(out char ch);

		public abstract string Substring(int index, int length);

		[Obsolete("Use Substring instead")]
		public string SubString(int index, int length)
		{
			return Substring(index, length);
		}
	}
}
