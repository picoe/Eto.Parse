using System;

namespace Eto.Parse
{
	public abstract class Scanner
	{
		public int Position { get; set; }

		public abstract int Advance(int length);

		public abstract bool IsEof { get; }

		public abstract bool ReadString(string matchString, bool caseSensitive);

		public abstract int ReadChar();

		public abstract int Peek();

		public abstract string Substring(int index, int length);

		#region Obsolete

		[Obsolete("Use Substring instead")]
		public string SubString(int index, int length)
		{
			return Substring(index, length);
		}

		[Obsolete("Use Peek() instead")]
		public char Current
		{
			get { return (char)Peek(); }
		}

		[Obsolete("Use Position property instead")]
		public void SetPosition(int position)
		{
			Position = position;
		}

		#endregion
	}
}
