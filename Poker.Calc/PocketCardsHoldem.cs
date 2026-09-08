using BinarySerializer;

using MemoryPools;

namespace Poker.Calc;

[BinarySerializable]
public class PocketCardsHoldem : IPocketCards, IEquatable<PocketCardsHoldem>
{
	[Tag(1)]
	public Card HighCard { get; private set; }

	[Tag(2)]
	public Card LowCard { get; private set; }

	public bool IsPair { get; private set; }

	public bool IsSuited { get; private set; }

	public Suitness Suitness { get; private set; }

	public InlineList<Card> Cards { get; private set; }

	public bool IsOffsuited => !IsSuited;

	public string AbbreviationAhJs => HighCard.Abbreviation + LowCard.Abbreviation;

	public PocketCardsHoldem(Card card1, Card card2)
	{
		Fill(card1, card2);
	}

	[BinaryDeserializationConstructor]
	public static PocketCardsHoldem FromOrderedCards(Card highCard, Card lowCard)
	{
		return Create(highCard, lowCard);
	}

	public static PocketCardsHoldem Create(Card card1, Card card2)
	{
		PocketCardsHoldem pocketCardsHoldem = ObjectPool<PocketCardsHoldem>.ThreadShared.RentObject();
		pocketCardsHoldem.Fill(card1, card2);
		return pocketCardsHoldem;
	}

	public PocketCardsHoldem()
	{
	}

	private PocketCardsHoldem Fill(Card cardOne, Card cardTwo)
	{
		if (cardOne.Equals(cardTwo))
		{
			throw new ArgumentException($"Pocket cards must contain distinct cards but was {cardOne}{cardTwo}");
		}
		IsPair = cardOne.Rank == cardTwo.Rank;
		if (cardOne.Compare(cardTwo) > 0)
		{
			Card highCard = cardOne;
			Card lowCard = cardTwo;
			HighCard = highCard;
			LowCard = lowCard;
		}
		else
		{
			Card lowCard = cardTwo;
			Card highCard = cardOne;
			HighCard = lowCard;
			LowCard = highCard;
		}
		IsSuited = HighCard.Suit == LowCard.Suit;
		Suitness = (IsSuited ? Suitness.Suited : Suitness.Offsuited);
		InlineList<Card> cards = default(InlineList<Card>);
		cards.Add(HighCard);
		cards.Add(LowCard);
		Cards = cards;
		return this;
	}

	public override string ToString()
	{
		return AbbreviationAhJs;
	}

	public bool Equals(PocketCardsHoldem other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (HighCard.Equals(other.HighCard))
		{
			return LowCard.Equals(other.LowCard);
		}
		return false;
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
		return Equals((PocketCardsHoldem)obj);
	}

	public override int GetHashCode()
	{
		return HighCard.GetHashCode() ^ LowCard.GetHashCode();
	}
}
