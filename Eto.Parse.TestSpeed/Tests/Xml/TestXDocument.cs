using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestXDocument : Test<XmlTestSuite>
	{
		public TestXDocument()
			: base("XDocument")
		{
		}

		public override void Warmup(XmlTestSuite suite)
		{
			var doc = XDocument.Load(new StringReader(suite.Xml));
		}

		public override void PerformTest(XmlTestSuite suite, StringBuilder output)
		{
			var doc = XDocument.Load(new StringReader(suite.Xml));
		}
	}
}

