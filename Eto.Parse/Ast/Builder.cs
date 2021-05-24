using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace Eto.Parse.Ast
{
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

        public PropertyBuilder<T, TRet> ChildProperty<TRet>(string name, Action<T, TRet> setValue)
        {
			return Children(name).Property(setValue);
        }

        public PropertyBuilder<T, TRet> ChildProperty<TRet>(Action<T, TRet> setValue)
        {
			return Children().Property(setValue);
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

        public virtual bool Visit(VisitArgs args)
        {
            for (int i = 0; i < Builders.Count; i++)
            {
                var builder = Builders[i];
                if (builder.Visit(args))
					return true;
            }
			return false;
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
