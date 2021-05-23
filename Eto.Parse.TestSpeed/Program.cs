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
			config.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
			config.AddExporter(DefaultConfig.Instance.GetExporters().ToArray());
			config.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());

			config.AddValidator(JitOptimizationsValidator.DontFailOnError);
			config.AddJob(Job.Default);
			config.AddDiagnoser(MemoryDiagnoser.Default);
			config.AddColumn(StatisticColumn.OperationsPerSecond);
			config.AddColumn(RankColumn.Arabic);

			var switcher = BenchmarkSwitcher.FromAssembly(typeof(MainClass).Assembly);
			switcher.Run(args, config);
		}
	}
}
