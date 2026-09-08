namespace Poker.Calc;

public static class HandValueHelper
{
	internal static CardRanks GetTopKickerRank(this int handRank)
	{
		return handRank.GetCardRank(16);
	}

	internal static CardRanks GetSecondKickerRank(this int handRank)
	{
		return handRank.GetCardRank(12);
	}

	internal static CardRanks GetThirdKickerRank(this int handRank)
	{
		return handRank.GetCardRank(8);
	}

	internal static CardRanks GetFourthKickerRank(this int handRank)
	{
		return handRank.GetCardRank(4);
	}

	internal static CardRanks GetFifthKickerRank(this int handRank)
	{
		return (CardRanks)(handRank & 0xF);
	}

	internal static CardRanks GetPairRank(this int handRank)
	{
		return handRank.GetCardRank(16);
	}

	internal static CardRanks GetPairTopKickerRank(this int handRank)
	{
		return handRank.GetCardRank(12);
	}

	internal static CardRanks GetPairSecondKickerRank(this int handRank)
	{
		return handRank.GetCardRank(8);
	}

	internal static CardRanks GetPairThirdKickerRank(this int handRank)
	{
		return handRank.GetCardRank(4);
	}

	internal static CardRanks GetTopPairRank(this int handRank)
	{
		return handRank.GetCardRank(16);
	}

	internal static CardRanks GetSecondPairRank(this int handRank)
	{
		return handRank.GetCardRank(12);
	}

	internal static CardRanks GetTwoPairsKickerRank(this int handRank)
	{
		return handRank.GetCardRank(8);
	}

	internal static CardRanks GetTripsRank(this int handRank)
	{
		return handRank.GetCardRank(16);
	}

	internal static CardRanks GetTripsTopKickerRank(this int handRank)
	{
		return handRank.GetCardRank(12);
	}

	internal static CardRanks GetTripsSecondKickerRank(this int handRank)
	{
		return handRank.GetCardRank(8);
	}

	internal static CardRanks GetStraightRank(this int handRank)
	{
		return handRank.GetCardRank(16);
	}

	internal static CardRanks GetFullHouseTripsRank(this int handRank)
	{
		return handRank.GetCardRank(16);
	}

	internal static CardRanks GetFullHousePairRank(this int handRank)
	{
		return handRank.GetCardRank(12);
	}

	internal static CardRanks GetQuadsRank(this int handRank)
	{
		return handRank.GetCardRank(16);
	}

	internal static CardRanks GetQuadsKickerRank(this int handRank)
	{
		return handRank.GetCardRank(12);
	}

	private static CardRanks GetCardRank(this int handRank, int shift)
	{
		return (CardRanks)((handRank & (15 << shift)) >> shift);
	}

	internal static HandValue ToHandValue(this int handRank, PokerGames game)
	{
		PokerHands pokerHand = handRank.GetPokerHand(game);
		return pokerHand switch
		{
			PokerHands.HighCard => new HandValue(handRank, pokerHand, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, handRank.GetTopKickerRank(), handRank.GetSecondKickerRank(), handRank.GetThirdKickerRank(), handRank.GetFourthKickerRank(), handRank.GetFifthKickerRank()),
			PokerHands.Pair => new HandValue(handRank, pokerHand, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, handRank.GetPairRank(), CardRanks.Deuce, CardRanks.Deuce, handRank.GetPairTopKickerRank(), handRank.GetPairSecondKickerRank(), handRank.GetPairThirdKickerRank()),
			PokerHands.TwoPairs => new HandValue(handRank, pokerHand, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, handRank.GetTopPairRank(), handRank.GetSecondPairRank(), handRank.GetTwoPairsKickerRank()),
			PokerHands.ThreeOfAKind => new HandValue(handRank, pokerHand, CardRanks.Deuce, CardRanks.Deuce, handRank.GetTripsRank(), CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, handRank.GetTripsTopKickerRank(), handRank.GetTripsSecondKickerRank()),
			PokerHands.Straight => new HandValue(handRank, pokerHand, CardRanks.Deuce, handRank.GetStraightRank()),
			PokerHands.Flush => new HandValue(handRank, pokerHand, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, handRank.GetTopKickerRank(), handRank.GetSecondKickerRank(), handRank.GetThirdKickerRank(), handRank.GetFourthKickerRank(), handRank.GetFifthKickerRank()),
			PokerHands.FullHouse => new HandValue(handRank, pokerHand, CardRanks.Deuce, CardRanks.Deuce, handRank.GetFullHouseTripsRank(), handRank.GetFullHousePairRank()),
			PokerHands.Quads => new HandValue(handRank, pokerHand, handRank.GetQuadsRank(), CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, CardRanks.Deuce, handRank.GetQuadsKickerRank()),
			PokerHands.StraightFlush => new HandValue(handRank, pokerHand, CardRanks.Deuce, handRank.GetStraightRank()),
			_ => throw new NotImplementedException(),
		};
	}

	public static long ExtractCardsInHandMask(this HandValue hand, long board, long allCards, int allCardsCount, PokerGames game)
	{
		InlineList<CardsMaskU> inlineList = hand.ExtractCardsInHand(board.ToCardsMaskU(), allCards.ToCardsMaskU(), allCardsCount, game);
		long num = 0L;
		InlineList<CardsMaskU>.Enumerator enumerator = inlineList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num |= enumerator.Current.CardsMask;
		}
		return num;
	}

	internal static InlineList<CardsMaskU> ExtractCardsInHand(this HandValue hand, CardsMaskU board, CardsMaskU allCards, int allCardsCount, PokerGames game)
	{
		InlineList<CardsMaskU> outResult = new InlineList<CardsMaskU>(5);
		bool requiresTwoPocketsInCombo = game.RequiresPocketsInHandCombo();
		switch (hand.Type)
		{
			case PokerHands.HighCard:
				outResult[0] = FindByRank(hand.TopKickerRank);
				outResult[1] = FindByRank(hand.SecondKickerRank);
				outResult[2] = FindByRank(hand.ThirdKickerRank);
				outResult[3] = FindByRank(hand.FourthKickerRank);
				outResult[4] = FindByRank(hand.FifthKickerRank);
				break;
			case PokerHands.Pair:
				FindRanksAndFillResult(hand.PairRank);
				outResult[2] = FindByRank(hand.TopKickerRank);
				outResult[3] = FindByRank(hand.SecondKickerRank);
				outResult[4] = FindByRank(hand.ThirdKickerRank);
				break;
			case PokerHands.TwoPairs:
				FindRanksAndFillResult(hand.TopPairRank);
				FindRanksAndFillResult(hand.SecondPairRank, 2);
				outResult[4] = FindByRank(hand.TopKickerRank);
				break;
			case PokerHands.ThreeOfAKind:
				FindRanksAndFillResult(hand.TripsRank);
				outResult[3] = FindByRank(hand.TopKickerRank);
				outResult[4] = FindByRank(hand.SecondKickerRank);
				break;
			case PokerHands.Straight:
			{
				CardRanks straightRank = hand.StraightRank;
				for (int k = 0; k < 5; k++)
				{
					outResult[k] = FindByRank(straightRank - k);
				}
				break;
			}
			case PokerHands.FullHouse:
				FindRanksAndFillResult(hand.TripsRank);
				FindRanksAndFillResult(hand.PairRank, 3);
				break;
			case PokerHands.Quads:
				FindRanksAndFillResult(hand.QuadsRank);
				outResult[4] = FindByRank(hand.TopKickerRank);
				break;
			case PokerHands.Flush:
			{
				Suits fiveCardsSuit2 = allCards.GetFiveCardsSuit(allCardsCount);
				outResult[0] = CardsMask.ToCardMaskU(hand.TopKickerRank, fiveCardsSuit2);
				outResult[1] = CardsMask.ToCardMaskU(hand.SecondKickerRank, fiveCardsSuit2);
				outResult[2] = CardsMask.ToCardMaskU(hand.ThirdKickerRank, fiveCardsSuit2);
				outResult[3] = CardsMask.ToCardMaskU(hand.FourthKickerRank, fiveCardsSuit2);
				outResult[4] = CardsMask.ToCardMaskU(hand.FifthKickerRank, fiveCardsSuit2);
				break;
			}
			case PokerHands.StraightFlush:
			{
				Suits fiveCardsSuit = allCards.GetFiveCardsSuit(allCardsCount);
				if (hand.StraightRank == CardRanks.Five)
				{
					for (int i = 0; i < 4; i++)
					{
						outResult[i] = CardsMask.ToCardMaskU(hand.StraightRank - i, fiveCardsSuit);
					}
					outResult[4] = CardsMask.ToCardMaskU(CardRanks.Ace, fiveCardsSuit);
				}
				else
				{
					for (int j = 0; j < 5; j++)
					{
						outResult[j] = CardsMask.ToCardMaskU(hand.StraightRank - j, fiveCardsSuit);
					}
				}
				break;
			}
			default:
				throw new NotImplementedException();
		}
		return outResult;
		CardsMaskU FindByRank(CardRanks rank)
		{
			if (game.IsShortDeckFamily() && rank == CardRanks.Five)
			{
				rank = CardRanks.Ace;
			}
			else if (rank == (CardRanks)(-1))
			{
				rank = CardRanks.Ace;
			}
			long num = 0L;
			for (int l = 0; l < 4; l++)
			{
				long num2 = (rank: rank, suit: (Suits)l).ToCardMaskPeval();
				if ((board.CardsMask & num2) != 0L)
				{
					return num2.ToCardsMaskU();
				}
				if ((allCards.CardsMask & num2) != 0L)
				{
					if (requiresTwoPocketsInCombo)
					{
						return num2.ToCardsMaskU();
					}
					num = num2;
				}
			}
			if (num != 0L)
			{
				return num.ToCardsMaskU();
			}
			throw new InvalidOperationException($"{rank} not found");
		}
		void FindRanksAndFillResult(CardRanks rank, int index = 0)
		{
			int num = 0;
			for (int l = 0; l < 4; l++)
			{
				long num2 = (rank: rank, suit: (Suits)l).ToCardMaskPeval();
				if ((allCards.CardsMask & num2) != 0L)
				{
					outResult[index + num] = num2.ToCardsMaskU();
					num++;
					if (index + num >= outResult.Count)
					{
						break;
					}
				}
			}
		}
	}

	internal static PokerHands GetPokerHand(this int handRank, PokerGames game)
	{
		if (!game.IsShortDeckFamily())
		{
			return (PokerHands)(handRank >> 24);
		}
		return game switch
		{
			PokerGames.ShortDeck => ShortDeck.HandRankToPokerHand(handRank),
			PokerGames.ShortDeckTbs => ShortDeckTbs.HandRankToPokerHand(handRank),
			_ => throw new NotImplementedException(),
		};
	}

	public static bool IsLessThanFlush(this HandValue handValue, PokerGames pokerGame)
	{
		return handValue.Type.IsLessThan(PokerHands.Flush, pokerGame);
	}

	public static bool IsLessThan(this PokerHands pokerHand, PokerHands other, PokerGames pokerGame)
	{
		return pokerGame switch
		{
			PokerGames.ShortDeck => pokerHand.GetShortDeckHandRank() < other.GetShortDeckHandRank(),
			PokerGames.ShortDeckTbs => pokerHand.GetShortDeckTripsBeatsStraightRank() < other.GetShortDeckTripsBeatsStraightRank(),
			_ => pokerHand < other,
		};
	}

	public static bool IsFlushOrHigherInTexasHoldem(this PokerHands hand)
	{
		return hand >= PokerHands.Flush;
	}

	public static bool IsStraightOrHigherTexasHoldem(this PokerHands hand)
	{
		return hand >= PokerHands.Straight;
	}

	internal static int GetShortDeckHandRank(this PokerHands pokerHand)
	{
		return pokerHand switch
		{
			PokerHands.Flush => 6,
			PokerHands.FullHouse => 5,
			PokerHands.Straight => 4,
			PokerHands.ThreeOfAKind => 3,
			_ => (int)pokerHand,
		};
	}

	internal static int GetShortDeckTripsBeatsStraightRank(this PokerHands pokerHand)
	{
		return pokerHand switch
		{
			PokerHands.Flush => 6,
			PokerHands.FullHouse => 5,
			PokerHands.Straight => 3,
			PokerHands.ThreeOfAKind => 4,
			_ => (int)pokerHand,
		};
	}
}
