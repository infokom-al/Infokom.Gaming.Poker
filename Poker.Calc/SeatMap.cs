using BinarySerializer;

using CSharpSerializer.Serialization;

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[DebuggerDisplay("{DebugView}")]
[BinarySerializable]
[JsonSerializeAsCollection]
public struct SeatMap<T> : IEquatable<SeatMap<T>>
{
	public struct Enumerator(SeatMap<T> seats) : IEnumerator
	{
		private readonly SeatMap<T> _seats = seats;

		private int _currentSeatNumber = 0;

		public (int seatNumber, T value) Current => (seatNumber: _currentSeatNumber, value: _seats[_currentSeatNumber]);

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			_currentSeatNumber++;
			if (_currentSeatNumber > 10)
			{
				return false;
			}
			if (_seats.Contains(_currentSeatNumber))
			{
				return true;
			}
			return MoveNext();
		}

		public void Reset()
		{
			_currentSeatNumber = 0;
		}
	}

	[Tag(2)]
	public Items10<T> InnerSeats;

	public static readonly SeatMap<T> Empty;

	[Tag(1)]
	public SeatNumberFlags SeatNumberFlags { get; private set; }

	public int Length => Count;

	public int Count => SeatNumberFlags.Count;

	public bool IsEmpty => Count == 0;

	public T Last => Get(SeatNumberFlags.Last);

	public int FirstSeatNumber => SeatNumberFlags.First;

	public int LastSeatNumber => SeatNumberFlags.Last;

	public T First => Get(FirstSeatNumber);

	public InlineList<T> Values
	{
		get
		{
			InlineList<T> result = default(InlineList<T>);
			for (int i = 1; i <= 10; i++)
			{
				if (SeatNumberFlags.Contains(i))
				{
					result.Add(InnerSeats[i - 1]);
				}
				if (1 << i - 1 >= SeatNumberFlags.Flags)
				{
					return result;
				}
			}
			return result;
		}
	}

	public InlineList<int> SeatNumbers => SeatNumberFlags.AsInlineList();

	public T this[int seatNumber]
	{
		get
		{
			return InnerSeats[seatNumber - 1];
		}
		private set
		{
			InnerSeats[seatNumber - 1] = value;
			SeatNumberFlags = SeatNumberFlags.Add(seatNumber);
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public List<(int seatNumber, T value)> DebugView => GetDebugView();

	public SeatMap(Items10<T> innerSeats, SeatNumberFlags seatNumberFlags)
	{
		InnerSeats = innerSeats;
		SeatNumberFlags = seatNumberFlags;
	}

	[JsonConstructor]
	public SeatMap(IEnumerable<(int seatNumber, T value)> seatValues)
	{
		InnerSeats = default(Items10<T>);
		SeatNumberFlags = SeatNumberFlags.Empty;
		foreach (var seatValue in seatValues)
		{
			int item = seatValue.seatNumber;
			T item2 = seatValue.value;
			SeatNumberFlags = SeatNumberFlags.Add(item);
			this[item] = item2;
		}
	}

	public SeatMap<T> With(int seatNumber, T value)
	{
		SeatMap<T> result = default(SeatMap<T>);
		result.InnerSeats = InnerSeats;
		result.SeatNumberFlags = SeatNumberFlags;
		result.Set(seatNumber, value);
		return result;
	}

	public bool Contains(int seatNumber)
	{
		return SeatNumberFlags.Contains(seatNumber);
	}

	public T Get(int seatNumber)
	{
		if (!TryGet(seatNumber, out var result))
		{
			throw new InvalidOperationException($"Seat #{seatNumber} not found");
		}
		return result;
	}

	public void Set(int seatNumber, T value)
	{
		this[seatNumber] = value;
	}

	public bool TryGet(int seatNumber, out T result)
	{
		if (Contains(seatNumber))
		{
			result = InnerSeats[seatNumber - 1];
			return true;
		}
		result = default(T);
		return false;
	}

	public SeatMap<T> Remove(int seatNumber)
	{
		SeatMap<T> result = this;
		result.SeatNumberFlags = result.SeatNumberFlags.Remove(seatNumber);
		return result;
	}

	public static implicit operator InlineList<T>(SeatMap<T> seats)
	{
		return seats.Values;
	}

	public readonly Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public List<(int seatNumber, T value)> GetDebugView()
	{
		List<(int, T)> list = new List<(int, T)>();
		Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (item, item2) = enumerator.Current;
			list.Add((item, item2));
		}
		return list;
	}

	public override string ToString()
	{
		return $"{Count} {typeof(T).Name} seats";
	}

	public string Print()
	{
		if (IsEmpty)
		{
			return "Empty SeatMap";
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
		handler.AppendFormatted(Count);
		handler.AppendLiteral("  seats:");
		stringBuilder3.AppendLine(ref handler);
		Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int seatNumber, T value) current = enumerator.Current;
			int item = current.seatNumber;
			T item2 = current.value;
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(7, 2, stringBuilder2);
			handler.AppendLiteral("Seat ");
			handler.AppendFormatted(item);
			handler.AppendLiteral(": ");
			handler.AppendFormatted(item2);
			stringBuilder4.AppendLine(ref handler);
		}
		return stringBuilder.ToString();
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is SeatMap<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(SeatMap<T> other)
	{
		if (Count == 0 && other.Count == 0)
		{
			return true;
		}
		if (other.Count != Count)
		{
			return false;
		}
		return Values.SequenceEqual(other.Values);
	}

	static SeatMap()
	{
	}
}
