using System.Collections.Immutable;

namespace Poker.Calc.Common;

public static class Dictionaries
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

	public static Dictionary<TKey, double> IncrementOrAdd<TKey>(this Dictionary<TKey, double> dictionary, TKey key, double delta = 1.0)
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

	public static ImmutableDictionary<TKey, int> AddOrIncrement<TKey>(this ImmutableDictionary<TKey, int> dictionary, TKey key, int delta = 1)
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			return dictionary.SetItem(key, value + delta);
		}
		return dictionary.SetItem(key, delta);
	}

	public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this IEnumerable<(TKey key, TValue value)> values)
	{
		return values.ToImmutableDictionary<(TKey, TValue), TKey, TValue>(((TKey key, TValue value) value) => value.key, ((TKey key, TValue value) value) => value.value);
	}
}
