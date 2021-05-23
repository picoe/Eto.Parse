using System;
using Eto.Parse.Parsers;
using Eto.Parse.Writers;
using NUnit.Framework;

namespace Eto.Parse.Tests.Samples
{
	[TestFixture]
	public class WidthHeightTests
	{
		[Test]
		public void TestWidthHeight()
		{
			var space = +Terminals.WhiteSpace;
			Parser eq = "=";

			var intParser = new NumberParser { AllowDecimal = false, AllowExponent = false, ValueType = typeof(int) };

			var width = ("width" & eq & intParser.Named("width")).Separate();
			var height = ("height" & eq & intParser.Named("height")).Separate();

			var whhw = (width & space & height) | (height & space & width);

			var whhwGrammar = new Grammar(whhw);
			Console.WriteLine(new DisplayParserWriter().Write(whhwGrammar));
			var matches = whhwGrammar.Match("width=400 height=800");
			Assert.That(matches.HasMatches);
			Assert.AreEqual(400, matches["width"].Value);
			Assert.AreEqual(800, matches["height"].Value);
		}
	}
}