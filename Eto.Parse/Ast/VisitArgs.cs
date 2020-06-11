using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{

	public class VisitArgs
	{
		public Match Match { get; set; }

		public object Instance { get; set; }

		object child;

		public object Child
		{
			get { return child; }
			set
			{
				child = value;
				ChildSet = true;
			}
		}

		public bool ChildSet { get; private set; }

		public void ResetChild()
		{
			ChildSet = false;
			child = null;
		}
	}

}
