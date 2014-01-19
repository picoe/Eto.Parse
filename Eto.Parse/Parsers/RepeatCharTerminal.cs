using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Parsers
{
	public sealed class RepeatCharItem : ICloneable
	{
		public Func<char, bool> Test { get; set; }
		public int Minimum { get; set; }
		public int Maximum { get; set; }

		protected RepeatCharItem(RepeatCharItem other)
		{
			Test = other.Test;
			Minimum = other.Minimum;
			Maximum = other.Maximum;
		}

		public RepeatCharItem(Func<char, bool> test, int minimum = 0, int maximum = int.MaxValue)
		{
			Test = test;
			Minimum = minimum;
			Maximum = maximum;
		}

		public static implicit operator RepeatCharItem(char literalChar)
		{
			return new RepeatCharItem(ch => ch == literalChar, 1, 1);
		}

		public object Clone()
		{
			return new RepeatCharItem(this);
		}
	}

	public class RepeatCharTerminal : Parser
	{
		readonly List<RepeatCharItem> _items;
		public IList<RepeatCharItem> Items { get { return _items; } }

		protected RepeatCharTerminal(RepeatCharTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
			_items = new List<RepeatCharItem>(other._items.Select(r => (RepeatCharItem)r.Clone()));
		}

		public RepeatCharTerminal()
		{
			_items = new List<RepeatCharItem>();
		}

		public RepeatCharTerminal(Func<char, bool> test, int minimum = 0, int maximum = int.MaxValue)
			: this(new RepeatCharItem(test, minimum, maximum))
		{
		}

		public RepeatCharTerminal(params RepeatCharItem[] items)
		{
			_items = items.ToList();
		}

		public RepeatCharTerminal(IEnumerable<RepeatCharItem> items)
		{
			_items = items.ToList();
		}

		public void Add(Func<char, bool> test, int minimum = 0, int maximum = int.MaxValue)
		{
			_items.Add(new RepeatCharItem(test, minimum, maximum));
		}

		protected override int InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			var length = 0;
			var pos = scanner.Position;
			var ch = scanner.ReadChar();
			for (int i = 0; i < _items.Count; i++)
			{
				var item = _items[i];
				var count = 0;
				while (ch != -1 && item.Test((char)ch))
				{
					length++;
					count++;
					ch = scanner.ReadChar();
					if (count >= item.Maximum)
						break;
				}
				if (count < item.Minimum)
				{
					scanner.Position = pos;
					return -1;
				}
			}
			scanner.Position = pos + length;
			return length;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new RepeatCharTerminal(this, args);
		}
	}
}

