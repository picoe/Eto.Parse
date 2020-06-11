using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Eto.Parse.TestSpeed.Tests.JsonAst
{
    public class SampleInfo
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public bool IsActive { get; set; }
        public string Balance { get; set; }
        public Uri PictureUrl { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
    }

    public class SampleObject
    {
        public int Id { get; set; }
        public string JsonRpc { get; set; }
        public int Total { get; set; }

        public List<SampleInfo> Result { get; set; }
    }


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

