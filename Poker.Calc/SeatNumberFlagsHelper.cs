namespace Poker.Calc;

public static class SeatNumberFlagsHelper
{
	public static SeatNumberFlags VerifyArgumentNotEmpty(this SeatNumberFlags seats, string argumentName)
	{
		if (seats.IsEmpty)
		{
			throw new ArgumentException("Argument cannot be empty.", argumentName);
		}
		return seats;
	}

	public static SeatNumberFlags ToSeatNumberFlags(this IEnumerable<int> seatNumbers)
	{
		SeatNumberFlags result = SeatNumberFlags.Empty;
		foreach (int seatNumber in seatNumbers)
		{
			result = result.Add(seatNumber);
		}
		return result;
	}

	public static SeatNumberFlags GetSeatNumberFlags<T>(this InlineList<T> seats) where T : IHasSeatNumber
	{
		SeatNumberFlags result = SeatNumberFlags.Empty;
		InlineList<T>.Enumerator enumerator = seats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = result.Add(enumerator.Current.SeatNumber);
		}
		return result;
	}

	public static InlineList<int> AsInlineList(this SeatNumberFlags seatNumberFlags)
	{
		InlineList<int> empty = InlineList<int>.Empty;
		SeatNumberFlags.Enumerator enumerator = seatNumberFlags.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			empty.Add(current);
		}
		return empty;
	}

	public static int GetNextSeatNumber(this int seatNumber, SeatNumberFlags allSeatNumbers)
	{
		if (!allSeatNumbers.AsInlineList().RoundEnumerateAfter(seatNumber).TryGetFirst(out int result))
		{
			throw new InvalidOperationException("No other seatNumber in the given seatNumbers");
		}
		return result;
	}

	public static List<int> ToList(this SeatNumberFlags flags)
	{
		List<int> list = new List<int>();
		SeatNumberFlags.Enumerator enumerator = flags.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			list.Add(current);
		}
		return list;
	}

	public static InlineList<int> ToInlineList(this SeatNumberFlags flags)
	{
		InlineList<int> result = default(InlineList<int>);
		SeatNumberFlags.Enumerator enumerator = flags.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			result.Add(current);
		}
		return result;
	}

	public static SeatNumberFlags GetIntersection(this SeatNumberFlags seatNumbers, SeatNumberFlags other)
	{
		return new SeatNumberFlags(seatNumbers.Flags & other.Flags);
	}
}
