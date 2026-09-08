using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace MemoryPools;

public static class SpanListFunctions
{
	public static Span<T> Where<T>(this Span<T> items, Func<T, bool> predicate)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(16);
		SpanList<T> spanList = new SpanList<T>(array);
		Span<T> span = items;
		for (int i = 0; i < span.Length; i++)
		{
			T val = span[i];
			if (spanList.Count >= array.Length)
			{
				spanList = spanList.Resize(array);
			}
			if (predicate(val))
			{
				spanList.Add(val);
			}
		}
		return spanList;
	}

	public static Span<T> Where<T, TArgumentOne>(this Span<T> items, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(16);
		SpanList<T> spanList = new SpanList<T>(array);
		Span<T> span = items;
		for (int i = 0; i < span.Length; i++)
		{
			T val = span[i];
			if (spanList.Count >= array.Length)
			{
				spanList = spanList.Resize(array);
			}
			if (predicate(val, argumentOne))
			{
				spanList.Add(val);
			}
		}
		return spanList;
	}

	public static SpanList<T> Resize<T>(this SpanList<T> items, T[] itemsBuffer)
	{
		itemsBuffer.ReturnToThreadSharedPool();
		T[] array = ArrayPool<T>.ThreadShared.GetArray(itemsBuffer.Length * 2);
		items.AsSpan.CopyTo(array);
		return new SpanList<T>(array, items.Count);
	}

	public static bool TryGet<T>(this SpanList<T> items, Func<T, bool> predicate, out T result)
	{
		SpanList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current))
			{
				result = current;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static int Count<T>(this SpanList<T> items, Func<T, bool> predicate)
	{
		int num = 0;
		SpanList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current))
			{
				num++;
			}
		}
		return num;
	}

	public static List<T> ToList<T>(this SpanList<T> items)
	{
		List<T> list = new List<T>(items.Count);
		SpanList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			list.Add(current);
		}
		return list;
	}

	public static ImmutableList<T> ToImmutableList<T>(this SpanList<T> items)
	{
		SpanList<T> spanList = items;
		int num = 0;
		T[] array = new T[spanList.Count];
		SpanList<T>.Enumerator enumerator = spanList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			array[num] = current;
			num++;
		}
		return ImmutableList.Create(new ReadOnlySpan<T>(array));
	}

	public static ImmutableArray<T> ToImmutableArray<T>(this SpanList<T> items)
	{
		SpanList<T> spanList = items;
		int num = 0;
		T[] array = new T[spanList.Count];
		SpanList<T>.Enumerator enumerator = spanList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			array[num] = current;
			num++;
		}
		return ImmutableCollectionsMarshal.AsImmutableArray(array);
	}

	public static HashSet<T> ToHashSet<T>(this SpanList<T> items)
	{
		HashSet<T> hashSet = new HashSet<T>(items.Count);
		SpanList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			hashSet.Add(current);
		}
		return hashSet;
	}

	public static ImmutableHashSet<T> ToImmutableHashSet<T>(this SpanList<T> items)
	{
		ImmutableHashSet<T>.Builder builder = ImmutableHashSet.CreateBuilder<T>();
		SpanList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			builder.Add(current);
		}
		return builder.ToImmutable();
	}

	public static SortedSet<T> ToSortedSet<T>(this SpanList<T> items)
	{
		SortedSet<T> sortedSet = new SortedSet<T>();
		SpanList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			sortedSet.Add(current);
		}
		return sortedSet;
	}

	public static MemoryList<T> ToMemoryList<T>(this SpanList<T> items)
	{
		return new MemoryList<T>(items.AsSpan);
	}
}
