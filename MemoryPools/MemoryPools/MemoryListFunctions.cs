using System;
using System.Collections.Generic;

namespace MemoryPools;

public static class MemoryListFunctions
{
	public static IEnumerable<T> Where<T>(this MemoryList<T> items, Func<T, bool> predicate)
	{
		MemoryList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current))
			{
				yield return current;
			}
		}
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this MemoryList<TIn> items, Func<TIn, IEnumerable<TOut>> selector)
	{
		MemoryList<TIn>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			foreach (TOut item in selector(current))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, MemoryList<TOut>> selector)
	{
		foreach (TIn item in items)
		{
			MemoryList<TOut>.Enumerator enumerator2 = selector(item).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				yield return enumerator2.Current;
			}
		}
	}

	public static bool SequenceEqual<T>(this MemoryList<T> first, MemoryList<T> second)
	{
		if (first.Count != second.Count)
		{
			return false;
		}
		for (int i = 0; i < first.Count; i++)
		{
			if (!object.Equals(first[i], second[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static T[] ToArray<T>(this MemoryList<T> items)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(items.Count);
		for (int i = 0; i < items.Count; i++)
		{
			array[i] = items[i];
		}
		return array;
	}

	public static MemoryList<T> ToMemoryList<T>(this IEnumerable<T> items)
	{
		MemoryList<T> result = default(MemoryList<T>);
		foreach (T item in items)
		{
			result = result.Add(item);
		}
		return result;
	}

	public static T[] ToArrayFromPool<T>(this Span<T> items)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(items.Length);
		items.CopyTo(array);
		return array;
	}

	public static T[] ToArrayFromPool<T>(this Span<T> items, ArrayPoolMap<T> arrayPool)
	{
		T[] array = arrayPool.GetArray(items.Length);
		items.CopyTo(array);
		return array;
	}

	public static T[] ToArrayFromPool<T>(this List<T> items)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(items.Count);
		CopyTo(items.GetPrivateItems(), array, items.Count);
		return array;
	}

	public static HashSet<T> ToHashSet<T>(this MemoryList<T> items)
	{
		HashSet<T> hashSet = new HashSet<T>(items.Count);
		MemoryList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			hashSet.Add(current);
		}
		return hashSet;
	}

	public static T[] CopyTo<T>(this T[] source, T[] destination, int count)
	{
		Array.Copy(source, 0, destination, 0, count);
		return destination;
	}

	public static T[] GetPrivateItems<T>(this List<T> list)
	{
		return ListItemsAccessor<T>.GetInternalItems(list);
	}

	public static int Sum<T>(this MemoryList<T> items, Func<T, int> selector)
	{
		int num = 0;
		MemoryList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			num += selector(current);
		}
		return num;
	}
}
