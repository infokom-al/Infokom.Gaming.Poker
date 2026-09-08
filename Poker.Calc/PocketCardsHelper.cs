namespace Poker.Calc;

public static class PocketCardsHelper
{
	public const byte UnknownPreflopCellIndex = byte.MaxValue;

	public static bool IsDead(this PocketCardsHoldem pocketCards, Span<Card> deadCards)
	{
		InlineList<Card>.Enumerator enumerator = pocketCards.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.IsDead(deadCards))
			{
				return true;
			}
		}
		return false;
	}

	public static PocketCardsHoldem ParsePocketCardsHoldem(this string cards)
	{
		return cards.ParseCards().ToPocketCardsHoldem();
	}

	public static PocketCardsOmaha ParsePocketCardsOmaha(this string cards)
	{
		return cards.ParseCards().ToPocketCardsOmaha();
	}

	public static long ToCardsMask(this IPocketCards cards)
	{
		return cards.Cards.ToCardsMask();
	}

	public static SeatMap<long> MapToCardMasks(this SeatMap<IPocketCards> pockets)
	{
		return pockets.Map((IPocketCards cards) => cards.ToCardsMask());
	}

	public static InlineList<long> ToCardsMasksArray(this in InlineList<IPocketCards> pockets)
	{
		return pockets.Map((IPocketCards cards) => cards.ToCardsMask());
	}

	public static InlineList<long> ToCardsMasksArray(this in Span<IPocketCards> pockets)
	{
		return pockets.MapToInlineList((IPocketCards cards) => cards.ToCardsMask());
	}

	public static InlineList<long> ToCardsMasksArray(this string[] pockets)
	{
		return pockets.Select((string x) => x.ParseCards().ToPocketCards()).ToArray().ToInlineList()
			.ToCardsMasksArray();
	}

	public static InlineList<int> ToCardsMaskCactus(this IPocketCards cards)
	{
		return cards.Cards.ToCardsMasksCactus();
	}

	public static SeatMap<InlineList<int>> MapToCardMaskCactus(this SeatMap<IPocketCards> pockets)
	{
		return SeatMapHelper.Map(in pockets, ToCardsMaskCactus);
	}

	public static PocketCardsOmaha ToPocketCardsOmaha(this InlineList<Card> cards)
	{
		return new PocketCardsOmaha(cards);
	}

	public static PocketCardsHoldem ToPocketCardsHoldem(this InlineList<Card> cards)
	{
		InlineListHelper.VerifyCollectionSize(in cards, 2);
		return PocketCardsHoldem.Create(cards[0], cards[1]);
	}

	public static IPocketCards ToPocketCards(this InlineList<Card> cards)
	{
		if (cards.Count != 2)
		{
			return cards.ToPocketCardsOmaha();
		}
		return cards.ToPocketCardsHoldem();
	}

	public static int GetPreflopCellNumber(this IPocketCards cards, PokerGames game)
	{
		return cards.GetPreflopCellIndex(game) + 1;
	}

	public static int GetPreflopCellIndex(this IPocketCards cards, PokerGames game)
	{
		return ((PocketCardsHoldem)cards).GetPreflopCellIndex();
	}

	private static int GetPreflopCellIndex(this PocketCardsHoldem cards)
	{
		if (cards.IsPair)
		{
			int num = (int)(12 - cards.HighCard.Rank);
			return 13 * num + num;
		}
		if (!cards.IsSuited)
		{
			return (int)(13 * (int)(12 - cards.LowCard.Rank) + 12 - cards.HighCard.Rank);
		}
		return (int)(13 * (int)(12 - cards.HighCard.Rank) + 12 - cards.LowCard.Rank);
	}

	public static bool IsShortDeck(this PocketCardsHoldem pocketCardsHoldem)
	{
		if (pocketCardsHoldem.HighCard.Rank >= CardRanks.Six)
		{
			return pocketCardsHoldem.LowCard.Rank >= CardRanks.Six;
		}
		return false;
	}
}
