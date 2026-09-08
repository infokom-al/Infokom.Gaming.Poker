using BinarySerializer;

namespace Poker.Calc;

[BinarySerializable]
public readonly struct SeatNumberFlags : IEquatable<SeatNumberFlags>
{
	public struct Enumerator(SeatNumberFlags flags)
	{
		public SeatNumberFlags _flags = flags;

		private int _currentSeatNumber = 0;

		public int Current => _currentSeatNumber;

		public bool MoveNext()
		{
			_currentSeatNumber++;
			if (_currentSeatNumber > 10)
			{
				return false;
			}
			if (_flags.Contains(_currentSeatNumber))
			{
				return true;
			}
			return MoveNext();
		}
	}

	public static readonly SeatNumberFlags AllSeatNumbers = new SeatNumberFlags(1023);

	public static SeatNumberFlags Empty = default(SeatNumberFlags);

	[Tag(1)]
	public int Flags { get; }

	public bool IsSingleSeat => Flags.IsPowerOfTwo();

	public int Count
	{
		get
		{
			if (Flags != 0)
			{
				if (!IsSingleSeat)
				{
					return Flags.BitsCount();
				}
				return 1;
			}
			return 0;
		}
	}

	public int First => Flags.GetLowestBitNumber();

	public int Last => Flags.GetHighestBitNumber();

	public bool IsEmpty => Flags == 0;

	public List<int> AsList
	{
		get
		{
			List<int> list = new List<int>();
			Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				list.Add(current);
			}
			return list;
		}
	}

	public SeatNumberFlags(int flags)
	{
		Flags = flags;
	}

	public SeatNumberFlags Add(int seatNumber)
	{
		return new SeatNumberFlags(Flags | (1 << seatNumber - 1));
	}

	public bool Contains(int seatNumber)
	{
		return (Flags & (1 << seatNumber - 1)) != 0;
	}

	public SeatNumberFlags Add(SeatNumberFlags other)
	{
		return new SeatNumberFlags(Flags | other.Flags);
	}

	public SeatNumberFlags Remove(SeatNumberFlags other)
	{
		return new SeatNumberFlags(Flags & ~other.Flags);
	}

	public SeatNumberFlags Remove(int seatNumber)
	{
		return new SeatNumberFlags(Flags & ~(1 << seatNumber - 1));
	}

	public int GetSingleSeatNumber()
	{
		if (!IsSingleSeat)
		{
			throw new InvalidOperationException($"Expecting a single seat but was {Count} seats");
		}
		return Last;
	}

	public static bool operator ==(SeatNumberFlags seatNumbers, SeatNumberFlags other)
	{
		return seatNumbers.Flags == other.Flags;
	}

	public static bool operator !=(SeatNumberFlags seatNumbers, SeatNumberFlags other)
	{
		return seatNumbers.Flags != other.Flags;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public override string ToString()
	{
		return "Seats #" + AsList.AggregateToString(",");
	}

	public override bool Equals(object? @object)
	{
		if (@object is SeatNumberFlags seatNumberFlags)
		{
			return seatNumberFlags.Flags == Flags;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Flags);
	}

	public bool Equals(SeatNumberFlags other)
	{
		return Flags == other.Flags;
	}
}
