using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{
	public interface IBuilder
	{
		string Name { get; }

		bool Visit(VisitArgs args);

		void Initialize();

		IEnumerable<IBuilder> Builders { get; }
	}

}
