using MemoryPools;

using Poker.Calc.Common;

using System.Collections.Immutable;

namespace Poker.Calc;

public static class StreetsHelper
{
	public static List<Streets> PostflopStreets = new List<Streets>
	{
		Streets.Flop,
		Streets.Turn,
		Streets.River
	};

	public static List<Streets> AllStreets = new List<Streets>
	{
		Streets.Preflop,
		Streets.Flop,
		Streets.Turn,
		Streets.River
	};

	public static Streets[] FlopTurnRiver = new Streets[3]
	{
		Streets.Flop,
		Streets.Turn,
		Streets.River
	};

	public static Streets[] TurnRiver = new Streets[2]
	{
		Streets.Turn,
		Streets.River
	};

	public static Streets[] RiverAsArray = new Streets[1] { Streets.River };

	public static Streets MaxStreet(this IEnumerable<Streets> streets)
	{
		return (Streets)streets.MaxOrDefault((Streets x) => (int)x);
	}

	public static Streets MinStreet(this IEnumerable<Streets> streets)
	{
		return (Streets)streets.MinOrDefault((Streets x) => (int)x);
	}

	public static Streets MinStreet(Streets street, Streets other)
	{
		if (street > other)
		{
			return other;
		}
		return street;
	}

	public static Streets CardsCountToStreet(this int cardsCount)
	{
		switch (cardsCount)
		{
			case 0:
				return Streets.Preflop;
			default:
				throw new ArgumentException($"Invalid board cards count {cardsCount}", "cardsCount");
			case 3:
			case 4:
			case 5:
				return (Streets)(cardsCount - 2);
		}
	}

	public static Streets? NextStreet(this Streets street)
	{
		if (street != Streets.River)
		{
			return street + 1;
		}
		return null;
	}

	public static Streets? PreviousStreet(this Streets street)
	{
		if (street != Streets.Preflop)
		{
			return street - 1;
		}
		return null;
	}

	public static Streets PreviousStreetOrThrow(this Streets street)
	{
		return street.PreviousStreet() ?? throw new InvalidOperationException($"{street} is the first street");
	}

	public static Streets NextStreetOrThrow(this Streets street)
	{
		return street.NextStreet() ?? throw new InvalidOperationException($"{street} is the last street");
	}

	public static bool IsLastStreet(this Streets street)
	{
		return street == Streets.River;
	}

	public static int CardsCount(this Streets street)
	{
		if (street != Streets.Preflop)
		{
			return (int)(street + 2);
		}
		return 0;
	}

	public static int CardsToDeal(this Streets street)
	{
		return street switch
		{
			Streets.Preflop => 5,
			Streets.Flop => 2,
			Streets.Turn => 1,
			Streets.River => 0,
			_ => throw new NotImplementedException(),
		};
	}

	public static int NewCardsCount(this Streets street)
	{
		return street switch
		{
			Streets.Flop => 3,
			Streets.Preflop => 0,
			_ => 1,
		};
	}

	public static bool IsPostflop(this Streets street)
	{
		return street != Streets.Preflop;
	}

	public static bool IsPreflop(this Streets street)
	{
		return street == Streets.Preflop;
	}

	public static int ToStreetIndex(this Streets street)
	{
		return (int)street;
	}

	public static HandStages GetHandStage(this Streets street)
	{
		if (street != Streets.Preflop)
		{
			return HandStages.Postflop;
		}
		return HandStages.Preflop;
	}

	public static Streets VerifyArgumentIsPostflop(this Streets street, string? argumentName = null)
	{
		if (!street.IsPostflop())
		{
			throw new ArgumentException("Expecting a postflop street", argumentName);
		}
		return street;
	}

	public static Streets MaxStreet(Streets street1, Streets street2)
	{
		if (street1 < street2)
		{
			return street2;
		}
		return street1;
	}

	public static Streets MaxStreet(Streets street1, Streets street2, Streets street3)
	{
		return MaxStreet(street1, MaxStreet(street2, street3));
	}

	public static Streets MaxStreet(Streets street1, Streets street2, Streets street3, Streets street4)
	{
		return MaxStreet(MaxStreet(street1, street2), MaxStreet(street3, street4));
	}

	public static Streets MaxStreetOrPreflop(this IEnumerable<Streets> streets)
	{
		return (Streets)streets.MaxOrDefault((Streets x) => (int)x);
	}

	public static Streets[] GetNextStreets(this Streets street)
	{
		return street switch
		{
			Streets.Preflop => FlopTurnRiver,
			Streets.Flop => TurnRiver,
			Streets.Turn => RiverAsArray,
			_ => throw new ArgumentException($"{street} doesn't have next streets"),
		};
	}

	public static bool ContainsStreet<T>(this IList<(Streets, T)> items, Streets street)
	{
		return items.Any<(Streets, T)>(((Streets, T) item) => item.Item1 == street);
	}

	public static T GetStreet<T>(this List<(Streets street, T)> items, Streets street)
	{
		foreach (var item in items)
		{
			if (item.street == street)
			{
				return item.Item2;
			}
		}
		throw new InvalidOperationException($"Street {street} not found");
	}

	public static void AddOrReplaceStreet<T>(this List<(Streets, T)> items, Streets street, T value)
	{
		int streetIndex = items.GetStreetIndex(street);
		if (streetIndex == -1)
		{
			items.Add((street, value));
		}
		else
		{
			items[streetIndex] = (street, value);
		}
	}

	public static int GetStreetIndex<T>(this List<(Streets, T)> items, Streets street)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].Item1 == street)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool TryGetStreet<T>(this IList<(Streets, T)> items, Streets street, out T result)
	{
		foreach (var item in items)
		{
			if (item.Item1 == street)
			{
				result = item.Item2;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static StreetMap<T> VerifyArgumentNonEmpty<T>(this in StreetMap<T> streets, string argument)
	{
		if (streets.IsEmpty)
		{
			throw new ArgumentException(argument + " is empty");
		}
		return streets;
	}

	public static StreetMap<T> ToStreetMap<T>(this InlineList<T> streets) where T : IHasStreet
	{
		StreetMap<T> result = default(StreetMap<T>);
		InlineList<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			result.Add(current);
		}
		return result;
	}

	public static StreetMap<TOut> ToStreetMap<TIn, TOut>(this InlineList<TIn> streets, Func<TIn, TOut> selector) where TOut : IHasStreet
	{
		StreetMap<TOut> result = default(StreetMap<TOut>);
		InlineList<TIn>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			result.Add(selector(current));
		}
		return result;
	}

	public static StreetMap<T> ToSingleStreetMap<T>(this T street) where T : IHasStreet
	{
		StreetMap<T> result = default(StreetMap<T>);
		result.Add(street);
		return result;
	}

	public static bool Any<T>(this in StreetMap<T> streets, Func<T, bool> predicate)
	{
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Any<T, TArgumentOne>(this in StreetMap<T> streets, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne)
	{
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item, argumentOne))
			{
				return true;
			}
		}
		return false;
	}

	public static StreetMap<TOut> Map<TIn, TOut>(this in StreetMap<TIn> streets, Func<TIn, TOut> selector)
	{
		StreetMap<TOut> result = default(StreetMap<TOut>);
		StreetMap<TIn>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (street, arg) = enumerator.Current;
			result.Add(street, selector(arg));
		}
		return result;
	}

	public static StreetMap<TOut> Map<TIn, TArgumentOne, TOut>(this in StreetMap<TIn> streets, Func<TIn, TArgumentOne, TOut> selector, TArgumentOne argumentOne)
	{
		StreetMap<TOut> result = default(StreetMap<TOut>);
		StreetMap<TIn>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (street, arg) = enumerator.Current;
			result.Add(street, selector(arg, argumentOne));
		}
		return result;
	}

	public static StreetMap<TOut> Map<TIn, TArgumentOne, TArgumentTwo, TOut>(this in StreetMap<TIn> streets, Func<TIn, TArgumentOne, TArgumentTwo, TOut> selector, TArgumentOne argumentOne, TArgumentTwo argumentTwo)
	{
		StreetMap<TOut> result = default(StreetMap<TOut>);
		StreetMap<TIn>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (street, arg) = enumerator.Current;
			result.Add(street, selector(arg, argumentOne, argumentTwo));
		}
		return result;
	}

	public static StreetMap<T> With<T>(this StreetMap<T> streets, T newStreet) where T : IHasStreet
	{
		streets.Add(newStreet.Street, newStreet);
		return streets;
	}

	public static StreetMap<T> With<T>(this StreetMap<T> streets, Streets street, T newValue)
	{
		streets.Add(street, newValue);
		return streets;
	}

	public static StreetMap<T> WithLast<T>(this StreetMap<T> streets, Func<T, T> getNew)
	{
		Streets lastStreet = streets.LastStreet;
		T value = getNew(streets.Last);
		streets.Add(lastStreet, value);
		return streets;
	}

	public static StreetMap<T> Where<T>(this in StreetMap<T> streets, Func<T, bool> predicate)
	{
		StreetMap<T> result = default(StreetMap<T>);
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (street, val) = enumerator.Current;
			if (predicate(val))
			{
				result.Add(street, val);
			}
		}
		return result;
	}

	public static InlineList<T> Reversed<T>(this in StreetMap<T> streets)
	{
		InlineList<T> result = default(InlineList<T>);
		InlineList<T>.Enumerator enumerator = streets.Values.Reverse<T>().GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			result.Add(current);
		}
		return result;
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this StreetMap<TIn> streets, Func<TIn, IEnumerable<TOut>> selector)
	{
		StreetMap<TIn>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn item = enumerator.Current.value;
			foreach (TOut item2 in selector(item))
			{
				yield return item2;
			}
		}
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this StreetMap<TIn> streets, Func<TIn, MemoryList<TOut>> selector)
	{
		StreetMap<TIn>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn item = enumerator.Current.value;
			MemoryList<TOut>.Enumerator enumerator2 = selector(item).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				yield return enumerator2.Current;
			}
		}
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this StreetMap<TIn> streets, Func<TIn, InlineList<TOut>> selector)
	{
		StreetMap<TIn>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn item = enumerator.Current.value;
			InlineList<TOut>.Enumerator enumerator2 = selector(item).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				yield return enumerator2.Current;
			}
		}
	}

	public static bool TryGet<T>(this in StreetMap<T> streets, Func<T, bool> predicate, out T result)
	{
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item))
			{
				result = item;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static bool TryGetReversed<T>(this in StreetMap<T> streets, Func<T, bool> predicate, out T result)
	{
		InlineList<T>.Enumerator enumerator = Reversed(in streets).GetEnumerator();
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

	public static StreetMap<TOut> ToStreetMap<TIn, TOut>(this IEnumerable<TIn> streets, Func<TIn, TOut> selector) where TIn : IHasStreet
	{
		StreetMap<TOut> result = default(StreetMap<TOut>);
		foreach (TIn street in streets)
		{
			result.Add(street.Street, selector(street));
		}
		return result;
	}

	public static bool TryGetLast<T>(this in StreetMap<T> streets, out T result)
	{
		if (streets.Count > 0)
		{
			result = streets.Last;
			return true;
		}
		result = default(T);
		return false;
	}

	public static bool TryGetLast<T>(this in StreetMap<T> street, Func<T, bool> predicate, out T result)
	{
		return street.Values.TryGetLast(predicate, out result);
	}

	public static int GetStreetNumber(this Streets street)
	{
		return (int)(street + 1);
	}

	public static StreetMap<T> ToStreetMap<T>(this IEnumerable<T> streets) where T : IHasStreet
	{
		StreetMap<T> result = default(StreetMap<T>);
		foreach (T street in streets)
		{
			result.Add(street.Street, street);
		}
		return result;
	}

	public static double Sum<T>(this in StreetMap<T> streets, Func<T, double> selector)
	{
		double num = 0.0;
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			num += selector(item);
		}
		return num;
	}

	public static int Sum<T>(this in StreetMap<T> streets, Func<T, int> selector)
	{
		int num = 0;
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			num += selector(item);
		}
		return num;
	}

	public static StreetMap<T> WithRemoved<T>(this StreetMap<T> streets, Streets street)
	{
		streets.Remove(street);
		return streets;
	}

	public static bool All<T>(this in StreetMap<T> streets, Func<T, bool> predicate)
	{
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (!predicate(item))
			{
				return false;
			}
		}
		return true;
	}

	public static T[] ToArray<T>(this in StreetMap<T> streets)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(streets.Count);
		int num = 0;
		StreetMap<T>.Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			array[num] = item;
			num++;
		}
		return array;
	}

	public static bool TryGetStreet<T>(this IEnumerable<T> items, Streets street, out T result) where T : IHasStreet
	{
		return items.TryGet((T item) => item.Street == street, out result);
	}

	public static ImmutableList<T> RemoveStreet<T>(this ImmutableList<T> items, Streets street) where T : IHasStreet
	{
		return items.RemoveAll((T item) => item.Street == street);
	}

	public static ImmutableList<T> ReplaceStreet<T>(this ImmutableList<T> items, T newItem) where T : IHasStreet
	{
		return items.Replace((T item) => item.Street == newItem.Street, newItem);
	}

	public static bool TryGetEnumStreet<TEnum>(this TEnum value, out Streets result)
	{
		if (value.TryGetEnumAttribute<StreetAttribute>(out var result2))
		{
			result = result2.Street;
			return true;
		}
		result = Streets.Preflop;
		return false;
	}

	public static Streets GetClosestPostflopStreet(this Streets street)
	{
		if (street != Streets.Preflop)
		{
			return street;
		}
		return Streets.Flop;
	}
}
