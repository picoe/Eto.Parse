using System.Collections.Generic;

namespace Eto.Parse.Ast
{
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

}
