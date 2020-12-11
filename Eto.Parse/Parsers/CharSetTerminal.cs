using System;
using System.Linq;
using Eto.Parse.Parsers;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class CharSetTerminal : CharTerminal
	{
		char[] lookupCharacters;
		HashSet<char> characterLookup;

		/// <summary>
		/// Gets or sets the minimum count of characters before a lookup hash table is created
		/// </summary>
		/// <value>The minimum lookup count.</value>
		public int MinLookupCount { get; set; }

		public char[] Characters { get; set; }

		protected CharSetTerminal(CharSetTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
			this.Characters = other.Characters != null ? (char[])other.Characters.Clone() : null;
			this.MinLookupCount = other.MinLookupCount;
		}

		public CharSetTerminal(params char[] chars)
		{
			this.Characters = (char[])chars.Clone();
			this.MinLookupCount = 100;
		}

		class LowerCharComparer : IEqualityComparer<char>
		{
			public bool Equals(char c1, char c2)
			{
				return char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2);
			}
			public int GetHashCode(char c1)
			{
				return char.ToLowerInvariant(c1).GetHashCode();
			}

		}
		protected override void InnerInitialize(ParserInitializeArgs args)
		{
			base.InnerInitialize(args);
			if (TestCaseSensitive)
			{
				lookupCharacters = Characters;
				if (lookupCharacters.Length >= MinLookupCount)
					characterLookup = new HashSet<char>(lookupCharacters);
			}
			else
			{
                lookupCharacters = new char[Characters.Length];
                for (int i = 0; i < Characters.Length; i++)
                {
                    lookupCharacters[i] = char.ToLowerInvariant(Characters[i]);
                }
                if (lookupCharacters.Length >= MinLookupCount)
					characterLookup = new HashSet<char>(lookupCharacters, new LowerCharComparer());
			}
		}

		protected override bool Test(char ch)
		{
			if (characterLookup != null)
				return characterLookup.Contains(ch);
			ch = TestCaseSensitive ? ch : char.ToLowerInvariant(ch);
			for (int i = 0; i < Characters.Length; i++)
			{
				if (lookupCharacters[i] == ch)
					return true;
			}
			return false;
		}

		protected override string CharName
		{
			get
			{ 
				var chars = string.Join(",", Characters.Select(CharToString));
				return string.Format("{0}", chars);
			}
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new CharSetTerminal(this, args);
		}
	}
}
