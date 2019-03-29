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

		public override void Visit(VisitArgs args)
		{
			if (CreateInstance != null)
			{
				var old = args.Instance;
				var val = CreateInstance();
				args.Instance = val;

				base.Visit(args);

				args.Instance = old;
				args.Child = val;
			}
			else
				base.Visit(args);
		}

	}

}
