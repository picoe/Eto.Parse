using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public struct ParseMatch
	{
		long index;
		int length;

		public long Index { get { return index; } }

		public int Length { get { return length; } }

		public long End
		{
			get { return (Length > 0) ? Index + Length - 1 : Index; }
		}
		
		public bool Success
		{
			get { return Length >= 0; }
		}

		public bool Failed
		{
			get { return Length < 0; }
		}
		
		public bool Empty
		{
			get { return Length == 0; }
		}

		public bool FailedOrEmpty
		{
			get { return Length <= 0; }
		}

		public ParseMatch(long offset, int length)
		{
			this.index = offset;
			this.length = length;
		}
		
		public static ParseMatch Merge(ParseMatch left, ParseMatch right)
		{
			if (right.FailedOrEmpty && !left.Failed)
				return left;
			else if (left.FailedOrEmpty && !right.Failed)
				return right;

			if (!left.Success || !right.Success) throw new ArgumentException("Can only merge successful matches", "match");
			
			long start = Math.Min(left.Index, right.Index);
			long end = Math.Max(left.End, right.End);
			return new ParseMatch(start, (int)(end - start + 1));
		}
	}
}
