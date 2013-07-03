using System;

namespace Eto.Parse
{
	public interface IScanner
	{
		long Position { get; set; }

		bool IsEnd { get; }

		char Peek { get; }

		void Read();

		string SubString(long index, int length);
	}
}
