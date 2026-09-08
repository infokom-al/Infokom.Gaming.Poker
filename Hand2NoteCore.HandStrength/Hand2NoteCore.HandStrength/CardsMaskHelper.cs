using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

internal static class CardsMaskHelper
{
	internal static int ToRanksMask(this CardsMaskU cards)
	{
		return cards.Spades | cards.Clubs | cards.Diamonds | cards.Hearts;
	}

	internal static CardRanks GetTopRank(this CardsMaskU cards)
	{
		return cards.ToRanksMask().GetTopCardRank();
	}

	internal static int GetCardsCount(this CardsMaskU cards)
	{
		return cards.Spades.GetRanksCount() + cards.Clubs.GetRanksCount() + cards.Diamonds.GetRanksCount() + cards.Hearts.GetRanksCount();
	}

	internal static CardRanks GetTopCardRank(this int ranksMask)
	{
		return (CardRanks)Poker.Calc.Tables.TopCardRankTable[ranksMask];
	}

	internal static int GetRanksCount(this int ranksMask)
	{
		return Poker.Calc.Tables.BitsCountTable[ranksMask];
	}

	internal static int GetRanksCount(this ushort ranksMask)
	{
		return Poker.Calc.Tables.BitsCountTable[ranksMask];
	}

	internal static bool IsPair(this CardsMaskU cards)
	{
		if (cards.ToRanksMask().GetRanksCount() == 1)
		{
			return cards.GetCardsCount() == 2;
		}
		return false;
	}

	internal static bool HasPair(this CardsMaskU cards)
	{
		return cards.ToRanksMask().GetRanksCount() < cards.GetCardsCount();
	}

	internal static bool IsTrips(this CardsMaskU cards)
	{
		if (cards.ToRanksMask().GetRanksCount() == 1)
		{
			return cards.GetCardsCount() == 3;
		}
		return false;
	}

	internal static bool HasTrips(this CardsMaskU cards)
	{
		return ((cards.Spades & cards.Clubs & cards.Diamonds) | (cards.Spades & cards.Clubs & cards.Hearts) | (cards.Spades & cards.Diamonds & cards.Hearts) | (cards.Clubs & cards.Diamonds & cards.Hearts)) != 0;
	}

	internal static bool IsSuited(this CardsMaskU cards, out Suits suit)
	{
		if (cards.Spades > 0)
		{
			suit = Suits.Spades;
			return cards.Spades == cards.CardsMask;
		}
		if (cards.Clubs > 0)
		{
			suit = Suits.Clubs;
			return cards.Clubs == cards.CardsMask >> 16;
		}
		if (cards.Diamonds > 0)
		{
			suit = Suits.Diamonds;
			return cards.Diamonds == cards.CardsMask >> 32;
		}
		if (cards.Hearts > 0)
		{
			suit = Suits.Hearts;
			return cards.Hearts == cards.CardsMask >> 48;
		}
		suit = Suits.Hearts;
		return false;
	}

	internal static bool IsSuited(this CardsMaskU cards)
	{
		if (cards.Spades > 0)
		{
			return cards.Spades == cards.CardsMask;
		}
		if (cards.Clubs > 0)
		{
			return cards.Clubs == cards.CardsMask >> 16;
		}
		if (cards.Diamonds > 0)
		{
			return cards.Diamonds == cards.CardsMask >> 32;
		}
		if (cards.Hearts > 0)
		{
			return cards.Hearts == cards.CardsMask >> 48;
		}
		return false;
	}

	internal static int GetSuitRanksMask(this CardsMaskU cards, Suits suit)
	{
		return suit switch
		{
			Suits.Spades => cards.Spades, 
			Suits.Clubs => cards.Clubs, 
			Suits.Hearts => cards.Hearts, 
			_ => cards.Diamonds, 
		};
	}

	internal static int GetCountOfSuit(this CardsMaskU cards, Suits suit)
	{
		return cards.GetSuitRanksMask(suit).GetRanksCount();
	}

	internal static bool IsSingleRank(this int ranksMask)
	{
		return ranksMask.GetRanksCount() == 1;
	}

	internal static bool HasAce(this int ranksMask)
	{
		return (ranksMask & 0x1000) != 0;
	}

	internal static bool ContainsCardRank(this int ranksMask, CardRanks cardRank)
	{
		return (ranksMask & (1 << (int)cardRank)) != 0;
	}

	internal static int GetStraightDrawOuts(this int ranksMask, PokerGames game)
	{
		if (!game.IsShortDeckFamily())
		{
			return Tables.StraightDrawOuts[ranksMask];
		}
		return Tables.StraightDrawOuts_ShortDeck[ranksMask];
	}

	internal static int ToRankMask(this CardRanks cardRank)
	{
		return 1 << (int)cardRank;
	}

	internal static CardRanks GetStraightRank(this int ranks, PokerGames game)
	{
		return game switch
		{
			PokerGames.ShortDeck => (CardRanks)Poker.Calc.Tables.StraightsShortDeck[ranks], 
			PokerGames.ShortDeckTbs => (CardRanks)Poker.Calc.Tables.StraightsShortDeckTbs[ranks], 
			_ => (CardRanks)Poker.Calc.Tables.StraightsTexasHoldem[ranks], 
		};
	}

	internal static CardRanks GetStraightRankTexasHoldem(this int ranks)
	{
		return (CardRanks)Poker.Calc.Tables.StraightsTexasHoldem[ranks];
	}

	internal static bool ContainsSuitedCardsCountAtLeast(this CardsMaskU cards, int count)
	{
		if (cards.Spades.GetRanksCount() < count && cards.Clubs.GetRanksCount() < count && cards.Hearts.GetRanksCount() < count)
		{
			return cards.Diamonds.GetRanksCount() >= count;
		}
		return true;
	}

	internal static bool ContainsSuitedCardsCountExactly(this CardsMaskU cards, int count)
	{
		if (cards.Spades.GetRanksCount() != count && cards.Clubs.GetRanksCount() != count && cards.Hearts.GetRanksCount() != count)
		{
			return cards.Diamonds.GetRanksCount() == count;
		}
		return true;
	}

	internal static int GetRankCount(this CardsMaskU cards, CardRanks rank)
	{
		int num = 0;
		int num2 = 1 << (int)rank;
		if ((cards.Spades & num2) != 0)
		{
			num++;
		}
		if ((cards.Clubs & num2) != 0)
		{
			num++;
		}
		if ((cards.Hearts & num2) != 0)
		{
			num++;
		}
		if ((cards.Diamonds & num2) != 0)
		{
			num++;
		}
		return num;
	}

	internal static bool IsRainbow(this CardsMaskU cards)
	{
		if (cards.Hearts.GetRanksCount() <= 1 && cards.Clubs.GetRanksCount() <= 1 && cards.Diamonds.GetRanksCount() <= 1)
		{
			return cards.Spades.GetRanksCount() <= 1;
		}
		return false;
	}

	internal static Suits GetCardSuit(this CardsMaskU card)
	{
		if (card.Spades > 0)
		{
			return Suits.Spades;
		}
		if (card.Hearts > 0)
		{
			return Suits.Hearts;
		}
		if (card.Clubs > 0)
		{
			return Suits.Clubs;
		}
		return Suits.Diamonds;
	}

	internal static CardsMaskU ToCardsMaskU(this InlineList<Card> cards)
	{
		return cards.ToCardsMask().ToCardsMaskU();
	}

	internal static CardsMaskU ToCardsMaskU(this long mask)
	{
		return new CardsMaskU
		{
			CardsMask = mask
		};
	}

	internal static long ToCardsMask(this InlineList<Card> cards)
	{
		long num = 0L;
		for (int i = 0; i < cards.Count; i++)
		{
			num |= cards[i].ToCardMask();
		}
		return num;
	}

	internal static CardsMaskU ToCardMaskU(this Card card)
	{
		return ToCardMask(card.Suit, card.Rank).ToCardsMaskU();
	}

	internal static long ToCardMask(this Card card)
	{
		return ToCardMask(card.Suit, card.Rank);
	}

	internal static long ToCardMask(Suits suit, CardRanks rank)
	{
		return Poker.Calc.Tables.CardsMasksPeval[(int)(rank + (int)suit * 13)];
	}
}
