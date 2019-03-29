using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{
	public class AstBuilder<T> : ChildBuilder<T>
	{
        bool initialized;

        public T Build(Match match)
		{
            if (!initialized)
            {
                Do(b => b.Initialize());
                initialized = true;
            }

			var args = new VisitArgs { Match = match };
			Visit(args);
			return (T)(args.Instance ?? args.Child);
		}
	}

}

