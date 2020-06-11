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
	public class TestXDocument : Benchmark<XmlSuite, XDocument>
	{
		public override XDocument Execute(XmlSuite suite)
		{
			return XDocument.Load(new StringReader(suite.Xml));
		}
	}
}