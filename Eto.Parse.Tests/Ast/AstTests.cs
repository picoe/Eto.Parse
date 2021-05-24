using System;
using System.Collections.Generic;
using Eto.Parse.Ast;
using Eto.Parse.Samples.Json;
using Eto.Parse.Samples.Json.AstObject;
using NUnit.Framework;

namespace Eto.Parse.Tests.Ast
{

	
	[TestFixture]
    public class AstTests
    {
		
		[Test]
		public void JsonToObjectAst()
		{
			var jsonString = @"{
    ""id"": 1,
    ""jsonrpc"": ""2.0"",
    ""total"": 100,
    ""result"": [
    {
	    ""id"": 0,
	    ""guid"": ""613cad29-f7dd-4ec6-be4d-259e37fe1261"",
	    ""isActive"": true,
	    ""balance"": ""$1,254.00"",
	    ""picture"": ""http://placehold.it/32x32"",
	    ""age"": 28,
	    ""name"": ""David Alvarado"",
	    ""gender"": ""male"",
	    ""company"": ""Petigems"",
	    ""email"": ""davidalvarado@petigems.com"",
	    ""phone"": ""+1 (967) 418-3091"",
	    ""address"": ""175 Opal Court, Worton, Montana, 3555"",
	    ""about"": ""Lorem officia incididunt id fugiat consectetur nulla ad ut fugiat qui tempor consequat qui occaecat. Nulla tempor voluptate sit excepteur amet. Magna culpa incididunt excepteur excepteur magna reprehenderit.\r\n"",
	    ""registered"": ""1988-04-25T01:51:39 +07:00"",
	    ""latitude"": 30.628037,
	    ""longitude"": 30.878798,
	    ""tags"": [
	        ""consectetur"",
	        ""ipsum"",
	        ""cupidatat"",
	        ""excepteur"",
	        ""dolore"",
	        ""quis"",
	        ""sint""
	    ],
	    ""friends"": [
	        {
	            ""id"": 0,
	            ""name"": ""Brewer Decker""
	        },
	        {
	            ""id"": 1,
	            ""name"": ""Carrie Mcconnell""
	        },
	        {
	            ""id"": 2,
	            ""name"": ""Melissa Kennedy""
	        }
	    ],
	    ""randomArrayItem"": ""cherry""
	}]
}";
			var jsonAst = new JsonObjectAstBuilder();
			var match = new JsonGrammar().Match(jsonString);
			var result = jsonAst.Build(match);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(1, result.Result.Count);
			var first = result.Result[0];
			Assert.AreEqual(new Guid("613cad29-f7dd-4ec6-be4d-259e37fe1261"), first.Guid);
			Assert.AreEqual("David Alvarado", first.Name);
			
		}
        
    }
}