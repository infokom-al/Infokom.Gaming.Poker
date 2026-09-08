using BinarySerializer;

using System.Text;

namespace Poker.Calc;

[BinarySerializable]
public struct HandValue : IComparable<HandValue>, IEquatable<HandValue>
{




	public HandValue()
	{
		this.Rank = 0;
		this.Type = PokerHands.HighCard;
		this.QuadsRank = CardRanks.Deuce;
		this.StraightRank = CardRanks.Deuce;
		this.TripsRank = CardRanks.Deuce;
		this.PairRank = CardRanks.Deuce;
		this.TopPairRank = CardRanks.Deuce;
		this.SecondPairRank = CardRanks.Deuce;
		this.TopKickerRank = CardRanks.Deuce;
		this.SecondKickerRank = CardRanks.Deuce;
		this.ThirdKickerRank = CardRanks.Deuce;
		this.FourthKickerRank = CardRanks.Deuce;
		this.FifthKickerRank = CardRanks.Deuce;
	}

	public HandValue(int rank, PokerHands type, CardRanks quadsRank = CardRanks.Deuce, CardRanks straightRank = CardRanks.Deuce, CardRanks tripsRank = CardRanks.Deuce, CardRanks pairRank = CardRanks.Deuce, CardRanks topPairRank = CardRanks.Deuce, CardRanks secondPairRank = CardRanks.Deuce, CardRanks topKickerRank = CardRanks.Deuce, CardRanks secondKickerRank = CardRanks.Deuce, CardRanks thirdKickerRank = CardRanks.Deuce, CardRanks fourthKickerRank = CardRanks.Deuce, CardRanks fifthKickerRank = CardRanks.Deuce)
	{
		this.Rank = rank;
		this.Type = type;
		this.QuadsRank = quadsRank;
		this.StraightRank = straightRank;
		this.TripsRank = tripsRank;
		this.PairRank = pairRank;
		this.TopPairRank = topPairRank;
		this.SecondPairRank = secondPairRank;
		this.TopKickerRank = topKickerRank;
		this.SecondKickerRank = secondKickerRank;
		this.ThirdKickerRank = thirdKickerRank;
		this.FourthKickerRank = fourthKickerRank;
		this.FifthKickerRank = fifthKickerRank;
	}


	[Tag(1)]
	public int Rank { get; private set; }

	[Tag(2)]
	public PokerHands Type { get; private set; }

	[Tag(3)]
	public CardRanks QuadsRank { get; private set; }

	[Tag(4)]
	public CardRanks StraightRank { get; private set; }

	[Tag(5)]
	public CardRanks TripsRank { get; private set; }

	[Tag(6)]
	public CardRanks PairRank { get; private set; }

	[Tag(7)]
	public CardRanks TopPairRank { get; private set; }

	[Tag(8)]
	public CardRanks SecondPairRank { get; private set; }

	[Tag(9)]
	public CardRanks TopKickerRank { get; private set; }

	[Tag(10)]
	public CardRanks SecondKickerRank { get; private set; }

	[Tag(11)]
	public CardRanks ThirdKickerRank { get; private set; }

	[Tag(12)]
	public CardRanks FourthKickerRank { get; private set; }

	[Tag(13)]
	public CardRanks FifthKickerRank { get; private set; }

	public readonly bool IsThreeOfAKind => this.Type == PokerHands.ThreeOfAKind;

	public readonly bool IsTwoPairs => this.Type == PokerHands.TwoPairs;

	public readonly bool IsFullHouseOrStronger => this.Type >= PokerHands.FullHouse;

	public readonly bool IsRoyalFlush
	{
		get
		{
			if (this.Type == PokerHands.StraightFlush)
			{
				return this.StraightRank == CardRanks.Ace;
			}
			return false;
		}
	}

	public bool IsStraightFlush => this.Type == PokerHands.StraightFlush;

	public bool IsQuads => this.Type == PokerHands.Quads;

	public bool IsWeaker(HandValue other)
	{
		return this.CompareTo(other) < 0;
	}

	public bool IsStrongerOrEqual(HandValue other)
	{
		return this.CompareTo(other) >= 0;
	}

	public readonly int CompareTo(HandValue other)
	{
		return this.Rank.CompareTo(other.Rank);
	}

	public override readonly string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(this.Type.ToString());
		if (this.QuadsRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.QuadsRank);
		}
		if (this.StraightRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.StraightRank);
		}
		if (this.TripsRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.TripsRank);
		}
		if (this.PairRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.PairRank);
		}
		if (this.TopPairRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.TopPairRank);
		}
		if (this.SecondPairRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.SecondPairRank);
		}
		if (this.TopKickerRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.TopKickerRank);
		}
		if (this.SecondKickerRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.SecondKickerRank);
		}
		if (this.ThirdKickerRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.ThirdKickerRank);
		}
		if (this.FourthKickerRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.FourthKickerRank);
		}
		if (this.FifthKickerRank > CardRanks.Deuce)
		{
			stringBuilder.Append(" " + this.FifthKickerRank);
		}
		return stringBuilder.ToString();
	}

	public readonly bool Equals(HandValue other)
	{
		return this.Rank == other.Rank;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj.GetType() != this.GetType())
		{
			return false;
		}
		return this.Equals((HandValue)obj);
	}

	public override readonly int GetHashCode()
	{
		return this.Rank;
	}

	public static bool operator ==(HandValue left, HandValue right) => left.Equals(right);

	public static bool operator !=(HandValue left, HandValue right) => !(left == right);

	public static bool operator <(HandValue left, HandValue right) => left.CompareTo(right) < 0;

	public static bool operator <=(HandValue left, HandValue right) => left.CompareTo(right) <= 0;

	public static bool operator >(HandValue left, HandValue right) => left.CompareTo(right) > 0;

	public static bool operator >=(HandValue left, HandValue right) => left.CompareTo(right) >= 0;
}
