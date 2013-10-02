using System.Collections.Generic;

namespace Eto.Parse
{
	/// <summary>
	/// Stack implementation using a non-shrinking list for great performance
	/// </summary>
	class SlimStack<T> : List<T>
	{
		int count;

		public new int Count
		{
			get { return count; }
		}

		public T Last
		{
			get { return this[count - 1]; }
			set { this[count - 1] = value; }
		}

		public SlimStack()
		{
		}

		public SlimStack(int capacity)
			: base(capacity)
		{
		}

		public void Push(T value)
		{
			if (count == base.Count)
				Add(value);
			else
				this[count] = value;
			count++;
		}

		public void PushDefault()
		{
			if (count == base.Count)
				Add(default(T));
			count++;
		}

		public T PopKeep()
		{
			return base[--count];
		}

		public T Pop()
		{
			var ret = base[--count];
			base[count] = default(T);
			return ret;
		}
	}
}

