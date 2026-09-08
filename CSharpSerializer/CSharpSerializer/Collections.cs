using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CSharpSerializer;

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

	public static ImmutableList<T> AddIfNotContains<T>(this ImmutableList<T> items, T item)
	{
		if (!items.Contains(item))
		{
			return items.Add(item);
		}
		return items;
	}

	public static IEnumerable Cast<T>(this IEnumerable<T> items, Type targetType)
	{
		return (IEnumerable)typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(targetType).Invoke(null, new object[1] { items });
	}
}
