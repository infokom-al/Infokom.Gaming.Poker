using MemoryPools;

namespace Poker.Calc;

public ref struct StreetSpan<T>
{
	private Span<T> _buffer;

	public int MaxCountPerStreet { get; }

	public StreetMap<int> Counts { get; private set; }

	public StreetSpan(Span<T> buffer, int maxCountPerStreet)
	{
		Counts = default(StreetMap<int>);
		_buffer = buffer;
		MaxCountPerStreet = maxCountPerStreet;
	}

	public void Add(Streets street, T value)
	{
		int count = GetCount(street);
		if (count == MaxCountPerStreet)
		{
			throw new InvalidOperationException($"Can't add more element to the buffer because the max count per street ({MaxCountPerStreet}) is reached.");
		}
		int index = (int)street * MaxCountPerStreet + count;
		_buffer[index] = value;
		Counts = Counts.With(street, count + 1);
	}

	public StreetMap<T[]> ToStreetMap()
	{
		StreetMap<T[]> result = default(StreetMap<T[]>);
		StreetMap<int>.Enumerator enumerator = Counts.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Streets item = enumerator.Current.street;
			result.Add(item, GetValues(item).GetArray());
		}
		return result;
	}

	public T[] GetAllStreetValuesArray()
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(Counts.Sum((int x) => x));
		SpanList<T> spanList = new SpanList<T>(array);
		StreetMap<int>.Enumerator enumerator = Counts.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Streets item = enumerator.Current.street;
			Span<T> values = GetValues(item);
			for (int num = 0; num < values.Length; num++)
			{
				T item2 = values[num];
				spanList.Add(item2);
			}
		}
		return array;
	}

	public Span<T> GetValues(Streets street)
	{
		return _buffer.Slice((int)street * MaxCountPerStreet, GetCount(street));
	}

	public int GetCount(Streets street)
	{
		if (!Counts.TryGet(street, out var result))
		{
			return 0;
		}
		return result;
	}
}
