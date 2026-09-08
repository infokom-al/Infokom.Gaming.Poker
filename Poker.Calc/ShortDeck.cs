namespace Poker.Calc;

internal static class ShortDeck
{
	private enum HandTypes
	{
		NoPair,
		Pair,
		TwoPairs,
		Trips,
		Straight,
		FullHouse,
		Flush,
		Quads,
		StraightFlush
	}

	public const int TotalComboCount = 630;

	internal const int PairHandRank = 16777216;

	internal const int TwoPairsHandRank = 33554432;

	internal const int TripsHandRank = 50331648;

	internal const int StraightHandRank = 67108864;

	internal const int FlushHandRank = 100663296;

	internal const int FullHouseHandRank = 83886080;

	internal const int QuadsHandRank = 117440512;

	internal const int StraightFlushHandRank = 134217728;

	internal static PokerHands HandRankToPokerHand(this int handRank)
	{
		int num = handRank >> 24;
		return num switch
		{
			5 => PokerHands.FullHouse,
			6 => PokerHands.Flush,
			_ => (PokerHands)num,
		};
	}

	private static int PairRank(int pairRank, int kickersRow)
	{
		return 16777216 + (pairRank << 16) + (kickersRow >> 8 << 4);
	}

	private static int TripsRank(int tripsRank, int kickersRow)
	{
		return 50331648 + (tripsRank << 16) + (kickersRow >> 12 << 8);
	}

	private static int TwoPairsRank(int pairsRow, int kickerRank)
	{
		return 33554432 + (pairsRow >> 12 << 12) + (kickerRank << 8);
	}

	private static int FullHouseRank(int tripsRank, int pairRank)
	{
		return 83886080 + (tripsRank << 16) + (pairRank << 12);
	}

	private static int QuadsRank(int quadsRank, int kickerRank)
	{
		return 117440512 + (quadsRank << 16) + (kickerRank << 12);
	}

	internal static int EvaluateCards(this CardsMaskU cards, int ranks, int cardsCount)
	{
		int num = ranks.BitsCount();
		if (num >= 5)
		{
			int flushRanksMaskSevenCards = cards.GetFlushRanksMaskSevenCards();
			if (flushRanksMaskSevenCards != 0)
			{
				return Tables.FlushesShortDeck[flushRanksMaskSevenCards];
			}
			int num2 = Tables.StraightsShortDeck[ranks];
			if (num2 != 0)
			{
				return num2;
			}
		}
		switch (cardsCount - num)
		{
			case 0:
				return Tables.NoPairTable[ranks];
			case 1:
			{
				int num4 = ranks ^ (cards.Hearts ^ cards.Diamonds ^ cards.Clubs ^ cards.Spades);
				return PairRank(num4.GetTopRank(), (ranks ^ num4).GetTopFiveRanks());
			}
			case 2:
			{
				int tripsRankMask2 = cards.GetTripsRankMask();
				if (tripsRankMask2 != 0)
				{
					return TripsRank(tripsRankMask2.GetTopRank(), (ranks ^ tripsRankMask2).GetTopFiveRanks());
				}
				int num3 = ranks ^ (cards.Spades ^ cards.Diamonds ^ cards.Hearts ^ cards.Clubs);
				return TwoPairsRank(num3.GetTopFiveRanks(), (ranks ^ num3).GetTopRank());
			}
			default:
			{
				int topRankMask = cards.GetQuadsRankMask().GetTopRankMask();
				if (topRankMask != 0)
				{
					return QuadsRank(topRankMask.GetTopRank(), (ranks ^ topRankMask).GetTopRank());
				}
				int tripsRankMask = cards.GetTripsRankMask();
				if (tripsRankMask != 0)
				{
					int topRankMask2 = tripsRankMask.GetTopRankMask();
					int ranksMask = ranks ^ topRankMask2 ^ cards.GetUniqueRanksMask();
					return FullHouseRank(topRankMask2.GetTopRank(), ranksMask.GetTopRank());
				}
				int uniqueRanksMask = cards.GetUniqueRanksMask();
				int ranksMask2 = ranks ^ uniqueRanksMask;
				ranksMask2 = ranksMask2.GetTopTwoRanksMask();
				return TwoPairsRank(ranksMask2.GetTopFiveRanks(), (ranks ^ ranksMask2).GetTopRank());
			}
		}
	}
}
