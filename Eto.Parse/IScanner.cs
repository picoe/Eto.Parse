using System;

namespace Eto.Parse
{
	public interface IScanner
	{
		int Position { get; set; }

		int Advance(int length);

		bool IsEof { get; }

		char Current { get; }

		bool ReadString(string matchString, bool caseSensitive, out int pos);

		bool ReadChar(out char ch);

		bool ReadChar(out char ch, out int pos);

		string SubString(int index, int length);
	}
}
