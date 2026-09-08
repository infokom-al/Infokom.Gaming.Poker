using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BinarySerializer;

internal static class Collections
{
	public static IEnumerable<T> WithMaxValue<T>(this IEnumerable<T> items, Func<T, IComparable> selector)
	{
		IComparable max = items.Select(selector).Max();
		return items.Where((T x) => selector(x).Equals(max));
	}

	public static List<TOut> MapToList<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> convert)
	{
		return enumerable.Select(convert).ToList();
	}

	public static ImmutableList<TOut> MapToImmutableList<TOut>(this IEnumerable enumerable, Func<object, TOut> convert)
	{
		ImmutableList<TOut>.Builder builder = ImmutableList.CreateBuilder<TOut>();
		foreach (object item in enumerable)
		{
			builder.Add(convert(item));
		}
		return builder.ToImmutableList();
	}

	public static ImmutableList<TOut> MapToImmutableList<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> convert)
	{
		return ImmutableList.CreateRange(enumerable.Select(convert));
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

	public static IEnumerable<T> ToSingleIEnumerable<T>(this T obj)
	{
		yield return obj;
	}

	public static ImmutableList<T> ToSingleImmutableList<T>(this T item)
	{
		return item.ToSingleIEnumerable().ToImmutableList();
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

	public static bool TryGetFirst<T>(this IEnumerable<T> items, out T result)
	{
		using (IEnumerator<T> enumerator = items.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				result = current;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static void VerifyCount<T>(this ImmutableList<T> list, int count)
	{
		if (list.Count != count)
		{
			throw new InvalidOperationException($"List is expected to have {count} items, but has {list.Count}");
		}
	}

	public static void VerifyDistinct<T>(this IEnumerable<T> items, string? message = null)
	{
		if (items.Distinct().Count() != items.Count())
		{
			throw new InvalidOperationException(message ?? "Expecting collection with distinct element");
		}
	}

	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey key, TValue value)> items)
	{
		return items.ToDictionary<(TKey, TValue), TKey, TValue>(((TKey key, TValue value) item) => item.key, ((TKey key, TValue value) item) => item.value);
	}
}
