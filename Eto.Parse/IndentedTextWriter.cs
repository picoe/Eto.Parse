using System;
using System.CodeDom.Compiler;
using System.IO;

namespace Eto.Parse
{
	#if PCL
	public class IndentedTextWriter : TextWriter
	{
		bool needsIndent = true;

		public const string DefaultTabString = "    ";

		string tabString;
		public int Indent { get; set; }
		public TextWriter InnerWriter { get; private set; }

		public IndentedTextWriter(TextWriter writer)
			: this(writer, DefaultTabString)
		{
		}

		public IndentedTextWriter(TextWriter writer, string tabString)
		{
			this.InnerWriter = writer;
			this.tabString = tabString;
		}

		public override void Write(char value)
		{
			InnerWriter.Write(value);
		}

		public override void Write(bool value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(char[] buffer)
		{
			WriteIndent();
			InnerWriter.Write(buffer);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			WriteIndent();
			InnerWriter.Write(buffer, index, count);
		}

		public override void Write(decimal value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(double value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(float value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(int value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(long value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(object value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(string format, params object[] arg)
		{
			WriteIndent();
			InnerWriter.Write(format, arg);
		}

		public override void Write(string value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(uint value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void Write(ulong value)
		{
			WriteIndent();
			InnerWriter.Write(value);
		}

		public override void WriteLine()
		{
			WriteIndent();
			InnerWriter.WriteLine();
			needsIndent = true;
		}

		public override void WriteLine(bool value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(char value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(char[] buffer)
		{
			WriteIndent();
			InnerWriter.WriteLine(buffer);
			needsIndent = true;
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			WriteIndent();
			InnerWriter.WriteLine(buffer, index, count);
			needsIndent = true;
		}

		public override void WriteLine(decimal value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(double value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(float value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(int value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(long value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(object value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(string format, params object[] arg)
		{
			WriteIndent();
			InnerWriter.WriteLine(format, arg);
			needsIndent = true;
		}

		public override void WriteLine(string value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(uint value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		public override void WriteLine(ulong value)
		{
			WriteIndent();
			InnerWriter.WriteLine(value);
			needsIndent = true;
		}

		void WriteIndent()
		{
			if (needsIndent)
			{
				for (int i = 0; i < Indent; i++)
				{
					InnerWriter.Write(tabString);
				}
			}
		}

		public override string NewLine
		{
			get { return InnerWriter.NewLine; }
			set { InnerWriter.NewLine = value; }
		}

		public override void Flush()
		{
			InnerWriter.Flush();
		}

		public override System.Threading.Tasks.Task FlushAsync()
		{
			return InnerWriter.FlushAsync();
		}

		public override IFormatProvider FormatProvider
		{
			get { return InnerWriter.FormatProvider; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && InnerWriter != null)
			{
				InnerWriter.Dispose();
				InnerWriter = null;
			}
			base.Dispose(disposing);
		}

		public override System.Text.Encoding Encoding
		{
			get { return InnerWriter.Encoding; }
		}
	}
	#endif
	
}
