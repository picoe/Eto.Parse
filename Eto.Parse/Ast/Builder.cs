using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace Eto.Parse.Ast
{
    public class ChildrenBuilder<T> : Builder<T>
    {
        public ChildrenBuilder()
        {
        }

        public override sealed void Visit(VisitArgs args)
        {
            var match = args.Match;
            if (Name == null)
            {
                var matches = match.Matches;
                var matchesCount = matches.Count;
                for (int i = 0; i < matchesCount; i++)
                {
                    args.Match = matches[i];
                    VisitMatch(args);
                }
            }
            else if (match.Name == Name)
            {
                VisitMatch(args);
            }
            else
            {
                var matches = match.Find(Name);
                foreach (var m in matches)
                {
                    args.Match = m;
                    VisitMatch(args);
                }
            }
            args.Match = match;
        }

        protected virtual void VisitMatch(VisitArgs args)
        {
            base.Visit(args);
        }
    }

    public class ConditionBuilder<T> : Builder<T>
    {
        public string Value { get; set; }

        public StringComparison Comparison { get; set; } = StringComparison.Ordinal;

        public override void Visit(VisitArgs args)
        {
            var old = args.Match;

            var match = old.Matches[Name];
            if (match.Success)
            {
                var val = Convert.ToString(match.Value);
                if (string.Equals(val, Value, Comparison))
                    base.Visit(args);
            }
            args.Match = old;
        }
    }

    public static class BuilderExtensions
    {
        public static ListBuilder<T, TRef> HasMany<T, TRef>(this Builder<T> builder)
            where T : ICollection<TRef>
        {
            var child = new ListBuilder<T, TRef>();
            child.Add = (o, v) => o.Add(v);
            builder.Builders.Add(child);
            return child;
        }

        public static KeyValueBuilder<T, TKey, TRef> HasKeyValue<T, TKey, TRef>(this Builder<T> builder, IBuilder keyBuilder, IBuilder valueBuilder)
            where T : IDictionary<TKey, TRef>
        {
            var child = new KeyValueBuilder<T, TKey, TRef>();
            child.KeyBuilder = keyBuilder;
            child.ValueBuilder = valueBuilder;
            child.Add = (o, k, v) => o.Add(k, v);
            builder.Builders.Add(child);
            return child;
        }
    }

    public class Builder<T> : IBuilder
    {
        public string Name { get; set; }

        public List<IBuilder> Builders { get; } = new List<IBuilder>();

        IEnumerable<IBuilder> IBuilder.Builders => GetBuilders();

        protected virtual IEnumerable<IBuilder> GetBuilders() => Builders;

        // IDictionary<string, IBuilder> builderLookup;
        // IList<IBuilder> namedBuilders;
        // IList<IBuilder> nullBuilders;

        public virtual void Initialize()
        {
            /*
			var allBuilders = GetBuilders();
			namedBuilders = allBuilders.Where(r => r.Name != null).ToList(); //.Union(builders.Where(r => r.Name == null)).ToList();
			if (namedBuilders.Count == 0)
				namedBuilders = null;
			//builderLookup = allBuilders.Where(r => r.Name != null).ToDictionary(r => r.Name);
			//if (builderLookup.Count == 0)
			//	builderLookup = null;
			nullBuilders = allBuilders.Where(r => r.Name == null).ToList();
			*/

        }

        public ChildrenBuilder<T> Children(string name = null)
        {
            var builder = new ChildrenBuilder<T>();
            builder.Name = name;
            Builders.Add(builder);
            return builder;
        }

        public CreateBuilder<TRef> Create<TRef>()
            where TRef : new()
        {
            return Create<TRef>(() => new TRef());
        }

        public CreateBuilder<TRef> Create<TRef>(Func<TRef> create)
        {
            var builder = new CreateBuilder<TRef> { CreateInstance = create };
            Builders.Add(builder);
            return builder;
        }

        public CreateBuilder<TRef> Create<TRef>(string name)
            where TRef : new()
        {
            return Create(name, () => new TRef());
        }

        public CreateBuilder<TRef> Create<TRef>(string name, Func<TRef> create)
        {
            var builder = Create(create);
            builder.Name = name;
            return builder;
        }

        public PropertyBuilder<T, TRet> Property<TRet>(string name, Action<T, TRet> setValue)
        {
            var builder = Property(setValue);
            builder.Name = name;
            return builder;
        }

        public PropertyBuilder<T, TRet> Property<TRet>(Action<T, TRet> setValue)
        {
            var builder = new PropertyBuilder<T, TRet>();
            builder.SetValue = setValue;
            Builders.Add(builder);
            return builder;
        }

        public ConditionBuilder<T> Condition(string name, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            var builder = new ConditionBuilder<T>();
            builder.Name = name;
            builder.Value = value;
            builder.Comparison = comparison;
            Builders.Add(builder);
            return builder;
        }

        public ListBuilder<T, TRef> HasMany<TColl, TRef>(string name, Func<T, TColl> property)
            where TRef : new()
            where TColl : class, ICollection<TRef>
        {
            return HasMany<TColl, TRef>(name, property, () => new TRef());
        }

        public ListBuilder<T, TRef> HasMany<TColl, TRef>(string name, Func<T, TColl> property, Func<TRef> create)
            where TColl : class, ICollection<TRef>
        {
            var builder = HasMany<TColl, TRef>(name, property, set: null);
            builder.Create(create);
            return builder;
        }

        public ListBuilder<T, TRef> HasMany<TColl, TRef>(string name, Func<T, TColl> get, Action<T, TColl> set = null)
            where TColl : class, ICollection<TRef>
        {
			var builder = HasMany<TColl, TRef>(get, set);
			builder.Name = name;
			return builder;
        }

        public ListBuilder<T, TRef> HasMany<TColl, TRef>(Func<T, TColl> get, Action<T, TColl> set = null)
            where TColl : class, ICollection<TRef>
        {
            var builder = new ListBuilder<T, TRef>();
            builder.Add = (o, v) =>
            {
                var list = get(o);
                if (list == null)
                {
                    if (set == null)
                        throw new InvalidOperationException("Could not set new collection instance to property");
                    list = Activator.CreateInstance<TColl>();
                    set(o, list);
                }
                list.Add(v);
            };
            Builders.Add(builder);
            return builder;
        }

        public KeyValueBuilder<T, TKey, TRef> HasKeyValue<TKey, TRef>(Func<T, IDictionary<TKey, TRef>> property, IBuilder keyBuilder, IBuilder valueBuilder)
        {
            var builder = new KeyValueBuilder<T, TKey, TRef>();
            builder.KeyBuilder = keyBuilder;
            builder.ValueBuilder = valueBuilder;
            builder.Add = (o, k, v) =>
            {
                var list = property(o);
                list.Add(k, v);
            };
            Builders.Add(builder);
            return builder;
        }

        public virtual void Visit(VisitArgs args)
        {
            /**
			var matchName = args.Match.Name;

			if (matchName != null && builderLookup != null && builderLookup.TryGetValue(matchName, out IBuilder builder))
            {
                builder.Visit(args);
                return;
            }
            /**
            if (namedBuilders != null)
			{
				var name = args.Match.Name;
				var builderCount = namedBuilders.Count;
				for (int i = 0; i < builderCount; i++)
				{
					var builder = namedBuilders[i];
					if (builder.Name == name)
					{
						builder.Visit(args);
					}
				}
			}
			/**
			var nullCount = nullBuilders.Count;
			for (int i = 0; i < nullCount; i++)
			{
				nullBuilders[i].Visit(args);
			}
			/**/
            for (int i = 0; i < Builders.Count; i++)
            {
                var builder = Builders[i];
                builder.Visit(args);
            }
            /**/
        }

        internal void Do(Action<IBuilder> action)
        {
            var inited = new HashSet<IBuilder>();
            var stack = new Stack<IBuilder>();
            stack.Push(this);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                action(current);
                inited.Add(current);
                foreach (var item in current.Builders)
                {
                    if (!inited.Contains(item))
                        stack.Push(item);
                }
            }
        }

    }

}
