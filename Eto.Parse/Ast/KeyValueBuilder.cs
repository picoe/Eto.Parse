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

		public override bool Visit(VisitArgs args)
		{
			TRef value = default(TRef);
			TKey key = default(TKey);

			var old = args.Match;

			var matches = args.Match.Matches;
			var matchCount = matches.Count;
			if (matchCount == 0)
				return false;
			var keySet = false;
			var valueSet = false;
			for (int i = 0; i < matchCount; i++)
			{
				var match = args.Match = matches[i];
				args.ResetChild();
				if (namedKey && !keySet && match.Name == keyName)
				{
					keySet = KeyBuilder.Visit(args);
					key = (TKey)args.Child;
					continue;
				}

				if (namedValue && match.Name == valueName)
				{
					valueSet = ValueBuilder.Visit(args);
					value = (TRef)args.Child;
					continue;
				}
				
				if (!namedKey && !keySet)
				{
					keySet = KeyBuilder.Visit(args);
					if (args.ChildSet)
					{
						key = (TKey)args.Child;
						continue;
					}
				}
				
				if (!namedValue)
				{
					valueSet = ValueBuilder.Visit(args);
					if (args.ChildSet)
					{
						value = (TRef)args.Child;
						continue;
					}
				}
				
				if (keySet && valueSet)
					break;
			}
			
			var ret = keySet && valueSet;
			
			if (ret)
				Add((T)args.Instance, key, value);
				
			args.Match = old;
			return ret;
		}
	}
}
