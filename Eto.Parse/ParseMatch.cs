using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public struct ParseMatch
	{
		int index;
		int length;

		public static readonly ParseMatch None = new ParseMatch(-1, -1);

		public int Index { get { return index; } }

		public int Length { get { return length; } set { length = value; } }

		public bool Success { get { return length >= 0; } }

		public bool Empty { get { return length == 0; } }

		public bool FailedOrEmpty { get { return length <= 0; } }

		public ParseMatch(int offset, int length)
		{
			this.index = offset;
			this.length = length;
		}
	}
}
