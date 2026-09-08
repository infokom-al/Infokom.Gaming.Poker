using MemoryPools;

namespace Poker.Calc;

public static class SpanHelper
{
	public static bool Any<T>(this Span<T> items, Func<T, bool> predicate)
	{
		Span<T> span = items;
		for (int i = 0; i < span.Length; i++)
		{
			T arg = span[i];
			if (predicate(arg))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Contains<T>(this Span<T> items, T item)
	{
		Span<T> span = items;
		for (int i = 0; i < span.Length; i++)
		{
			T val = span[i];
			if (object.Equals(item, val))
			{
				return true;
			}
		}
		return false;
	}

	public static List<T> Except<T>(this IEnumerable<T> items, Span<T> other)
	{
		List<T> list = new List<T>();
		foreach (T item in items)
		{
			if (!other.Contains(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static bool TryGetLast<T>(this Span<T> items, out T result)
	{
		if (items.Length > 0)
		{
			result = items[items.Length - 1];
			return true;
		}
		result = default(T);
		return false;
	}

	public static T[] GetArray<T>(this Span<T> span)
	{
		if (span.Length == 0)
		{
			return EmptyArray<T>.Value;
		}
		T[] array = ArrayPool<T>.ThreadShared.GetArray(span.Length);
		span.CopyTo(array);
		return array;
	}

	public static Span<T> GetHead<T>(this Span<T> span, int headLength)
	{
		return span.Slice(0, headLength);
	}

	public static Span<T> CutHead<T>(this Span<T> span, int headLength)
	{
		return span.Slice(headLength, span.Length - headLength);
	}

	public static Span<T> CutTail<T>(this Span<T> span, int tailLength)
	{
		return span.Slice(0, span.Length - tailLength);
	}

	public static Span<T> GetTail<T>(this Span<T> span, int tailLength)
	{
		return span.Slice(span.Length - tailLength, tailLength);
	}
}
