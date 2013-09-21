using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using Eto.Parse.Parsers;

namespace Eto.Parse.Tests.Behaviours
{
	[TestFixture]
	public class AutocompleteTests
	{
		class CarGrammar : Grammar
		{
			private static readonly Parser WS = +Terminals.SingleLineWhiteSpace;

			public CarGrammar():base("procedure")
			{
				var writables = ((Parser)"[brake]" | "[throttle]").Named("telemetry");
				var readables = (writables | "[velocity]").Named("telemetry");
				var number = new Eto.Parse.Parsers.NumberParser { AllowDecimal = true, AllowExponent = true, AllowSign = false }; 
				// (it sure would be nice to specify a range on that number)
				// - please file an enhancement request on github (;
				var writer = ((Parser)"set").Named("action") & WS & writables & WS & "at" & WS & number.Named("value") & ~WS & ~(Parser)"%";
				var op = ((Parser)"<" | ">" | "=").Named("operator");
				var unit = ((Parser)"m/s" | "kph" | "mph").Named("unit");
				var reader = ((Parser)"until").Named("action") & WS & readables & WS & op & ~WS & ~unit;

				var row = (~(reader | writer) & Terminals.Eol).Named("row");
				Inner = +row & ~row; 
				// the optional at the end will never match, since it will be consumed by +row.
				// however, it will give us the expected results for the next line.
			}
		}

		[Test]
		public void SuccessMatch()
		{
			var parser = new CarGrammar();

			var match = parser.Match("set [throttle] at 15 %\r\n");
			Assert.IsTrue(match.Success, match.ErrorMessage);
			Assert.AreEqual(3, match.Errors.Count()); // row, action(set), action(until)
			CollectionAssert.AreEquivalent(new string[] { "row", "action", "action" }, match.Errors.Select(r => r.Name));
			Assert.AreEqual(24, match.ErrorIndex);
			CollectionAssert.AreEqual(new string[] { "set", "until" }, FindPossibilities(match));
		}

		[Test]
		public void EmptyInput()
		{
			var parser = new CarGrammar();
			var match = parser.Match("");
			Assert.IsFalse(match.Success, match.ErrorMessage);
			Assert.AreEqual(3, match.Errors.Count()); // row, action(set), action(until)
			CollectionAssert.AreEquivalent(new string[] { "row", "action", "action" }, match.Errors.Select(r => r.Name));
			Assert.AreEqual(0, match.ErrorIndex);
			CollectionAssert.AreEqual(new string[] { "set", "until" }, FindPossibilities(match));
		}

		[Test]
		public void FailedOnAddErrorParser()
		{
			var parser = new CarGrammar();
			var match = parser.Match("set ");
			Assert.IsFalse(match.Success, match.ErrorMessage);
			Assert.AreEqual(1, match.Errors.Count()); // telemetry was the error
			Assert.AreEqual(4, match.ErrorIndex);
			CollectionAssert.AreEqual(new string[] { "[brake]", "[throttle]" }, FindPossibilities(match));
		}

		[Test]
		public void FailedOnNoAddError()
		{
			var parser = new CarGrammar();
			var match = parser.Match("set [throttle] ");
			Assert.IsFalse(match.Success, match.ErrorMessage);
			Assert.AreEqual(2, match.Errors.Count()); // we'll get 'row', and 'until' because 'set' and '[throttle]' matched successfully.
			Assert.AreEqual(0, match.ErrorIndex); // because we didn't set AddError for the un-matched 'at' terminal
			Assert.AreEqual(15, match.ChildErrorIndex); // this is where the actual error was, but we don't know what to expect
		}

		private static IEnumerable<string> FindPossibilities(GrammarMatch match)
		{
			var literals = new List<string>();
			foreach (var child in match.Errors)
			{
				literals.AddRange(FindPossibilities(child));
			}
			return literals.Distinct().OrderBy(l => l);
		}

		private static IEnumerable<string> FindPossibilities(Parser match)
		{
			if (match is Eto.Parse.Parsers.LiteralTerminal)
				yield return ((Eto.Parse.Parsers.LiteralTerminal)match).Value;

			var seq = match as SequenceParser;
			if (seq != null)
			{
				foreach (var child in FindPossibilities(seq.Items[0]))
					yield return child;
			}
			var alt = match as AlternativeParser;
			if (alt != null)
			{
				foreach (var child in alt.Items)
				{
					foreach (var altchild in FindPossibilities(child))
						yield return altchild;
				}
			}
			var unary = match as UnaryParser;
			if (unary != null)
			{
				foreach (var child in FindPossibilities(unary.Inner))
					yield return child;
			}
		}
	}
}

