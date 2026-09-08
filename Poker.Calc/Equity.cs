namespace Poker.Calc;

public static class Equity
{
	public static (double firstPlayerEquity, double secondPlayerEquity) GetHoldemPreflopEquityHeadsUp(this PocketCardsHoldem cards, PocketCardsHoldem other, PokerGames game)
	{
		double holdemEquityFromTable = cards.GetHoldemEquityFromTable(other, game);
		return (firstPlayerEquity: holdemEquityFromTable, secondPlayerEquity: 1.0 - holdemEquityFromTable);
	}

	private static double GetHoldemEquityFromTable(this PocketCardsHoldem cards, PocketCardsHoldem other, PokerGames game)
	{
		bool flag = IsSwapNeeded();
		int num = (flag ? GetEquityTableIndex(other, cards, game) : GetEquityTableIndex(cards, other, game));
		double num2 = game switch
		{
			PokerGames.TexasHoldem => Tables.EquityTexasHoldemTable[num],
			PokerGames.ShortDeck => Tables.EquityShortDeckTable[num],
			PokerGames.ShortDeckTbs => Tables.EquityShortDeckTbsTable[num],
			_ => throw new NotImplementedException(),
		};
		if (!flag)
		{
			return num2;
		}
		return 1.0 - num2;


		bool IsSwapNeeded()
		{
			if (cards.IsPair)
			{
				if (other.IsPair)
				{
					return cards.GetCellIndex() > other.GetCellIndex();
				}
				return false;
			}
			if (other.IsPair)
			{
				return true;
			}
			if (cards.IsSuited)
			{
				if (other.IsSuited)
				{
					return cards.GetCellIndex() > other.GetCellIndex();
				}
				return false;
			}
			if (other.IsSuited)
			{
				return true;
			}
			return cards.GetCellIndex() > other.GetCellIndex();
		}

		int GetEquityTableIndex(PocketCardsHoldem pocketCardsHoldem, PocketCardsHoldem pocketCardsHoldem2, PokerGames pokerGames)
		{
			PreflopRangeHoldemCell cardsCell = pocketCardsHoldem.GetPreflopRangeHoldemCell();
			PreflopRangeHoldemCell otherCell = pocketCardsHoldem2.GetPreflopRangeHoldemCell();
			int startIndex = GetCellVsCellStartIndex();
			if (pocketCardsHoldem.IsPair)
			{
				return ComputePairIndex();
			}
			if (pocketCardsHoldem.IsSuited)
			{
				return ComputeSuitedIndex();
			}
			return ComputeOffsuitedIndex();
			int ComputeOffsuitedIndex()
			{
				int num3 = pocketCardsHoldem.CountSuitBlockers(pocketCardsHoldem2);
				if (num3 == 0)
				{
					return startIndex;
				}
				startIndex++;
				if (cardsCell.Column != otherCell.Column)
				{
					if (cardsCell.Row != otherCell.Row)
					{
						if (num3 == 2 && pocketCardsHoldem.HighCard.Suit == pocketCardsHoldem2.HighCard.Suit)
						{
							return startIndex;
						}
						startIndex++;
					}
					if (pocketCardsHoldem.HighCard.Suit == pocketCardsHoldem2.HighCard.Suit)
					{
						return startIndex;
					}
					startIndex++;
				}
				if (cardsCell.Row != otherCell.Column)
				{
					if (num3 == 2 && pocketCardsHoldem.HighCard.Suit == pocketCardsHoldem2.LowCard.Suit)
					{
						return startIndex;
					}
					startIndex++;
					if (pocketCardsHoldem.LowCard.Suit == pocketCardsHoldem2.HighCard.Suit)
					{
						return startIndex;
					}
					startIndex++;
				}
				if (pocketCardsHoldem.HighCard.Suit == pocketCardsHoldem2.LowCard.Suit)
				{
					return startIndex;
				}
				startIndex++;
				if (cardsCell.Row != otherCell.Row)
				{
					return startIndex;
				}
				throw new InvalidOperationException("Should not get here");
			}
			int ComputePairIndex()
			{
				if (pocketCardsHoldem2.IsPair)
				{
					return PairVsPair();
				}
				if (pocketCardsHoldem2.IsSuited)
				{
					return PairVsSuit();
				}
				return PairVsOfSuit();
			}
			int ComputeSuitedIndex()
			{
				if (!pocketCardsHoldem2.IsSuited)
				{
					return SuitedVsOfSuited();
				}
				return SuitedVsSuited();
			}
			int GetCellVsCellStartIndex()
			{
				int num3 = cardsCell.GetPreflopRangeIndex(pokerGames) * pokerGames.GetFullPreflopRangeCellsCount() + otherCell.GetPreflopRangeIndex(pokerGames);
				if (!pokerGames.IsShortDeckFamily())
				{
					return Tables.EquityTexasHoldemTableCellVsCellIndexes[num3];
				}
				return Tables.EquityShortDeckTableCellVsCellIndexes[num3];
			}



			int PairVsOfSuit()
			{
				if (cards.HasNoSuitBlockers(other))
				{
					return startIndex;
				}
				if (cardsCell.IsTwoSuitBlockersPossible(otherCell))
				{
					startIndex++;
				}
				if (cards.HasTwoSuitBlockers(other))
				{
					return startIndex;
				}
				if (!cardsCell.IsTwoSuitBlockersPossible(otherCell))
				{
					return startIndex + 1;
				}
				startIndex++;
				if (cards.HasSuitBlockerAnyVsHigh(other))
				{
					return startIndex;
				}
				return startIndex + 1;
			}
			int PairVsPair()
			{
				if (cards.HasNoSuitBlockers(other))
				{
					return startIndex;
				}
				if (cards.HasOneSuitBlocker(other))
				{
					return startIndex + 1;
				}
				return startIndex + 2;
			}
			int PairVsSuit()
			{
				if (cards.HasNoSuitBlockers(other))
				{
					return startIndex;
				}
				return startIndex + 1;
			}
			int SuitedVsOfSuited()
			{
				if (cards.HasNoSuitBlockers(other))
				{
					return startIndex;
				}
				startIndex++;
				if (cards.HasSuitBlockerAnyVsHigh(other))
				{
					return startIndex;
				}
				if (cardsCell.Row != otherCell.Column && cardsCell.Column != otherCell.Column)
				{
					startIndex++;
				}
				return startIndex;
			}
			int SuitedVsSuited()
			{
				if (!cards.HasSuitBlockerAny(other))
				{
					return startIndex;
				}
				if (!cardsCell.HasRankBlocker(otherCell))
				{
					startIndex++;
				}
				return startIndex;
			}
		}
	}

	public static IEnumerable<long> GenerateAllBoards(long[] pockets, long boardCards, PokerGames game)
	{
		long deadCards = pockets.AsSpan().CardsMasksToSingleMask() | boardCards;
		int cardsToGenerate = 5 - boardCards.GetCardIndexesFromCardMaskPeval().Count();
		foreach (long item in GenerateBoard(0, -1, boardCards, deadCards))
		{
			yield return item;
		}
		IEnumerable<long> GenerateBoard(int generatedCards, int lastCard, long num2, long num4)
		{
			generatedCards++;
			for (int card = lastCard + 1; card < 52; card++)
			{
				if (card.IsCardIndexHitsPokerGame(game))
				{
					long num = num2;
					long num3 = card.CardIndexToCardMaskPeval();
					if ((num3 & num4) == 0L)
					{
						num |= num3;
						if (generatedCards == cardsToGenerate)
						{
							yield return num;
						}
						else
						{
							int generatedCards2 = generatedCards;
							int lastCard2 = card;
							long boardCards2 = num;
							long deadCards2;
							num4 = (deadCards2 = num4 | num3);
							foreach (long item2 in GenerateBoard(generatedCards2, lastCard2, boardCards2, deadCards2))
							{
								yield return item2;
							}
						}
					}
				}
			}
		}
	}

	internal static SeatMap<double> ComputeExactEquityHoldem(this SeatMap<long> pockets, long deadCards, PokerGames game)
	{
		SeatMap<double> seats = pockets.ComputeBoardsWon(deadCards, game);
		double divisor = seats.Sum();
		return seats.Divide(divisor);
	}

	internal static SeatMap<double> ComputePreflopExactEquityHoldem(this SeatMap<IPocketCards> cards, PokerGames game)
	{
		return cards.MapToCardMasks().ComputeExactEquityHoldem(0L, game);
	}

	public static SeatMap<double> ComputeExactEquityHoldem(this IPocketCards cards1, IPocketCards cards2)
	{
		SeatMap<IPocketCards> pockets = default;
		pockets.Set(1, cards1);
		pockets.Set(2, cards2);
		return pockets.MapToCardMasks().ComputeExactEquityHoldem(0L, PokerGames.TexasHoldem);
	}

	public static SeatMap<double> ComputeBoardsWon(this SeatMap<long> pockets, long deadCards, PokerGames game)
	{
		_ = pockets.Length;
		SeatMap<double> result = default(SeatMap<double>);
		GameBoards gameBoards = game.GetGameBoards();
		long pocketsMask = pockets.Values.AsSpan().CardsMasksToSingleMask();
		foreach (SuitsBoards board in gameBoards.Boards)
		{
			HandleBoards(pockets, board.Boards, board.Suit, board.Count, ref result);
		}
		return result;
		void HandleBoards(SeatMap<long> seats, List<long> boards, Suits suit, int boardSuitedCards, ref SeatMap<double> result2)
		{
			SeatMap<bool> isFlushPossible = seats.Map((long cardsMask) => cardsMask.GetSuitsCount(suit) + boardSuitedCards >= 5);
			for (int num = 0; num < boards.Count; num++)
			{
				HandleBoard(seats, boards[num], ref result2);
			}
			void HandleBoard(SeatMap<long> pockets2, long board, ref SeatMap<double> wins)
			{
				if ((board & deadCards) == deadCards && (board & pocketsMask) == 0L)
				{
					wins.IncrementWins(pockets2, board, 1.0, game, isFlushPossible);
				}
			}
		}
	}


	public static SeatMap<double> ParallelComputeExactEquityHoldem(this IPocketCards cards1, IPocketCards cards2, PokerGames game)
	{
		SeatMap<IPocketCards> pockets = default;
		pockets.Set(1, cards1);
		pockets.Set(2, cards2);
		return pockets.MapToCardMasks().ParallelComputeExactEquityHoldem(0L, game);
	}

	internal static SeatMap<double> ParallelComputeExactEquityHoldem(this SeatMap<long> pockets, long deadCards, PokerGames game)
	{
		SeatMap<double> seats = pockets.ParallelComputeBoardsWon(deadCards, game);
		double divisor = seats.Sum();
		return seats.Divide(divisor);
	}

	public static SeatMap<double> ParallelComputeBoardsWon(this SeatMap<long> pockets, long deadCards, PokerGames game)
	{
		_ = pockets.Length;
		SeatMap<double> result = default(SeatMap<double>);
		GameBoards gameBoards = game.GetGameBoards();
		long pocketsMask = pockets.Values.AsSpan().CardsMasksToSingleMask();

		// Parallelize the outer loop
		System.Threading.Tasks.Parallel.ForEach(gameBoards.Boards, board =>
		{
			HandleBoards(pockets, board.Boards, board.Suit, board.Count, ref result);
		});

		return result;

		void HandleBoards(SeatMap<long> seats, List<long> boards, Suits suit, int boardSuitedCards, ref SeatMap<double> result2)
		{
			SeatMap<bool> isFlushPossible = seats.Map((long cardsMask) => cardsMask.GetSuitsCount(suit) + boardSuitedCards >= 5);
			for (int num = 0; num < boards.Count; num++)
			{
				HandleBoard(seats, boards[num], ref result2);
			}
			void HandleBoard(SeatMap<long> pockets2, long board, ref SeatMap<double> wins)
			{
				if ((board & deadCards) == deadCards && (board & pocketsMask) == 0L)
				{
					wins.IncrementWins(pockets2, board, 1.0, game, isFlushPossible);
				}
			}
		}
	}

	public static BoardMap<SeatMap<double>> ComputeEquities<T>(this in SeatMap<T> players, PokerGames game, int preflopTrials, IBoard board, Span<Card> deadCards) where T : IHasPocketCards
	{
		BoardMap<SeatMap<double>> result = default(BoardMap<SeatMap<double>>);
		BoardMap<Board>.Enumerator enumerator = board.GetBoardMap().GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (boardNumber, board2) = enumerator.Current;
			result.Set(boardNumber, ComputeEquities(in players, game, preflopTrials, board2, deadCards));
		}
		return result;
	}

	public static SeatMap<double> ComputeEquities<T>(this in SeatMap<T> players, PokerGames game, int preflopTrials, Board board, Span<Card> deadCards) where T : IHasPocketCards
	{
		return players.Map((T player) => player.PocketCards ?? throw new InvalidOperationException($"Pockets cards of the player {player} was null")).ComputeEquities(game, preflopTrials, board, deadCards);
	}

	public static BoardMap<SeatMap<double>> ComputeEquities(this in SeatMap<IPocketCards> pockets, PokerGames game, int preflopTrials, IBoard board, Span<Card> deadCards)
	{
		BoardMap<SeatMap<double>> result = default(BoardMap<SeatMap<double>>);
		BoardMap<Board>.Enumerator enumerator = board.GetBoardMap().GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (boardNumber, board2) = enumerator.Current;
			result.Set(boardNumber, pockets.ComputeEquities(game, preflopTrials, board2, deadCards));
		}
		return result;
	}

	public static SeatMap<double> ComputeEquities(this in SeatMap<IPocketCards> pockets, PokerGames game, int preflopTrials, Board board, Span<Card> deadCards)
	{
		return board.Street switch
		{
			Streets.Preflop => pockets.ComputeEquityPreflop(game, preflopTrials, deadCards),
			Streets.Flop => pockets.ComputeEquitiesFlop(board, game, deadCards),
			Streets.Turn => pockets.ComputeEquitiesTurn(board, game, deadCards),
			Streets.River => throw new ArgumentException("Can't compute equity on river", "board"),
			_ => throw new NotImplementedException(),
		};
	}

	public static InlineList<(IPreflopRange range, double equity)> ComputeEquity(this IList<IPreflopRange> playerRanges, PokerGames game, BoardRange boardRange, InlineList<Card> deadCards, int trials)
	{
		Verify();
		if (boardRange.ContainsDeadCards(deadCards))
		{
			throw new ArgumentException("Given range contains dead cards. It is responsibility of the caller to remove dead cards.", "boardRange");
		}
		List<long> items = boardRange.GetRiverBoardsRange(deadCards, game).GetBoardMasksSample(trials).ToList();
		long[][] cardMasks = playerRanges.Select((IPreflopRange weightedRange) => weightedRange.ToPocketsRangeHoldem()).GetCardMasks();
		double[][] array = playerRanges.Select((IPreflopRange weightedRange) => (!(weightedRange is PocketsRangeWeightedHoldem rangeWeighted)) ? new double[0] : rangeWeighted.GetWeights()).ToArray();
		int num = cardMasks.Length;
		SeatMap<double> equities = default(SeatMap<double>);
		SeatMap<long> pockets = default(SeatMap<long>);
		FastRandom random = FastRandomPool.ThreadShared.GetRandom();
		int num2 = 0;
		for (int num3 = 0; num3 < trials; num3++)
		{
			long num4 = 0L;
			bool flag = true;
			for (int num5 = 0; num5 < num; num5++)
			{
				long num6 = ((array[num5].Length == 0) ? cardMasks[num5].GetRandomItem(random) : cardMasks[num5].GetRandomItem(array[num5], random));
				if ((num4 & num6) != 0L)
				{
					flag = false;
					break;
				}
				num4 |= num6;
				pockets.Set(num5 + 1, num6);
			}
			if (flag)
			{
				long randomItem = items.GetRandomItem(random);
				if ((randomItem & num4) == 0L)
				{
					num2++;
					equities.IncrementWins(pockets, randomItem, 1.0, game);
				}
			}
		}
		if (num2 == 0)
		{
			throw new ImpossibleSimulationException();
		}
		SeatNumberFlags.Enumerator enumerator = equities.SeatNumberFlags.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			equities.Set(current, equities.Get(current) / (double)num2);
		}
		return playerRanges.WithIndex().MapToInlineList(((int index, IPreflopRange value) item) => (preflopRange: item.value, equity: equities.Get(item.index + 1)));
		void Verify()
		{
			if (game.RequiresPocketsInHandCombo())
			{
				throw new InvalidOperationException($"Call another overload for {game}");
			}
			if (game == PokerGames.ShortDeck)
			{
				playerRanges.ForEach(delegate (IPreflopRange range)
				{
					range.VerifyShortDeck();
				});
				boardRange.VerifyShortDeck();
			}
			trials.VerifyArgumentPositive("trials");
			foreach (IPreflopRange playerRange in playerRanges)
			{
				playerRange.VerifyNotEmpty();
				if (playerRange.GetPocketCardsHoldem().ContainsDeadCards(deadCards))
				{
					throw new ArgumentException("Given range contains dead cards. It is responsibility of the caller to remove dead cards.", "playerRanges");
				}
			}
		}
	}

	public static SeatMap<double> ComputeEquityPreflop(this SeatMap<IPocketCards> pockets, PokerGames game, int trials, Span<Card> deadCards)
	{
		if (pockets.Length < 2)
		{
			throw new ArgumentException("pockets");
		}
		if (trials <= 0)
		{
			throw new ArgumentException("trials");
		}
		if (game.IsTwoPocketsGame() && pockets.Length == 2)
		{
			(double firstPlayerEquity, double secondPlayerEquity) holdemPreflopEquityHeadsUp = pockets.First.VerifyType<PocketCardsHoldem>().GetHoldemPreflopEquityHeadsUp(pockets.Last.VerifyType<PocketCardsHoldem>(), game);
			double item = holdemPreflopEquityHeadsUp.firstPlayerEquity;
			double item2 = holdemPreflopEquityHeadsUp.secondPlayerEquity;
			SeatMap<double> result = default(SeatMap<double>);
			result.Set(pockets.FirstSeatNumber, item);
			result.Set(pockets.LastSeatNumber, item2);
			return result;
		}
		if (!game.RequiresPocketsInHandCombo())
		{
			return ComputeEquityPreflop(pockets.MapToCardMasks(), deadCards.ToCardsMask(), game, trials);
		}
		return pockets.ComputeEquityPreflopCactus(game, trials, deadCards);
	}

	internal static SeatMap<double> ComputeEquityPreflop(SeatMap<long> pockets, long deadCards, PokerGames game, int trials)
	{
		long deadCardsMask = pockets.Values.AsSpan().CardsMasksToSingleMask() | deadCards;
		FastRandom random = FastRandomPool.ThreadShared.GetRandom();
		SeatMap<double> wins = default(SeatMap<double>);
		int cardsInDeck = game.CardsInDeck();
		for (int i = 0; i < trials; i++)
		{
			long randomCardsMasks = random.GetRandomCardsMasks(deadCardsMask, 5, cardsInDeck);
			wins.IncrementWins(pockets, randomCardsMasks, 1.0, game);
		}
		return wins.Divide(trials);
	}

	internal static SeatMap<double> ComputeEquityPreflopCactus(this SeatMap<IPocketCards> pockets, PokerGames game, int trials, Span<Card> deadCards)
	{
		long deadCardsIndexesMask = deadCards.ToIndexCardsMask() | pockets.ToIndexCardMask();
		_ = pockets.Length;
		FastRandom random = FastRandomPool.ThreadShared.GetRandom();
		SeatMap<double> wins = default(SeatMap<double>);
		SeatMap<InlineList<int>> pockets2 = pockets.MapToCardMaskCactus();
		int cardsInDeck = game.CardsInDeck();
		for (int i = 0; i < trials; i++)
		{
			InlineList<int> randomCardsMasksCactus = random.GetRandomCardsMasksCactus(deadCardsIndexesMask, 5, cardsInDeck);
			wins.IncrementWinsCactus(in pockets2, randomCardsMasksCactus, game, 1.0);
		}
		return wins.Divide(trials);
	}

	internal static double GetEquityAgainstRandomHand(this IPocketCards pocket, PokerGames game, int trials)
	{
		int cardsInDeck = game.CardsInDeck();
		FastRandom random = FastRandomPool.ThreadShared.GetRandom();
		int cardsCount = game.PocketCardsCount();
		long num = pocket.Cards.ToIndexCardsMask();
		InlineList<int> value = pocket.Cards.Map((Card card) => card.ToCardMaskCactus());
		SeatMap<double> wins = default(SeatMap<double>);
		for (int num2 = 0; num2 < trials; num2++)
		{
			InlineList<Card> items = random.GetRandomCardsInlineList(cardsCount, cardsInDeck, num);
			long indexMask = items.GetIndexMask();
			InlineList<int> randomCardsMasksCactus = random.GetRandomCardsMasksCactus(indexMask | num, 5, cardsInDeck);
			SeatMap<InlineList<int>> pockets = default(SeatMap<InlineList<int>>);
			pockets.Set(1, value);
			pockets.Set(2, items.Map((Card card) => card.ToCardMaskCactus()));
			wins.IncrementWinsCactus(in pockets, randomCardsMasksCactus, game, 1.0);
		}
		return wins.Get(1) / (double)trials;
	}

	public static SeatMap<double> ComputeEquitiesFlop(this in SeatMap<IPocketCards> pockets, Board board, PokerGames game, Span<Card> deadCards)
	{
		if (pockets.Length < 2)
		{
			throw new ArgumentException("pockets");
		}
		if (board.Street != Streets.Flop)
		{
			throw new ArgumentException("board");
		}
		if (!game.RequiresPocketsInHandCombo())
		{
			return ComputeEquityFlop(pockets.MapToCardMasks(), board.ToBoardCardsMask(), deadCards.ToCardsMask(), game);
		}
		return ComputeEquityFlopCactus(pockets, board, game, deadCards);
	}

	internal static SeatMap<double> ComputeEquityFlopCactus(SeatMap<IPocketCards> pockets, Board board, PokerGames game, Span<Card> deadCards)
	{
		long num = deadCards.ToIndexCardsMask() | pockets.ToIndexCardMask() | board.Cards.ToIndexCardsMask();
		_ = pockets.Length;
		SeatMap<InlineList<int>> pockets2 = pockets.Map((IPocketCards cards) => cards.ToCardsMaskCactus());
		SeatMap<double> wins = default(SeatMap<double>);
		InlineList<int> board2 = new InlineList<int>(5);
		for (int num2 = 0; num2 < 3; num2++)
		{
			board2[num2] = board.Cards[num2].ToCardMaskCactus();
		}
		int num3 = 0;
		for (int num4 = 0; num4 < 51; num4++)
		{
			if (!num4.IsCardIndexHitsPokerGame(game) || (num & (1L << num4)) != 0L)
			{
				continue;
			}
			board2[3] = num4.CardIndexToCardMaskCactus();
			for (int num5 = num4 + 1; num5 < 52; num5++)
			{
				if (num5.IsCardIndexHitsPokerGame(game) && (num & (1L << num5)) == 0L)
				{
					board2[4] = num5.CardIndexToCardMaskCactus();
					num3++;
					wins.IncrementWinsCactus(in pockets2, board2, game, 1.0);
				}
			}
		}
		return wins.Divide(num3);
	}

	internal static SeatMap<double> ComputeEquityFlop(SeatMap<long> pockets, long board, long deadCards, PokerGames game)
	{
		long num = pockets.Values.AsSpan().CardsMasksToSingleMask() | board;
		deadCards |= num;
		_ = pockets.Length;
		SeatMap<bool> isFlushPossible = default(SeatMap<bool>);
		bool flag = false;
		SeatMap<long>.Enumerator enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int seatNumber, long value) current = enumerator.Current;
			var (seatNumber, _) = current;
			if ((current.value | board).HasMinSuitedCardsCount(3))
			{
				isFlushPossible.Set(seatNumber, value: true);
				flag = true;
			}
			else
			{
				isFlushPossible.Set(seatNumber, value: false);
			}
		}
		SeatMap<double> wins = default(SeatMap<double>);
		int num2 = 0;
		if (flag)
		{
			for (int i = 0; i < 51; i++)
			{
				if (!i.IsCardIndexHitsPokerGame(game))
				{
					continue;
				}
				long num3 = i.CardIndexToCardMaskPeval();
				if ((num3 & deadCards) != 0L)
				{
					continue;
				}
				for (int j = i + 1; j < 52; j++)
				{
					if (j.IsCardIndexHitsPokerGame(game))
					{
						long num4 = j.CardIndexToCardMaskPeval();
						if ((num4 & deadCards) == 0L)
						{
							long board2 = board | num3 | num4;
							num2++;
							wins.IncrementWins(pockets, board2, 1.0, game, isFlushPossible);
						}
					}
				}
			}
			return wins.Divide(num2);
		}
		double num5 = 0.0;
		for (int k = (int)game.MinCardRank(); k <= 12; k++)
		{
			int rankCountFromCardsMask = deadCards.GetRankCountFromCardsMask(k);
			if (rankCountFromCardsMask >= 4)
			{
				continue;
			}
			for (int l = ((rankCountFromCardsMask == 3) ? (k + 1) : k); l <= 12; l++)
			{
				int rankCountFromCardsMask2 = deadCards.GetRankCountFromCardsMask(l);
				if (rankCountFromCardsMask2 < 4)
				{
					long num6 = GetCardOfRank(deadCards, k);
					long num7 = GetCardOfRank(deadCards | num6, l);
					long board3 = board | num6 | num7;
					double num8 = ((k != l) ? ((double)((4 - rankCountFromCardsMask) * (4 - rankCountFromCardsMask2)) / 16.0) : ((double)((4 - rankCountFromCardsMask) * (3 - rankCountFromCardsMask2)) / 32.0));
					num5 += num8;
					wins.IncrementWins(pockets, board3, num8, game, isFlushPossible);
				}
			}
		}
		return wins.Divide(num5);
		static long GetCardOfRank(long cards, int rank)
		{
			for (int m = 0; m <= 3; m++)
			{
				long num9 = (rank: (CardRanks)rank, suit: (Suits)m).ToCardMaskPeval();
				if ((num9 & cards) == 0L)
				{
					return num9;
				}
			}
			return 0L;
		}
	}

	public static SeatMap<double> ComputeEquitiesTurn(this in SeatMap<IPocketCards> pockets, Board board, PokerGames game, Span<Card> deadCards)
	{
		if (pockets.Length < 2)
		{
			throw new ArgumentException("Excepting pockets of at least 2 players in order to compute equity", "pockets");
		}
		if (board.Street != Streets.Turn)
		{
			throw new ArgumentException("board");
		}
		if (game.IsOmahaFamily())
		{
			return pockets.ComputeEquityTurnCactus(board, game, deadCards);
		}
		return ComputeEquitiesTurn(pockets.MapToCardMasks(), board.ToBoardCardsMask(), deadCards.ToCardsMask(), game);
	}

	private static long ToIndexCardMask(this Card card)
	{
		return 1L << card.Index;
	}

	public static long ToIndexCardMask(this SeatMap<IPocketCards> pockets)
	{
		long num = 0L;
		InlineList<IPocketCards>.Enumerator enumerator = pockets.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			IPocketCards current = enumerator.Current;
			num |= current.Cards.ToIndexCardsMask();
		}
		return num;
	}

	private static long ToIndexCardsMask(this InlineList<Card> cards)
	{
		return ((Span<Card>)cards).ToIndexCardsMask();
	}

	private static long ToIndexCardsMask(this Span<Card> cards)
	{
		long num = 0L;
		Span<Card> span = cards;
		for (int i = 0; i < span.Length; i++)
		{
			Card card = span[i];
			num |= card.ToIndexCardMask();
		}
		return num;
	}

	internal static SeatMap<double> ComputeEquityTurnCactus(this in SeatMap<IPocketCards> pockets, Board board, PokerGames game, Span<Card> deadCards)
	{
		long num = deadCards.ToIndexCardsMask() | pockets.ToIndexCardMask() | board.Cards.ToIndexCardsMask();
		_ = pockets.Length;
		int num2 = 0;
		new InlineList<double>(pockets.Length);
		SeatMap<double> wins = default(SeatMap<double>);
		SeatMap<InlineList<int>> pockets2 = pockets.Map((IPocketCards x) => x.Cards.ToCardsMasksCactus());
		int[] array = new int[5];
		for (int num3 = 0; num3 < 4; num3++)
		{
			array[num3] = board.Cards[num3].ToCardMaskCactus();
		}
		for (int num4 = 0; num4 < 52; num4++)
		{
			if (num4.IsCardIndexHitsPokerGame(game) && (num & (1L << num4)) == 0L)
			{
				array[4] = num4.CardIndexToCardMaskCactus();
				num2++;
				wins.IncrementWinsCactus(in pockets2, array.ToInlineList(), game, 1.0);
			}
		}
		return wins.Divide(num2);
	}

	internal static SeatMap<double> ComputeEquitiesTurn(SeatMap<long> pockets, long board, long deadCards, PokerGames game)
	{
		if (board == 0L)
		{
			throw new ArgumentException("board");
		}
		long num = pockets.Values.AsSpan().CardsMasksToSingleMask() | board;
		deadCards |= num;
		int num2 = 0;
		SeatMap<double> wins = default(SeatMap<double>);
		SeatMap<bool> isFlushPossible = default(SeatMap<bool>);
		SeatMap<long>.Enumerator enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, num3) = enumerator.Current;
			isFlushPossible.Set(seatNumber, (num3 | board).HasMinSuitedCardsCount(4));
		}
		for (int i = 0; i < 52; i++)
		{
			if (i.IsCardIndexHitsPokerGame(game))
			{
				long num4 = i.CardIndexToCardMaskPeval();
				if ((deadCards & num4) == 0L)
				{
					num2++;
					wins.IncrementWins(pockets, board | num4, 1.0, game, isFlushPossible);
				}
			}
		}
		return wins.Divide(num2);
	}

	internal static SeatMap<double> ComputeEquitiesTurn(SeatMap<long> pockets, long[][] pocketsSeparate, long board, Span<long> boardSeparate, long deadCards, PokerGames game)
	{
		if (board == 0L)
		{
			throw new ArgumentException("board");
		}
		long num = pockets.Values.AsSpan().CardsMasksToSingleMask() | board;
		deadCards |= num;
		int num2 = 0;
		SeatMap<double> wins = default(SeatMap<double>);
		long[] array = new long[5];
		for (int i = 0; i < 4; i++)
		{
			array[i] = boardSeparate[i];
		}
		for (int j = 0; j < 52; j++)
		{
			if (j.IsCardIndexHitsPokerGame(game))
			{
				long num3 = j.CardIndexToCardMaskPeval();
				if ((deadCards & num3) == 0L)
				{
					num2++;
					array[4] = num3;
					wins.IncrementWins(pockets, pocketsSeparate, board | num3, array, game, 1.0);
				}
			}
		}
		return wins.Divide(num2);
	}

	private static void IncrementWins(this ref SeatMap<double> wins, SeatMap<long> pockets, long board, double incrementPerWin, PokerGames game, SeatMap<bool> isFlushPossible)
	{
		int num = 0;
		int num2 = 0;
		SeatMap<int> seatMap = default(SeatMap<int>);
		SeatMap<long>.Enumerator enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int seatNumber, long value) current = enumerator.Current;
			int item = current.seatNumber;
			CardsMaskU cardsMaskU = (current.value | board).CardsMaskToCardsMaskU();
			int num3 = Evaluate.EvaluateHandRank7Cards(cardsMaskU, cardsMaskU.RanksMask(), game, isFlushPossible.Get(item));
			if (num3 > num)
			{
				num = num3;
				num2 = 1;
			}
			else if (num3 == num)
			{
				num2++;
			}
			seatMap.Set(item, num3);
		}
		enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int item2 = enumerator.Current.seatNumber;
			if (seatMap.Get(item2) == num)
			{
				wins.Increment(item2, incrementPerWin / (double)num2);
			}
		}
	}

	private static void IncrementWins(this ref SeatMap<double> wins, SeatMap<long> pockets, long board, double incrementPerWin, PokerGames game)
	{
		int num = 0;
		int num2 = 0;
		SeatMap<int> seatMap = default(SeatMap<int>);
		SeatMap<long>.Enumerator enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int seatNumber, long value) current = enumerator.Current;
			int item = current.seatNumber;
			CardsMaskU cardsMaskU = (current.value | board).CardsMaskToCardsMaskU();
			int num3 = Evaluate.EvaluateHandRank7Cards(cardsMaskU, cardsMaskU.RanksMask(), game);
			if (num3 > num)
			{
				num = num3;
				num2 = 1;
			}
			else if (num3 == num)
			{
				num2++;
			}
			seatMap.Set(item, num3);
		}
		enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int item2 = enumerator.Current.seatNumber;
			if (seatMap.Get(item2) == num)
			{
				wins.Increment(item2, incrementPerWin / (double)num2);
			}
		}
	}

	private static void IncrementWinsCactus(this ref SeatMap<double> wins, in SeatMap<InlineList<int>> pockets, InlineList<int> board, PokerGames game, double incrementPerWin)
	{
		int num = 0;
		int num2 = 0;
		SeatMap<int> seatMap = default(SeatMap<int>);
		SeatMap<InlineList<int>>.Enumerator enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int seatNumber, InlineList<int> value) current = enumerator.Current;
			int item = current.seatNumber;
			int num3 = Evaluate.EvaluateHandRankRiverCactus(current.value, board, game);
			if (num3 > num)
			{
				num = num3;
				num2 = 1;
			}
			else if (num3 == num)
			{
				num2++;
			}
			seatMap.Set(item, num3);
		}
		enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int item2 = enumerator.Current.seatNumber;
			if (seatMap.Get(item2) == num)
			{
				wins.Increment(item2, incrementPerWin / (double)num2);
			}
		}
	}

	private static void IncrementWins(this ref SeatMap<double> wins, SeatMap<long> pockets, long[][] pocketsSeparate, long board, Span<long> boardSeparate, PokerGames game, double incrementPerWin)
	{
		int num = 0;
		int num2 = 0;
		SeatMap<int> seatMap = default(SeatMap<int>);
		SeatMap<long>.Enumerator enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int item = enumerator.Current.seatNumber;
			int num3 = Evaluate.EvaluateHandRankRiver(pockets.Values.AsSpan(), pocketsSeparate, board, boardSeparate, game);
			if (num3 > num)
			{
				num = num3;
				num2 = 1;
			}
			else if (num3 == num)
			{
				num2++;
			}
			seatMap.Set(item, num3);
		}
		enumerator = pockets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int item2 = enumerator.Current.seatNumber;
			if (seatMap.Get(item2) == num)
			{
				wins.Increment(item2, incrementPerWin / (double)num2);
			}
		}
	}

	public static int CountSuitBlockers(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		int num = 0;
		if (cards.HighCard.Suit == other.HighCard.Suit || cards.HighCard.Suit == other.LowCard.Suit)
		{
			num++;
		}
		if (cards.LowCard.Suit == other.HighCard.Suit || cards.LowCard.Suit == other.LowCard.Suit)
		{
			num++;
		}
		return num;
	}

	public static bool HasNoSuitBlockers(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.HighCard.Suit == other.HighCard.Suit)
		{
			return false;
		}
		if (cards.LowCard.Suit == other.HighCard.Suit)
		{
			return false;
		}
		if (cards.HighCard.Suit == other.LowCard.Suit)
		{
			return false;
		}
		if (cards.LowCard.Suit == other.LowCard.Suit)
		{
			return false;
		}
		return true;
	}

	public static bool HasSuitBlockerAny(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		return !cards.HasNoSuitBlockers(other);
	}

	public static bool HasSuitBlockerAnyVsHigh(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.HighCard.Suit == other.HighCard.Suit)
		{
			if (cards.HighCard.Suit != other.LowCard.Suit)
			{
				return cards.LowCard.Suit != other.LowCard.Suit;
			}
			return false;
		}
		if (cards.LowCard.Suit == other.HighCard.Suit)
		{
			if (cards.HighCard.Suit != other.LowCard.Suit)
			{
				return cards.LowCard.Suit != other.LowCard.Suit;
			}
			return false;
		}
		return false;
	}

	public static bool HasSuitBlockerAnyVsLow(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.HighCard.Suit == other.LowCard.Suit)
		{
			if (cards.HighCard.Suit != other.HighCard.Suit)
			{
				return cards.LowCard.Suit != other.HighCard.Suit;
			}
			return false;
		}
		if (cards.LowCard.Suit == other.LowCard.Suit)
		{
			if (cards.HighCard.Suit != other.HighCard.Suit)
			{
				return cards.LowCard.Suit != other.HighCard.Suit;
			}
			return false;
		}
		return false;
	}

	public static bool HasTwoSuitBlockers(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.HighCard.Suit != other.HighCard.Suit && cards.HighCard.Suit != other.LowCard.Suit)
		{
			return false;
		}
		if (cards.LowCard.Suit != other.HighCard.Suit && cards.LowCard.Suit != other.LowCard.Suit)
		{
			return false;
		}
		return true;
	}

	public static bool IsTwoSuitBlockersPossible(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.Row != other.Row)
		{
			return cell.Column != other.Column;
		}
		return false;
	}

	public static bool HasOneSuitBlocker(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		return cards.CountSuitBlockers(other) == 1;
	}

	public static bool HasTwoRankBlockers(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		return cards.Cards.All((Card x) => other.Cards.Any((Card y) => y.Rank == x.Rank));
	}

	public static bool HasOneRankBlocker(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		return cards.Cards.Count((Card x) => other.Cards.Any((Card y) => y.Rank == x.Rank)) == 1;
	}

	public static bool HasRankBlocker(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		return cell.Ranks().Any((CardRanks x) => other.Ranks().Any((CardRanks y) => y == x));
	}

	public static bool HasSuitBlockerHighRankVsHighRank(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.HighCard.Suit == other.HighCard.Suit)
		{
			return cards.LowCard.Suit != other.LowCard.Suit;
		}
		return false;
	}

	public static bool HasSuitBlockerHighRankVsLowRank(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.HighCard.Suit == other.LowCard.Suit)
		{
			return cards.LowCard.Suit != other.HighCard.Suit;
		}
		return false;
	}

	public static bool HasSuitBlockerLowRankVsHighRank(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.LowCard.Suit == other.HighCard.Suit)
		{
			return cards.HighCard.Suit != other.LowCard.Suit;
		}
		return false;
	}

	public static bool HasSuitBlockerLowRankVsLowRank(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		if (cards.LowCard.Suit == other.LowCard.Suit)
		{
			return cards.HighCard.Suit != other.HighCard.Suit;
		}
		return false;
	}

	public static bool Intersects(this PocketCardsHoldem cards, PocketCardsHoldem other)
	{
		return cards.Cards.Intersects(other.Cards);
	}
}
