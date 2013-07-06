using System;

namespace Eto.Parse
{
	public interface IScanner
	{
		int Position { get; set; }

		bool IsEnd { get; }

		char Peek { get; }

		void Read();

		string SubString(int index, int length);
	}
}
