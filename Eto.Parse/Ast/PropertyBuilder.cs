using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{
	public class ValueBuilder : IBuilder
	{
		public void Visit(VisitArgs args)
		{
			args.Child = args.Match.Value;
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

		public override void Visit(VisitArgs args)
		{
			var instance = args.Instance;
			object val;
			if (Name != null)
			{
				var match = args.Match[Name];
				if (!match.Success)
					return;
				val = match.Value;
			}
			else
			{
				val = args.Match.Value;
			}

			if (!(val is TRet))
				val = Convert.ChangeType(val, typeof(TRet));

			SetValue((T)instance, (TRet)val); 

			args.Child = val;
		}
	}

}
