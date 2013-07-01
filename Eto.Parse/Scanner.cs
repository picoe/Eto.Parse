using System;

namespace Eto.Parse
{
	public abstract class Scanner
	{
		ParseMatch lastError = null;

		public abstract long Offset { get; set; }

		public abstract bool IsEnd { get; }

		public abstract char Peek { get; }

		public abstract void Read();

		public abstract string SubString(long offset, int length);

		public ParseMatch LastError
		{
			get { return lastError; }
		}

		public virtual ParseMatch EmptyMatch
		{
			get { return new ParseMatch(this, Math.Max(0, Offset), 0); }
		}
	}
}
