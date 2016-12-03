using System;
using System.IO;
using System.Collections.Generic;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class JsonTestSuite : TestSuite
	{
		public string CompareProperty { get; set; }

		public string Json { get; private set; }

		public JsonTestSuite(string name = null, string sample = "sample-large.json")
			: base("Json" + (name != null ? " " + name : ""))
		{
			Iterations = 1000;
			CompareProperty = "id";
			//CompareOutput = true;

			sample = "Eto.Parse.TestSpeed.Tests.Json." + sample;
			Json = new StreamReader(GetType().Assembly.GetManifestResourceStream(sample)).ReadToEnd();
		}

		public override IEnumerable<ITest> GetTests()
		{
			yield return new TestEtoDirect();
			yield return new TestEtoHelpers();
			yield return new TestNewtonsoft();
			yield return new TestServiceStack();
			yield return new TestIrony();
			yield return new TestBsnGold();
			yield return new TestSpracheJSON();
			//yield return new TestGold();
		}
	}
}

