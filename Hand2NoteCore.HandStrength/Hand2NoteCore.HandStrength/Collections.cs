using System;
using System.Collections.Generic;

namespace Hand2NoteCore.HandStrength;

internal static class Collections
{
	public static Dictionary<TKey, int> IncrementOrAdd<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int delta = 1)
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			dictionary[key] = value + delta;
		}
		else
		{
			dictionary[key] = delta;
		}
		return dictionary;
	}

	public static bool TryGet<T>(this IEnumerable<T> items, Func<T, bool> predicate, out T? result)
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

	public static void AddOrAddToList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
	{
		if (dictionary.TryGetValue(key, out List<TValue> value2))
		{
			value2.Add(value);
			return;
		}
		dictionary[key] = new List<TValue> { value };
	}

	public static IEnumerable<T> ToSingleIEnumerable<T>(this T obj)
	{
		yield return obj;
	}
}
