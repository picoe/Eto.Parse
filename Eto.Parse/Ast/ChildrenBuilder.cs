namespace Eto.Parse.Ast
{
	public class ChildrenBuilder<T> : Builder<T>
    {
        public ChildrenBuilder()
        {
        }

        public override sealed bool Visit(VisitArgs args)
        {
            var match = args.Match;
			var ret = false;
            if (Name == null)
            {
                var matches = match.Matches;
                var matchesCount = matches.Count;
				for (int i = 0; i < matchesCount; i++)
				{
					args.Match = matches[i];
					ret |= VisitMatch(args);
				}
            }
            else
            {
                var matches = match.Matches;
                var matchesCount = matches.Count;
				for (int i = 0; i < matches.Count; i++)
                {
					var m = matches[i];
					if (m.Name != Name)
						continue;
						
					args.Match = m;
                    ret |= VisitMatch(args);
                }
            }
            args.Match = match;
			return ret;
        }

        protected virtual bool VisitMatch(VisitArgs args)
        {
			args.ResetChild();
            base.Visit(args);
			return args.ChildSet;
        }
    }

}
