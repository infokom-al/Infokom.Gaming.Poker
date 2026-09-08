using System;
using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

public static class RelevantHandValueHelper
{
	private const CardRanks MiddlePairMinRankHoldem = CardRanks.Six;

	private const CardRanks MiddlePairMinRankShortDeck = CardRanks.Nine;

	public static int CompareByTopCards(this RelevantFlopType flop, RelevantFlopType other)
	{
		RelevantFlopType highCard = flop.GetHighCard();
		RelevantFlopType highCard2 = other.GetHighCard();
		if (highCard != highCard2)
		{
			return highCard.CompareTo(highCard2);
		}
		RelevantFlopType secondCard = flop.GetSecondCard();
		RelevantFlopType secondCard2 = other.GetSecondCard();
		if (secondCard != secondCard2)
		{
			return secondCard.CompareTo(secondCard2);
		}
		RelevantFlopType thirdCard = flop.GetThirdCard();
		RelevantFlopType thirdCard2 = other.GetThirdCard();
		return thirdCard.CompareTo(thirdCard2);
	}

	public static int GetNumberToCompare(this RelevantFlopType flop)
	{
		int num = GetFirstBitSet((long)flop.GetHighCard(), 0, 13);
		int num2 = GetFirstBitSet((long)flop.GetSecondCard(), 13, 12);
		int num3 = GetFirstBitSet((long)flop.GetThirdCard(), 25, 11);
		return num * 10000 + num2 * 100 + num3;
		static int GetFirstBitSet(long card, int start, int maxBits)
		{
			for (int num4 = maxBits; num4 > 0; num4--)
			{
				if ((card & (1L << start + maxBits - num4)) != 0L)
				{
					return num4;
				}
			}
			return 0;
		}
	}

	public static RelevantFlopType GetHighCard(this RelevantFlopType flop)
	{
		return flop & (RelevantFlopType.HighCardFiveOrLess | RelevantFlopType.HighCardA | RelevantFlopType.HighCardK | RelevantFlopType.HighCardQ | RelevantFlopType.HighCardJ | RelevantFlopType.HighCardT | RelevantFlopType.HighCard9 | RelevantFlopType.HighCard8 | RelevantFlopType.HighCard7 | RelevantFlopType.HighCard6);
	}

	public static RelevantFlopType GetSecondCard(this RelevantFlopType flop)
	{
		return flop & (RelevantFlopType.SecondCardFiveOrLess | RelevantFlopType.SecondCardK | RelevantFlopType.SecondCardQ | RelevantFlopType.SecondCardJ | RelevantFlopType.SecondCardT | RelevantFlopType.SecondCard9 | RelevantFlopType.SecondCard8 | RelevantFlopType.SecondCard7 | RelevantFlopType.SecondCard6);
	}

	public static RelevantFlopType GetThirdCard(this RelevantFlopType flop)
	{
		return flop & (RelevantFlopType.ThirdCardFiveOrLess | RelevantFlopType.ThirdCardQ | RelevantFlopType.ThirdCardJ | RelevantFlopType.ThirdCardT | RelevantFlopType.ThirdCard9 | RelevantFlopType.ThirdCard8 | RelevantFlopType.ThirdCard7 | RelevantFlopType.ThirdCard6);
	}

	public static CardRanks GetTurnCardRank(this RelevantTurnType turn)
	{
		return (CardRanks)(GetFirstBitSet((long)(turn & (RelevantTurnType.TurnCardFiveOrLess | RelevantTurnType.TurnCardA | RelevantTurnType.TurnCardK | RelevantTurnType.TurnCardQ | RelevantTurnType.TurnCardJ | RelevantTurnType.TurnCardT | RelevantTurnType.TurnCard9 | RelevantTurnType.TurnCard8 | RelevantTurnType.TurnCard7 | RelevantTurnType.TurnCard6)), 0, 13) - 1);
		static int GetFirstBitSet(long card, int start, int maxBits)
		{
			for (int num = maxBits; num > 0; num--)
			{
				if ((card & (1L << start + maxBits - num)) != 0L)
				{
					return num;
				}
			}
			return 0;
		}
	}

	public static CardRanks GetRiverCardRank(this RelevantRiverType river)
	{
		return (CardRanks)(GetFirstBitSet((long)(river & (RelevantRiverType.RiverCardFiveOrLess | RelevantRiverType.RiverCardA | RelevantRiverType.RiverCardK | RelevantRiverType.RiverCardQ | RelevantRiverType.RiverCardJ | RelevantRiverType.RiverCardT | RelevantRiverType.RiverCard9 | RelevantRiverType.RiverCard8 | RelevantRiverType.RiverCard7 | RelevantRiverType.RiverCard6)), 0, 13) - 1);
		static int GetFirstBitSet(long card, int start, int maxBits)
		{
			for (int num = maxBits; num > 0; num--)
			{
				if ((card & (1L << start + maxBits - num)) != 0L)
				{
					return num;
				}
			}
			return 0;
		}
	}

	public static int CompareByTopCard(this RelevantTurnType turn, RelevantTurnType other)
	{
		CardRanks turnCardRank = turn.GetTurnCardRank();
		CardRanks turnCardRank2 = other.GetTurnCardRank();
		return turnCardRank.CompareTo(turnCardRank2);
	}

	public static RelevantHandValues GetRelevantHandValueMask(this IPocketCards pockets, Board board, Streets lastStreet, PokerGames game)
	{
		if (lastStreet == Streets.Preflop)
		{
			return RelevantHandValues.None;
		}
		return new RelevantHandValues(pockets.ComputeRelevantHandValue(board.Slice(Streets.Flop), game), (lastStreet >= Streets.Turn) ? pockets.ComputeRelevantHandValue(board.Slice(Streets.Turn), game) : ((RelevantHandValue)0L), (lastStreet == Streets.River) ? pockets.ComputeRelevantHandValue(board, game) : ((RelevantHandValue)0L));
	}

	public static RelevantHandBoards GetRelevantHandBoards(this Board board, PokerGames game)
	{
		if (board.IsPreflop)
		{
			return RelevantHandBoards.Default;
		}
		return new RelevantHandBoards(board.ComputeRelevantFlopType(game), (board.Street >= Streets.Turn) ? board.ComputeRelevantTurnType(game) : ((RelevantTurnType)0uL), (board.Street == Streets.River) ? board.ComputeRelevantRiverType(game) : ((RelevantRiverType)0uL));
	}

	internal static ComputeRelevantHandContextOmaha ToComputeRelevantHandContextOmaha(this IPocketCards pockets, Board board, PokerGames game)
	{
		HandValue hand = pockets.ComputeHandValue(board, game);
		long num = board.Cards.ToCardsMask();
		long num2 = pockets.Cards.ToCardsMask();
		return ToComputeRelevantHandContextOmaha(hand, num2, hand.ExtractCardsInHandMask(num, num2 | num, board.Cards.Length + pockets.Cards.Length, game), num, board.ComputeHandValue(game), pockets, game, board);
	}

	internal static ComputeRelevantHandContextOmaha ToComputeRelevantHandContextOmaha(HandValue hand, long pocketsMask, long cardsInHand, long boardMask, HandValue boardHand, IPocketCards pockets, PokerGames game, Board board)
	{
		return new ComputeRelevantHandContextOmaha(game, pockets.Cards.ToCardsMaskU(), board.Cards.ToCardsMaskU(), pockets.Cards.MapToArray<Card, CardsMaskU>(CardsMaskHelper.ToCardMaskU), board.Cards.MapToArray<Card, CardsMaskU>(CardsMaskHelper.ToCardMaskU), hand, board.Street, (pocketsMask & cardsInHand).ToCardsMaskU(), (boardMask & cardsInHand).ToCardsMaskU(), boardHand, board.FlopCards.ToCardsMask().ToCardsMaskU());
	}

	internal static HandValue ComputeHandValue(this Board board, PokerGames game)
	{
		return board.Cards.ComputeHandValue(game.IsOmahaFamily() ? PokerGames.TexasHoldem : game);
	}

	public static RelevantHandValue ComputeRelevantHandValue(this IPocketCards pockets, Board board, PokerGames game)
	{
		HandValue boardHand = board.ComputeHandValue(game);
		HandValue hand = pockets.ComputeHandValue(board, game);
		if (boardHand.Rank == hand.Rank && ((boardHand.Type == PokerHands.StraightFlush && boardHand.StraightRank == CardRanks.Ace) || (boardHand.Type == PokerHands.Quads && ((boardHand.QuadsRank == CardRanks.Ace && boardHand.TopKickerRank == CardRanks.King) || boardHand.TopKickerRank == CardRanks.Ace))))
		{
			return RelevantHandValue.BestPossibleHandOnBoard;
		}
		long pocketsMask = pockets.Cards.ToCardsMask();
		long boardMask = board.Cards.ToCardsMask();
		long cardsInHand = hand.ExtractCardsInHandMask(boardMask, pocketsMask | boardMask, board.Cards.Length + pockets.Cards.Length, game);
		long flopBoardMask = board.FlopCards.ToCardsMask();
		(ComputeDrawResult, ComputePairResult, ComputeOvercardsResult) tuple = Compute();
		ComputeDrawResult item = tuple.Item1;
		ComputePairResult item2 = tuple.Item2;
		ComputeOvercardsResult item3 = tuple.Item3;
		RelevantHandValue relevantHandValue = (RelevantHandValue)0L;
		CardsMaskU cards = (pocketsMask & cardsInHand).ToCardsMaskU();
		int cardsCount = cards.GetCardsCount();
		CardsMaskU cards2 = (boardMask & cardsInHand).ToCardsMaskU();
		bool flag = game.IsOmahaFamily() && cards2.IsTrips() && boardHand.Type == PokerHands.FullHouse && cards.IsPair();
		bool flag2 = false;
		bool flag3 = hand.Rank <= boardHand.Rank;
		if (item3.IsHighCard || cardsCount == 0 || flag3)
		{
			flag2 = true;
			relevantHandValue |= RelevantHandValue.Air;
			if (item3.IsHighCard)
			{
				relevantHandValue = (RelevantHandValue)((long)relevantHandValue | (long)(1 << (int)(12 - item3.HighCardRank)));
			}
			if (item3.IsTwoOvercards)
			{
				relevantHandValue |= RelevantHandValue.AirWithTwoOverCards;
			}
			else if (item3.IsOneOvercard)
			{
				relevantHandValue |= RelevantHandValue.AirWithOneOverCard;
			}
			else if (item3.IsNoOvercards)
			{
				relevantHandValue |= RelevantHandValue.AirWithoutOverCards;
			}
		}
		if (item2.IsPair)
		{
			RelevantHandValue relevantHandValue2 = relevantHandValue;
			relevantHandValue = (RelevantHandValue)((long)relevantHandValue2 | (item2.PairTypes switch
			{
				PairTypes.TopPair => 131072L, 
				PairTypes.MiddlePair => 262144L, 
				PairTypes.LowPair => 524288L, 
				PairTypes.OverPair => 16777216L, 
				PairTypes.PocketMiddlePair => 33554432L, 
				PairTypes.PocketLowPair => 67108864L, 
				_ => throw new NotImplementedException(), 
			}));
			if (item2.PairTypes == PairTypes.TopPair)
			{
				if (item2.KickerIndex == 1)
				{
					relevantHandValue |= RelevantHandValue.TopPairTopKicker;
				}
				else if (item2.KickerIndex == 2)
				{
					relevantHandValue |= RelevantHandValue.TopPairSecondKicker;
				}
				else if (item2.KickerIndex >= 3)
				{
					relevantHandValue |= RelevantHandValue.TopPairThirdOrLessKicker;
				}
			}
			else if (item2.PairTypes == PairTypes.MiddlePair && item2.KickerIndex == 1)
			{
				relevantHandValue |= RelevantHandValue.MiddlePairTopKicker;
			}
		}
		if (hand.Type == PokerHands.TwoPairs && !flag2 && !item2.IsPair)
		{
			relevantHandValue |= RelevantHandValue.TwoPairs;
		}
		else if (hand.Type == PokerHands.ThreeOfAKind)
		{
			if (boardMask.ToCardsMaskU().HasPair())
			{
				if (!flag2 && !item2.IsPair && !boardMask.ToCardsMaskU().HasTrips())
				{
					relevantHandValue |= RelevantHandValue.Trips;
				}
			}
			else
			{
				relevantHandValue |= RelevantHandValue.Set;
			}
		}
		if (hand.Type == PokerHands.Straight && cardsCount > 0)
		{
			relevantHandValue |= RelevantHandValue.Straight;
		}
		else if (hand.Type == PokerHands.Flush && cardsCount > 0)
		{
			relevantHandValue |= RelevantHandValue.Flush;
		}
		else if (cardsCount > 0 && !flag2 && (!item2.IsPair || flag))
		{
			if (hand.Type == PokerHands.Quads)
			{
				relevantHandValue |= RelevantHandValue.Quads;
			}
			else if (hand.Type == PokerHands.FullHouse)
			{
				relevantHandValue |= RelevantHandValue.FullHouse;
			}
			else if (hand.Type == PokerHands.StraightFlush)
			{
				relevantHandValue |= RelevantHandValue.StraightFlush;
			}
		}
		else if ((item.IsFlushDraw || item.IsStraightDraw) && item2.IsPair)
		{
			relevantHandValue |= RelevantHandValue.ComboDrawWithPair;
		}
		else if (((item.IsFlushDraw && (item.IsGutshot || item.IsStraightDraw)) || (item.StraightOutsCount >= 10 && item.FlushOutsCount >= 12)) && !item2.IsPair)
		{
			relevantHandValue |= RelevantHandValue.ComboDrawWithoutPair;
		}
		if (hand.Type.IsLessThan(PokerHands.Flush, game))
		{
			if (item.IsFlushDraw)
			{
				relevantHandValue |= RelevantHandValue.FlushDraw;
				if (item.IsTurnedFlushDrawBackdoor)
				{
					relevantHandValue |= RelevantHandValue.BackdoorFlushDraw;
				}
				if (item.IsNutsHighFlushDraw)
				{
					relevantHandValue |= RelevantHandValue.NutsFlushDraw;
				}
				if (item.IsSecondNutsHighFlushDraw)
				{
					relevantHandValue |= RelevantHandValue.SecondNutsFlushDraw;
				}
				if (item.IsThirdNutsHighFlushDraw)
				{
					relevantHandValue |= RelevantHandValue.ThirdNutsFlushDraw;
				}
				if (item.IsFourthOrLowerNutsHighFlushDraw)
				{
					relevantHandValue |= RelevantHandValue.FourthOrLowerNutsFlushDraw;
				}
				if (item.IsDoubleFlushDraw)
				{
					relevantHandValue |= RelevantHandValue.DoubleFlushDraw;
				}
			}
			else if ((board.Street == Streets.Flop && item.IsTwoPocketsBackdoor) || item.IsTopFlushBackdoor)
			{
				relevantHandValue |= RelevantHandValue.BackdoorFlushDraw;
			}
		}
		if (hand.Type.IsLessThan(PokerHands.Straight, game))
		{
			if (item.IsStraightDraw)
			{
				relevantHandValue |= RelevantHandValue.StraightDraw;
				if (item.IsBottomStraightDrawWithOnePocketCard)
				{
					relevantHandValue |= RelevantHandValue.BottomStraightDrawOneHeroCard;
				}
			}
			else if (item.IsGutshot)
			{
				relevantHandValue |= RelevantHandValue.Gutshot;
			}
		}
		return relevantHandValue;
		(ComputeDrawResult, ComputePairResult, ComputeOvercardsResult) Compute()
		{
			if (game.IsOmahaFamily())
			{
				ComputeRelevantHandContextOmaha ctx = ToComputeRelevantHandContextOmaha(hand, pocketsMask, cardsInHand, boardMask, boardHand, pockets, game, board);
				return (ctx.ComputeDrawOmaha(), ctx.ComputePairOmaha(), ctx.ComputeOvercardsOmaha());
			}
			ComputeRelevantHandContext ctx2 = new ComputeRelevantHandContext(game, pocketsMask.ToCardsMaskU(), boardMask.ToCardsMaskU(), hand, boardHand, board.Street, (pocketsMask & cardsInHand).ToCardsMaskU(), flopBoardMask.ToCardsMaskU());
			return (ctx2.ComputeDraw(), ctx2.ComputePair(), ctx2.ComputeOvercards());
		}
	}

	public static RelevantFlopType ComputeRelevantFlopType(this Board board, PokerGames game)
	{
		InlineList<CardsMaskU> board2 = default(InlineList<CardsMaskU>);
		for (int i = 0; i < 3; i++)
		{
			Card card = board.Cards[i];
			board2.Add(card.ToCardMaskU());
		}
		return board.ToCardsMask(3).ToCardsMaskU().ComputeRelevantFlopType(board2.ComputeFlopComboIndex(), game);
	}

	public static RelevantTurnType ComputeRelevantTurnType(this Board board, PokerGames game)
	{
		InlineList<CardsMaskU> board2 = default(InlineList<CardsMaskU>);
		for (int i = 0; i < 4; i++)
		{
			Card card = board.Cards[i];
			board2.Add(card.ToCardMaskU());
		}
		return board.ToCardsMask(4).ToCardsMaskU().ComputeRelevantTurnType(board2.ComputeFlopComboIndex(), board2[3], game);
	}

	public static RelevantRiverType ComputeRelevantRiverType(this Board board, PokerGames game)
	{
		InlineList<CardsMaskU> board2 = default(InlineList<CardsMaskU>);
		for (int i = 0; i < 5; i++)
		{
			Card card = board.Cards[i];
			board2.Add(card.ToCardMaskU());
		}
		return board.ToBoardCardsMask().ToCardsMaskU().ComputeRelevantRiverType(board2[3].ComputeTurnComboIndex(board2.ComputeFlopComboIndex()), board2[3], board2[4], game);
	}

	internal static int ComputeTurnComboIndex(this CardsMaskU turnCard, int flopComboIndex)
	{
		return (int)(flopComboIndex * 13 + turnCard.GetTopRank());
	}

	internal static int ComputeFlopComboIndex(this InlineList<CardsMaskU> board)
	{
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			num = (int)(num * 13 + board[i].GetTopRank());
		}
		return num;
	}

	internal static RelevantFlopType ComputeRelevantFlopType(this CardsMaskU boardMask, int boardRanksCombo, PokerGames game)
	{
		RelevantFlopType relevantFlopType = (boardMask.IsSuited() ? RelevantFlopType.AllThreeCardsOfOneSuit : (boardMask.ContainsSuitedCardsCountAtLeast(2) ? RelevantFlopType.ExactlyTwoCardsOfOneSuit : RelevantFlopType.AllThreeCardsOfDifferentSuit));
		if (!game.IsShortDeckFamily())
		{
			return (RelevantFlopType)((long)relevantFlopType | Tables.RelevantFlopTypesTexasHoldem[boardRanksCombo]);
		}
		return (RelevantFlopType)((long)relevantFlopType | Tables.RelevantFlopTypesShortDeck[boardRanksCombo]);
	}

	internal static RelevantTurnType ComputeRelevantTurnType(this CardsMaskU boardMask, int flopComboIndex, CardsMaskU turnCard, PokerGames game)
	{
		long[] array = (game.IsShortDeckFamily() ? Tables.RelevantTurnTypesShortDeck : Tables.RelevantTurnTypesTexasHoldem);
		return (RelevantTurnType)((ulong)ComputeSuitness() | (ulong)array[(int)(flopComboIndex * 13 + turnCard.GetTopRank())]);
		RelevantTurnType ComputeSuitness()
		{
			if (boardMask.IsSuited())
			{
				return RelevantTurnType.AllFourCardsOfOneSuit;
			}
			if (boardMask.IsRainbow())
			{
				return RelevantTurnType.FourDifferentSuitOnBoard;
			}
			CardsMaskU cards = new CardsMaskU
			{
				CardsMask = (boardMask.CardsMask ^ turnCard.CardsMask)
			};
			switch (cards.GetCountOfSuit(turnCard.GetCardSuit()))
			{
			case 2:
				return RelevantTurnType.TurnIsTheThirdCardOfSuit;
			case 1:
				if (!cards.IsRainbow())
				{
					return RelevantTurnType.TwoFlushDraw;
				}
				break;
			}
			return (RelevantTurnType)0uL;
		}
	}

	internal static RelevantRiverType ComputeRelevantRiverType(this CardsMaskU board, int turnComboIndex, CardsMaskU turnCard, CardsMaskU riverCard, PokerGames game)
	{
		long[] array = (game.IsShortDeckFamily() ? Tables.RelevantRiverTypesShortDeck : Tables.RelevantRiverTypesTexasHoldem);
		return (RelevantRiverType)((ulong)ComputeSuitness() | (ulong)array[(int)(turnComboIndex * 13 + riverCard.GetTopRank())]);
		RelevantRiverType ComputeSuitness()
		{
			if (board.IsSuited())
			{
				return RelevantRiverType.FiveFlushCardOnBoard;
			}
			if (board.ContainsSuitedCardsCountExactly(4))
			{
				return RelevantRiverType.FourFlushCardsOnBoard;
			}
			Suits cardSuit = riverCard.GetCardSuit();
			if (new CardsMaskU
			{
				CardsMask = (board.CardsMask ^ riverCard.CardsMask)
			}.GetCountOfSuit(cardSuit) == 2)
			{
				if (turnCard.GetCardSuit() == cardSuit)
				{
					return RelevantRiverType.ThirdCardToFlush | RelevantRiverType.ThirdCardToBackdoorFlush;
				}
				return RelevantRiverType.ThirdCardToFlush;
			}
			return (RelevantRiverType)0uL;
		}
	}

	internal static ComputePairResult ComputePairOmaha(this ComputeRelevantHandContextOmaha ctx)
	{
		if (!ctx.PokerGame.IsOmahaFamily())
		{
			throw new InvalidOperationException($"Use another overload to compute pair type in {ctx.PokerGame}");
		}
		if (ctx.HandType.IsStraightOrHigherTexasHoldem() && ctx.HandType != PokerHands.FullHouse)
		{
			return default(ComputePairResult);
		}
		if (ctx.HandType == PokerHands.HighCard)
		{
			return default(ComputePairResult);
		}
		CardsMaskU pocketsInHandMask = ctx.PocketsInHandMask;
		CardsMaskU boardCardsInHandMask = ctx.BoardCardsInHandMask;
		bool flag = false;
		CardRanks cardRanks = CardRanks.Deuce;
		if (ctx.HandType == PokerHands.Pair)
		{
			if (ctx.BoardHandType != PokerHands.Pair)
			{
				cardRanks = ctx.HandValue.PairRank;
				flag = true;
			}
		}
		else if (ctx.HandType == PokerHands.TwoPairs)
		{
			if (pocketsInHandMask.IsPair())
			{
				cardRanks = pocketsInHandMask.GetTopRank();
				flag = true;
			}
			else if (ctx.BoardHandType == PokerHands.Pair)
			{
				if (ctx.HandValue.SecondPairRank == ctx.BoardHandValue.PairRank)
				{
					cardRanks = ctx.HandValue.TopPairRank;
					flag = true;
				}
				else if (ctx.HandValue.TopPairRank == ctx.BoardHandValue.PairRank)
				{
					cardRanks = ctx.HandValue.PairRank;
					flag = true;
				}
			}
			else if (ctx.BoardHandType == PokerHands.TwoPairs && ctx.BoardHandValue.TopPairRank == ctx.HandValue.SecondPairRank)
			{
				cardRanks = ctx.HandValue.TopPairRank;
				flag = true;
			}
		}
		else if (ctx.HandType == PokerHands.FullHouse && pocketsInHandMask.IsPair() && boardCardsInHandMask.IsTrips())
		{
			cardRanks = ctx.HandValue.PairRank;
			flag = true;
		}
		PairTypes pairTypes = PairTypes.None;
		CardRanks cardRanks2 = CardRanks.Deuce;
		int kickerIndex = 0;
		if (flag)
		{
			int num = ctx.BoardMask.ToRanksMask();
			CardRanks topCardRank = num.GetTopCardRank();
			CardRanks topCardRank2 = (num ^ topCardRank.ToRankMask()).GetTopCardRank();
			if (ctx.PocketsInHandMask.HasPair())
			{
				pairTypes = ((cardRanks > topCardRank) ? PairTypes.OverPair : ((cardRanks <= topCardRank2 || cardRanks < CardRanks.Six) ? PairTypes.PocketLowPair : PairTypes.PocketMiddlePair));
			}
			else
			{
				pairTypes = ((cardRanks == topCardRank) ? PairTypes.TopPair : ((cardRanks != topCardRank2 || cardRanks < CardRanks.Six) ? PairTypes.LowPair : PairTypes.MiddlePair));
				int num2 = ctx.PocketsInHandMask.ToRanksMask() ^ cardRanks.ToRankMask();
				if (num2 != 0)
				{
					cardRanks2 = num2.GetTopCardRank();
					int num3 = 1;
					CardRanks cardRanks3 = (CardRanks)13;
					do
					{
						cardRanks3--;
						if (!num.ContainsCardRank(cardRanks3))
						{
							if (cardRanks3 == cardRanks2)
							{
								kickerIndex = num3;
								break;
							}
							num3++;
						}
					}
					while (cardRanks3 != CardRanks.Deuce);
				}
			}
		}
		return new ComputePairResult(flag, pairTypes, kickerIndex, cardRanks, cardRanks2);
	}

	internal static ComputePairResult ComputePair(this ComputeRelevantHandContext ctx)
	{
		if (ctx.PokerGame.IsOmahaFamily())
		{
			throw new InvalidOperationException("Use another overload to compute pair type in omaha");
		}
		if (ctx.HandType == PokerHands.HighCard || ctx.HandType == PokerHands.ThreeOfAKind || ctx.HandType == PokerHands.Straight || ctx.HandType == PokerHands.Flush || ctx.HandType == PokerHands.Quads || ctx.HandType == PokerHands.StraightFlush)
		{
			return default(ComputePairResult);
		}
		if (ctx.PocketsInHandMask.CardsMask == 0L)
		{
			return default(ComputePairResult);
		}
		if (ctx.IsBestHandOnBoard)
		{
			return default(ComputePairResult);
		}
		bool flag = false;
		CardRanks cardRanks = CardRanks.Deuce;
		ctx.PocketsInHandMask.IsPair();
		if (ctx.BoardHandType == PokerHands.HighCard)
		{
			if (ctx.HandType != PokerHands.Pair)
			{
				return default(ComputePairResult);
			}
			flag = true;
			cardRanks = ctx.HandValue.PairRank;
		}
		else if (ctx.BoardHandType == PokerHands.Pair)
		{
			if (ctx.HandType == PokerHands.Pair || ctx.HandType == PokerHands.FullHouse)
			{
				return default(ComputePairResult);
			}
			if (ctx.HandValue.TopPairRank == ctx.BoardHandValue.PairRank)
			{
				flag = true;
				cardRanks = ctx.HandValue.SecondPairRank;
			}
			else
			{
				if (ctx.HandValue.SecondPairRank != ctx.BoardHandValue.PairRank)
				{
					return default(ComputePairResult);
				}
				flag = true;
				cardRanks = ctx.HandValue.TopPairRank;
			}
		}
		else if (ctx.BoardHandType == PokerHands.TwoPairs)
		{
			if (ctx.HandType == PokerHands.FullHouse)
			{
				return default(ComputePairResult);
			}
			if (ctx.BoardHandValue.TopPairRank == ctx.HandValue.TopPairRank)
			{
				if (ctx.BoardHandValue.SecondPairRank == ctx.HandValue.SecondPairRank)
				{
					return default(ComputePairResult);
				}
				flag = true;
				cardRanks = ctx.HandValue.SecondPairRank;
			}
			else
			{
				flag = true;
				cardRanks = ctx.HandValue.TopPairRank;
			}
		}
		else if (ctx.BoardHandType == PokerHands.ThreeOfAKind)
		{
			if (ctx.HandType != PokerHands.FullHouse || ctx.HandValue.TripsRank != ctx.BoardHandValue.TripsRank)
			{
				return default(ComputePairResult);
			}
			flag = true;
			cardRanks = ctx.HandValue.PairRank;
		}
		else if (ctx.BoardHandType == PokerHands.FullHouse)
		{
			if (ctx.HandType != PokerHands.FullHouse || ctx.HandValue.TripsRank != ctx.BoardHandValue.TripsRank || ctx.HandValue.PairRank == ctx.BoardHandValue.PairRank)
			{
				return default(ComputePairResult);
			}
			flag = true;
			cardRanks = ctx.HandValue.PairRank;
		}
		PairTypes pairTypes = PairTypes.None;
		CardRanks cardRanks2 = CardRanks.Deuce;
		int kickerIndex = 0;
		if (flag)
		{
			int num = ctx.BoardMask.ToRanksMask();
			CardRanks topCardRank = num.GetTopCardRank();
			CardRanks topCardRank2 = (num ^ topCardRank.ToRankMask()).GetTopCardRank();
			if (ctx.PocketsMask.IsPair())
			{
				pairTypes = ((cardRanks > topCardRank) ? PairTypes.OverPair : ((cardRanks <= topCardRank2 || (int)cardRanks < (ctx.IsShortDeckFamily ? 7 : 4)) ? PairTypes.PocketLowPair : PairTypes.PocketMiddlePair));
			}
			else
			{
				pairTypes = ((cardRanks == topCardRank) ? PairTypes.TopPair : ((cardRanks != topCardRank2 || (int)cardRanks < (ctx.IsShortDeckFamily ? 7 : 4)) ? PairTypes.LowPair : PairTypes.MiddlePair));
				int num2 = ctx.PocketsInHandMask.ToRanksMask() ^ cardRanks.ToRankMask();
				if (num2 != 0)
				{
					cardRanks2 = num2.GetTopCardRank();
					int num3 = 1;
					CardRanks cardRanks3 = (CardRanks)13;
					do
					{
						cardRanks3--;
						if (!num.ContainsCardRank(cardRanks3))
						{
							if (cardRanks3 == cardRanks2)
							{
								kickerIndex = num3;
								break;
							}
							num3++;
						}
					}
					while (cardRanks3 != CardRanks.Deuce);
				}
			}
		}
		return new ComputePairResult(flag, pairTypes, kickerIndex, cardRanks, cardRanks2);
	}

	internal static ComputeDrawResult ComputeDrawOmaha(this ComputeRelevantHandContextOmaha ctx)
	{
		if (ctx.Street == Streets.River)
		{
			return default(ComputeDrawResult);
		}
		if (!ctx.PokerGame.IsOmahaFamily())
		{
			throw new InvalidOperationException($"Use another overload for to compute draw for {ctx.PokerGame}");
		}
		if (ctx.HandType.IsFlushOrHigherInTexasHoldem())
		{
			return default(ComputeDrawResult);
		}
		bool flag = false;
		bool isDoubleFlushDraw = false;
		bool isNutsHighFlushDraw = false;
		int num = 0;
		bool isTwoPocketsBackdoor = false;
		int num2 = 0;
		bool isTopFlushBackdoor = false;
		bool isTurnedFlushDrawBackdoor = false;
		CardsMaskU pocketsMask = ctx.PocketsMask;
		CardsMaskU boardMask = ctx.BoardMask;
		CardsMaskU cards = new CardsMaskU
		{
			CardsMask = (pocketsMask.CardsMask | boardMask.CardsMask)
		};
		if (pocketsMask.ContainsSuitedCardsCountAtLeast(2))
		{
			for (Suits suits = Suits.Hearts; suits < (Suits)4; suits++)
			{
				if (pocketsMask.GetCountOfSuit(suits) < 2)
				{
					continue;
				}
				int suitRanksMask = boardMask.GetSuitRanksMask(suits);
				int countOfSuit = boardMask.GetCountOfSuit(suits);
				if (countOfSuit >= 3)
				{
					return default(ComputeDrawResult);
				}
				switch (countOfSuit)
				{
				case 2:
					if (flag)
					{
						isDoubleFlushDraw = true;
					}
					else
					{
						flag = true;
					}
					if (ctx.Street == Streets.Turn && ctx.FlopBoardMask.GetCountOfSuit(suits) == 1)
					{
						isTurnedFlushDrawBackdoor = true;
					}
					if (pocketsMask.GetSuitRanksMask(suits).ContainsCardRank(suitRanksMask.GetTopFlushRank()))
					{
						isNutsHighFlushDraw = true;
					}
					num += 11 - pocketsMask.GetCountOfSuit(suits);
					break;
				case 1:
					if (ctx.Street == Streets.Flop)
					{
						isTwoPocketsBackdoor = true;
						num2++;
						if (pocketsMask.GetSuitRanksMask(suits).HasAce())
						{
							isTopFlushBackdoor = true;
						}
					}
					break;
				}
			}
		}
		int num3 = ctx.Street.CardsCount();
		int num4 = 0;
		bool isStraightDraw = false;
		bool isGutshot = false;
		int num5 = 0;
		CardRanks gutshotOutRank = CardRanks.Deuce;
		if (!ctx.HandType.IsStraightOrHigherTexasHoldem())
		{
			for (int i = 0; i < ctx.PokerGame.PocketCardsCount() - 1; i++)
			{
				for (int j = i + 1; j < ctx.PokerGame.PocketCardsCount(); j++)
				{
					CardsMaskU cards2 = ctx.PocketsCards[i];
					CardsMaskU cards3 = ctx.PocketsCards[j];
					CardRanks rank1 = cards2.GetTopRank();
					CardRanks rank2 = cards3.GetTopRank();
					int num6 = rank1.ToRankMask() | rank2.ToRankMask();
					if (!CanBeStraightRanks())
					{
						continue;
					}
					int num7 = new CardsMaskU
					{
						CardsMask = (cards2.CardsMask | cards3.CardsMask | boardMask.CardsMask)
					}.ToRanksMask();
					int num8 = Math.Max(-1, Math.Min((int)rank1, (int)rank2) - 3);
					int num9 = Math.Min(12, Math.Max((int)rank1, (int)rank2) + 3);
					for (int k = num8; k <= num9; k++)
					{
						int num10 = ((k == -1) ? CardRanks.Ace : ((CardRanks)k)).ToRankMask();
						if ((num4 & num10) != 0 || (num10 | num7).GetStraightRankTexasHoldem() <= CardRanks.Deuce)
						{
							continue;
						}
						int num11 = num6 | num10;
						for (int l = 0; l < num3 - 1; l++)
						{
							for (int m = l + 1; m < num3; m++)
							{
								if ((num11 | ctx.BoardCards[l].ToRanksMask() | ctx.BoardCards[m].ToRanksMask()).GetStraightRankTexasHoldem() > CardRanks.Deuce)
								{
									num4 |= num10;
									break;
								}
							}
						}
					}
					bool CanBeStraightRanks()
					{
						if (rank1 == rank2)
						{
							return false;
						}
						if (rank1 == CardRanks.Ace)
						{
							if (rank2 < CardRanks.Ten)
							{
								return rank2 <= CardRanks.Five;
							}
							return true;
						}
						if (rank2 == CardRanks.Ace)
						{
							if (rank1 < CardRanks.Ten)
							{
								return rank1 <= CardRanks.Five;
							}
							return true;
						}
						if (rank1 <= rank2)
						{
							return rank2 - rank1 <= 5;
						}
						return rank1 - rank2 <= 5;
					}
				}
			}
		}
		if (num4 != 0)
		{
			for (CardRanks cardRanks = CardRanks.Deuce; cardRanks <= CardRanks.Ace; cardRanks++)
			{
				if (!num4.ContainsCardRank(cardRanks))
				{
					continue;
				}
				num5 += 4 - cards.GetRankCount(cardRanks);
				for (int n = 0; n < 4; n++)
				{
					Suits suit = (Suits)n;
					if ((cards.CardsMask & CardsMaskHelper.ToCardMask(suit, cardRanks)) == 0L && (cards.GetSuitRanksMask(suit) | cardRanks.ToRankMask()).GetStraightRankTexasHoldem() <= CardRanks.Deuce && pocketsMask.GetCountOfSuit(suit) >= 2 && boardMask.GetCountOfSuit(suit) >= 2)
					{
						num5--;
					}
				}
			}
			if (num5 > 4)
			{
				isStraightDraw = true;
			}
			else if (num5 > 0)
			{
				isGutshot = true;
				gutshotOutRank = num4.GetTopCardRank();
			}
		}
		return new ComputeDrawResult(isBottomStraightDrawWithOnePocketCard: false, isNutsHighFlushDraw, isSecondNutsHighFlushDraw: false, isThirdNutsHighFlushDraw: false, isFourthOrLowerNutsHighFlushDraw: false, flag, isStraightDraw, isTwoPocketsBackdoor, isTopFlushBackdoor, isTurnedFlushDrawBackdoor, isGutshot, gutshotOutRank, isDoubleFlushDraw, num, num5, num2);
	}

	private static CardRanks GetTopFlushRank(this int boardSuitedMask)
	{
		if (!boardSuitedMask.ContainsCardRank(CardRanks.Ace))
		{
			return CardRanks.Ace;
		}
		if (!boardSuitedMask.ContainsCardRank(CardRanks.King))
		{
			return CardRanks.King;
		}
		if (!boardSuitedMask.ContainsCardRank(CardRanks.Queen))
		{
			return CardRanks.Queen;
		}
		return CardRanks.Jack;
	}

	private static CardRanks GetFlushRank(this int boardSuitedMask, FlushRankType flushRankType)
	{
		return GetRank();
		CardRanks GetRank(CardRanks rank = CardRanks.Ace, int counter = 1)
		{
			CardRanks cardRanks = rank;
			while ((int)cardRanks >= 9 - counter)
			{
				if (!boardSuitedMask.ContainsCardRank(cardRanks))
				{
					if (counter == (int)flushRankType)
					{
						return cardRanks;
					}
					return GetRank(cardRanks - 1, counter + 1);
				}
				cardRanks--;
			}
			return (CardRanks)(8 - counter);
		}
	}

	internal static ComputeDrawResult ComputeDraw(this ComputeRelevantHandContext ctx)
	{
		if (ctx.Street == Streets.River)
		{
			return default(ComputeDrawResult);
		}
		if (ctx.PokerGame.IsOmahaFamily())
		{
			throw new InvalidOperationException("Use another overload for omaha");
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool isFourthOrLowerNutsHighFlushDraw = false;
		int num = 0;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		int pocketRanks = ctx.PocketsMask.ToRanksMask();
		if (ctx.PocketsMask.IsSuited(out var suit))
		{
			int suitRanksMask = ctx.BoardMask.GetSuitRanksMask(suit);
			switch (suitRanksMask.GetRanksCount())
			{
			case 2:
				flag = true;
				flag7 = ctx.Street == Streets.Turn && ctx.FlopBoardMask.GetCountOfSuit(suit) == 1;
				flag2 = pocketRanks.ContainsCardRank(suitRanksMask.GetTopFlushRank());
				flag3 = pocketRanks.ContainsCardRank(suitRanksMask.GetFlushRank(FlushRankType.SecondBestFlushRank));
				flag4 = pocketRanks.ContainsCardRank(suitRanksMask.GetFlushRank(FlushRankType.ThirdBestFlushRank));
				isFourthOrLowerNutsHighFlushDraw = !(flag2 || flag3 || flag4);
				num = 9;
				break;
			case 1:
				if (!ctx.BoardMask.IsTrips() && ctx.Street == Streets.Flop)
				{
					flag5 = true;
				}
				break;
			}
		}
		else
		{
			for (Suits suits = Suits.Hearts; suits < (Suits)4; suits++)
			{
				int suitRanksMask2 = ctx.PocketsMask.GetSuitRanksMask(suits);
				if (suitRanksMask2 != 0)
				{
					int suitRanksMask3 = ctx.BoardMask.GetSuitRanksMask(suits);
					if (suitRanksMask3.GetRanksCount() == 3)
					{
						flag = true;
						flag7 = ctx.Street == Streets.Turn && ctx.FlopBoardMask.GetCountOfSuit(suits) == 2 && suitRanksMask2.ContainsCardRank(suitRanksMask3.GetTopFlushRank());
						flag2 = suitRanksMask2.ContainsCardRank(suitRanksMask3.GetTopFlushRank());
						flag3 = suitRanksMask2.ContainsCardRank(suitRanksMask3.GetFlushRank(FlushRankType.SecondBestFlushRank));
						flag4 = suitRanksMask2.ContainsCardRank(suitRanksMask3.GetFlushRank(FlushRankType.ThirdBestFlushRank));
						isFourthOrLowerNutsHighFlushDraw = !(flag2 || flag3 || flag4);
						num = 9;
					}
					else if (suitRanksMask3.GetRanksCount() == (int)(ctx.Street + 1) && suitRanksMask2.ContainsCardRank(suitRanksMask3.GetTopFlushRank()))
					{
						flag6 = true;
					}
				}
			}
		}
		bool flag8 = false;
		bool flag9 = false;
		CardRanks cardRanks = CardRanks.Deuce;
		bool flag10 = false;
		int num2 = 0;
		int boardRanks = ctx.BoardMask.ToRanksMask();
		int ranksMask = boardRanks & pocketRanks;
		int allCardsRanks = boardRanks | pocketRanks;
		int topOutRank;
		if (ranksMask.GetRanksCount() < 2 && (!pocketRanks.IsSingleRank() || !ranksMask.IsSingleRank()))
		{
			int straightDrawOuts = allCardsRanks.GetStraightDrawOuts(ctx.PokerGame);
			topOutRank = straightDrawOuts.GetTopRankMask();
			int topRankMask = pocketRanks.GetTopRankMask();
			if (straightDrawOuts != 0 && !OutGivesTheSameStraightOnBoard())
			{
				int ranksCount = straightDrawOuts.GetRanksCount();
				if (ranksCount == 1)
				{
					flag9 = true;
					cardRanks = straightDrawOuts.GetTopCardRank();
				}
				else
				{
					flag10 = true;
					flag8 = IsBottomStraightDrawWithOnePocketCard();
				}
				num2 = 4 * ranksCount;
			}
		}
		bool isBottomStraightDrawWithOnePocketCard = flag8;
		bool isNutsHighFlushDraw = flag2;
		bool isFlushDraw = flag;
		bool isStraightDraw = flag10;
		bool isTwoPocketsBackdoor = flag5;
		bool isTopFlushBackdoor = flag6;
		bool isTurnedFlushDrawBackdoor = flag7;
		bool isGutshot = flag9;
		CardRanks gutshotOutRank = cardRanks;
		int flushOutsCount = num;
		int straightOutsCount = num2;
		int backdoorCount = (flag5 ? 1 : 0);
		return new ComputeDrawResult(isBottomStraightDrawWithOnePocketCard, isNutsHighFlushDraw, flag3, flag4, isFourthOrLowerNutsHighFlushDraw, isFlushDraw, isStraightDraw, isTwoPocketsBackdoor, isTopFlushBackdoor, isTurnedFlushDrawBackdoor, isGutshot, gutshotOutRank, isDoubleFlushDraw: false, flushOutsCount, straightOutsCount, backdoorCount);
		bool IsBottomStraightDrawWithOnePocketCard()
		{
			int straightDrawOuts2 = (boardRanks | topOutRank).GetStraightDrawOuts(ctx.PokerGame);
			if (straightDrawOuts2.GetRanksCount() == 2)
			{
				int num3 = straightDrawOuts2 ^ straightDrawOuts2.GetTopRankMask();
				return (pocketRanks & num3) != 0;
			}
			if (straightDrawOuts2.GetRanksCount() == 1 && boardRanks.ContainsCardRank(CardRanks.Jack) && boardRanks.ContainsCardRank(CardRanks.Queen) && boardRanks.ContainsCardRank(CardRanks.King))
			{
				return (pocketRanks & straightDrawOuts2) != 0;
			}
			return false;
		}
		bool OutGivesTheSameStraightOnBoard()
		{
			CardRanks straightRank = (topOutRank | allCardsRanks).GetStraightRank(ctx.PokerGame);
			CardRanks straightRank2 = (topOutRank | boardRanks).GetStraightRank(ctx.PokerGame);
			return straightRank == straightRank2;
		}
	}

	internal static ComputeOvercardsResult ComputeOvercardsOmaha(this ComputeRelevantHandContextOmaha ctx)
	{
		if (!ctx.PokerGame.IsOmahaFamily())
		{
			throw new InvalidOperationException($"Use another overload to compute overcards in {ctx.PokerGame}");
		}
		if (ctx.PocketsInHandMask.IsPair())
		{
			return default(ComputeOvercardsResult);
		}
		if (ctx.HandType != PokerHands.HighCard && ctx.HandType != PokerHands.Pair && ctx.HandType != PokerHands.ThreeOfAKind)
		{
			return default(ComputeOvercardsResult);
		}
		if (ctx.HandType == PokerHands.ThreeOfAKind)
		{
			if (ctx.BoardCardsInHandMask.HasTrips())
			{
				return new ComputeOvercardsResult(isHighCard: true, GetOvercardsType(ctx.HandValue.TripsRank), ctx.PocketsInHandMask.GetTopRank());
			}
			return default(ComputeOvercardsResult);
		}
		if (ctx.HandType == PokerHands.HighCard)
		{
			return new ComputeOvercardsResult(isHighCard: true, GetOvercardsType(ctx.BoardHandValue.TopPairRank), ctx.PocketsInHandMask.GetTopRank());
		}
		if (ctx.HandType == PokerHands.Pair && ctx.BoardHandValue.Type == PokerHands.Pair)
		{
			return new ComputeOvercardsResult(isHighCard: true, GetOvercardsType(ctx.BoardMask.GetTopRank()), ctx.PocketsInHandMask.GetTopRank());
		}
		return default(ComputeOvercardsResult);
		OvercardsTypes GetOvercardsType(CardRanks highCardRank)
		{
			if (ctx.HandValue.SecondKickerRank > highCardRank)
			{
				return OvercardsTypes.TwoOvercards;
			}
			if (ctx.HandValue.TopKickerRank > highCardRank)
			{
				return OvercardsTypes.OneOvercard;
			}
			return OvercardsTypes.NoOvercards;
		}
	}

	internal static ComputeOvercardsResult ComputeOvercards(this ComputeRelevantHandContext ctx)
	{
		if (ctx.PokerGame.IsOmahaFamily())
		{
			throw new InvalidOperationException("Use another overload to compute overcards in omaha");
		}
		if (ctx.IsBestHandOnBoard)
		{
			return default(ComputeOvercardsResult);
		}
		if (ctx.HandValue.Type == PokerHands.Straight || ctx.HandValue.Type == PokerHands.Flush || ctx.HandValue.Type == PokerHands.StraightFlush || ctx.HandValue.Type == PokerHands.FullHouse)
		{
			return default(ComputeOvercardsResult);
		}
		if (ctx.HandType != ctx.BoardHandValue.Type && ctx.HandValue.Type != PokerHands.HighCard)
		{
			return default(ComputeOvercardsResult);
		}
		if (ctx.HandType == PokerHands.HighCard)
		{
			return new ComputeOvercardsResult(isHighCard: true, highCardRank: ctx.PocketsMask.GetTopRank(), type: GetOvercardsType());
		}
		if (ctx.HandType == PokerHands.TwoPairs && (ctx.HandValue.TopPairRank != ctx.BoardHandValue.TopPairRank || ctx.HandValue.SecondPairRank != ctx.BoardHandValue.SecondPairRank))
		{
			return default(ComputeOvercardsResult);
		}
		if (ctx.BoardHandValue.Type == PokerHands.Quads)
		{
			if (ctx.BoardHandValue.TopKickerRank == CardRanks.Ace)
			{
				return default(ComputeOvercardsResult);
			}
			return new ComputeOvercardsResult(isHighCard: true, OvercardsTypes.NoOvercards, ctx.HandValue.TopKickerRank);
		}
		return new ComputeOvercardsResult(isHighCard: true, GetOvercardsType(), GetHighCardRank());
		CardRanks GetHighCardRank()
		{
			if (ctx.HandValue.TopKickerRank > ctx.BoardHandValue.TopKickerRank)
			{
				return ctx.HandValue.TopKickerRank;
			}
			if (ctx.HandValue.SecondKickerRank > ctx.BoardHandValue.SecondKickerRank)
			{
				return ctx.HandValue.SecondKickerRank;
			}
			if (ctx.HandValue.ThirdKickerRank > ctx.BoardHandValue.ThirdKickerRank)
			{
				return ctx.HandValue.ThirdKickerRank;
			}
			if (ctx.HandValue.FourthKickerRank > ctx.BoardHandValue.FourthKickerRank)
			{
				return ctx.HandValue.FourthKickerRank;
			}
			if (ctx.HandValue.FifthKickerRank > ctx.BoardHandValue.FifthKickerRank)
			{
				return ctx.HandValue.FifthKickerRank;
			}
			return CardRanks.Deuce;
		}
		OvercardsTypes GetOvercardsType()
		{
			if (ctx.HandValue.SecondKickerRank > ctx.BoardHandValue.TopKickerRank)
			{
				return OvercardsTypes.TwoOvercards;
			}
			if (ctx.HandValue.TopKickerRank > ctx.BoardHandValue.TopKickerRank)
			{
				return OvercardsTypes.OneOvercard;
			}
			return OvercardsTypes.NoOvercards;
		}
	}
}
