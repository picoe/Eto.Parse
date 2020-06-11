using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Parse.Ast
{
	public class KeyValueBuilder<T, TKey, TRef> : Builder<TRef>
	{
		public IBuilder KeyBuilder { get; set; }

		public IBuilder ValueBuilder { get; set; }

		public Action<T, TKey, TRef> Add { get; set; }

		protected override IEnumerable<IBuilder> GetBuilders()
		{
			return base.GetBuilders().Union(new [] { KeyBuilder, ValueBuilder });
		}

		bool namedKey;
		bool namedValue;
		string keyName;
		string valueName;
		public override void Initialize()
		{
			base.Initialize();
			namedKey = KeyBuilder.Name != null;
			keyName = KeyBuilder.Name;
			namedValue = ValueBuilder.Name != null;
			valueName = ValueBuilder.Name;
		}

		public override void Visit(VisitArgs args)
		{
			TRef value = default(TRef);
			TKey key = default(TKey);

			var old = args.Match;

			var matches = args.Match.Matches;
			var matchCount = matches.Count;
			for (int i = 0; i < matchCount; i++)
			{
				var match = args.Match = matches[i];
				args.ResetChild();
				if (namedKey && match.Name == keyName)
				{
					KeyBuilder.Visit(args);
					key = (TKey)args.Child;
					continue;
				}

				if (namedValue && match.Name == valueName)
				{
					ValueBuilder.Visit(args);
					value = (TRef)args.Child;
					continue;
				}
				if (!namedKey)
				{
					KeyBuilder.Visit(args);
					if (args.ChildSet)
					{
						key = (TKey)args.Child;
						continue;
					}
				}
				if (!namedValue)
				{
					ValueBuilder.Visit(args);
					if (args.ChildSet)
					{
						value = (TRef)args.Child;
						continue;
					}
				}
			}

			Add((T)args.Instance, key, value);
			args.Match = old;
		}
	}
}
