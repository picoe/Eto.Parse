Eto.Parse
=========
### A recursive descent LL(k) parser framework for .NET

Links
-----

* Join the [forums](http://groups.google.com/group/eto-parse)
* Chat in [#eto.parse](http://webchat.freenode.net/?channels=eto.parse) on freenode
* Download using [nuget](https://www.nuget.org/packages/Eto.Parse/) with Visual Studio or [Xamarin Studio nuget addin](https://github.com/mrward/monodevelop-nuget-addin)

Description
-----------

Eto.Parse is a highly optimized recursive decent parser framework that can be used to create parsers for [context-free grammars](http://en.wikipedia.org/wiki/Context-free_grammar) that go beyond the capability of regular expressions.

You can use [BNF](https://en.wikipedia.org/wiki/Backus–Naur_Form), [EBNF](http://en.wikipedia.org/wiki/Extended_Backus–Naur_Form), or [Gold Meta-Language](http://goldparser.org/doc/grammars) grammars to define your parser, code them directly using a [Fluent API](http://en.wikipedia.org/wiki/Fluent_interface), or use shorthand operators (or a mix of each).

### Why not use RegEx?

Regular Expressions work great when the syntax is not complex, but fall short especially when dealing with any recursive syntax using some form of brackets or grouping concepts. 

For example, creating a math parser using RegEx cannot validate (directly) that there are matching brackets.  E.g. "((1+2)*3)", or "{ 'my': 'value', 'is' : {'recursive': true } }"

### Matching

The framework has been put together to get at the relevant values as easily as possible.  Each parser can be *named*, which then builds a tree of named matches that represent the interesting sections of the parsed input. You can use events on the named sections to perform logic when they match, or just parse the match tree directly.

### Left Recursion

One rather cumbersome issue to deal with using recursive descent parsers is [left recursion](http://en.wikipedia.org/wiki/Left_recursion). Eto.Parse automatically identifies left recursive grammars and transforms them into a repeating pattern.

Performance
-----------

Eto.Parse has been highly optimized for performance and memory usage. For example, here's a comparison parsing a large JSON string 1000 times (times in seconds):

### Speed

Test              | Parsing | Slower than best |  Warmup | Slower than best
----------------- | ------: | :--------------: | ------: | :--------------:
Eto.Parse         |  2,327s |     1,00x        |  0,008s |     1,00x
Newtonsoft Json   |  2,523s |     1,08x        |  0,068s |     8,08x
ServiceStack.Text |  2,854s |     1,23x        |  0,066s |     7,78x
Irony             | 25,401s |    10,92x        |  0,188s |    22,28x
bsn.GoldParser    | 11,186s |     4,81x        |  0,013s |     1,49x
NFX.JSON          | 11,847s |     5,09x        |  0,187s |    22,10x
SpracheJSON       | 92,774s |    39,88x        |  0,189s |    22,37x

(Warmup is the time it takes to initialize the engine for the first time and perform the first parse of the json string).

### Memory & Objects

Framework        |  Allocated  | More than best | # Objects | More than best
---------------- | ----------: | :------------: | --------: | :------------:
Eto.Parse        |   553.99 MB |      1.00x     |  15268050 |    1.00x
Newtonsoft.Json  | 1,074.27 MB |      1.94x     |  21562432 |    1.41x
ServiceStack.Text| 2,540.91 MB |      4.59x     |  15738493 |    1.03x
Irony            | 4,351.44 MB |      7.85x     |  94831118 |    6.21x
bsn.GoldParser   | 2,012.16 MB |      3.63x     |  74387176 |    4.87x

Example
-------

For example, the following defines a simple hello world parser in **Fluent API**:

	// optional repeating whitespace
	var ws = Terminals.WhiteSpace.Repeat(0);

	// parse a value with or without brackets
	var valueParser = Terminals.Set('(')
		.Then(Terminals.AnyChar.Repeat().Until(ws.Then(')')).Named("value"))
		.Then(Terminals.Set(')'))
		.SeparatedBy(ws)
		.Or(Terminals.WhiteSpace.Inverse().Repeat().Named("value"));

	// our grammar
	var grammar = new Grammar(
		ws
		.Then(valueParser.Named("first"))
		.Then(valueParser.Named("second"))
		.Then(Terminals.End)
		.SeparatedBy(ws)
	);

Or using **shorthand operators**:

	// optional repeating whitespace
	var ws = -Terminals.WhiteSpace;

	// parse a value with or without brackets
	Parser valueParser = 
		('(' & ws & (+Terminals.AnyChar ^ (ws & ')')).Named("value") & ws & ')')
		| (+!Terminals.WhiteSpace).Named("value");

	// our grammar
	var grammar = new Grammar(
		ws & valueParser.Named("first") & 
		ws & valueParser.Named("second") & 
		ws & Terminals.End
	);

Or, using **EBNF**:

	var grammar = new EbnfGrammar().Build(@"
	(* := is an extension to define a literal with no whitespace between repeats and sequences *)
	ws := {? Terminals.WhiteSpace ?};
	
	letter or digit := ? Terminals.LetterOrDigit ?;
	
	simple value := letter or digit, {letter or digit};
	
	bracket value = simple value, {simple value};
	
	optional bracket = '(', bracket value, ')' | simple value;
	
	first = optional bracket;
	
	second = optional bracket;
	
	grammar = ws, first, second, ws;
	", "grammar");

These can parse the following text input:

	var input = "  hello ( parsing world )  ";
	var match = grammar.Match(input);
	
	var firstValue = match["first"]["value"].Value;
	var secondValue = match["second"]["value"].Value;

**firstValue** will equal "hello", and **secondValue** will equal "parsing world".


License
-------

Licensed under [MIT](http://opensource.org/licenses/MIT).

See LICENSE file for full license.