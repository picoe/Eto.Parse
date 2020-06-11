using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

namespace Eto.Parse.Ast
{
    public class CreateBuilder<T> : ChildrenBuilder<T>
    {
        public Func<T> CreateInstance { get; set; }

        protected override void VisitMatch(VisitArgs args)
        {
            var oldMatch = args.Match;
            if (Name != null && oldMatch.Name != Name)
            {
                var match = oldMatch[Name];
                if (!match.Success)
                    return;
                args.Match = match;
            }

            if (CreateInstance != null)
            {
                var old = args.Instance;
                var val = CreateInstance();
                args.Instance = val;

                base.VisitMatch(args);

                args.Instance = old;
                args.Child = val;
            }
            else
                base.VisitMatch(args);

            args.Match = oldMatch;
        }

    }

}
