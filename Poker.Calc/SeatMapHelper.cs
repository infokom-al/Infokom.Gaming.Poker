using MemoryPools;

using System.Text;

namespace Poker.Calc;

public static class SeatMapHelper
{
	public static SeatMap<T> ToSeatMap<T>(this IEnumerable<T> items) where T : IHasSeatNumber
	{
		SeatMap<T> result = default(SeatMap<T>);
		foreach (T item in items)
		{
			result.Set(item.SeatNumber, item);
		}
		return result;
	}

	public static SeatMap<T> ToSeatMap<T>(this in InlineList<T> items) where T : IHasSeatNumber
	{
		SeatMap<T> result = default(SeatMap<T>);
		InlineList<T>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			result.Set(current.SeatNumber, current);
		}
		return result;
	}

	public static SeatMap<TOut> ToSeatMap<TIn, TOut>(this in InlineList<TIn> items, Func<TIn, TOut> selector) where TIn : IHasSeatNumber
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		InlineList<TIn>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			result.Set(current.SeatNumber, selector(current));
		}
		return result;
	}

	public static SeatMap<T> ToSeatMap<T>(this in SeatNumberFlags seatNumbers, Func<int, T> selector)
	{
		SeatMap<T> result = default(SeatMap<T>);
		SeatNumberFlags.Enumerator enumerator = seatNumbers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			result.Set(current, selector(current));
		}
		return result;
	}

	public static SeatMap<T> ToSeatMap<TArgumentOne, T>(this in SeatNumberFlags seatNumbers, Func<int, TArgumentOne, T> selector, TArgumentOne argumentOne)
	{
		SeatMap<T> result = default(SeatMap<T>);
		SeatNumberFlags.Enumerator enumerator = seatNumbers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			result.Set(current, selector(current, argumentOne));
		}
		return result;
	}

	public static SeatMap<TOut> Map<TIn, TOut>(this in SeatMap<TIn> seats, Func<TIn, TOut> convert)
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		SeatMap<TIn>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, arg) = enumerator.Current;
			result.Set(seatNumber, convert(arg));
		}
		return result;
	}

	public static SeatMap<TOut> MapSeatNumbers<TIn, TOut>(this in SeatMap<TIn> seats, Func<TIn, TOut> convert) where TOut : IHasSeatNumber
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		InlineList<TIn>.Enumerator enumerator = seats.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn current = enumerator.Current;
			result.Set(convert(current).SeatNumber, convert(current));
		}
		return result;
	}

	public static void Set<T>(this ref SeatMap<T> seats, T seat) where T : IHasSeatNumber
	{
		seats.Set(seat.SeatNumber, seat);
	}

	public static SeatMap<T> With<T>(this SeatMap<T> seats, IEnumerable<T> newSeats) where T : IHasSeatNumber
	{
		foreach (T newSeat in newSeats)
		{
			seats = seats.With(newSeat);
		}
		return seats;
	}

	public static SeatMap<T> With<T>(this in SeatMap<T> seats, T seat) where T : IHasSeatNumber
	{
		return seats.With(seat.SeatNumber, seat);
	}

	public static SeatMap<TOut> Map<TIn, TArgument, TOut>(this in SeatMap<TIn> seats, Func<TIn, TArgument, TOut> convert, TArgument argument)
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		SeatMap<TIn>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, arg) = enumerator.Current;
			result.Set(seatNumber, convert(arg, argument));
		}
		return result;
	}

	public static SeatMap<TOut> Map<TIn, TOut>(this in SeatMap<TIn> seats, Func<int, TIn, TOut> convert)
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		SeatMap<TIn>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (num, arg) = enumerator.Current;
			result.Set(num, convert(num, arg));
		}
		return result;
	}

	public static SeatMap<TOut> Map<TIn, TArgumentOne, TArgumentTwo, TOut>(this in SeatMap<TIn> seats, Func<TIn, TArgumentOne, TArgumentTwo, TOut> convert, TArgumentOne argumentOne, TArgumentTwo argumentTwo)
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		SeatMap<TIn>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, arg) = enumerator.Current;
			result.Set(seatNumber, convert(arg, argumentOne, argumentTwo));
		}
		return result;
	}

	public static SeatMap<TOut> Map<TIn, TArgumentOne, TArgumentTwo, TArgumentThree, TOut>(this in SeatMap<TIn> seats, Func<TIn, TArgumentOne, TArgumentTwo, TArgumentThree, TOut> convert, TArgumentOne argumentOne, TArgumentTwo argumentTwo, TArgumentThree argumentThree)
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		SeatMap<TIn>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, arg) = enumerator.Current;
			result.Set(seatNumber, convert(arg, argumentOne, argumentTwo, argumentThree));
		}
		return result;
	}

	public static SeatNumberFlags ToSeatNumberFlags(this int seatNumber)
	{
		return default(SeatNumberFlags).Add(seatNumber);
	}

	public static SeatNumberFlags ToSeatNumberFlags(this int[] seatNumbers)
	{
		SeatNumberFlags result = default(SeatNumberFlags);
		foreach (int seatNumber in seatNumbers)
		{
			result = result.Add(seatNumber);
		}
		return result;
	}

	public static InlineList<T> OrderValuesBy<T, TOrder>(this in SeatMap<T> seats, Func<T, TOrder> selector) where TOrder : IComparable<TOrder>
	{
		return seats.OrderBy(selector).Map(((int seatNumber, T seat) seat) => seat.seat);
	}

	public static InlineList<(int seatNumber, T seat)> OrderBy<T, TOrder>(this in SeatMap<T> seats, Func<T, TOrder> selector) where TOrder : IComparable<TOrder>
	{
		InlineList<(int, T)> result = new InlineList<(int, T)>(seats.Count);
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (item, item2) = enumerator.Current;
			result.Add((item, item2));
		}
		for (int i = 1; i < result.Count; i++)
		{
			(int, T) value = result[i];
			int num = i - 1;
			while (num >= 0 && selector(result[num].Item2).CompareTo(selector(value.Item2)) > 0)
			{
				result[num + 1] = result[num];
				num--;
			}
			result[num + 1] = value;
		}
		return result;
	}

	public static InlineList<T> OrderValuesByDescending<T>(this SeatMap<T> seats) where T : IComparable<T>
	{
		return seats.OrderValuesByDescending((T x) => x);
	}

	public static int Count<T>(this in SeatMap<T> seats, Func<T, bool> predicate)
	{
		int num = 0;
		InlineList<T>.Enumerator enumerator = seats.Values.GetEnumerator();
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

	public static int Count<T, TArgumentOne>(this in SeatMap<T> seats, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne)
	{
		int num = 0;
		InlineList<T>.Enumerator enumerator = seats.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (predicate(current, argumentOne))
			{
				num++;
			}
		}
		return num;
	}

	public static SeatMap<T> ToSeatMap<T>(this in InlineList<(int seatNumber, T value)> seats)
	{
		SeatMap<T> result = default(SeatMap<T>);
		InlineList<(int, T)>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, value) = enumerator.Current;
			result.Set(seatNumber, value);
		}
		return result;
	}

	public static double Sum(this in SeatMap<double> seats)
	{
		double num = 0.0;
		InlineList<double>.Enumerator enumerator = seats.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			double current = enumerator.Current;
			num += current;
		}
		return num;
	}

	public static double Sum<T>(this in SeatMap<T> seats, Func<T, double> selector)
	{
		double num = 0.0;
		InlineList<T>.Enumerator enumerator = seats.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			num += selector(current);
		}
		return num;
	}

	public static double Sum<T, TArgumentOne>(this in SeatMap<T> seats, Func<T, TArgumentOne, double> selector, TArgumentOne argumentOne)
	{
		double num = 0.0;
		InlineList<T>.Enumerator enumerator = seats.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			num += selector(current, argumentOne);
		}
		return num;
	}

	public static bool Any<T>(this in SeatMap<T> seats, Func<T, bool> predicate)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
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

	public static bool Any<T, TArgument>(this in SeatMap<T> seats, Func<T, TArgument, bool> predicate, TArgument argument)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item, argument))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Any<T, TArgumentOne, TArgumentTwo>(this in SeatMap<T> seats, Func<T, TArgumentOne, TArgumentTwo, bool> predicate, TArgumentOne argumentOne, TArgumentTwo argumentTwo)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item, argumentOne, argumentTwo))
			{
				return true;
			}
		}
		return false;
	}

	public static SeatMap<T> Where<T>(this in SeatMap<T> seats, Func<T, bool> predicate)
	{
		SeatMap<T> result = default(SeatMap<T>);
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, val) = enumerator.Current;
			if (predicate(val))
			{
				result.Set(seatNumber, val);
			}
		}
		return result;
	}

	public static SeatMap<T> Where<T, TArgument>(this in SeatMap<T> seats, Func<T, TArgument, bool> predicate, TArgument argument)
	{
		SeatMap<T> result = default(SeatMap<T>);
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, val) = enumerator.Current;
			if (predicate(val, argument))
			{
				result.Set(seatNumber, val);
			}
		}
		return result;
	}

	public static SeatMap<T> Where<T, TArgumentOne, TArgumentTwo>(this in SeatMap<T> seats, Func<T, TArgumentOne, TArgumentTwo, bool> predicate, TArgumentOne argumentOne, TArgumentTwo argumentTwo)
	{
		SeatMap<T> result = default(SeatMap<T>);
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, val) = enumerator.Current;
			if (predicate(val, argumentOne, argumentTwo))
			{
				result.Set(seatNumber, val);
			}
		}
		return result;
	}

	public static void AddOrIncrement(this ref SeatMap<double> seats, int seatNumber, double delta)
	{
		if (seats.TryGet(seatNumber, out var result))
		{
			seats.Set(seatNumber, result + delta);
		}
		else
		{
			seats.Set(seatNumber, delta);
		}
	}

	public static bool TryGet<T>(this in SeatMap<T> seats, Func<T, bool> predicate, out T result)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
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

	public static int GetSeatNumber<T>(this in SeatMap<T> seats, T value)
	{
		if (!seats.TryGetSeatNumber(value, out var result))
		{
			throw new InvalidOperationException($"Failed to get seat number of {value}");
		}
		return result;
	}

	public static bool TryGetSeatNumber<T>(this in SeatMap<T> seats, T value, out int result)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int seatNumber, T value) current = enumerator.Current;
			var (num, _) = current;
			if (object.Equals(current.value, value))
			{
				result = num;
				return true;
			}
		}
		result = 0;
		return false;
	}

	public static bool TryGetSeatNumber<T>(this in SeatMap<T> seats, Func<T, bool> predicate, out int result)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (num, arg) = enumerator.Current;
			if (predicate(arg))
			{
				result = num;
				return true;
			}
		}
		result = 0;
		return false;
	}

	public static bool TryGet<T, TArgumentOne>(this in SeatMap<T> seats, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne, out T result)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item, argumentOne))
			{
				result = item;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static void VerifyAll<T>(this in SeatMap<T> seats, Func<T, bool> predicate, string message)
	{
		if (!seats.All(predicate))
		{
			throw new InvalidOperationException(message);
		}
	}

	public static void VerifyAny<T>(this in SeatMap<T> seats, Func<T, bool> predicate, string message)
	{
		if (!seats.Any(predicate))
		{
			throw new InvalidOperationException(message);
		}
	}

	public static void ForEach<T>(this in SeatMap<T> seats, Action<T> action)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			action(item);
		}
	}

	public static SeatMap<T> VerifyArgumentNonEmpty<T>(this in SeatMap<T> seats, string argumentName)
	{
		if (seats.IsEmpty)
		{
			throw new ArgumentException(argumentName + " was empty");
		}
		return seats;
	}

	public static SeatMap<T> Replace<T>(this in SeatMap<T> seats, int seatNumber, Func<T, T> getNew)
	{
		SeatMap<T> result = seats;
		result.Set(seatNumber, getNew(seats.Get(seatNumber)));
		return result;
	}

	public static IEnumerable<TOut> SelectMany<TIn, TOut>(this SeatMap<TIn> seats, Func<TIn, InlineList<TOut>> selector)
	{
		InlineList<TIn>.Enumerator enumerator = seats.Values.GetEnumerator();
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

	public static SeatMap<T> ToSingleSeatMap<T>(this (int seatNumber, T value) item)
	{
		SeatMap<T> result = default(SeatMap<T>);
		result.Set(item.seatNumber, item.value);
		return result;
	}

	public static SeatMap<T> ToSeatMap<T>(this IEnumerable<(int seatNumber, T value)> seats)
	{
		SeatMap<T> result = default(SeatMap<T>);
		foreach (var (seatNumber, value) in seats)
		{
			result.Set(seatNumber, value);
		}
		return result;
	}

	public static bool All<T>(this in SeatMap<T> seats, Func<T, bool> predicate)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
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

	public static bool All<T, TArgument>(this in SeatMap<T> seats, Func<T, TArgument, bool> predicate, TArgument argument)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (!predicate(item, argument))
			{
				return false;
			}
		}
		return true;
	}

	public static SeatMap<T> Except<T>(this in SeatMap<T> seats, SeatNumberFlags seatNumbers)
	{
		SeatMap<T> empty = SeatMap<T>.Empty;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, value) = enumerator.Current;
			if (!seatNumbers.Contains(seatNumber))
			{
				empty.Set(seatNumber, value);
			}
		}
		return empty;
	}

	public static SeatMap<T> Except<T>(this in SeatMap<T> seats, int seatNumber)
	{
		SeatMap<T> empty = SeatMap<T>.Empty;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (num, value) = enumerator.Current;
			if (num != seatNumber)
			{
				empty.Set(num, value);
			}
		}
		return empty;
	}

	public static SeatMap<T> Except<T>(this in SeatMap<T> seats, T seat)
	{
		SeatMap<T> empty = SeatMap<T>.Empty;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, val) = enumerator.Current;
			if (!EqualityComparer<T>.Default.Equals(val, seat))
			{
				empty.Set(seatNumber, val);
			}
		}
		return empty;
	}

	public static T First<T>(this in SeatMap<T> seats, Func<T, bool> predicate)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item))
			{
				return item;
			}
		}
		throw new InvalidOperationException("No element satisfies the condition in predicate.");
	}

	public static int GetSeatNumberWithMinValue<T, K>(this in SeatMap<T> seats, Func<T, K> getValue) where K : IComparable<K>
	{
		int result = 0;
		K other = default(K);
		bool flag = false;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int seatNumber, T value) current = enumerator.Current;
			int item = current.seatNumber;
			T item2 = current.value;
			K val = getValue(item2);
			if (!flag || val.CompareTo(other) < 0)
			{
				other = val;
				result = item;
				flag = true;
			}
		}
		if (!flag)
		{
			throw new InvalidOperationException("SeatMap contains no elements.");
		}
		return result;
	}

	public static bool IsCountAtLeast<T>(this in SeatMap<T> seats, Func<T, bool> predicate, int count)
	{
		int num = 0;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item) && ++num >= count)
			{
				return true;
			}
		}
		return false;
	}

	public static bool TryGet<T>(this in SeatMap<T> seats, string username, out T result) where T : IHasUsername
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (item.Username == username)
			{
				result = item;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static double Max<T>(this in SeatMap<T> seats, Func<T, double> selector)
	{
		if (seats.Count == 0)
		{
			return 0.0;
		}
		double num = double.MinValue;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			double num2 = selector(item);
			if (num2.IsGreater(num))
			{
				num = num2;
			}
		}
		return num;
	}

	public static int Max<T>(this in SeatMap<T> seats, Func<T, int> selector)
	{
		int num = int.MinValue;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			int num2 = selector(item);
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static int MaxOrZero(this in SeatMap<int> seats)
	{
		if (seats.Count == 0)
		{
			return 0;
		}
		return seats.Max();
	}

	public static int Max(this in SeatMap<int> seats)
	{
		int num = int.MinValue;
		SeatMap<int>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int item = enumerator.Current.value;
			if (item > num)
			{
				num = item;
			}
		}
		return num;
	}

	public static double MaxOrZero(this in SeatMap<double> seats)
	{
		if (seats.Count == 0)
		{
			return 0.0;
		}
		return seats.Max();
	}

	public static double Max(this in SeatMap<double> seats)
	{
		double num = double.MinValue;
		SeatMap<double>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			double item = enumerator.Current.value;
			if (item.IsGreater(num))
			{
				num = item;
			}
		}
		return num;
	}

	public static double Min(this in SeatMap<double> seats)
	{
		if (seats.Count == 0)
		{
			return 0.0;
		}
		double num = double.MaxValue;
		SeatMap<double>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			double item = enumerator.Current.value;
			if (item.IsLess(num))
			{
				num = item;
			}
		}
		return num;
	}

	public static bool TryGetFirst<T>(this in SeatMap<T> seats, out T result)
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		if (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			result = item;
			return true;
		}
		result = default(T);
		return false;
	}

	public static SeatMap<double> Divide(this in SeatMap<double> seats, double divisor)
	{
		SeatMap<double> empty = SeatMap<double>.Empty;
		SeatMap<double>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, num) = enumerator.Current;
			empty.Set(seatNumber, num / divisor);
		}
		return empty;
	}

	public static void VerifyDistictBy<T, K>(this in SeatMap<T> seats, Func<T, K> selector, string? message = null)
	{
		HashSet<K> hashSet = ObjectPool<HashSet<K>>.ThreadShared.RentObject();
		hashSet.Clear();
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			K item2 = selector(item);
			if (hashSet.Contains(item2))
			{
				throw new ArgumentException(message ?? (seats.GetType().GetNameWithGenerics() + " contains duplicates (" + typeof(K).Name + ")"));
			}
			hashSet.Add(item2);
		}
	}

	public static InlineList<(int seatNumber, T value)> RoundEnumerateSince<T>(this in SeatMap<T> seats, int seatNumber)
	{
		InlineList<(int, T)> result = default(InlineList<(int, T)>);
		bool flag = false;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (num, item) = enumerator.Current;
			if (num == seatNumber)
			{
				flag = true;
			}
			if (flag)
			{
				result.Add((num, item));
			}
		}
		if (!flag)
		{
			enumerator = seats.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (item2, item3) = enumerator.Current;
				result.Add((item2, item3));
			}
		}
		else
		{
			enumerator = seats.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (num2, item4) = enumerator.Current;
				if (num2 == seatNumber)
				{
					break;
				}
				result.Add((num2, item4));
			}
		}
		return result;
	}

	public static InlineList<(int seatNumber, T value)> RoundEnumerateAfter<T>(this in SeatMap<T> seats, int seatNumber)
	{
		InlineList<(int, T)> result = default(InlineList<(int, T)>);
		SeatMap<T>.Enumerator enumerator;
		if (seats.LastSeatNumber <= seatNumber)
		{
			enumerator = seats.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (item, item2) = enumerator.Current;
				result.Add((item, item2));
			}
			return result;
		}
		bool flag = false;
		enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (num, item3) = enumerator.Current;
			if (!flag)
			{
				if (num == seatNumber)
				{
					flag = true;
				}
				else if (num > seatNumber)
				{
					flag = true;
					result.Add((num, item3));
				}
			}
			else
			{
				result.Add((num, item3));
			}
		}
		if (flag)
		{
			enumerator = seats.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (num2, item4) = enumerator.Current;
				if (num2 <= seatNumber)
				{
					result.Add((num2, item4));
				}
				if (num2 >= seatNumber)
				{
					break;
				}
			}
		}
		return result;
	}

	public static string AggregateToString<T>(this in SeatMap<T> seats, string separator)
	{
		StringBuilder stringBuilder = new StringBuilder();
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}

	public static InlineList<T> RoundEnumerateValuesAfter<T>(this in SeatMap<T> seats, int seatNumber)
	{
		InlineList<T> result = default(InlineList<T>);
		SeatMap<T>.Enumerator enumerator;
		if (seats.LastSeatNumber <= seatNumber)
		{
			enumerator = seats.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T item = enumerator.Current.value;
				result.Add(item);
			}
			return result;
		}
		bool flag = false;
		enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (num, seat) = enumerator.Current;
			if (!flag)
			{
				if (num == seatNumber)
				{
					flag = true;
				}
				else if (num > seatNumber)
				{
					flag = true;
					result.Add(seat);
				}
			}
			else
			{
				result.Add(seat);
			}
		}
		if (flag)
		{
			enumerator = seats.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (num2, seat2) = enumerator.Current;
				if (num2 <= seatNumber)
				{
					result.Add(seat2);
				}
				if (num2 >= seatNumber)
				{
					break;
				}
			}
		}
		return result;
	}

	public static InlineList<T> OrderValuesByDescending<T, TOrder>(this in SeatMap<T> seats, Func<T, TOrder> selector, IComparer<TOrder> comparer)
	{
		return seats.Values.OrderByDescending(selector, comparer);
	}

	public static InlineList<T> OrderValuesByDescending<T, TOrder>(this in SeatMap<T> seats, Func<T, TOrder> selector) where TOrder : IComparable<TOrder>
	{
		return seats.Values.OrderByDescending(selector);
	}

	public static InlineList<(int seatNumber, T value)> OrderByDescending<T>(this in SeatMap<T> seats) where T : IComparable<T>
	{
		InlineList<(int, T)> items = default(InlineList<(int, T)>);
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (item, item2) = enumerator.Current;
			items.Add((item, item2));
		}
		return items.OrderByDescending<(int, T), T>(((int seatNumber, T value) tuple2) => tuple2.value);
	}

	public static void Increment(this ref SeatMap<double> seats, int seatNumber, double delta)
	{
		if (seats.TryGet(seatNumber, out var result))
		{
			seats.Set(seatNumber, result + delta);
		}
		else
		{
			seats.Set(seatNumber, delta);
		}
	}

	public static (int seatNumber, T value)[] ToArrayWithSeatNumbers<T>(this in SeatMap<T> seats)
	{
		(int, T)[] array = new (int, T)[seats.Count];
		int num = 0;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int, T) current = enumerator.Current;
			array[num] = current;
			num++;
		}
		return array;
	}

	public static T[] ToArray<T>(this in SeatMap<T> seats)
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(seats.Count);
		int num = 0;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			array[num] = item;
			num++;
		}
		return array;
	}

	public static T GetNextSeat<T>(this T seat, SeatMap<T> seats) where T : IHasSeatNumber
	{
		return seats.Get(seat.SeatNumber.GetNextSeatNumber(seats.SeatNumberFlags));
	}

	public static T GetNextSeatUntil<T>(this T seat, SeatMap<T> seats, Func<T, bool> predicate) where T : IHasSeatNumber
	{
		for (int i = 0; i < seats.Count; i++)
		{
			seat = seat.GetNextSeat(seats);
			if (predicate(seat))
			{
				return seat;
			}
		}
		throw new InvalidOperationException("No seat found matching the predicate.");
	}

	public static T WithMaxValue<T, TComparable>(this in SeatMap<T> seats, Func<T, TComparable> selector) where TComparable : IComparable<TComparable>
	{
		return seats.Values.OrderByDescending(selector).First;
	}

	public static List<TOut> MapToList<TIn, TOut>(this in SeatMap<TIn> seats, Func<TIn, TOut> selector)
	{
		List<TOut> list = new List<TOut>();
		SeatMap<TIn>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TIn item = enumerator.Current.value;
			list.Add(selector(item));
		}
		return list;
	}

	public static SeatMap<T> WithSeat<T>(this SeatMap<T> seats, int seatNumber, Func<T, T> getNew)
	{
		if (!seats.Contains(seatNumber))
		{
			throw new ArgumentException($"Seat number {seatNumber} does not exist in the seat map.", "seatNumber");
		}
		T value = getNew(seats[seatNumber]);
		seats.Set(seatNumber, value);
		return seats;
	}

	public static SeatMap<T> RemoveAll<T>(this in SeatMap<T> seats, Func<T, bool> predicate)
	{
		SeatMap<T> result = seats;
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, arg) = enumerator.Current;
			if (predicate(arg))
			{
				result = result.Remove(seatNumber);
			}
		}
		return result;
	}

	public static SeatMap<T> VerifyArgumentNotEmpty<T>(this in SeatMap<T> seats, string argumentName)
	{
		if (seats.IsEmpty)
		{
			throw new ArgumentException("The seat map cannot be empty.", argumentName);
		}
		return seats;
	}

	public static SeatMap<TOut> Zip<TInOne, TInTwo, TOut>(this in SeatMap<TInOne> seats, SeatMap<TInTwo> other, Func<TInOne, TInTwo, TOut> selector)
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		SeatMap<TInOne>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, arg) = enumerator.Current;
			if (other.Contains(seatNumber))
			{
				TInTwo arg2 = other[seatNumber];
				TOut value = selector(arg, arg2);
				result.Set(seatNumber, value);
			}
		}
		return result;
	}

	public static T Get<T>(this in SeatMap<T> seats, string username) where T : IHasUsername
	{
		SeatMap<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (item.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
			{
				return item;
			}
		}
		throw new KeyNotFoundException("No seat with username '" + username + "' was found.");
	}

	public static SeatMap<TOut> ToSeatMap<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, int> getSeatNumber, Func<TIn, TOut> getValue)
	{
		SeatMap<TOut> result = default(SeatMap<TOut>);
		foreach (TIn item in items)
		{
			result.Set(getSeatNumber(item), getValue(item));
		}
		return result;
	}

	public static SeatMap<TOut> ToSeatMap<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, (int seatNumber, TOut value)> selector)
	{
		return items.ToSeatMap((TIn item) => selector(item).Item1, (TIn item) => selector(item).Item2);
	}

	public static HashSet<int> ToHashSet(this in InlineList<int> list)
	{
		return list.AsEnumerable().ToHashSet();
	}

	public static bool TryGet<T>(this IEnumerable<T> players, string username, out T result) where T : IHasUsername
	{
		return players.TryGet((T player) => player.Username == username, out result);
	}

	public static SeatMap<T> SetSeats<T>(this SeatMap<T> seats, IEnumerable<T> newSeats) where T : IHasSeatNumber
	{
		SeatMap<T> result = seats;
		foreach (T newSeat in newSeats)
		{
			result.Set(newSeat.SeatNumber, newSeat);
		}
		return result;
	}

	public static SeatMap<string> GetUsernames<T>(this SeatMap<T> seats) where T : IHasSeatNumber, IHasUsername
	{
		return seats.Map((T seat) => seat.Username);
	}
}
