using System.Collections.Generic;
using System.Collections.Immutable;

namespace MemoryPools;

internal static class Dictionaries
{
	public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this IEnumerable<(TKey key, TValue value)> values)
	{
		return values.ToImmutableDictionary<(TKey, TValue), TKey, TValue>(((TKey key, TValue value) value) => value.key, ((TKey key, TValue value) value) => value.value);
	}

	public static void AddOrIncrement<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int value = 1)
	{
		if (dictionary.ContainsKey(key))
		{
			dictionary[key] += value;
		}
		else
		{
			dictionary.Add(key, value);
		}
	}

	public static ImmutableDictionary<TKey, int> AddOrIncrement<TKey>(this ImmutableDictionary<TKey, int> dictionary, TKey key, int delta = 1)
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			return dictionary.SetItem(key, value + delta);
		}
		return dictionary.SetItem(key, delta);
	}
}
