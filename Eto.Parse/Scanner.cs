using System;

namespace Eto.Parse
{
	public abstract class Scanner
	{
		int position;

		public int Position { get { return position; } protected set { position = value; } }

		public virtual void SetPosition(int position)
		{
			this.position = position;
		}

		public abstract int Advance(int length);

		public abstract bool IsEof { get; }

		public abstract char Current { get; }

		public abstract bool ReadString(string matchString, bool caseSensitive);

		public abstract bool ReadChar(out char ch);

		public abstract string SubString(int index, int length);
	}
}
