using MemoryPools;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public static class InlineListHelper
{
	public static readonly List<JsonConverter> InlineListConverters;

	public static InlineList<T> ToInlineList<T>(this IEnumerable<T> items)
	{
		InlineList<T> result = default(InlineList<T>);
		result.AddRange(items);
		return result;
	}

	public static InlineList<T> ToSingleInlineList<T>(this T item)
	{
		InlineList<T> result = default(InlineList<T>);
		result.Add(item);
		return result;
	}

	public static InlineList<T> RemoveAll<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		InlineList<T> result = default(InlineList<T>);
		for (int i = 0; i < items.Count; i++)
		{
			if (!predicate(items[i]))
			{
				result.Add(items[i]);
			}
		}
		return result;
	}

	public static InlineList<T> Where<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		InlineList<T> result = default(InlineList<T>);
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i]))
			{
				result.Add(items[i]);
			}
		}
		return result;
	}

	public static InlineList<T> Where<T, TArgumentOne>(this in InlineList<T> items, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne)
	{
		InlineList<T> result = default(InlineList<T>);
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i], argumentOne))
			{
				result.Add(items[i]);
			}
		}
		return result;
	}

	public static InlineList<TOut> MapToInlineList<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> selector)
	{
		InlineList<TOut> result = default(InlineList<TOut>);
		foreach (TIn item in items)
		{
			result.Add(selector(item));
		}
		return result;
	}

	public static InlineList<TOut> MapToInlineList<TIn, TOut>(this Span<TIn> items, Func<TIn, TOut> selector)
	{
		InlineList<TOut> result = default(InlineList<TOut>);
		Span<TIn> span = items;
		for (int i = 0; i < span.Length; i++)
		{
			TIn arg = span[i];
			result.Add(selector(arg));
		}
		return result;
	}

	public static InlineList<TOut> MapToInlineList<TIn, TOut>(this IEnumerable<TIn> items, Func<int, TIn, TOut> selector)
	{
		InlineList<TOut> result = default(InlineList<TOut>);
		int num = 0;
		foreach (TIn item in items)
		{
			result.Add(selector(num, item));
			num++;
		}
		return result;
	}

	public static InlineList<TOut> Map<TIn, TOut>(this in InlineList<TIn> items, Func<TIn, TOut> selector)
	{
		InlineList<TOut> result = default(InlineList<TOut>);
		InlineList<TIn>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			result.Add(selector(current));
		}
		return result;
	}

	public static InlineList<TOut> Map<TIn, TArgument, TOut>(this in InlineList<TIn> items, Func<TIn, TArgument, TOut> selector, TArgument argument)
	{
		InlineList<TOut> result = default(InlineList<TOut>);
		InlineList<TIn>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			result.Add(selector(current, argument));
		}
		return result;
	}

	public static TOut[] MapToArray<TIn, TOut>(this in InlineList<TIn> items, Func<TIn, TOut> selector)
	{
		TOut[] array = ArrayPool<TOut>.ThreadShared.GetArray(items.Count);
		for (int i = 0; i < items.Count; i++)
		{
			array[i] = selector(items[i]);
		}
		return array;
	}

	public static int Count<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i]))
			{
				num++;
			}
		}
		return num;
	}

	public static int Count<T>(this in InlineList<T> items, T item)
	{
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			if (object.Equals(items[i], item))
			{
				num++;
			}
		}
		return num;
	}

	public static int Count<T, TArgument>(this in InlineList<T> items, Func<T, TArgument, bool> predicate, TArgument argument)
	{
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i], argument))
			{
				num++;
			}
		}
		return num;
	}

	public static List<T> ToList<T>(this in InlineList<T> items)
	{
		return items.AsEnumerable().ToList();
	}

	public static T[] ToArray<T>(this in InlineList<T> items)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(items.Count);
		for (int i = 0; i < items.Count; i++)
		{
			array[i] = items[i];
		}
		return array;
	}

	public static bool Any<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static T Max<T>(this in InlineList<T> items) where T : IComparable<T>
	{
		if (items.Count == 0)
		{
			throw new InvalidOperationException("Cannot find the maximum of an empty list.");
		}
		T val = items[0];
		for (int i = 1; i < items.Count; i++)
		{
			if (items[i].CompareTo(val) > 0)
			{
				val = items[i];
			}
		}
		return val;
	}

	public static InlineList<T> Ordered<T>(this in InlineList<T> items) where T : IComparable<T>
	{
		return items.OrderBy((T item) => item);
	}

	public static InlineList<double> Ordered(this in InlineList<double> items)
	{
		InlineList<double> result = items;
		bool flag;
		do
		{
			flag = false;
			for (int i = 0; i < result.Count - 1; i++)
			{
				if (result[i].CompareTo(result[i + 1]) > 0)
				{
					double value = result[i];
					result[i] = result[i + 1];
					result[i + 1] = value;
					flag = true;
				}
			}
		}
		while (flag);
		return result;
	}

	public static InlineList<TItem> OrderByDescending<TItem, TComparable>(this in InlineList<TItem> items, Func<TItem, TComparable> selector) where TComparable : IComparable<TComparable>
	{
		return items.OrderBy(SelectorDescendingComparer<TItem, TComparable>.Create(selector));
	}

	public static InlineList<TItem> OrderByDescending<TItem, TComparableFirst, TComparableSecond>(this in InlineList<TItem> items, Func<TItem, TComparableFirst> selector, Func<TItem, TComparableSecond> thenBy) where TComparableFirst : IComparable<TComparableFirst> where TComparableSecond : IComparable<TComparableSecond>
	{
		return items.OrderBy(new ThenByComparer<TItem, TComparableFirst, TComparableSecond>(selector, thenBy));
	}

	public static InlineList<TItem> OrderBy<TItem, TComparable>(this in InlineList<TItem> items, Func<TItem, TComparable> selector) where TComparable : IComparable<TComparable>
	{
		return items.OrderBy(SelectorComparer<TItem, TComparable>.Create(selector));
	}

	public static InlineList<T> OrderBy<T>(this in InlineList<T> items, IComparer<T> comparer)
	{
		InlineList<T> result = items;
		bool flag;
		do
		{
			flag = false;
			for (int i = 0; i < result.Count - 1; i++)
			{
				if (comparer.Compare(result[i], result[i + 1]) > 0)
				{
					T value = result[i];
					result[i] = result[i + 1];
					result[i + 1] = value;
					flag = true;
				}
			}
		}
		while (flag);
		return result;
	}

	public static double Sum(this in InlineList<double> items)
	{
		double num = 0.0;
		InlineList<double>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			double current = enumerator.Current;
			num += current;
		}
		return num;
	}

	public static double Sum<T>(this in InlineList<T> items, Func<T, double> selector)
	{
		double num = 0.0;
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			num += selector(current);
		}
		return num;
	}

	public static int Sum<T>(this in InlineList<T> items, Func<T, int> selector)
	{
		int num = 0;
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			num += selector(current);
		}
		return num;
	}

	public static double Sum<T, TArgument>(this in InlineList<T> items, Func<T, TArgument, double> selector, TArgument argument)
	{
		double num = 0.0;
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			num += selector(current, argument);
		}
		return num;
	}

	public static InlineList<T> ToInlineList10<T>(this Span<T> span)
	{
		InlineList<T> result = new InlineList<T>(span.Length);
		for (int i = 0; i < span.Length; i++)
		{
			result[i] = span[i];
		}
		return result;
	}

	public static InlineList<(T1, T2)> TupleToInlineList<T1, T2>(this IList<T1> source, Span<T2> second)
	{
		InlineList<(T1, T2)> result = default(InlineList<(T1, T2)>);
		for (int i = 0; i < second.Length; i++)
		{
			result.Add((source[i], second[i]));
		}
		return result;
	}

	public static InlineList<T> VerifyAll<T>(this in InlineList<T> items, Func<T, bool> predicate, string message)
	{
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (!predicate(current))
			{
				throw new InvalidOperationException(message);
			}
		}
		return items;
	}

	public static int IndexOf<T>(this in InlineList<T> items, T item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (object.Equals(item, items[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public static bool All<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (!predicate(current))
			{
				return false;
			}
		}
		return true;
	}

	public static bool All<T, TArgumentOne>(this in InlineList<T> items, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne)
	{
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (!predicate(current, argumentOne))
			{
				return false;
			}
		}
		return true;
	}

	public static T WithMinValue<T, K>(this in InlineList<T> items, Func<T, K> selector) where K : IComparable<K>
	{
		if (items.Count == 0)
		{
			throw new InvalidOperationException("Cannot find minimum value of an empty list.");
		}
		T val = items[0];
		K other = selector(val);
		for (int i = 1; i < items.Count; i++)
		{
			K val2 = selector(items[i]);
			if (val2.CompareTo(other) < 0)
			{
				other = val2;
				val = items[i];
			}
		}
		return val;
	}

	public static T WithMaxValue<T, K>(this in InlineList<T> items, Func<T, K> selector) where K : IComparable<K>
	{
		if (items.Count == 0)
		{
			throw new InvalidOperationException("Cannot find maximum value of an empty list.");
		}
		T val = items[0];
		K other = selector(val);
		for (int i = 1; i < items.Count; i++)
		{
			K val2 = selector(items[i]);
			if (val2.CompareTo(other) > 0)
			{
				other = val2;
				val = items[i];
			}
		}
		return val;
	}

	public static T First<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current))
			{
				return current;
			}
		}
		throw new InvalidOperationException("Item not found in the collection");
	}

	public static bool IsCountAtLeast<T>(this in InlineList<T> items, Func<T, bool> predicate, int count)
	{
		int num = 0;
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current))
			{
				num++;
				if (num >= count)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static InlineList<T> RoundEnumerateSince<T>(this in InlineList<T> items, T item)
	{
		return items.RoundEnumerateSince((T other) => object.Equals(other, item));
	}

	public static InlineList<T> RoundEnumerateSince<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		InlineList<T> result = default(InlineList<T>);
		int? num = null;
		for (int i = 0; i < items.Count; i++)
		{
			T val = items[i];
			if (num.HasValue)
			{
				result.Add(val);
			}
			else if (predicate(val))
			{
				num = i;
				result.Add(val);
			}
		}
		if (!num.HasValue)
		{
			throw new InvalidOperationException("Starting item not found in the list");
		}
		for (int j = 0; j < num; j++)
		{
			result.Add(items[j]);
		}
		return result;
	}

	public static InlineList<T> RoundEnumerateAfter<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		InlineList<T> result = default(InlineList<T>);
		int num = -1;
		for (int i = 0; i < items.Count; i++)
		{
			if (num == -1 && predicate(items[i]))
			{
				num = i + 1;
			}
		}
		if (num == -1)
		{
			throw new InvalidOperationException("The item matching the predicate was not found.");
		}
		for (int j = num; j < items.Count; j++)
		{
			result.Add(items[j]);
		}
		for (int k = 0; k < num; k++)
		{
			result.Add(items[k]);
		}
		return result;
	}

	public static InlineList<T> RoundEnumerateAfter<T>(this in InlineList<T> items, T item)
	{
		return items.RoundEnumerateAfter((T x) => object.Equals(x, item));
	}

	public static InlineList<T> RoundEnumerateAfterFirst<T, TArgumentOne>(this in InlineList<T> items, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne)
	{
		InlineList<T> result = default(InlineList<T>);
		int? num = null;
		for (int i = 0; i < items.Count; i++)
		{
			if (num.HasValue)
			{
				result.Add(items[i]);
			}
			else if (predicate(items[i], argumentOne))
			{
				num = i;
			}
		}
		if (!num.HasValue)
		{
			throw new InvalidOperationException("Starting item not found in the list");
		}
		for (int j = 0; j <= num; j++)
		{
			result.Add(items[j]);
		}
		return result;
	}

	public static InlineList<T> RoundEnumerateAfterFirst<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		return items.RoundEnumerateAfterFirst((T item, Func<T, bool> func) => func(item), predicate);
	}

	public static InlineList<T> RoundEnumerateBackAfter<T>(this in InlineList<T> items, T item)
	{
		return items.RoundEnumerateBackAfter((T other) => object.Equals(item, other));
	}

	public static InlineList<T> RoundEnumerateBackAfter<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		return items.RoundEnumerateBackAfter((T item, Func<T, bool> func) => func(item), predicate);
	}

	public static InlineList<T> RoundEnumerateBackAfter<T, TArgumentOne>(this in InlineList<T> items, Func<T, TArgumentOne, bool> predicate, TArgumentOne argument)
	{
		int? num = null;
		InlineList<T> result = default(InlineList<T>);
		for (int num2 = items.Count - 1; num2 >= 0; num2--)
		{
			if (num.HasValue)
			{
				result.Add(items[num2]);
			}
			else if (predicate(items[num2], argument))
			{
				_ = items[num2];
				num = num2;
			}
		}
		if (!num.HasValue)
		{
			throw new InvalidOperationException("Starting item not found in the list");
		}
		for (int num3 = items.Count - 1; num3 >= num; num3--)
		{
			result.Add(items[num3]);
		}
		return result;
	}

	public static InlineList<T> TakeWhile<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		if (items.Count == 0)
		{
			return items;
		}
		InlineList<T> result = default(InlineList<T>);
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current))
			{
				result.Add(current);
				continue;
			}
			return result;
		}
		return result;
	}

	public static InlineList<T> SkipWhile<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		if (items.Count == 0)
		{
			return items;
		}
		InlineList<T> result = default(InlineList<T>);
		bool flag = false;
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (flag)
			{
				result.Add(current);
			}
			else if (!predicate(current))
			{
				flag = true;
				result.Add(current);
			}
		}
		return result;
	}

	public static InlineList<T> Except<T>(this in InlineList<T> items, T item)
	{
		InlineList<T> result = default(InlineList<T>);
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (!object.Equals(current, item))
			{
				result.Add(current);
			}
		}
		return result;
	}

	public static InlineList<T> Except<T>(this in InlineList<T> items, InlineList<T> other)
	{
		InlineList<T> result = default(InlineList<T>);
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (!other.Contains(current))
			{
				result.Add(current);
			}
		}
		return result;
	}

	public static InlineList<T> Except<T>(this in InlineList<T> items, Func<T, bool> predicate)
	{
		InlineList<T> result = default(InlineList<T>);
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (!predicate(current))
			{
				result.Add(current);
			}
		}
		return result;
	}

	public static InlineList<T> VerifyArgumentOrdered<T>(this in InlineList<T> items, string argumentName) where T : IComparable<T>
	{
		for (int i = 0; i < items.Count - 1; i++)
		{
			if (items[i].CompareTo(items[i + 1]) > 0)
			{
				throw new ArgumentException("The argument '" + argumentName + "' must be an ordered list.", argumentName);
			}
		}
		return items;
	}

	public static InlineList<T> VerifyArgumentOrderedByDescending<T, TEnum>(this in InlineList<T> items, Func<T, TEnum> selector, string argumentName) where TEnum : Enum
	{
		for (int i = 0; i < items.Count - 1; i++)
		{
			if (selector(items[i]).CompareTo(selector(items[i + 1])) < 0)
			{
				throw new ArgumentException("The argument '" + argumentName + "' must be an ordered list.", argumentName);
			}
		}
		return items;
	}

	public static InlineList<T> VerifyArgumentOrderedBy<T, TEnum>(this in InlineList<T> items, Func<T, TEnum> selector, string argumentName) where TEnum : Enum
	{
		for (int i = 0; i < items.Count - 1; i++)
		{
			if (selector(items[i]).CompareTo(selector(items[i + 1])) > 0)
			{
				throw new ArgumentException("The argument '" + argumentName + "' must be an ordered list.", argumentName);
			}
		}
		return items;
	}

	public static bool Contains<T>(this in InlineList<T> items, T item)
	{
		return items.Any((T other, T val) => other.Equals(val), item);
	}

	public static InlineList<T> Replace<T>(this InlineList<T> items, Func<T, bool> predicate, Func<T, T> getNew)
	{
		for (int i = 0; i < items.Count; i++)
		{
			T arg = items[i];
			if (predicate(arg))
			{
				items[i] = getNew(arg);
				return items;
			}
		}
		throw new InvalidOperationException("Item with the given predicate not found");
	}

	public static InlineList<T> Replace<T>(this InlineList<T> items, Func<T, bool> predicate, T newItem)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i]))
			{
				items[i] = newItem;
				return items;
			}
		}
		throw new InvalidOperationException("Item with the given predicate not found");
	}

	public static InlineList<T> ReplaceOrAdd<T>(this InlineList<T> items, Func<T, bool> predicate, T newItem)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (predicate(items[i]))
			{
				items[i] = newItem;
				return items;
			}
		}
		items.Add(newItem);
		return items;
	}

	public static void VerifyCollectionSize<T>(this in InlineList<T> items, int count)
	{
		if (items.Count != count)
		{
			throw new InvalidOperationException($"Expected collection of size {count} but was {items.Count}");
		}
	}

	public static TOut Aggregate<TIn, TOut>(this in InlineList<TIn> items, TOut seed, Func<TOut, TIn, TOut> aggregate)
	{
		TOut val = seed;
		InlineList<TIn>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			val = aggregate(val, current);
		}
		return val;
	}

	public static string AggregateToString<T>(this in InlineList<T> items)
	{
		return AggregateToString(in items, string.Empty);
	}

	public static string AggregateToString<T>(this in InlineList<T> items, string separator)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < items.Count; i++)
		{
			stringBuilder.Append(items[i].ToString());
			if (i != items.Count - 1)
			{
				stringBuilder.Append(separator);
			}
		}
		return stringBuilder.ToString();
	}

	public static InlineList<(int index, T value)> WithIndex<T>(this in InlineList<T> items)
	{
		InlineList<(int, T)> result = default(InlineList<(int, T)>);
		for (int i = 0; i < items.Count; i++)
		{
			result.Add((i, items[i]));
		}
		return result;
	}

	public static InlineList<T> SetLast<T>(this in InlineList<T> items, T item)
	{
		if (items.Count == 0)
		{
			throw new InvalidOperationException("The inline list is empty");
		}
		InlineList<T> result = items;
		result[items.Count - 1] = item;
		return result;
	}

	public static InlineList<T> Distinct<T>(this in InlineList<T> items)
	{
		InlineList<T> items2 = default(InlineList<T>);
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (!items2.Contains(current))
			{
				items2.Add(current);
			}
		}
		return items2;
	}

	public static InlineList<T> DistinctBy<T, U>(this in InlineList<T> items, Func<T, U> selector)
	{
		HashSet<U> hashSet = new HashSet<U>();
		InlineList<T> result = default(InlineList<T>);
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (hashSet.Add(selector(current)))
			{
				result.Add(current);
			}
		}
		return result;
	}

	public static InlineList<T> Take<T>(this in InlineList<T> items, int count)
	{
		InlineList<T> result = default(InlineList<T>);
		for (int i = 0; i < count && i < items.Count; i++)
		{
			result.Add(items[i]);
		}
		return result;
	}

	public static InlineList<T> Skip<T>(this in InlineList<T> items, int count)
	{
		InlineList<T> result = default(InlineList<T>);
		for (int i = count; i < items.Count; i++)
		{
			result.Add(items[i]);
		}
		return result;
	}

	public static bool TryGet<T>(this in InlineList<T> items, Func<T, bool> predicate, out T result)
	{
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
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

	public static bool TryGet<T, TArgumentOne>(this in InlineList<T> items, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne, out T result)
	{
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current, argumentOne))
			{
				result = current;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static bool TryGetFirst<T>(this in InlineList<T> items, out T result)
	{
		if (items.Count > 0)
		{
			result = items[0];
			return true;
		}
		result = default(T);
		return false;
	}

	public static InlineList<T> GetRange<T>(this in InlineList<T> items, int start, int count)
	{
		InlineList<T> result = InlineList<T>.Empty;
		for (int i = start; i < start + count && i < items.Count; i++)
		{
			result = result.With(items[i]);
		}
		return result;
	}

	public static InlineList<T> Concat<T>(this in InlineList<T> items, InlineList<T> other)
	{
		InlineList<T> result = items;
		InlineList<T>.Enumerator enumerator = other.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			result = result.With(current);
		}
		return result;
	}

	public static InlineList<T> GetIntersection<T>(this in InlineList<T> items, InlineList<T> other)
	{
		InlineList<T> result = InlineList<T>.Empty;
		for (int i = 0; i < items.Count; i++)
		{
			for (int j = 0; j < other.Count; j++)
			{
				if (EqualityComparer<T>.Default.Equals(items[i], other[j]))
				{
					result = result.With(items[i]);
					break;
				}
			}
		}
		return result;
	}

	public static bool Intersects<T>(this in InlineList<T> items, InlineList<T> other)
	{
		for (int i = 0; i < items.Count; i++)
		{
			for (int j = 0; j < other.Count; j++)
			{
				if (EqualityComparer<T>.Default.Equals(items[i], other[j]))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool SequenceEqual<T>(this in InlineList<T> items, InlineList<T> other)
	{
		if (items.Count != other.Count)
		{
			return false;
		}
		for (int i = 0; i < items.Count; i++)
		{
			if (!EqualityComparer<T>.Default.Equals(items[i], other[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool SequenceEqual<T>(this in InlineList<T> items, InlineList<T> other, IEqualityComparer<T> comparer)
	{
		if (items.Count != other.Count)
		{
			return false;
		}
		for (int i = 0; i < items.Count; i++)
		{
			if (!comparer.Equals(items[i], other[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this InlineList<TIn> items, Func<TIn, InlineList<TOut>> selector)
	{
		InlineList<TIn>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			InlineList<TOut>.Enumerator enumerator2 = selector(current).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				yield return enumerator2.Current;
			}
		}
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this InlineList<TIn> items, Func<TIn, IEnumerable<TOut>> selector)
	{
		InlineList<TIn>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			foreach (TOut item in selector(current))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<T> AsEnumerable<T>(this InlineList<T> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			yield return items[i];
		}
	}

	public static string AggregateToString<TIn, TOut>(this in InlineList<TIn> items, Func<TIn, TOut> selector, string separator)
	{
		return string.Join(separator, items.AsEnumerable().Select(selector));
	}

	public static ImmutableList<TOut> MapToImmutableList<TIn, TOut>(this in InlineList<TIn> items, Func<TIn, TOut> selector)
	{
		return items.AsEnumerable().Select(selector).ToImmutableList();
	}

	public static bool Any<T, TArgument>(this in InlineList<T> items, Func<T, TArgument, bool> predicate, TArgument argument)
	{
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current, argument))
			{
				return true;
			}
		}
		return false;
	}

	public static InlineList<TItem> OrderByDescending<TItem, TComparable>(this in InlineList<TItem> items, Func<TItem, TComparable> selector, IComparer<TComparable> comparer)
	{
		Comparer<TItem> comparer2 = Comparer<TItem>.Create((TItem x, TItem y) => comparer.Compare(selector(y), selector(x)));
		return items.OrderBy(comparer2);
	}

	public static bool SequenceEquals<T>(this in InlineList<T> list, InlineList<T> other)
	{
		if (list.Count != other.Count)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (!EqualityComparer<T>.Default.Equals(list[i], other[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static K Max<T, K>(this in InlineList<T> items, Func<T, K> selector) where K : IComparable<K>
	{
		if (items.IsEmpty)
		{
			return default(K);
		}
		K val = selector(items[0]);
		for (int i = 1; i < items.Count; i++)
		{
			K val2 = selector(items[i]);
			if (val2.CompareTo(val) > 0)
			{
				val = val2;
			}
		}
		return val;
	}

	public static K Min<T, K>(this in InlineList<T> items, Func<T, K> selector) where K : IComparable<K>
	{
		if (items.IsEmpty)
		{
			return default(K);
		}
		K val = selector(items[0]);
		for (int i = 1; i < items.Count; i++)
		{
			K val2 = selector(items[i]);
			if (val2.CompareTo(val) < 0)
			{
				val = val2;
			}
		}
		return val;
	}

	public static InlineList<T> ExceptLast<T>(this in InlineList<T> items)
	{
		if (items.IsEmpty)
		{
			throw new InvalidOperationException("Cannot exclude the last element from an empty list.");
		}
		return GetRange(in items, 0, items.Count - 1);
	}

	public static void ForEach<T>(this in InlineList<T> items, Action<T> action)
	{
		for (int i = 0; i < items.Count; i++)
		{
			action(items[i]);
		}
	}

	public static bool TryGetLast<T>(this in InlineList<T> items, Func<T, bool> predicate, out T result)
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			if (predicate(items[num]))
			{
				result = items[num];
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static InlineList<T> VerifyArgumentNotEmpty<T>(this in InlineList<T> items, string argumentName)
	{
		if (items.IsEmpty)
		{
			throw new ArgumentException("Argument cannot be empty.", argumentName);
		}
		return items;
	}

	public static InlineList<T> Reverse<T>(this in InlineList<T> items)
	{
		InlineList<T> result = InlineList<T>.Empty;
		for (int num = items.Count - 1; num >= 0; num--)
		{
			result = result.With(items[num]);
		}
		return result;
	}

	public static InlineList<T> AddRange<T>(this in InlineList<T> items, InlineList<T> other)
	{
		InlineList<T> result = items;
		for (int i = 0; i < other.Count; i++)
		{
			result = result.With(other[i]);
		}
		return result;
	}

	public static ImmutableList<T> ToImmutableList<T>(this InlineList<T> items)
	{
		ImmutableList<T> immutableList = ImmutableList<T>.Empty;
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			immutableList = immutableList.Add(current);
		}
		return immutableList;
	}

	public static InlineList<T> Flatten<T>(this InlineList<InlineList<T>> lists)
	{
		InlineList<T> items = InlineList<T>.Empty;
		InlineList<InlineList<T>>.Enumerator enumerator = lists.GetEnumerator();
		while (enumerator.MoveNext())
		{
			InlineList<T> current = enumerator.Current;
			items = items.AddRange(current);
		}
		return items;
	}

	public static InlineList<CardRanks> GetRanks(this in InlineList<Card> cards)
	{
		return cards.Map((Card card) => card.Rank);
	}

	public static Dictionary<TKey, TValue> ToDictionary<TItem, TKey, TValue>(this InlineList<TItem> items, Func<TItem, TKey> keySelector, Func<TItem, TValue> valueSelector)
	{
		return items.Aggregate(new Dictionary<TKey, TValue>(), delegate (Dictionary<TKey, TValue> dict, TItem item)
		{
			dict[keySelector(item)] = valueSelector(item);
			return dict;
		});
	}

	static InlineListHelper()
	{
		int num = 6;
		List<JsonConverter> list = new List<JsonConverter>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<JsonConverter> span = CollectionsMarshal.AsSpan(list);
		span[0] = new InlineListConverter<int?>();
		span[1] = new InlineListConverter<int>();
		span[2] = new InlineListConverter<double?>();
		span[3] = new InlineListConverter<double>();
		span[4] = new InlineListConverter<string>();
		span[5] = new InlineListConverter<string>();
		InlineListConverters = list;
	}
}
