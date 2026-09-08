// Poker.Calc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Poker.Calc.TexasHoldem
using Poker.Calc;

internal static class TexasHoldem
{
	private enum HandTypes
	{
		NoPair,
		Pair,
		TwoPairs,
		Trips,
		Straight,
		Flush,
		FullHouse,
		Quads,
		StraightFlush
	}

	public const int TotalComboCount = 1326;

	internal const int PairHandRank = 16777216;

	internal const int TwoPairsHandRank = 33554432;

	internal const int TripsHandRank = 50331648;

	internal const int StraightHandRank = 67108864;

	internal const int FlushHandRank = 83886080;

	internal const int FullHouseHandRank = 100663296;

	internal const int QuadsHandRank = 117440512;

	internal const int StraightFlushHandRank = 134217728;

	private static readonly short[] _hash_adjust = new short[512]
	{
		0, 5628, 7017, 1298, 2918, 2442, 8070, 6383, 6383, 7425,
		2442, 5628, 8044, 7425, 3155, 6383, 2918, 7452, 1533, 6849,
		5586, 7452, 7452, 1533, 2209, 6029, 2794, 3509, 7992, 7733,
		7452, 131, 6029, 4491, 1814, 7452, 6110, 3155, 7077, 6675,
		532, 1334, 7555, 5325, 3056, 1403, 1403, 3969, 4491, 1403,
		7592, 522, 8070, 1403, 0, 1905, 3584, 2918, 922, 3304,
		6675, 0, 7622, 7017, 3210, 2139, 1403, 5225, 0, 3969,
		7992, 5743, 5499, 5499, 5345, 7452, 522, 305, 3056, 7017,
		7017, 2139, 1338, 3056, 7452, 1403, 6799, 3204, 3290, 4099,
		1814, 2191, 4099, 5743, 1570, 1334, 7363, 1905, 0, 6799,
		4400, 1480, 6029, 1905, 0, 7525, 2028, 2794, 131, 7646,
		3155, 4986, 1858, 2442, 7992, 1607, 3584, 4986, 706, 6029,
		5345, 7622, 6322, 5196, 1905, 6847, 218, 1785, 0, 4099,
		2981, 6849, 4751, 3950, 7733, 3056, 5499, 4055, 6849, 1533,
		131, 5196, 2918, 3879, 5325, 2794, 6029, 0, 0, 322,
		7452, 6178, 2918, 2320, 6675, 3056, 6675, 1533, 6029, 1428,
		2280, 2171, 6788, 7452, 3325, 107, 4262, 311, 5562, 7857,
		6110, 2139, 4942, 4600, 1905, 0, 3083, 5345, 7452, 6675,
		0, 6112, 4099, 7017, 1338, 6799, 2918, 1232, 3584, 522,
		6029, 5325, 1403, 6759, 6849, 508, 6675, 2987, 7745, 6870,
		896, 7452, 1232, 4400, 12, 2981, 3850, 4491, 6849, 0,
		6675, 747, 4491, 7525, 6675, 7452, 7992, 6921, 7323, 6849,
		3056, 1199, 2139, 6029, 6029, 190, 4351, 7891, 4400, 7134,
		1533, 1194, 3950, 6675, 5345, 6383, 7622, 131, 1905, 2883,
		6383, 1533, 5345, 2794, 4303, 1403, 0, 1338, 2794, 992,
		4871, 6383, 4099, 2794, 3889, 6184, 3304, 1905, 6383, 3950,
		3056, 522, 1810, 3975, 7622, 7452, 522, 6799, 5866, 7084,
		7622, 6528, 2798, 7452, 1810, 7907, 642, 5345, 1905, 6849,
		6675, 7745, 2918, 4751, 3229, 2139, 6029, 5207, 6601, 2139,
		7452, 5890, 1428, 5628, 7622, 2139, 3146, 2400, 578, 941,
		7672, 1814, 3210, 1533, 4491, 12, 2918, 1900, 7425, 2794,
		2987, 3465, 1377, 3822, 3969, 3210, 859, 5499, 6878, 1377,
		3056, 4027, 8065, 8065, 5207, 4400, 4303, 3210, 3210, 0,
		6675, 357, 5628, 5512, 1905, 3452, 1403, 7646, 859, 6788,
		3210, 2139, 378, 5663, 7733, 870, 0, 4491, 4813, 2110,
		578, 2139, 3056, 4099, 1905, 1298, 4672, 2191, 3950, 5499,
		3969, 4974, 6323, 6029, 7414, 6383, 0, 4974, 3210, 795,
		4099, 131, 5345, 5345, 6576, 1810, 1621, 4400, 2918, 1905,
		2442, 2679, 6322, 7452, 2110, 1403, 6383, 2653, 5132, 6856,
		7841, 2794, 6110, 2028, 6675, 7425, 6999, 7441, 6029, 183,
		6675, 4400, 859, 1403, 2794, 5985, 5345, 1533, 322, 4400,
		1227, 5890, 4474, 4491, 3574, 8166, 6849, 7086, 5345, 5345,
		5459, 3584, 6675, 3969, 7579, 8044, 2295, 2577, 1480, 5743,
		3304, 5499, 330, 4303, 6863, 3822, 4600, 4751, 5628, 3822,
		2918, 6675, 2400, 6663, 1403, 6849, 6029, 3145, 6110, 3210,
		747, 3229, 3056, 2918, 7733, 330, 4055, 7322, 5628, 2987,
		3056, 1905, 2903, 669, 5325, 2845, 4099, 5225, 6283, 4099,
		5000, 642, 4055, 5345, 8034, 2918, 1041, 5769, 7051, 1538,
		2918, 3366, 608, 4303, 3921, 0, 2918, 1905, 218, 6687,
		5963, 859, 3083, 2987, 896, 5056, 1905, 2918, 4415, 7966,
		7646, 2883, 5628, 7017, 8029, 6528, 4474, 6322, 5562, 6669,
		4610, 7006
	};

	internal static int PairRank(int pairRank, int kickersRow)
	{
		return 16777216 + (pairRank << 16) + (kickersRow >> 8 << 4);
	}

	internal static int TripsRank(int tripsRank, int kickersRow)
	{
		return 50331648 + (tripsRank << 16) + (kickersRow >> 12 << 8);
	}

	internal static int TwoPairsRank(int pairsRow, int kickerRank)
	{
		return 33554432 + (pairsRow >> 12 << 12) + (kickerRank << 8);
	}

	internal static int FullHouseRank(int tripsRank, int pairRank)
	{
		return 100663296 + (tripsRank << 16) + (pairRank << 12);
	}

	internal static int QuadsRank(int quadsRank, int kickerRank)
	{
		return 117440512 + (quadsRank << 16) + (kickerRank << 12);
	}

	internal static int EvaluateSevenCards(this CardsMaskU cards, int ranks, bool isFlushPossible)
	{
		return cards.EvaluateCards(ranks, 7, isFlushPossible);
	}

	internal static int EvaluateSevenCards(this CardsMaskU cards, int ranks)
	{
		return EvaluateCards(cards, ranks, 7);
	}



	internal static int EvaluateCards(this ulong cardsMask, uint ranksMask, int cardsCount)
	{
		return EvaluateCards(cardsMask.CardsMaskToCardsMaskU(), (int)ranksMask, cardsCount);
	}

	internal static int EvaluateCards(this CardsMaskU cards, int ranks, int cardsCount, bool IsFlushPossible)
	{
		int num = ranks.BitsCount();
		if (num >= 5)
		{
			if (IsFlushPossible)
			{
				int flushRanksMaskSevenCards = cards.GetFlushRanksMaskSevenCards();
				if (flushRanksMaskSevenCards != 0)
				{
					return Tables.FlushesTexasHoldem[flushRanksMaskSevenCards];
				}
			}
			int num2 = Tables.StraightsTexasHoldem[ranks];
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

	internal static int EvaluateCards(this CardsMaskU cards, int ranks, int cardsCount)
	{
		int num = ranks.BitsCount();
		if (num >= 5)
		{
			int flushRanksMaskSevenCards = cards.GetFlushRanksMaskSevenCards();
			if (flushRanksMaskSevenCards != 0)
			{
				return Tables.FlushesTexasHoldem[flushRanksMaskSevenCards];
			}
			int num2 = Tables.StraightsTexasHoldem[ranks];
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

	internal static int EvaluateFiveCards(this InlineList<Card> cards)
	{
		return EvaluateHandRank(cards[0].Index.CardIndexToCardMaskCactus(), cards[1].Index.CardIndexToCardMaskCactus(), cards[2].Index.CardIndexToCardMaskCactus(), cards[3].Index.CardIndexToCardMaskCactus(), cards[4].Index.CardIndexToCardMaskCactus());
	}

	public static int EvaluateHandRank(int c1, int c2, int c3, int c4, int c5)
	{
		int num = (c1 | c2 | c3 | c4 | c5) >> 16;
		if ((c1 & c2 & c3 & c4 & c5 & 0xF000) != 0)
		{
			return Tables.FlushesCactus[num];
		}
		int num2 = Tables.Unique5CactusTexasHoldem[num];
		if (num2 != 0)
		{
			return num2;
		}
		uint u = (uint)((c1 & 0xFF) * (c2 & 0xFF) * (c3 & 0xFF) * (c4 & 0xFF) * (c5 & 0xFF));
		return Tables.HashValuesCactusTexasHoldem[GetHash(u)];
		static uint GetHash(uint num3)
		{
			num3 += 3910838837u;
			num3 ^= num3 >> 16;
			num3 += num3 << 8;
			num3 ^= num3 >> 4;
			uint num4 = (num3 >> 8) & 0x1FF;
			return (num3 + (num3 << 2) >> 19) ^ (uint)_hash_adjust[num4];
		}
	}
}
