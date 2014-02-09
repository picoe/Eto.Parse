using System;
using System.Linq;
using System.Collections.Generic;
using Eto.Parse.Parsers;
using System.IO;
using System.CodeDom.Compiler;
using Eto.Parse.Writers;

namespace Eto.Parse.Grammars
{
	public class GoldDefinition
	{
		Parser separator;

		public Dictionary<string, string> Properties { get; private set; }

		public Dictionary<string, Parser> Sets { get; private set; }

		public Dictionary<string, Parser> Terminals { get; private set; }

		public Dictionary<string, UnaryParser> Rules { get; private set; }

		public Parser Comment { get { return Terminals.ContainsKey("Comment") ? Terminals["Comment"] : null; } }

		public Parser Whitespace
		{ 
			get { return Terminals.ContainsKey("Whitespace") ? Terminals["Whitespace"] : null; }
			set
			{
				Terminals["Whitespace"] = value; 
				ClearSeparator();
			}
		}

		public Parser NewLine { get { return Terminals.ContainsKey("NewLine") ? Terminals["NewLine"] : null; } }

		internal void ClearSeparator()
		{
			separator = null;
		}

		public Parser Separator
		{
			get
			{
				if (separator == null)
					CreateSeparator();
				return separator;
			}
		}

		void CreateSeparator()
		{
			var alt = new AlternativeParser();
			var p = Comment;
			if (p != null)
				alt.Items.Add(p);
			p = Whitespace;
			if (p != null)
				alt.Items.Add(p);
			if (alt.Items.Count == 0)
				separator = null;
			else
				separator = -alt;
		}

		internal string GrammarName
		{
			get
			{
				string name;
				if (Properties.TryGetValue("Start Symbol", out name))
					return name.TrimStart('<').TrimEnd('>');
				else
					return null;
			}
		}

		public Grammar Grammar
		{ 
			get
			{
				UnaryParser parser;
				var symbol = GrammarName;
				if (!string.IsNullOrEmpty(symbol) && Rules.TryGetValue(symbol, out parser))
					return parser as Grammar;
				else
					return null;
			}
		}

		public GoldDefinition()
		{
			Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			Sets = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ "HT", Parse.Terminals.Set(0x09) },
				{ "LF", Parse.Terminals.Set(0x0A) },
				{ "VT", Parse.Terminals.Set(0x0B) },
				{ "FF", Parse.Terminals.Set(0x0C) },
				{ "CR", Parse.Terminals.Set(0x0D) },
				{ "Space", Parse.Terminals.Set(0x20) },
				{ "NBSP", Parse.Terminals.Set(0xA0) },
				{ "LS", Parse.Terminals.Set(0x2028) },
				{ "PS", Parse.Terminals.Set(0x2029) },

				{ "Number", Parse.Terminals.Range(0x30, 0x39) },
				{ "Digit", Parse.Terminals.Range(0x30, 0x39) },
				{ "Letter", Parse.Terminals.Range(0x41, 0x58) | Parse.Terminals.Range(0x61, 0x78) },
				{ "AlphaNumeric", Parse.Terminals.Range(0x30, 0x39) | Parse.Terminals.Range(0x41, 0x5A) | Parse.Terminals.Range(0x61, 0x7A) },
				{ "Printable", Parse.Terminals.Range(0x20, 0x7E) | Parse.Terminals.Set(0xA0) },
				{ "Letter Extended", Parse.Terminals.Range(0xC0, 0xD6) | Parse.Terminals.Range(0xD8, 0xF6) | Parse.Terminals.Range(0xF8, 0xFF) },
				{ "Printable Extended", Parse.Terminals.Range(0xA1, 0xFF) },
				{ "Whitespace", Parse.Terminals.Range(0x09, 0x0D) | Parse.Terminals.Set(0x20, 0xA0) },
			};
			Terminals = new Dictionary<string, Parser>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ "Whitespace", +Sets["Whitespace"] }
			};
			Rules = new Dictionary<string, UnaryParser>(StringComparer.InvariantCultureIgnoreCase);
			CreateSeparator();
		}
	}
}
