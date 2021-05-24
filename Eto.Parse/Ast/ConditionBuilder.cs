using System;

namespace Eto.Parse.Ast
{
	public class ConditionBuilder<T> : Builder<T>
    {
        public string Value { get; set; }

        public StringComparison Comparison { get; set; } = StringComparison.Ordinal;

        public override bool Visit(VisitArgs args)
        {
			var old = args.Match;
			bool ret = false;
            var match = args.Match.Matches[Name];
            if (match.Success)
            {
                var val = match.StringValue;
                if (string.Equals(val, Value, Comparison))
				{
                    ret = base.Visit(args);
				}
            }
			args.Match = old;
			return ret;
        }
    }

}
