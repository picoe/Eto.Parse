using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParseMatch
	{


		public long Index { get; protected set; }

		public int Length { get; protected set; }

		public long End
		{
			get { return (Length > 0) ? Index + Length - 1 : Index; }
		}
		
		public bool Success
		{
			get { return Length >= 0; }
		}
		
		public bool Empty
		{
			get { return Length == 0; }
		}
		
		public ParseMatch(long offset, int length)
		{
			this.Index = offset;
			this.Length = length;
		}
		
		public static ParseMatch Merge(ParseMatch left, ParseMatch right)
		{
			if (right == null || (right.Empty && left != null))
				return left;
			else if (left == null || (left.Empty && right != null))
				return right;

			if (!left.Success || !right.Success) throw new ArgumentException("Can only merge successful matches", "match");
			
			long start = Math.Min(left.Index, right.Index);
			long end = Math.Max(left.End, right.End);
			return new ParseMatch(start, (int)(end - start + 1));
		}

		public static bool operator true(ParseMatch match)
		{
			return match.Success;
		}

		public static bool operator false(ParseMatch match)
		{
			return !match.Success;
		}
	}
}
