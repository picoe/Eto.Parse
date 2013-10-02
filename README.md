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

One rather cumbersome issue to deal with using recursive descent parsers is [left recursion](http://en.wikipedia.org/wiki/Left_recursion). Eto.Parse automatically identifies left recursive grammars and will parse them with no issues by transforming them into a repeating pattern.

Performance
-----------

Eto.Parse has been highly optimized for performance and memory usage. For example, here's a comparison parsing a large JSON string 100 times (times in seconds):

### Speed

Test             | Parsing | Slower than best |  Warmup | Slower than best---------------- | ------: | :--------------: | ------: | :--------------:Eto.Parse        |  0.258s |     1.00x        |  0.064s |     1.00xNewtonsoft Json  |  0.263s |     1.02x        |  0.075s |     1.18xIrony            |  2.523s |     9.78x        |  0.266s |     4.18xGold Parser      | 35.217s |   136.47x        |  0.405s |     6.36x
(Warmup is the time it takes to initialize the engine for the first time and perform the first parse of the json string).

### Memory & Objects

Framework        |  Allocated  | More than best | # Objects | More than best
---------------- | ----------: | :------------: | --------: | :------------:
Eto.Parse        |    56.89 MB |      1x        | 1541401   |    1x
Newtonsoft.Json  |   109.39 MB |      1.92x     | 2176395   |    1.41x
Irony            |   440.21 MB |      7.74x     | 9572011   |    6.21x
Gold             | 4,609.45 MB |     81.02x     | 121366066 |   78.74x

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