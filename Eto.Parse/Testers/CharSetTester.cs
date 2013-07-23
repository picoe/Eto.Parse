using System;
using System.Linq;

namespace Eto.Parse.Testers
{
	public class CharSetTester : ICharTester
	{
		char[] characters;
		char[] lowerCharacters;

		public char[] Characters
		{
			get { return characters; }
			set
			{
				characters = value;
				lowerCharacters = new char[characters.Length];
				for (int i = 0; i < characters.Length; i++)
				{
					lowerCharacters[i] = char.ToLowerInvariant(characters[i]);
				}
			}
		}

		public CharSetTester(params char[] chars)
		{
			this.Characters = (char[])chars.Clone();
		}

		public bool Test(char ch, bool caseSensitive)
		{
			if (caseSensitive)
			{
				for (int i = 0; i < characters.Length; i++)
				{
					if (characters[i] == ch)
						return true;
				}
			}
			else
			{
				for (int i = 0; i < lowerCharacters.Length; i++)
				{
					if (lowerCharacters[i] == char.ToLowerInvariant(ch))
						return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			var chars = string.Join(",", Characters.Select(c => char.IsControl(c) || char.IsWhiteSpace(c) ? string.Format("0x{0:x2}", (int)c) : string.Format("'{0}'", c)));
			return string.Format("{0}", chars);
		}
	}
}
