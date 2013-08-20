using System;
using System.Linq;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class CharSetTerminal : CharTerminal
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

		protected CharSetTerminal(CharSetTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
			this.Characters = other.characters != null ? (char[])other.characters.Clone() : null;
		}

		public CharSetTerminal(params char[] chars)
		{
			this.Characters = (char[])chars.Clone();
		}

		protected override bool Test(char ch, bool caseSensitive)
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

		protected override string CharName
		{
			get
			{ 
				var chars = string.Join(",", Characters.Select(c => char.IsControl(c) || char.IsWhiteSpace(c) ? string.Format("0x{0:x2}", (int)c) : string.Format("'{0}'", c)));
				return string.Format("{0}", chars);
			}
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new CharSetTerminal(this, args);
		}
	}
}
