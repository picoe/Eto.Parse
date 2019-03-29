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

		void Visit(VisitArgs args);

		void Initialize();

		IEnumerable<IBuilder> Builders { get; }
	}

}
