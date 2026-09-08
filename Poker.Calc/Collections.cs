using System.Collections.Immutable;

namespace Poker.Calc;

internal static class Collections
{
	public static bool AreEqual<T>(this IEnumerable<T> firstItems, IEnumerable<T> secondItems)
	{
		List<T> list = firstItems.ToList();
		List<T> list2 = secondItems.ToList();
		if (list.All(list2.Contains))
		{
			return list.Count == list2.Count;
		}
		return false;
	}

	public static IEnumerable<T> ToSingleIEnumerable<T>(this T obj)
	{
		yield return obj;
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

	public static int MinOrDefault<T>(this IEnumerable<T> items, Func<T, int> selector)
	{
		int num = int.MaxValue;
		bool flag = false;
		foreach (T item in items)
		{
			int num2 = selector(item);
			if (num2 < num)
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

	public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> items, int startAtIndex = 0)
	{
		int index = startAtIndex;
		foreach (T item in items)
		{
			yield return (index: index, value: item);
			index++;
		}
	}

	public static IEnumerable<(T1, T2)> TupleWith<T1, T2>(this IEnumerable<T1> source, IEnumerable<T2> second)
	{
		return source.Zip(second, (T1 x, T2 y) => (x: x, y: y));
	}

	public static List<TOut> MapToList<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> selector)
	{
		return items.Select(selector).ToList();
	}

	public static HashSet<TOut> MapToHashSet<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> selector)
	{
		return items.Select(selector).ToHashSet();
	}

	public static TOut[] MapToArray<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> selector)
	{
		return items.Select(selector).ToArray();
	}

	public static ImmutableList<TOut> MapToImmutableList<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> selector)
	{
		return items.Select(selector).ToImmutableList();
	}

	public static List<T> VerifyCollectionSize<T>(this List<T> list, int size)
	{
		if (list.Count != size)
		{
			throw new InvalidOperationException($"Expecting collection to be a size of {size} but was {list.Count}");
		}
		return list;
	}

	public static IList<T> VerifyCollectionSize<T>(this IList<T> list, int size)
	{
		if (list.Count != size)
		{
			throw new InvalidOperationException($"Expecting collection to be a size of {size} but was {list.Count}");
		}
		return list;
	}

	public static ImmutableList<T> VerifyCollectionSize<T>(this ImmutableList<T> list, int size)
	{
		if (list.Count != size)
		{
			throw new InvalidOperationException($"Expecting collection to be a size of {size} but was {list.Count}");
		}
		return list;
	}

	public static T[] VerifyCollectionSize<T>(this T[] items, int size)
	{
		if (items.Length != size)
		{
			throw new InvalidOperationException($"Expecting collection to be a size of {size} but was {items.Length}");
		}
		return items;
	}

	public static void VerifyCollectionNotEmpty<T>(this IEnumerable<T> items)
	{
		if (!items.Any())
		{
			throw new InvalidOperationException("Collection is not expected to be empty");
		}
	}

	public static void VerifyCollectionArgumentNotEmpty<T>(this IEnumerable<T> items, string argumentName)
	{
		if (!items.Any())
		{
			throw new ArgumentException("Collection is not expected to be empty", argumentName);
		}
	}

	public static void VerifyCollectionArgumentDistinct<T>(this ICollection<T> list, string? argumentName = null)
	{
		if (list.Distinct().Count() != list.Count)
		{
			throw new ArgumentException("Expecting collection with distinct element", "argumentName");
		}
	}

	public static void VerifyDistinct<T>(this IList<T> items, string? message = null)
	{
		if (items.Distinct().Count() != items.Count())
		{
			throw new InvalidOperationException(message ?? "Expecting collection with distinct element");
		}
	}

	public static void RemoveAll<T>(this HashSet<T> set, IEnumerable<T> items)
	{
		foreach (T item in items)
		{
			set.Remove(item);
		}
	}

	public static IEnumerable<T> ExceptLast<T>(this ICollection<T> collection)
	{
		if (collection.Count <= 1)
		{
			return collection;
		}
		return collection.Take(collection.Count - 1);
	}

	public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
	{
		foreach (T item in items)
		{
			action(item);
		}
	}

	public static T[] Clear<T>(this T[] array)
	{
		Array.Clear(array, 0, array.Length);
		return array;
	}

	public static T[] GetRange<T>(this T[] array, int fromIndex, int count)
	{
		T[] array2 = new T[count];
		for (int i = 0; i < count; i++)
		{
			array2[i] = array[fromIndex + i];
		}
		return array2;
	}

	public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
	{
		foreach (var (result, arg) in items.WithIndex())
		{
			if (predicate(arg))
			{
				return result;
			}
		}
		return -1;
	}

	public static ImmutableList<T> ToSingleImmutableList<T>(this T item)
	{
		return item.ToSingleIEnumerable().ToImmutableList();
	}

	public static bool TryGet<T>(this IEnumerable<T> items, Func<T, bool> predicate, out T result)
	{
		foreach (T item in items)
		{
			if (predicate(item))
			{
				result = item;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static ImmutableList<T> Replace<T>(this ImmutableList<T> items, Func<T, bool> predicate, T newItem)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i]))
			{
				return items.SetItem(i, newItem);
			}
		}
		throw new InvalidOperationException("Item with the given predicate not found");
	}

	public static IEnumerable<Memory<T>> GetMemoryChunks<T>(this T[] items, int chunkSize)
	{
		int chunkCount = items.Length / chunkSize;
		for (int i = 0; i < chunkCount; i++)
		{
			yield return items.AsMemory().Slice(i * chunkSize, chunkSize);
		}
		int num = chunkCount % chunkSize;
		if (num != 0)
		{
			yield return items.AsMemory().Slice(items.Length - num, num);
		}
	}

	public static bool SequenceEqual<T>(this ImmutableList<T> items, ImmutableList<T> other)
	{
		if (items == other)
		{
			return true;
		}
		return Enumerable.SequenceEqual(items, other);
	}

	public static IEnumerable<T> Reversed<T>(this IList<T> items)
	{
		return items.Reverse();
	}
}
