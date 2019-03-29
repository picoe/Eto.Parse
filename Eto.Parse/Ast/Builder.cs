using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace Eto.Parse.Ast
{
	public class ChildBuilder<T> : Builder<T>
	{
		public override void Visit(VisitArgs args)
		{
			var match = args.Match;
			var matches = match.Matches;
			var matchesCount = matches.Count;
			for (int i = 0; i < matchesCount; i++)
			{
				args.Match = matches[i];
				base.Visit(args);
			}
			args.Match = match;
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

		IDictionary<string, IBuilder> builderLookup;
		IList<IBuilder> namedBuilders;
		IList<IBuilder> nullBuilders;

		public virtual void Initialize()
		{
			var allBuilders = GetBuilders();
			namedBuilders = allBuilders.Where(r => r.Name != null).ToList(); //.Union(builders.Where(r => r.Name == null)).ToList();
			if (namedBuilders.Count == 0)
				namedBuilders = null;
			builderLookup = allBuilders.Where(r => r.Name != null).ToDictionary(r => r.Name);
			if (builderLookup.Count == 0)
				builderLookup = null;
			nullBuilders = allBuilders.Where(r => r.Name == null).ToList();
			
		}

		public ChildBuilder<T> Children()
		{
			var builder = new ChildBuilder<T>();
			Builders.Add(builder);
			return builder;
		}

		public CreateBuilder<TRef> CreatedBy<TRef>(Func<TRef> create, Action<CreateBuilder<TRef>> map = null)
		{
			var builder = new CreateBuilder<TRef> { CreateInstance = create };
            map?.Invoke(builder);
            Builders.Add(builder);
            return builder;
		}

		public CreateBuilder<TRef> CreatedBy<TRef>(string name, Action<CreateBuilder<TRef>> map = null)
			where TRef : new()
		{
			return CreatedBy(name, () => new TRef(), map);
		}

		public CreateBuilder<TRef> CreatedBy<TRef>(string name, Func<TRef> create, Action<CreateBuilder<TRef>> map = null)
		{
			var builder = CreatedBy(create, map);
			builder.Name = name;
			return builder;
		}

		public PropertyBuilder<T, TRet> Property<TRet>(string name, Action<T, TRet> setValue, Action<PropertyBuilder<T, TRet>> map = null)
		{
			var builder = Property(setValue, map);
			builder.Name = name;
			return builder;
		}

		public PropertyBuilder<T, TRet> Property<TRet>(Action<T, TRet> setValue, Action<PropertyBuilder<T, TRet>> map = null)
		{
			var builder = new PropertyBuilder<T, TRet>();
			builder.SetValue = setValue;
            map?.Invoke(builder);
            Builders.Add(builder);
            return builder;
		}

		public ListBuilder<T, TRef> HasMany<TColl, TRef>(string name, Func<T, TColl> property, Action<ListBuilder<T, TRef>> map = null)
			where TRef : new()
			where TColl : class, ICollection<TRef>
		{
			return HasMany<TColl, TRef>(name, property, () => new TRef(), map);
		}

		public ListBuilder<T, TRef> HasMany<TColl, TRef>(string name, Func<T, TColl> property, Func<TRef> create, Action<ListBuilder<T, TRef>> map = null)
			where TColl : class, ICollection<TRef>
		{
			var builder = HasMany<TColl, TRef>(property, set: null, map: map);
			builder.CreatedBy(create);
			builder.Name = name;
			return builder;
		}

		public ListBuilder<T, TRef> HasMany<TColl, TRef>(Func<T, TColl> get, Action<T, TColl> set = null, Action<ListBuilder<T, TRef>> map = null)
			where TColl : class, ICollection<TRef>
		{
			var builder = new ListBuilder<T, TRef>();
			builder.Add = (o, v) =>
			{
				var list = get(o);
				if (list == null)
				{
					if (set != null)
						throw new InvalidOperationException("Could not set new collection instance to property");
					list = Activator.CreateInstance<TColl>();
					set(o, list);
				}
				list.Add(v);
			};
            map?.Invoke(builder);
            Builders.Add(builder);
            return builder;
		}

		public KeyValueBuilder<T, TKey, TRef> HasKeyValue<TKey, TRef>(Func<T, IDictionary<TKey, TRef>> property, IBuilder keyBuilder, IBuilder valueBuilder, Action<KeyValueBuilder<T, TKey, TRef>> map = null)
		{
			var builder = new KeyValueBuilder<T, TKey, TRef>();
			builder.KeyBuilder = keyBuilder;
			builder.ValueBuilder = valueBuilder;
			builder.Add = (o, k, v) =>
			{
				var list = property(o);
				list.Add(k, v);
			};
            map?.Invoke(builder);
            Builders.Add(builder);
            return builder;
		}

		public virtual void Visit(VisitArgs args)
		{
            /**/
            if (builderLookup != null && builderLookup.TryGetValue(args.Match.Name, out IBuilder builder))
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
						return;
					}
				}
			}
			/**/
			var nullCount = nullBuilders.Count;
			for (int i = 0; i < nullCount; i++)
			{
				nullBuilders[i].Visit(args);
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
