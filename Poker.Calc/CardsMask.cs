namespace Poker.Calc;

public static class CardsMask
{
	internal static int Spades(this long cardsMask)
	{
		return (int)(cardsMask & 0xFFFF);
	}

	internal static int Clubs(this long cardsMask)
	{
		return (int)((cardsMask & 0xFFFF0000u) >> 16);
	}

	internal static int Diamonds(this long cardsMask)
	{
		return (int)((cardsMask & 0xFFFF00000000L) >> 32);
	}

	internal static int Hearts(this long cardsMask)
	{
		return (int)(cardsMask >> 48);
	}

	internal static int NotSpades(this CardsMaskU cards)
	{
		return cards.Clubs | cards.Diamonds | cards.Hearts;
	}

	internal static int NotClubs(this CardsMaskU cards)
	{
		return cards.Spades | cards.Diamonds | cards.Hearts;
	}

	internal static int NotDiamonds(this CardsMaskU cards)
	{
		return cards.Clubs | cards.Spades | cards.Hearts;
	}

	internal static int NotHearts(this CardsMaskU cards)
	{
		return cards.Clubs | cards.Diamonds | cards.Spades;
	}

	internal static CardsMaskU CardsMaskToCardsMaskU(this ulong cardsMask)
	{
		return new CardsMaskU
		{
			CardsMask = (long)cardsMask
		};
	}

	internal static CardsMaskU CardsMaskToCardsMaskU(this long cardsMask)
	{
		return new CardsMaskU
		{
			CardsMask = cardsMask
		};
	}

	internal static int RanksMask(this CardsMaskU cards)
	{
		return cards.Spades | cards.Clubs | cards.Diamonds | cards.Hearts;
	}

	internal static int CardsMaskToRanksMask(this long cardsMask)
	{
		return cardsMask.Spades() | cardsMask.Clubs() | cardsMask.Diamonds() | cardsMask.Hearts();
	}

	internal static int GetTripsRankMask(this CardsMaskU cards)
	{
		return ((cards.Clubs & cards.Diamonds) | (cards.Hearts & cards.Spades)) & ((cards.Clubs & cards.Hearts) | (cards.Diamonds & cards.Spades));
	}

	internal static int GetQuadsRankMask(this CardsMaskU cards)
	{
		return cards.Spades & cards.Clubs & cards.Diamonds & cards.Hearts;
	}

	internal static long CardIndexToCardMaskPeval(this int index)
	{
		return Tables.CardsMasksPeval[index];
	}

	internal static IEnumerable<int> GetCardIndexesFromCardMaskPeval(this long cards)
	{
		for (int index = 0; index < 52; index++)
		{
			if ((cards & Tables.CardsMasksPeval[index]) != 0L)
			{
				yield return index;
			}
		}
	}

	public static IEnumerable<Card> GetCards(this long cardsMask)
	{
		return cardsMask.GetCardIndexesFromCardMaskPeval().MapToList((int x) => x.CardIndexToCard());
	}

	internal static long ToCardMaskPeval(this (CardRanks rank, Suits suit) x)
	{
		return Tables.CardsMasksPeval[(int)(x.rank + (int)x.suit * 13)];
	}

	internal static CardsMaskU ToCardMaskU(CardRanks rank, Suits suit)
	{
		return new CardsMaskU
		{
			CardsMask = Tables.CardsMasksPeval[(int)(rank + (int)suit * 13)]
		};
	}

	internal static CardsMaskU ToCardsMaskU(this long cards)
	{
		return new CardsMaskU
		{
			CardsMask = cards
		};
	}

	internal static bool HasMinSuitedCardsCount(this long cardsMask, int minCount)
	{
		if (cardsMask.Spades().BitsCount() < minCount && cardsMask.Clubs().BitsCount() < minCount && cardsMask.Hearts().BitsCount() < minCount)
		{
			return cardsMask.Diamonds().BitsCount() >= minCount;
		}
		return true;
	}

	internal static int GetSuitsCount(this long cardsMask, Suits suits)
	{
		return suits switch
		{
			Suits.Hearts => cardsMask.Hearts().BitsCount(),
			Suits.Spades => cardsMask.Spades().BitsCount(),
			Suits.Clubs => cardsMask.Clubs().BitsCount(),
			Suits.Diamonds => cardsMask.Diamonds().BitsCount(),
			_ => throw new NotImplementedException(),
		};
	}

	internal static bool GetSuitsWithCount(this long cardsMask, int minCount, out Suits suits)
	{
		if (cardsMask.Spades().BitsCount() >= minCount)
		{
			suits = Suits.Spades;
			return true;
		}
		if (cardsMask.Clubs().BitsCount() >= minCount)
		{
			suits = Suits.Clubs;
			return true;
		}
		if (cardsMask.Hearts().BitsCount() >= minCount)
		{
			suits = Suits.Hearts;
			return true;
		}
		if (cardsMask.Diamonds().BitsCount() >= minCount)
		{
			suits = Suits.Diamonds;
			return true;
		}
		suits = Suits.Hearts;
		return false;
	}

	internal static int GetFlushRanksMaskSevenCards(this CardsMaskU cards)
	{
		if (cards.Spades.BitsCount() >= 5)
		{
			return cards.Spades;
		}
		if (cards.Clubs.BitsCount() >= 5)
		{
			return cards.Clubs;
		}
		if (cards.Hearts.BitsCount() >= 5)
		{
			return cards.Hearts;
		}
		if (cards.Diamonds.BitsCount() >= 5)
		{
			return cards.Diamonds;
		}
		return 0;
	}

	internal static int GetRankCountFromCardsMask(this long cards, int rank)
	{
		int num = 0;
		int num2 = 1 << rank;
		if ((cards.Spades() & num2) != 0)
		{
			num++;
		}
		if ((cards.Clubs() & num2) != 0)
		{
			num++;
		}
		if ((cards.Diamonds() & num2) != 0)
		{
			num++;
		}
		if ((cards.Hearts() & num2) != 0)
		{
			num++;
		}
		return num;
	}

	internal static int GetUniqueRanksMask(this CardsMaskU cards)
	{
		return (cards.Spades & ~cards.NotSpades()) | (cards.Clubs & ~cards.NotClubs()) | (cards.Hearts & ~cards.NotHearts()) | (cards.Diamonds & ~cards.NotDiamonds());
	}

	public static long CardsMasksToSingleMask(this Span<long> cardsMasks)
	{
		long num = cardsMasks[0];
		for (int i = 1; i < cardsMasks.Length; i++)
		{
			num |= cardsMasks[i];
		}
		return num;
	}

	internal static bool TryFindFiveCardsSuit(this CardsMaskU cards, int cardsCount, out Suits resultSuit, out long suitedCards)
	{
		suitedCards = cards.Spades;
		resultSuit = Suits.Spades;
		int num = Tables.BitsCountTable[suitedCards];
		if (num >= 5)
		{
			return true;
		}
		suitedCards = cards.Clubs;
		int num2 = Tables.BitsCountTable[suitedCards];
		if (num2 >= 5)
		{
			resultSuit = Suits.Clubs;
			return true;
		}
		num += num2;
		if (num > cardsCount - 5)
		{
			return false;
		}
		suitedCards = cards.Diamonds;
		num2 = Tables.BitsCountTable[suitedCards];
		if (num2 >= 5)
		{
			resultSuit = Suits.Diamonds;
			return true;
		}
		num += num2;
		if (num > cardsCount - 5)
		{
			return false;
		}
		suitedCards = cards.Hearts;
		resultSuit = Suits.Hearts;
		return true;
	}

	internal static Suits GetFiveCardsSuit(this CardsMaskU cards, int cardsCount)
	{
		int num = Tables.BitsCountTable[cards.Spades];
		if (num >= 5)
		{
			return Suits.Spades;
		}
		int num2 = Tables.BitsCountTable[cards.Clubs];
		if (num2 >= 5)
		{
			return Suits.Clubs;
		}
		num += num2;
		if (num > cardsCount - 5)
		{
			throw new InvalidOperationException("No five suited cards found");
		}
		num2 = Tables.BitsCountTable[cards.Diamonds];
		if (num2 >= 5)
		{
			return Suits.Diamonds;
		}
		num += num2;
		if (num > cardsCount - 5)
		{
			throw new InvalidOperationException("No five suited cards found");
		}
		return Suits.Hearts;
	}
}
