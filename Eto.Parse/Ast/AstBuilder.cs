using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{
	public class AstBuilder<T> : ChildrenBuilder<T>
	{
        bool initialized;

		public override void Initialize()
		{
			if (initialized)
				return;
			base.Initialize();
			initialized = true;
			Do(b => b.Initialize());
		}

		public T Build(Match match)
		{
            if (!initialized)
				Initialize();

			var args = new VisitArgs { Match = match };
			Visit(args);
			return (T)(args.Instance ?? args.Child);
		}
	}

}

