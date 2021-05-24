using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{
	public class CreateBuilder<T> : Builder<T>
	{
		public Func<T> CreateInstance { get; set; }

		public override bool Visit(VisitArgs args)
		{
			var oldMatch = args.Match;
			if (Name != null && oldMatch.Name != Name)
			{
				return false;
			}
			bool ret;

			if (CreateInstance != null)
			{
				var old = args.Instance;
				var val = CreateInstance();
				args.Instance = val;

				base.Visit(args);
				ret = true;

				args.Instance = old;
				args.Child = val;
			}
			else
				ret = base.Visit(args);

			args.Match = oldMatch;
			return ret;
		}

	}

}
