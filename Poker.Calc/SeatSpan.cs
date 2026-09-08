namespace Poker.Calc;

public ref struct SeatSpan<T>
{
	private Span<T> _buffer;

	public int MaxCountPerSeat { get; }

	public SeatMap<int> Counts { get; private set; }

	public SeatSpan(Span<T> buffer, int maxCountPerSeat)
	{
		Counts = default(SeatMap<int>);
		_buffer = buffer;
		if (buffer.Length < 10 * maxCountPerSeat)
		{
			throw new InvalidOperationException("Buffer size (" + buffer.Length.Quoted() + ") is too small for the maxCountSeat=" + maxCountPerSeat.Quoted());
		}
		MaxCountPerSeat = maxCountPerSeat.VerifyArgumentPositive("maxCountPerSeat");
	}

	public void Add(int seatNumber, T value)
	{
		int count = GetCount(seatNumber);
		if (count == MaxCountPerSeat)
		{
			throw new InvalidOperationException($"Can't add more element to the buffer because the max count per seat ({MaxCountPerSeat}) is reached.");
		}
		int index = (seatNumber - 1) * MaxCountPerSeat + count;
		_buffer[index] = value;
		Counts = Counts.With(seatNumber, count + 1);
	}

	public Span<T> GetSeatValues(int seatNumber)
	{
		return _buffer.Slice((seatNumber - 1) * MaxCountPerSeat, GetCount(seatNumber));
	}

	public int GetCount(int seatNumber)
	{
		if (!Counts.TryGet(seatNumber, out var result))
		{
			return 0;
		}
		return result;
	}

	public SeatMap<T[]> ToSeatMap()
	{
		SeatMap<T[]> result = default(SeatMap<T[]>);
		InlineList<int>.Enumerator enumerator = Counts.SeatNumbers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			result.Set(current, GetSeatValues(current).ToArray());
		}
		return result;
	}
}
