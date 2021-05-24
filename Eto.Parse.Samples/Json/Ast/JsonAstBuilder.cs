using System;
using Eto.Parse.Ast;

namespace Eto.Parse.Samples.Json.Ast
{
	/// <summary>
	/// Ast builder to generic token objects, for any json schema
	/// </summary>
	public class JsonAstBuilder : AstBuilder<JsonToken>
	{
		public JsonAstBuilder()
		{
			var token = new Builder<JsonToken>();

			var jobject = Create("object", () => new JsonObject());
			jobject.Children().HasKeyValue<JsonObject, string, JsonToken>(
				new ValueBuilder { Name = "name" },
				token
			);

			var jarray = Create("array", () => new JsonArray());
			jarray.HasMany<JsonArray, JsonToken>().Builders.Add(token);

			token.Create("string", () => new JsonValue()).Property<string>((o, v) => o.Value = v);
			token.Builders.Add(jobject);
			token.Builders.Add(jarray);
			token.Create("number", () => new JsonValue()).Property<decimal>((o, v) => o.Value = v);
			token.Create("bool", () => new JsonValue()).Property<bool>((o, v) => o.Value = v);
			token.Create(() => (JsonToken)null);
		}
	}
}

