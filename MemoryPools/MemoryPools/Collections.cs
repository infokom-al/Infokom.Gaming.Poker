using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MemoryPools;

internal static class Collections
{
	public static int MaxOrDefault(this IEnumerable<int> items)
	{
		return items.MaxOrDefault((int x) => x);
	}

	public static int MaxOrDefault<T>(this IEnumerable<T> items, Func<T, int> selector)
	{
		int num = int.MinValue;
		bool flag = false;
		foreach (T item in items)
		{
			int num2 = selector(item);
			if (num2 > num)
			{
				num = num2;
			}
			if (!flag)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return 0;
		}
		return num;
	}

	public static ImmutableList<TOut> MapToImmutableList<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> convert)
	{
		return ImmutableList.CreateRange(enumerable.Select(convert));
	}

	public static List<TOut> MapToList<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> convert)
	{
		return enumerable.Select(convert).ToList();
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

	public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
	{
		foreach (T item in items)
		{
			action(item);
		}
	}

	public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> items)
	{
		int index = 0;
		foreach (T item in items)
		{
			yield return (index: index, value: item);
			index++;
		}
	}
}
