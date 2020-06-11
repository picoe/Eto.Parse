using System;
using System.Collections.Generic;
using System.Text;


namespace Eto.Parse.Samples.Json.Ast
{
	public abstract class JsonToken
	{
		internal static string IndentString(string str, string indent = "  ")
		{
			var sb = new StringBuilder();
			var lines = str.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				sb.AppendLine(indent + line);
			}
			return sb.ToString();
		}

	}

    public class JsonValue : JsonToken
    {
		public object Value { get; set; }

        public override string ToString()
		{
			return string.Format("[{0}: Value={1}]", GetType().Name, Value);
		}
	}

	public class JsonArray : JsonToken, IList<JsonToken>, ICollection<JsonToken>
	{
		List<JsonToken> nodes = new List<JsonToken>();

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Array: Nodes=");
			foreach (var node in nodes)
			{
				if (node == null)
					continue;
				sb.AppendLine(IndentString(node.ToString()));
			}
			return sb.ToString();
		}

		#region IList implementation

		public int IndexOf(JsonToken item)
		{
			return nodes.IndexOf(item);
		}

		public void Insert(int index, JsonToken item)
		{
			nodes.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			nodes.RemoveAt(index);
		}

		public JsonToken this[int index]
		{
			get { return nodes[index]; }
			set { nodes[index] = value; }
		}

		#endregion

		#region ICollection implementation

		public void Add(JsonToken item)
		{
			nodes.Add(item);
		}

		public void Clear()
		{
			nodes.Clear();
		}

		public bool Contains(JsonToken item)
		{
			return nodes.Contains(item);
		}

		public void CopyTo(JsonToken[] array, int arrayIndex)
		{
			nodes.CopyTo(array, arrayIndex);
		}

		public bool Remove(JsonToken item)
		{
			return nodes.Remove(item);
		}

		public int Count
		{
			get { return nodes.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<JsonToken>)nodes).IsReadOnly; }
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<JsonToken> GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}

	public class JsonObject : JsonToken, IDictionary<string, JsonToken>
	{
		Dictionary<string, JsonToken> properties = new Dictionary<string, JsonToken>(StringComparer.Ordinal);

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendLine("Object: Properties=");
			foreach (var prop in properties)
			{
				sb.AppendLine(string.Format("  {0}={1}", prop.Key, prop.Value));
			}
			return sb.ToString();
		}

		#region IDictionary implementation

		public void Add(string key, JsonToken value)
		{
			properties.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return properties.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return properties.Remove(key);
		}

		public bool TryGetValue(string key, out JsonToken value)
		{
			return properties.TryGetValue(key, out value);
		}

		public JsonToken this[string index]
		{
			get { return properties[index]; }
			set { properties[index] = value; }
		}

		public ICollection<string> Keys
		{
			get { return properties.Keys; }
		}

		public ICollection<JsonToken> Values
		{
			get { return properties.Values; }
		}

		#endregion

		#region ICollection implementation

		public void Add(KeyValuePair<string, JsonToken> item)
		{
			properties.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			properties.Clear();
		}

		public bool Contains(KeyValuePair<string, JsonToken> item)
		{
			return ((ICollection<KeyValuePair<string, JsonToken>>)properties).Contains(item);
		}

		public void CopyTo(KeyValuePair<string, JsonToken>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, JsonToken>>)properties).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, JsonToken> item)
		{
			return ((ICollection<KeyValuePair<string, JsonToken>>)properties).Remove(item);
		}

		public int Count
		{
			get { return properties.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<string, JsonToken>>)properties).IsReadOnly; }
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<KeyValuePair<string, JsonToken>> GetEnumerator()
		{
			return properties.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return properties.GetEnumerator();
		}

		#endregion
	}
}

