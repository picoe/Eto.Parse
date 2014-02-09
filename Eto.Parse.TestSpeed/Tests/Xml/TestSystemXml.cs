using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.Xml;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestSystemXml : Test<XmlTestSuite>
	{
		public TestSystemXml()
			: base("System.Xml")
		{
		}

		public override void Warmup(XmlTestSuite suite)
		{
			var doc = new XmlDocument();
			doc.LoadXml(suite.Xml);
		}

		public override void PerformTest(XmlTestSuite suite, StringBuilder output)
		{
			var doc = new XmlDocument();
			doc.LoadXml(suite.Xml);
		}
	}
}

