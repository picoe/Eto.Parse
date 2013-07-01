using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParseMatch
	{
		public Scanner Scanner { get; private set; }

		public long Index { get; protected set; }

		public int Length { get; protected set; }

		public long End
		{
			get { return (Length > 0) ? Index + Length - 1 : Index; }
		}
		
		public string Value
		{
			get { return Success ? Scanner.SubString(Index, Length) : null; }
		}
		
		public bool Success
		{
			get { return Length >= 0; }
		}
		
		public bool Empty
		{
			get { return Length == 0; }
		}
		
		public ParseMatch(Scanner scanner, long offset, int length)
		{
			this.Scanner = scanner;
			this.Index = offset;
			this.Length = length;
		}
		
		public static ParseMatch Merge(ParseMatch left, ParseMatch right)
		{
			if (right == null || right.Empty)
				return left;
			else if (left == null || left.Empty)
				return right;

			if (!left.Success || !right.Success) throw new ArgumentException("Can only merge successful matches", "match");
			
			long start = Math.Min(left.Index, right.Index);
			long end = Math.Max(left.End, right.End);
			return new ParseMatch(left.Scanner, start, (int)(end - start + 1));
		}

		public override string ToString()
		{
			return Value ?? string.Empty;
		}
	}
}
