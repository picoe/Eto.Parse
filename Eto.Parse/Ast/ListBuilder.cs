using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace Eto.Parse.Ast
{

	public class ListBuilder<T, TRef> : Builder<TRef>
	{
		public Action<T, TRef> Add { get; set; }

		public override bool Visit(VisitArgs args)
		{
			var oldMatch = args.Match;
			IList<Match> matches;
			var ret = false;
			if (Name != null)
			{
				matches = args.Match.Find(Name).ToList();
			}
			else
			{
				matches = args.Match.Matches;
			}

            for (int i = 0; i < matches.Count; i++)
			{
				Match match = matches[i];
				args.Match = match;
				args.ResetChild();
				base.Visit(args);
				if (args.ChildSet)
				{
					Add((T)args.Instance, (TRef)args.Child);
					ret = true;
				}
			}
			args.Match = oldMatch;
			return ret;
		}
	}

}
