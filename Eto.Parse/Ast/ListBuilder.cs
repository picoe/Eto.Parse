using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{

	public class ListBuilder<T, TRef> : Builder<TRef>
	{
		public Action<T, TRef> Add { get; set; }

		public override void Visit(VisitArgs args)
		{
			args.ResetChild();
			base.Visit(args);
			if (args.ChildSet)
				Add((T)args.Instance, (TRef)args.Child);
		}
	}

}
