using System;

namespace Eto.Parse
{
	public abstract class Scanner
	{
		ParseMatch lastError = null;

		public abstract long Offset { get; set; }

		public abstract bool IsEnd { get; }

		public abstract char Current { get; }

		public abstract bool Read();

		public abstract string SubString(long offset, int length);

		public ParseMatch LastError
		{
			get { return lastError; }
		}

		public virtual ParseMatch EmptyMatch
		{
			get { return new ParseMatch(this, Math.Max(0, Offset), 0); }
		}

		public virtual ParseMatch NoMatch(long offset)
		{
			ParseMatch match = NoMatch();
			this.Offset = offset;
			return match;
		}

		public virtual ParseMatch NoMatch()
		{
			ParseMatch match = new ParseMatch(this, Math.Max(0, this.Offset), -1);
			if (lastError == null || lastError.Offset < match.Offset)
				lastError = match;
			return match;
		}
	}
}
