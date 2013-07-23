using System;
using NUnit.Framework;
using Eto.Parse.Grammars;
using Eto.Parse.Writers;
using System.Linq;

namespace Eto.Parse.Tests
{
	[TestFixture]
	public class GoldParserTests
	{
		const string goldBnf = @"
!-----------------------------------------------------------------------------------
! GOLD Meta-Language
!
! This is the very simple grammar used to define grammars using the GOLD Parser.
! The grammar was revised for version 2.0.5 of the GOLD Parser Builder. The changes
! were designed to:
!
!   1. Make it easier to use line comments to disable individual rules. 
!   2. Allow the developer to use optional newlines for readability.
! 
! www.devincook.com/goldparser 
! -----------------------------------------------------------------------------------
 
 
""Name""         = 'GOLD Meta-Language'
""Version""      = '2.6.0'
""Author""       = 'Devin Cook'
""About""        = 'This grammar defines the GOLD Meta-Language.'

""Start Symbol"" = <Grammar>


! The token definitions are very complex. Many definitions allow an 
! ""Override Sequence"" such as the backslash in C. In this case, it is 
! single quotes. Not all the tokens have overrides. I only added them where
! their use could be justified.


! ====================================================================
! Special Terminals
! ====================================================================

{Parameter Ch}   = {Printable}    - [""] - ['']
{Nonterminal Ch} = {Alphanumeric} + [_-.] + {Space} 
{Terminal Ch}    = {Alphanumeric} + [_-.] 
{Literal Ch}     = {Printable}    - ['']       !Basically anything, DO NOT CHANGE!
{Set Literal Ch} = {Printable}    - ['['']'] - ['']
{Set Name Ch}    = {Printable}    - [{}]

ParameterName  = '""' {Parameter Ch}+ '""' 
Nonterminal    = '<' {Nonterminal Ch}+ '>'
Terminal       = {Terminal Ch}+  | '' {Literal Ch}* ''  
SetLiteral     = '[' ({Set Literal Ch} | '' {Literal Ch}* '' )+ ']'
SetName        = '{' {Set Name Ch}+ '}'


! ====================================================================
! Line-Based Grammar Declarations
! ====================================================================

{Whitespace Ch} = {Whitespace} - {CR} - {LF}

Whitespace = {Whitespace Ch}+
Newline    = {CR}{LF} | {CR} | {LF}  

! ====================================================================
! Comments
! ====================================================================

Comment Line  = '!'
Comment Start = '!*'
Comment End   = '*!'


! ====================================================================
! Rules
! ====================================================================

<Grammar>  ::= <nl opt> <Content>     ! The <nl opt> here removes all newlines before the first definition

<Content> ::= <Content> <Definition> 
            | <Definition>

<Definition> ::= <Parameter>
               | <Set Decl>
               | <Terminal Decl>
               | <Rule Decl>
                

! Optional series of New Line - use below is restricted
<nl opt> ::= NewLine <nl opt>
           |

! One or more New Lines
<nl> ::= NewLine  <nl>
       | NewLine 

! ====================================================================
! Parameter Definition
! ====================================================================

<Parameter> ::= ParameterName <nl opt> '=' <Parameter Body> <nl>

<Parameter Body>  ::= <Parameter Body> <nl opt> '|' <Parameter Items>  
                    | <Parameter Items> 

<Parameter Items> ::= <Parameter Items> <Parameter Item> 
                    | <Parameter Item>

<Parameter Item>  ::= ParameterName 
                    | Terminal    
                    | SetLiteral    
                    | SetName       
                    | Nonterminal

! ====================================================================
! Set Definition
! ====================================================================

<Set Decl>  ::= SetName <nl opt> '=' <Set Exp> <nl>

<Set Exp>   ::= <Set Exp> <nl opt> '+' <Set Item>
              | <Set Exp> <nl opt> '-' <Set Item>
              | <Set Item>
            
<Set Item>  ::= SetLiteral         ! [ ... ]
              | SetName            ! { ... }

! ====================================================================
! Terminal Definition
! ====================================================================
             
<Terminal Decl> ::= <Terminal Name> <nl opt> '=' <Reg Exp> <nl>
 
<Terminal Name> ::= <Terminal Name> Terminal
                  | Terminal 


<Reg Exp>       ::= <Reg Exp> <nl opt> '|' <Reg Exp Seq>
                  | <Reg Exp Seq>

<Reg Exp Seq>   ::= <Reg Exp Seq> <Reg Exp Item>
                  | <Reg Exp Item> 

<Reg Exp Item>  ::= SetLiteral          <Kleene Opt>
                  | SetName             <Kleene Opt>
                  | Terminal            <Kleene Opt>
                  | '(' <Reg Exp 2> ')' <Kleene Opt>

!No newlines allowed

<Reg Exp 2>     ::= <Reg Exp 2> '|' <Reg Exp Seq>
                  | <Reg Exp Seq>

<Kleene Opt> ::= '+'
               | '?' 
               | '*' 
               | 

! ====================================================================
! Rule Definition
! ====================================================================

<Rule Decl>  ::= Nonterminal <nl opt> '::=' <Handles> <nl>  

<Handles>    ::= <Handles> <nl opt> '|' <Handle>
               | <Handle>
             
<Handle>     ::= <Handle> <Symbol>   !Zero or more               
               |                     !Leave the entry blank - makes a ""null""

<Symbol>     ::= Terminal
               | Nonterminal           
";

		static readonly string[] GOLD_RULES = new string[]
		{
			"Grammar", "Content", "Definition", "nl opt", "nl", "Parameter", "Parameter Body", "Parameter Items", "Parameter Item", "Set Decl", "Set Exp", "Set Item", "Terminal Decl", "Terminal Name", "Reg Exp", "Reg Exp Seq", "Reg Exp Item", "Reg Exp 2", "Kleene Opt", "Rule Decl", "Handles", "Handle", "Symbol"
		};

		[Test]
		public void TestParsing()
		{
			var goldParser = new GoldGrammar();
			var definition = goldParser.Build(goldBnf);

			// check rules
			CollectionAssert.AreEquivalent(GOLD_RULES, definition.Rules.Keys);
		}

		[Test]
		public void ToCode()
		{
			// test a round trip to code
			var code = new GoldGrammar().ToCode(goldBnf, "MyGoldGrammar");

			// execute generated code
			var generatedGoldParser = Helper.Create<Grammar>(code, "MyGoldGrammar");

			// match using generated parser
			var match = generatedGoldParser.Match(goldBnf);

			Assert.IsTrue(match.Success, "Error: {0}", match.ErrorMessage);

			// check rules
			var rules = match.Find("Rule Decl", true).Select(r => r["Nonterminal"].Value.TrimStart('<').TrimEnd('>')).ToArray();
			CollectionAssert.AreEquivalent(GOLD_RULES, rules);
		}
	}
}

