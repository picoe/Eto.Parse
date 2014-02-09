using System;
using System.IO;
using System.Collections.Generic;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class XmlTestSuite : TestSuite
	{
		public string Xml { get; private set; }

		public XmlTestSuite(string name = null, string sample = "sample-large.xml")
			: base("Xml" + (name != null ? " " + name : ""))
		{
			Iterations = 10;

			sample = "Eto.Parse.TestSpeed.Tests.Xml." + sample;
			Xml = new StreamReader(typeof(MainClass).Assembly.GetManifestResourceStream(sample)).ReadToEnd();
		}

		public override IEnumerable<ITest> GetTests()
		{
			yield return new TestEto();
			yield return new TestEtoFromGold();
			yield return new TestSystemXml();
			yield return new TestXDocument();
			yield return new TestBsnGold();
		}
	}
}

