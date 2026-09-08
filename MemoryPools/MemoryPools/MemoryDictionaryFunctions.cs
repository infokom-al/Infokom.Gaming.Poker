using System;
using System.Collections.Immutable;

namespace MemoryPools;

public static class MemoryDictionaryFunctions
{
	public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this MemoryDictionary<TKey, TValue> dictionary) where TKey : IEquatable<TKey>
	{
		ImmutableDictionary<TKey, TValue>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
		MemoryDictionary<TKey, TValue>.Enumerator enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (key, value) = enumerator.Current;
			builder.Add(key, value);
		}
		return builder.ToImmutable();
	}

	public static MemoryDictionary<TKey, TValue> DeepCloneValues<TKey, TValue>(this MemoryDictionary<TKey, TValue> dictionary, Func<TValue, TValue> cloneValue) where TKey : IEquatable<TKey>
	{
		return dictionary.DeepClone((TKey key) => key, cloneValue);
	}

	public static MemoryDictionary<TKey, TValue> DeepClone<TKey, TValue>(this MemoryDictionary<TKey, TValue> dictionary, Func<TKey, TKey> cloneKey, Func<TValue, TValue> cloneValue) where TKey : IEquatable<TKey>
	{
		MemoryDictionary<TKey, TValue> result = new MemoryDictionary<TKey, TValue>();
		MemoryDictionary<TKey, TValue>.Enumerator enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(TKey Key, TValue Value) current = enumerator.Current;
			TKey item = current.Key;
			TValue item2 = current.Value;
			result = result.Add(cloneKey(item), cloneValue(item2));
		}
		return result;
	}
}
