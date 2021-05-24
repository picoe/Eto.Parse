using System;
using System.Collections.Generic;
using Eto.Parse.Ast;

namespace Eto.Parse.Samples.Json.AstObject
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
		public double Latitude { get; set; }
	}

	public class SampleObject
	{
		public int Id { get; set; }
		public string JsonRpc { get; set; }
		public int Total { get; set; }

		public List<SampleInfo> Result { get; set; }
	}


	/// <summary>
	/// Ast for json to a concrete object, for a specific json schema
	/// </summary>
	public class JsonObjectAstBuilder : AstBuilder<SampleObject>
	{
		public JsonObjectAstBuilder()
		{
			var top = this.Create<SampleObject>("object");
			var properties = top.Children("property");
			properties.Condition("name", "id").ChildProperty<int>("number", (o, v) => o.Id = v);
			properties.Condition("name", "jsonrpc").ChildProperty<string>("string", (o, v) => o.JsonRpc = v);
			properties.Condition("name", "total").ChildProperty<int>("number", (o, v) => o.Total = v);
			var child = properties.Condition("name", "result").Children("array")
				.HasMany<List<SampleInfo>, SampleInfo>(c => c.Result, (c, v) => c.Result = v)
				.Create<SampleInfo>("object").Children("property");

			child.Condition("name", "id").ChildProperty<int>("number", (o, v) => o.Id = v);
			child.Condition("name", "age").ChildProperty<int>("number", (o, v) => o.Age = v);
			child.Condition("name", "isActive").ChildProperty<bool>("bool", (o, v) => o.IsActive = v);
			child.Condition("name", "name").ChildProperty<string>("string", (o, v) => o.Name = v);
			child.Condition("name", "guid").ChildProperty<string>("string", (o, v) =>
			{
				if (Guid.TryParse(v, out var g)) o.Guid = g;
			});
			child.Condition("name", "latitude").ChildProperty<double>("number", (o, v) => o.Latitude = v);

			Initialize();

		}
	}
}