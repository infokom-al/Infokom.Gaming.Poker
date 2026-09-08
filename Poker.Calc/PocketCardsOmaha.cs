using BinarySerializer;

using MemoryPools;

namespace Poker.Calc;

[BinarySerializable]
public class PocketCardsOmaha : IPocketCards, IEquatable<PocketCardsOmaha>
{
	[Tag(1)]
	public InlineList<Card> Cards { get; private set; }

	public PocketCardsOmaha(InlineList<Card> cards)
	{
		if (cards.Count < 4)
		{
			throw new InvalidOperationException($"Omaha pocket cards must contain 4 cards but was {cards.Count}");
		}
		Cards = cards.OrderByRankThenBySuit();
	}

	[BinaryDeserializationConstructor]
	public static PocketCardsOmaha Create(InlineList<Card> cards)
	{
		if (cards.Count < 4)
		{
			throw new InvalidOperationException($"Omaha pocket cards must contain 4 cards but was {cards.Count}");
		}
		PocketCardsOmaha pocketCardsOmaha = ObjectPool<PocketCardsOmaha>.ThreadShared.RentObject();
		pocketCardsOmaha.Cards = cards.OrderByRankThenBySuit();
		return pocketCardsOmaha;
	}

	public PocketCardsOmaha()
	{
		Cards = default(InlineList<Card>);
	}

	public override string ToString()
	{
		return Cards.OrderBy((Card card) => (int)card.Rank).GetAbbreviation();
	}

	public bool Equals(PocketCardsOmaha? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return Cards.Ordered<Card>().SequenceEqual(other.Cards.Ordered<Card>());
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((PocketCardsOmaha)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Cards);
	}
}
