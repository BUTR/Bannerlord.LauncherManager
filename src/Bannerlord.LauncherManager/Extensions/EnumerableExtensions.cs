using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Extensions;

public static class EnumerableExtensions
{
    public readonly struct EnumerableMetadata<T>
    {
        public T Value { get; private init; }
        public int Index { get; private init; }
        public bool IsFirst { get; private init; }
        public bool IsLast { get; private init; }

        public EnumerableMetadata(T value, int index, bool isFirst, bool isLast) : this()
        {
            Value = value;
            Index = index;
            IsFirst = isFirst;
            IsLast = isLast;
        }
    }

    public static IEnumerable<EnumerableMetadata<T>> WithMetadata<T>(this IEnumerable<T> elements)
    {
        using var enumerator = elements.GetEnumerator();
        var isFirst = true;
        var hasNext = enumerator.MoveNext();
        var index = 0;
        while (hasNext)
        {
            var current = enumerator.Current;
            hasNext = enumerator.MoveNext();
            yield return new EnumerableMetadata<T>(current, index, isFirst, !hasNext);
            isFirst = false;
            index++;
        }
    }

#if !NETSTANDARD2_1_OR_GREATER
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) => new(enumerable);
#endif
}