using System;
using NUnit.Framework;
using Eto.Parse;
using System.Diagnostics;
using Eto.Parse.Grammars;
using System.Linq;

namespace Eto.Parse.Tests.Grammars
{
	[TestFixture]
	public class EbnfTests
	{
		const string ebnf = @"
(*
  This example defines Extended BNF
  informally. Many of the syntax rules include
  a comment to explain their meaning; inside a
  comment a meta identifier is enclosed in angle
  brackets < and > to avoid confusion with
  similar English words. The non-terminal symbols
  <letter>, <decimal digit> and <character> are
  not defined. The position of <comments> is
  stated in a comment but not formally defined.
*)
syntax = syntax rule, {syntax rule};

syntax rule
  = meta identifier, ’=’, definitions list, ’;’
  (* A <syntax rule> defines the sequences of
    symbols represented by a <meta identifier> *);

definitions list
  = single definition, {’|’, single definition}
  (* | separates alternative <single definitions> *);

single definition = term, {’,’, term}
  (* , separates successive <terms> *);

term = factor, [’-’, exception]
  (* A <term> represents any sequence of symbols
    that is defined by the <factor> but
    not defined by the <exception> *);

exception = factor
  (* A <factor> may be used as an <exception>
    if it could be replaced by a <factor>
    containing no <meta identifiers> *);

factor = [integer, ’*’], primary
  (* The <integer> specifies the number of
    repetitions of the <primary> *);

primary
  = optional sequence | repeated sequence
  | special sequence | grouped sequence
  | meta identifier | terminal string | empty;

empty = ;

optional sequence = ’[’, definitions list, ’]’
  (* The brackets [ and ] enclose symbols
    which are optional *);

repeated sequence = ’{’, definitions list, ’}’
  (* The brackets { and } enclose symbols
    which may be repeated any number of times *);

grouped sequence = ’(’, definitions list, ’)’
  (* The brackets ( and ) allow any
    <definitions list> to be a <primary> *);

terminal string
  = ""’"", character - ""’"", {character - ""’""}, ""’""
  | ’""’, character - ’""’, {character - ’""’}, ’""’
  (* A <terminal string> represents the
    <characters> between the quote symbols
    ’_’ or ""_"" *);

meta identifier = letter, {letter | decimal digit}
  (* A <meta identifier> is the name of a
    syntactic element of the language being
    defined *);

integer = decimal digit, {decimal digit};

special sequence = ’?’, {character - ’?’}, ’?’
  (* The meaning of a <special sequence> is not
    defined in the standard metalanguage. *);

comment = ’(*’, {comment symbol - ’*)’}, ’*)’
  (* A comment is allowed anywhere outside a
    <terminal string>, <meta identifier>,
    <integer> or <special sequence> *);

comment symbol
  = comment | terminal string | special sequence | character;
";

		void SetEbnfRules(Grammar myEbnf)
		{
			// EBNF spec does not identify terminals and other special rules, so we define them here so we can round
			// trip the original defintion.
			// For you own grammars, there is an extension of the EbnfGrammar that allows you to define terminals
			// by using := instead of = for your rule.
			myEbnf["meta identifier"].SeparateChildrenBy(-Terminals.SingleLineWhiteSpace);
			myEbnf["integer"].SeparateChildrenBy(null);
			myEbnf["terminal string"].SeparateChildrenBy(null);
		}

		[Test]
		public void TestEbnf()
		{
			var ebnfGrammar = new EbnfGrammar();

			var myEbnf = ebnfGrammar.Build(ebnf, "syntax");
			SetEbnfRules(myEbnf);

			var match = myEbnf.Match(ebnf);
			Assert.IsTrue(match.Success, match.ErrorMessage);
		}

		[Test]
		public void EbnfToCode()
		{
			var ebnfGrammar = new EbnfGrammar();

			var code = ebnfGrammar.ToCode(ebnf, "syntax", "MyEbnfGrammar");

			var myEbnf = Helper.Create<Grammar>(code, "MyEbnfGrammar");
			SetEbnfRules(myEbnf);

			var match = myEbnf.Match(ebnf);
			Assert.IsTrue(match.Success, match.ErrorMessage);
		}

		[Test]
		public void Simple()
		{
			var grammarString = @"
ws := {? Terminals.WhiteSpace ?};

letter or digit := ? Terminals.LetterOrDigit ?;

simple value := letter or digit, {letter or digit};

bracket value = simple value, {simple value};

optional bracket = '(', bracket value, ')' | simple value;

first = optional bracket;

second = optional bracket;

grammar = ws, first, second, ws;
";

			var input = "  hello ( parsing world )  ";

			// our grammar
			var grammar = new EbnfGrammar().Build(grammarString, "grammar");

			var match = grammar.Match(input);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			Assert.AreEqual("hello", match["first"]["simple value", true].Value);
			Assert.AreEqual("parsing world", match["second"]["bracket value", true].Value);
		}
	}
}

