using Newtonsoft.Json;
using Eto.Parse.Samples.Json.AstObject;

namespace Eto.Parse.TestSpeed.Tests.JsonAst
{

	public class NewtonsoftJsonBenchmark : Benchmark<JsonAstSuite, SampleObject>
    {
        public override SampleObject Execute(JsonAstSuite suite)
        {
            return JsonConvert.DeserializeObject<SampleObject>(suite.Json);
        }

        public override bool Verify(JsonAstSuite suite, SampleObject result)
        {
            return result != null;
        }
    }

}

