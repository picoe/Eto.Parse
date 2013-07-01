using System;

namespace Eto.Parse
{
	public interface IScanner
	{
		long Offset { get; set; }

		bool IsEnd { get; }

		char Peek { get; }

		void Read();

		string SubString(long offset, int length);
	}
}
