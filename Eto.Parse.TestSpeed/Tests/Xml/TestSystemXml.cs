using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.Xml;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestSystemXml : Benchmark<XmlSuite, XmlDocument>
	{
		public override XmlDocument Execute(XmlSuite suite)
		{
			var doc = new XmlDocument();
			doc.LoadXml(suite.Xml);
			return doc;
		}
	}
}
