using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{
	public class ValueBuilder : IBuilder
	{
		public bool Visit(VisitArgs args)
		{
			args.Child = args.Match.Value;
			return true;
		}

		public string Name { get; set; }

		public void Initialize()
		{
		}

		public IEnumerable<IBuilder> Builders
		{
			get { yield break; }
		}
	}

	public class PropertyBuilder<T, TRet> : Builder<TRet>
	{
		public Action<T, TRet> SetValue { get; set; }

		public override bool Visit(VisitArgs args)
		{
			var instance = args.Instance;
			object val;
			if (Name != null && args.Match.Name != Name)
			{
				return false;
			}
			else
			{
				val = args.Match.Value;
			}

			if (!(val is TRet))
				val = Convert.ChangeType(val, typeof(TRet));

			SetValue((T)instance, (TRet)val); 

			args.Child = val;
			return true;
		}
	}

}
