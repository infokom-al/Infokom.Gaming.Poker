namespace Poker.Calc;

internal static class ShortDeckTbs
{
	private enum HandTypes
	{
		NoPair,
		Pair,
		TwoPairs,
		Straight,
		Trips,
		FullHouse,
		Flush,
		Quads,
		StraightFlush
	}

	private const int PairHandRank = 16777216;

	private const int TwoPairsHandRank = 33554432;

	private const int TripsHandRank = 67108864;

	private const int StraightHandRank = 50331648;

	private const int FullHouseHandRank = 83886080;

	private const int QuadsHandRank = 117440512;

	internal static PokerHands HandRankToPokerHand(this int handRank)
	{
		int num = handRank >> 24;
		return num switch
		{
			3 => PokerHands.Straight,
			4 => PokerHands.ThreeOfAKind,
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
		return 67108864 + (tripsRank << 16) + (kickersRow >> 12 << 8);
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
		int num2 = cardsCount - num;
		int num3 = 0;
		if (num >= 5)
		{
			int flushRanksMaskSevenCards = cards.GetFlushRanksMaskSevenCards();
			if (flushRanksMaskSevenCards != 0)
			{
				return Tables.FlushesShortDeck[flushRanksMaskSevenCards];
			}
			num3 = Tables.StraightsShortDeckTbs[ranks];
			if (num3 != 0 && num2 < 2)
			{
				return num3;
			}
		}
		switch (num2)
		{
			case 0:
				return Tables.NoPairTable[ranks];
			case 1:
			{
				int num5 = ranks ^ (cards.Hearts ^ cards.Diamonds ^ cards.Clubs ^ cards.Spades);
				return PairRank(num5.GetTopRank(), (ranks ^ num5).GetTopFiveRanks());
			}
			case 2:
			{
				int tripsRankMask2 = cards.GetTripsRankMask();
				if (tripsRankMask2 != 0)
				{
					return TripsRank(tripsRankMask2.GetTopRank(), (ranks ^ tripsRankMask2).GetTopFiveRanks());
				}
				if (num3 != 0)
				{
					return num3;
				}
				int num4 = ranks ^ (cards.Spades ^ cards.Diamonds ^ cards.Hearts ^ cards.Clubs);
				return TwoPairsRank(num4.GetTopFiveRanks(), (ranks ^ num4).GetTopRank());
			}
			default:
			{
				if (num3 != 0)
				{
					return num3;
				}
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
