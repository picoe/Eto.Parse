using System;
using System.IO;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class XmlSuite : BenchmarkSuite
	{
		public string Xml { get; private set; }

		public XmlSuite(string name = null, string sample = "sample-large.xml")
		{
			sample = "Eto.Parse.TestSpeed.Tests.Xml." + sample;
			Xml = new StreamReader(typeof(XmlSuite).Assembly.GetManifestResourceStream(sample)).ReadToEnd();
		}


		[Benchmark]
		public void Eto() => RunBenchmark<EtoBenchmark>();

		[Benchmark]
		public void EtoFromEbnf() => RunBenchmark<TestEtoFromEbnf>();

		[Benchmark]
		public void EtoFromGold() => RunBenchmark<TestEtoFromGold>();

		[Benchmark]
		public void SystemXml() => RunBenchmark<TestSystemXml>();

		[Benchmark]
		public void XDocument() => RunBenchmark<TestXDocument>();

		[Benchmark]
		public void BsnGold() => RunBenchmark<TestBsnGold>();
	}
}

