using System;
using System.IO;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Tests.Markdown;
using Eto.Parse.Samples.Markdown;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Eto.Parse.TestSpeed
{
	static class MainClass
	{
		public static void Main(string[] args)
		{
			new Tests.JsonAst.JsonAstLarge().VerifyAll();
			new Tests.Json.JsonLarge().VerifyAll();

			var config = new ManualConfig();
			config.Add(DefaultConfig.Instance.GetLoggers().ToArray());
			config.Add(DefaultConfig.Instance.GetExporters().ToArray());
			config.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());

			config.Add(JitOptimizationsValidator.DontFailOnError);
			config.Add(Job.Default);
			config.Add(MemoryDiagnoser.Default);
			config.Add(StatisticColumn.OperationsPerSecond);
			config.Add(RankColumn.Arabic);

			var switcher = BenchmarkSwitcher.FromAssembly(typeof(MainClass).Assembly);
			switcher.Run(args, config);
		}
	}
}
