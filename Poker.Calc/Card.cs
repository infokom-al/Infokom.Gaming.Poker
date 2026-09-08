using BinarySerializer;

using Poker.Calc.Common;

using System.Text.Json.Serialization;

namespace Poker.Calc;

[BinarySerializable]
public struct Card : IEquatable<Card>, IComparable<Card>
{
	public static Suits[] AllSuits;

	[Tag(1)]
	public CardRanks Rank { get; private set; }

	[Tag(2)]
	public Suits Suit { get; private set; }

	public int Index => (int)(13 * (int)Suit + Rank);

	public int RankIndex => (int)Rank;

	public string Abbreviation => Rank.ToAbbreviation() + Suit.ToAbbreviation();

	public string RankAbbreviation => Rank.ToAbbreviation();

	[JsonConstructor]
	public Card(CardRanks rank, Suits suit)
	{
		Rank = rank;
		Suit = suit;
	}

	public Card()
	{
		Rank = CardRanks.Deuce;
		Suit = Suits.Hearts;
	}

	public static Card FromCardIndex(int index)
	{
		return new Card((CardRanks)(index % 13), (Suits)(index / 13));
	}

	public override string ToString()
	{
		return Abbreviation;
	}

	public int CompareTo(Card other)
	{
		int num = Rank.CompareTo(other.Rank);
		if (num != 0)
		{
			return num;
		}
		return Suit.CompareTo(other.Suit);
	}

	public bool Equals(Card other)
	{
		if (Rank == other.Rank)
		{
			return Suit == other.Suit;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((int)Rank * 397) ^ (int)Suit;
	}

	public static bool operator ==(Card card1, Card card2)
	{
		return card1.Equals(card2);
	}

	public static bool operator !=(Card card1, Card card2)
	{
		return !card1.Equals(card2);
	}

	static Card()
	{
		AllSuits = EnumHelper.GetValues<Suits>().ToArray();
	}
}
