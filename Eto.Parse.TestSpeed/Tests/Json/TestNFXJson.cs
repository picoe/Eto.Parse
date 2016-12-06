using System;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestNFXJson : Test<JsonTestSuite>
	{
		public TestNFXJson()
			: base("NFX.JSON")
		{
		}

		public override void Warmup(JsonTestSuite suite)
		{
			global::NFX.Serialization.JSON.JSONExtensions.JSONToDynamic(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var json = global::NFX.Serialization.JSON.JSONExtensions.JSONToDynamic(suite.Json);
			if (suite.CompareOutput)
			{
				var result = json["result"];
				foreach (var item in result)
				{
					var id = item[suite.CompareProperty];
					output.Append(id);
				}
			}
		}
	}
}
