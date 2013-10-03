using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Parse.Samples.Json
{
	/// <summary>
	/// Base token class of an element matched in a json string
	/// </summary>
	/// <seealso cref="JsonGrammar"/>
	public abstract class JsonToken
	{
		static JsonGrammar grammar;
		protected static JsonGrammar Grammar { get { return grammar ?? (grammar = new JsonGrammar()); } }

		static readonly JsonNull nullLiteral = new JsonNull();

		/// <summary>
		/// Gets a singleton null json token for properties and elements that are null
		/// </summary>
		/// <value>The null value json token</value>
		public static JsonNull Null { get { return nullLiteral; } }

		/// <summary>
		/// Gets or sets the match that this token represents
		/// </summary>
		/// <value>The match for this token</value>
		public Match Match { get; set; }

		/// <summary>
		/// Gets the object value of this token
		/// </summary>
		/// <remarks>
		/// This will return an object representation of this token as follows:
		/// - <see cref="JsonObject"/>: returns a Dictionary&lt;string, object&gt;
		/// - <see cref="JsonArray"/>: returns a List&lt;object&gt;
		/// - <see cref="JsonLiteral"/>: returns a string, decimal, or boolean depending on the literal type
		/// - <see cref="JsonNull"/>: returns a null
		/// 
		/// For objects/arrays, it is not performant to call this method unless you want the entire child
		/// tree to be transformed into object values.
		/// </remarks>
		/// <value>The value representation of this token</value>
		public abstract object Value { get; }

		/// <summary>
		/// Parses the specified json into a token value
		/// </summary>
		/// <param name="json">Json string to parse</param>
		public static JsonToken Parse(string json)
		{
			var match = Grammar.Match(json);
			if (!match.Success)
				throw new ArgumentOutOfRangeException("json", string.Format("Invalid Json string: {0}", match.ErrorMessage));
			return GetToken(match.Matches.First());
		}

		internal static JsonToken GetToken(Match match)
		{
			switch (match.Name)
			{
				case "object":
					return new JsonObject { Match = match };
				case "array":
					return new JsonArray { Match = match };
				case "null":
					return JsonToken.Null;
				default:
					return new JsonLiteral { Match = match };
			}
		}

		internal static object GetObjectValue(Match match)
		{
			switch (match.Name)
			{
				case "object":
					return JsonObject.GetValue(match);
				case "array":
					return JsonArray.GetValue(match);
				case "null":
					return null;
				default:
					return match.Value;
			}
		}
	}

	/// <summary>
	/// Defines a null value json token
	/// </summary>
	public class JsonNull : JsonToken
	{
		/// <summary>
		/// Gets the object value of this token, which is null
		/// </summary>
		/// <value>The value of this token</value>
		public override object Value
		{
			get { return null; }
		}
	}

	/// <summary>
	/// Defines a literal json token, such as a string, number, or boolean value
	/// </summary>
	public class JsonLiteral : JsonToken
	{
		object value;

		/// <summary>
		/// Gets the object value of this token
		/// </summary>
		/// <value>The value of this token</value>
		public override object Value
		{
			get { return value ?? (value = Match.Value); }
		}
	}

	/// <summary>
	/// Defines an object json token which contains multiple named properties
	/// </summary>
	public class JsonObject : JsonToken, IDictionary<string, JsonToken>
	{
		/// <summary>
		/// Parses the specified json into a json object
		/// </summary>
		/// <remarks>
		/// Calling this assumes that the json string begins with an object.
		/// Otherwise, call <see cref="JsonToken.Parse"/> instead to return either a <see cref="JsonArray"/>
		/// or <see cref="JsonObject"/>
		/// </remarks>
		/// <param name="json">Json string to parse</param>
		public new static JsonObject Parse(string json)
		{
			return (JsonObject)JsonToken.Parse(json);
		}

		public static Match GetProperty(Match match, string propertyName)
		{
			return match.Matches.FirstOrDefault(r => r.Matches[0].StringValue == propertyName).Matches[1];
		}

		public static object GetValue(Match match)
		{
			var dic = new Dictionary<string, object>(match.Matches.Count);
			foreach (var child in match.Matches)
			{
				dic.Add(child.Matches[0].StringValue, GetObjectValue(child.Matches[1]));
			}
			return dic;
		}

		public override object Value
		{
			get { return GetValue(Match); }
		}

		public void Add(string key, JsonToken value)
		{
			throw ReadOnlyException();
		}

		public bool ContainsKey(string key)
		{
			return Match.Matches.Any(r => r.Matches[0].StringValue == key);
		}

		public bool Remove(string key)
		{
			throw ReadOnlyException();
		}

		public bool TryGetValue(string key, out JsonToken value)
		{
			var match = GetProperty(Match, key);
			if (match != null)
			{
				value = GetToken(match.Matches[1]);
				return true;
			}
			value = null;
			return false;
		}

		public JsonToken this [string index]
		{
			get
			{ 
				return GetToken(GetProperty(Match, index));
			}
			set { throw ReadOnlyException(); }
		}

		public ICollection<string> Keys
		{
			get { return new ReadOnlyCollection<string>(Match.Matches.Select(r => r.Matches[0].StringValue).ToList()); }
		}

		public ICollection<JsonToken> Values
		{
			get { return new ReadOnlyCollection<JsonToken>(Match.Matches.Select(r => GetToken(r.Matches[1])).ToList()); }
		}

		public void Add(KeyValuePair<string, JsonToken> item)
		{
			throw ReadOnlyException();
		}

		public void Clear()
		{
			throw ReadOnlyException();
		}

		public bool Contains(KeyValuePair<string, JsonToken> pair)
		{
			JsonToken y;
			return TryGetValue(pair.Key, out y) && EqualityComparer<JsonToken>.Default.Equals(pair.Value, y);
		}

		public void CopyTo(KeyValuePair<string, JsonToken>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public bool Remove(KeyValuePair<string, JsonToken> item)
		{
			throw ReadOnlyException();
		}

		public int Count
		{
			get { return Match.Matches.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		IEnumerable<KeyValuePair<string, JsonToken>> Enumerate()
		{
			for (int i = 0; i < Match.Matches.Count; i++)
			{
				var match = Match.Matches[i];
				yield return new KeyValuePair<string, JsonToken>(match.Matches[0].StringValue, GetToken(match.Matches[1]));
			}
		}

		public IEnumerator<KeyValuePair<string, JsonToken>> GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		static Exception ReadOnlyException()
		{
			return new NotSupportedException("This dictionary is read-only");
		}
	}

	public class JsonArray : JsonToken, IList<JsonToken>
	{
		/// <summary>
		/// Parses the specified json into a json array
		/// </summary>
		/// <remarks>
		/// Calling this assumes that the json string begins with an array.
		/// Otherwise, call <see cref="JsonToken.Parse"/> instead to return either a <see cref="JsonArray"/>
		/// or <see cref="JsonObject"/>
		/// </remarks>
		/// <param name="json">Json string to parse</param>
		public new static JsonArray Parse(string json)
		{
			return (JsonArray)JsonToken.Parse(json);
		}

		public static object GetValue(Match match)
		{
			var list = new List<object>(match.Matches.Count);
			foreach (var child in match.Matches)
			{
				list.Add(GetObjectValue(child));
			}
			return list;
		}

		public override object Value
		{
			get { return GetValue(Match); }
		}

		public int IndexOf(JsonToken item)
		{
			return Match.Matches.IndexOf(item.Match);
		}

		public void Insert(int index, JsonToken item)
		{
			throw ReadOnlyException();
		}

		public void RemoveAt(int index)
		{
			throw ReadOnlyException();
		}

		public JsonToken this [int index]
		{
			get { return GetToken(Match.Matches[index]); }
			set { throw ReadOnlyException(); }
		}

		static Exception ReadOnlyException()
		{
			return new NotSupportedException("This list is read-only");
		}

		public void Add(JsonToken item)
		{
			throw ReadOnlyException();
		}

		public void Clear()
		{
			throw ReadOnlyException();
		}

		public bool Contains(JsonToken item)
		{
			return Match.Matches.Contains(item.Match);
		}

		public void CopyTo(JsonToken[] array, int arrayIndex)
		{
			Match.Matches.Select(GetToken).ToArray().CopyTo(array, arrayIndex);
		}

		public bool Remove(JsonToken item)
		{
			throw ReadOnlyException();
		}

		public int Count
		{
			get { return Match.Matches.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		class Enumerator : IEnumerator<JsonToken>
		{
			readonly MatchCollection matches;
			int index = -1;

			public Enumerator(MatchCollection matches)
			{
				this.matches = matches;
			}

			public bool MoveNext()
			{
				index++;
				return index < matches.Count;
			}

			public void Reset()
			{
				index = -1;
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}

			public void Dispose()
			{
			}

			public JsonToken Current
			{
				get { return GetToken(matches[index]); }
			}
		}

		public IEnumerator<JsonToken> GetEnumerator()
		{
			return new Enumerator(Match.Matches);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

