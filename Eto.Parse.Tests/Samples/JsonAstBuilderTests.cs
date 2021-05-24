using Eto.Parse.Samples.Json;
using Eto.Parse.Samples.Json.Ast;
using NUnit.Framework;

namespace Eto.Parse.Tests.Samples
{
	[TestFixture]
    public class JsonAstBuilderTests
    {
		[Test]
		public void SimpleArray()
		{
			var jsonString = "[\"First\", \"Second\", 3, true]";
			var ast = new JsonAstBuilder();
			var grammar = new JsonGrammar();
			var result = ast.Build(grammar.Match(jsonString));
			Assert.IsInstanceOf<JsonArray>(result);
			var array = (JsonArray)result;
			Assert.AreEqual(4, array.Count);
			Assert.IsInstanceOf<JsonValue>(array[0]);
			Assert.AreEqual("First", ((JsonValue)array[0]).Value);
			Assert.AreEqual("Second", ((JsonValue)array[1]).Value);
			Assert.AreEqual(3, ((JsonValue)array[2]).Value);
			Assert.AreEqual(true, ((JsonValue)array[3]).Value);
		}
        
		[Test]
		public void SimpleObject()
		{
			var jsonString = "{\"String\":\"String Value\", \"Number\":5, \"Boolean\" : true }";
			var ast = new JsonAstBuilder();
			var grammar = new JsonGrammar();
			var result = ast.Build(grammar.Match(jsonString));
			Assert.IsInstanceOf<JsonObject>(result);
			var obj = (JsonObject)result;
			Assert.AreEqual(3, obj.Keys.Count);
			Assert.AreEqual("String Value", ((JsonValue)obj["String"]).Value);
			Assert.AreEqual(5, ((JsonValue)obj["Number"]).Value);
			Assert.AreEqual(true, ((JsonValue)obj["Boolean"]).Value);
		}
    }
}