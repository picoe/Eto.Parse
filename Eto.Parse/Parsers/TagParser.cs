using System;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class TagParser : UnaryParser
	{
		/// <summary>
		/// Sets the tag to add to the parsing tree for all children parsers
		/// </summary>
		/// <value>The tag to add for all children.</value>
		public string AddTag { get; set; }

		/// <summary>
		/// Only match when the specified tag is defined
		/// </summary>
		/// <value>The tag to require when matching.</value>
		public string IncludeTag { get; set; }

		/// <summary>
		/// Only match when the specified tag is NOT defined
		/// </summary>
		/// <value>The tag to exclude the child from matching.</value>
		public string ExcludeTag { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the tags only apply when at the same position the tag was added.
		/// </summary>
		/// <value><c>true</c> if allow with different position; otherwise, <c>false</c>.</value>
		public bool AllowWithDifferentPosition { get; set; }

		static readonly object tagKey = new object();

		protected TagParser(TagParser other, ParserCloneArgs args)
			: base(other, args)
		{
			AddTag = other.AddTag;
			IncludeTag = other.IncludeTag;
			ExcludeTag = other.ExcludeTag;
			AllowWithDifferentPosition = other.AllowWithDifferentPosition;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Parse.Parsers.TagParser"/> class.
		/// </summary>
		public TagParser()
		{
		}

		protected override int InnerParse(ParseArgs args)
		{
			object tagsObject;
			if (!args.Properties.TryGetValue(tagKey, out tagsObject))
			{
				args.Properties[tagKey] = tagsObject = new HashSet<string>();
			}
			var tags = (HashSet<string>)tagsObject;
			var pos = args.Scanner.Position;
			if (AllowWithDifferentPosition)
			{
				if (!string.IsNullOrEmpty(ExcludeTag) && tags.Contains(ExcludeTag + pos))
				{
					return -1;
				}
				if (!string.IsNullOrEmpty(IncludeTag) && !tags.Contains(IncludeTag + pos))
				{
					return -1;
				}

				if (!string.IsNullOrEmpty(AddTag))
				{
					tags.Add(AddTag + pos);
					var ret = base.InnerParse(args);
					tags.Remove(AddTag);
					return ret;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(ExcludeTag) && tags.Contains(ExcludeTag))
				{
					return -1;
				}

				if (!string.IsNullOrEmpty(IncludeTag) && !tags.Contains(IncludeTag))
				{
					return -1;
				}

				if (!string.IsNullOrEmpty(AddTag))
				{
					var added = tags.Add(AddTag);
					var ret = base.InnerParse(args);
					if (added)
						tags.Remove(AddTag);
					return ret;
				}
			}
			return base.InnerParse(args);
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new TagParser(this, args);
		}
	}
}

