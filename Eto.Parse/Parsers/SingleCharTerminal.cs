using System;
using System.Linq;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class SingleCharTerminal : CharTerminal
	{
		char character;
		char lowerCharacter;

		public char Character
		{
			get { return character; }
			set
			{
				character = value;
				lowerCharacter = char.ToLowerInvariant(value);
			}
		}

		protected SingleCharTerminal(SingleCharTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
			this.Character = other.character;
		}

		public SingleCharTerminal()
		{
		}

		public SingleCharTerminal(char character)
		{
			this.Character = character;
		}

		protected override bool Test(char ch)
		{
			if (TestCaseSensitive)
			{
				return character == ch;
			}
			else
			{
				return char.ToLowerInvariant(ch) == lowerCharacter;
			}
		}

		protected override string CharName
		{
			get
			{ 
				return string.Format("'{0}'", character);
			}
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new SingleCharTerminal(this, args);
		}
	}
}
