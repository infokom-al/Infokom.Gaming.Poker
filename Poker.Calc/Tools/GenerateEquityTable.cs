namespace Poker.Calc.Tools;

public static class GenerateEquityTable
{
	private static readonly PokerGames Game = PokerGames.TexasHoldem;

	private static readonly int PairVsAllSuited = ((Game == PokerGames.TexasHoldem) ? 144 : 64);

	private static readonly int PairVsAllOffsuited = ((Game == PokerGames.TexasHoldem) ? 288 : 128);

	private static readonly int SuitedVsAllOffsuited = ((Game == PokerGames.TexasHoldem) ? 210 : 92);

	private static readonly int AllPairs = ((Game == PokerGames.TexasHoldem) ? 5863 : 1845);

	private static readonly int AllSuited = ((Game == PokerGames.TexasHoldem) ? 21606 : 4356);

	private static readonly int Cells = ((Game == PokerGames.TexasHoldem) ? 169 : 4356);

	private static PreflopRangeHoldemCell LastSuitedCell => "32s".ParsePreflopRangeHoldemCell();

	private static PreflopRangeHoldemCell LastOffsuitedCell => "32o".ParsePreflopRangeHoldemCell();

	public static (List<double>, List<(string, double)>) Generate()
	{
		return GenerateEquityLookUpTable(Game);
	}

	public static (List<double>, List<(string, double)>) GenerateEquityLookUpTable(PokerGames game)
	{
		List<double> result = new List<double>();
		List<(string, double)> resultToCards = new List<(string, double)>();
		PreflopRangeHoldem preflopRange = ((game == PokerGames.TexasHoldem) ? PreflopRangeHoldem.FullRangeTexasHoldem : PreflopRangeHoldem.FullRangeShortDeck);
		foreach (PreflopRangeHoldemCell pairCell in preflopRange.PairCells)
		{
			PairVsSuited(pairCell);
			PairVsOfSuited(pairCell);
			PairVsPair(pairCell);
		}
		preflopRange.SuitedCells.AsParallel().AsOrdered().ForEach(delegate (PreflopRangeHoldemCell x)
		{
			SuitedVsOfSuited(x);
			SuitedVsSuited(x);
		});
		preflopRange.OffsuitedCells.Where((PreflopRangeHoldemCell x) => !x.IsPair).AsParallel().AsOrdered()
			.ForEach(OfSuitedVsOfSuited);
		return (result, resultToCards);
		void AddNewResult(PocketCardsHoldem cards, PocketCardsHoldem other)
		{
			double first = cards.ComputeExactEquityHoldem(other).First;
			result.Add(first);
			resultToCards.Add(($"{cards} vs {other}", first));
		}
		static PocketCardsHoldem GetPocketCardsWithAnySuitBlocker(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard in cell.GetPocketCards(cards))
			{
				if (cards.HasSuitBlockerAny(pocketCard))
				{
					return pocketCard;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithNoSuitBlockers(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard2 in cell.GetPocketCards(cards))
			{
				if (cards.HasNoSuitBlockers(pocketCard2))
				{
					return pocketCard2;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithOneSuitBlocker(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCards in cell.GetPocketCards(cards))
			{
				if (cards.Cards.Count((Card x) => pocketCards.Cards.Any((Card y) => y.Suit == x.Suit)) == 1)
				{
					return pocketCards;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithSuitBlockerAnyVsHigh(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard3 in cell.GetPocketCards(cards))
			{
				if (cards.HasSuitBlockerAnyVsHigh(pocketCard3))
				{
					return pocketCard3;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithSuitBlockerAnyVsLow(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard4 in cell.GetPocketCards(cards))
			{
				if (cards.HasSuitBlockerAnyVsLow(pocketCard4))
				{
					return pocketCard4;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithSuitBlockerHighRankVsHighRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard5 in cell.GetPocketCards(cards))
			{
				if (cards.HasSuitBlockerHighRankVsHighRank(pocketCard5))
				{
					return pocketCard5;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithSuitBlockerHighRankVsLowRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard6 in cell.GetPocketCards(cards))
			{
				if (cards.HasSuitBlockerHighRankVsLowRank(pocketCard6))
				{
					return pocketCard6;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithSuitBlockerLowRankVsHighRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard7 in cell.GetPocketCards(cards))
			{
				if (cards.HasSuitBlockerLowRankVsHighRank(pocketCard7))
				{
					return pocketCard7;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithSuitBlockerLowRankVsLowRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard8 in cell.GetPocketCards(cards))
			{
				if (cards.HasSuitBlockerLowRankVsLowRank(pocketCard8))
				{
					return pocketCard8;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithTwoSuitBlockers(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard9 in cell.GetPocketCards(cards))
			{
				if (cards.HasTwoSuitBlockers(pocketCard9))
				{
					return pocketCard9;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithTwoSuitBlockersHighVsHigh(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard10 in cell.GetPocketCards(cards))
			{
				if (cards.HasTwoSuitBlockers(pocketCard10) && cards.HighCard.Suit == pocketCard10.HighCard.Suit)
				{
					return pocketCard10;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		static PocketCardsHoldem GetPocketCardsWithTwoSuitBlockersHighVsLow(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (PocketCardsHoldem pocketCard11 in cell.GetPocketCards(cards))
			{
				if (cards.HasTwoSuitBlockers(pocketCard11) && cards.HighCard.Suit == pocketCard11.LowCard.Suit)
				{
					return pocketCard11;
				}
			}
			throw new InvalidOperationException("Should not get here");
		}
		void OfSuitedVsOfSuited(PreflopRangeHoldemCell cell)
		{
			PocketCardsHoldem cards = cell.GetPocketCards().First();
			foreach (PreflopRangeHoldemCell item in preflopRange.OffsuitedCells.Where((PreflopRangeHoldemCell x) => !x.IsPair && x.Row >= cell.Row))
			{
				if (item.Row != cell.Row || item.Column >= cell.Column)
				{
					AddNewResult(cards, GetPocketCardsWithNoSuitBlockers(item, cards));
					if (cell.Column != item.Column)
					{
						if (cell.Row != item.Row)
						{
							AddNewResult(cards, GetPocketCardsWithTwoSuitBlockersHighVsHigh(item, cards));
						}
						AddNewResult(cards, GetPocketCardsWithSuitBlockerHighRankVsHighRank(item, cards));
					}
					if (cell.Row != item.Column)
					{
						AddNewResult(cards, GetPocketCardsWithTwoSuitBlockersHighVsLow(item, cards));
						AddNewResult(cards, GetPocketCardsWithSuitBlockerLowRankVsHighRank(item, cards));
					}
					AddNewResult(cards, GetPocketCardsWithSuitBlockerHighRankVsLowRank(item, cards));
					if (cell.Row != item.Row)
					{
						AddNewResult(cards, GetPocketCardsWithSuitBlockerLowRankVsLowRank(item, cards));
					}
				}
			}
		}
		void PairVsOfSuited(PreflopRangeHoldemCell cell)
		{
			PocketCardsHoldem cards = cell.GetPocketCards().First();
			foreach (PreflopRangeHoldemCell item2 in preflopRange.OffsuitedCells.Where((PreflopRangeHoldemCell x) => !x.IsPair))
			{
				AddNewResult(cards, GetPocketCardsWithNoSuitBlockers(item2, cards));
				if (cell.Row != item2.Row && cell.Column != item2.Column)
				{
					AddNewResult(cards, GetPocketCardsWithTwoSuitBlockers(item2, cards));
				}
				if (cell.Column != item2.Column)
				{
					AddNewResult(cards, GetPocketCardsWithSuitBlockerAnyVsHigh(item2, cards));
				}
				if (cell.Row != item2.Row)
				{
					AddNewResult(cards, GetPocketCardsWithSuitBlockerAnyVsLow(item2, cards));
				}
			}
		}
		void PairVsPair(PreflopRangeHoldemCell cell)
		{
			PocketCardsHoldem cards = cell.GetPocketCards().First();
			foreach (PreflopRangeHoldemCell item3 in preflopRange.PairCells.Where((PreflopRangeHoldemCell x) => x.Row >= cell.Row))
			{
				AddNewResult(cards, GetPocketCardsWithNoSuitBlockers(item3, cards));
				if (cell.Row != item3.Row)
				{
					AddNewResult(cards, GetPocketCardsWithOneSuitBlocker(item3, cards));
					AddNewResult(cards, GetPocketCardsWithTwoSuitBlockers(item3, cards));
				}
			}
		}
		void PairVsSuited(PreflopRangeHoldemCell cell)
		{
			PocketCardsHoldem cards = cell.GetPocketCards().First();
			foreach (PreflopRangeHoldemCell suitedCell in preflopRange.SuitedCells)
			{
				AddNewResult(cards, GetPocketCardsWithNoSuitBlockers(suitedCell, cards));
				if (cell.Row != suitedCell.Row && cell.Column != suitedCell.Column)
				{
					AddNewResult(cards, GetPocketCardsWithAnySuitBlocker(suitedCell, cards));
				}
			}
		}
		void SuitedVsOfSuited(PreflopRangeHoldemCell cell)
		{
			PocketCardsHoldem cards = cell.GetPocketCards().First();
			foreach (PreflopRangeHoldemCell item4 in preflopRange.OffsuitedCells.Where((PreflopRangeHoldemCell x) => !x.IsPair))
			{
				AddNewResult(cards, GetPocketCardsWithNoSuitBlockers(item4, cards));
				if (cell.Row != item4.Column && cell.Column != item4.Column)
				{
					AddNewResult(cards, GetPocketCardsWithSuitBlockerAnyVsHigh(item4, cards));
				}
				if (cell.Column != item4.Row && cell.Row != item4.Row)
				{
					AddNewResult(cards, GetPocketCardsWithSuitBlockerAnyVsLow(item4, cards));
				}
			}
		}

		void SuitedVsSuited(PreflopRangeHoldemCell cell)
		{
			PocketCardsHoldem cards = cell.GetPocketCards().First();
			foreach (PreflopRangeHoldemCell item5 in preflopRange.SuitedCells.Where((PreflopRangeHoldemCell x) => x.Row >= cell.Row))
			{
				if (item5.Row != cell.Row || item5.Column >= cell.Column)
				{
					AddNewResult(cards, GetPocketCardsWithNoSuitBlockers(item5, cards));
					if (cell.Row != item5.Row && cell.Column != item5.Row && cell.Column != item5.Column)
					{
						AddNewResult(cards, GetPocketCardsWithAnySuitBlocker(item5, cards));
					}
				}
			}
		}
	}




	public static IEnumerable<int> GetAllCellVsCellStartIndexes()
	{
		foreach (PreflopRangeHoldemCell cell in PreflopRangeHoldem.AllCells(Game))
		{
			foreach (PreflopRangeHoldemCell item in PreflopRangeHoldem.AllCells(Game))
			{
				yield return cell.IsSwapNeeded(item) ? item.ComputeCellVsCellIndex(cell) : cell.ComputeCellVsCellIndex(item);
			}
		}
	}

	public static bool IsSwapNeeded(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.IsPair)
		{
			if (other.IsPair)
			{
				return cell.GetPreflopRangeIndex(Game) > other.GetPreflopRangeIndex(Game);
			}
			return false;
		}
		if (other.IsPair)
		{
			return true;
		}
		if (cell.IsSuited)
		{
			if (other.IsSuited)
			{
				return cell.GetPreflopRangeIndex(Game) > other.GetPreflopRangeIndex(Game);
			}
			return false;
		}
		if (other.IsSuited)
		{
			return true;
		}
		return cell.GetPreflopRangeIndex(Game) > other.GetPreflopRangeIndex(Game);
	}

	private static int PairCellResults(PreflopRangeHoldemCell cell)
	{
		return PairVsAllSuited + PairVsAllOffsuited + cell.GetPairVsAllPairsResults();
	}

	private static int SuitedCellResults(PreflopRangeHoldemCell cell)
	{
		return SuitedVsAllOffsuited + cell.GetSuitedVsAllSuitedResults();
	}

	private static int GetPairVsAllPairsResults(this PreflopRangeHoldemCell cell)
	{
		return (Game.RanksInDeck() - cell.Row) * 3 - 2;
	}

	private static int GetSuitedVsAllSuitedResults(this PreflopRangeHoldemCell cell)
	{
		return (from x in PreflopRangeHoldem.AllSuitedCells(Game)
			   where x.GetPreflopRangeIndex(Game) >= cell.GetPreflopRangeIndex(Game)
			   select x).SumResults(cell, LastSuitedCell);
	}

	private static int GetOffsuitedVsAllSOffsuitedResults(PreflopRangeHoldemCell cell)
	{
		return (from x in PreflopRangeHoldem.AllOffsuitedCells(Game)
			   where x.GetPreflopRangeIndex(Game) >= cell.GetPreflopRangeIndex(Game)
			   select x).SumResults(cell, LastOffsuitedCell);
	}

	public static int GetEquityTableCellStartIndex(this PreflopRangeHoldemCell cell)
	{
		if (cell.IsPair)
		{
			return cell.GetPairCellStartIndex();
		}
		if (cell.IsSuited)
		{
			return cell.GetSuitedCellStartIndex();
		}
		return cell.GetOffsuitedCellStartIndex();
	}

	private static int GetPairCellStartIndex(this PreflopRangeHoldemCell cell)
	{
		return (from x in PreflopRangeHoldem.AllPairCells(Game)
			   where x.GetPreflopRangeIndex(Game) < cell.GetPreflopRangeIndex(Game)
			   select x).Sum((Func<PreflopRangeHoldemCell, int>)PairCellResults);
	}

	private static int GetSuitedCellStartIndex(this PreflopRangeHoldemCell cell)
	{
		return AllPairs + (from x in PreflopRangeHoldem.AllSuitedCells(Game)
					    where x.GetPreflopRangeIndex(Game) < cell.GetPreflopRangeIndex(Game)
					    select x).Sum((Func<PreflopRangeHoldemCell, int>)SuitedCellResults);
	}

	private static int GetOffsuitedCellStartIndex(this PreflopRangeHoldemCell cell)
	{
		return AllPairs + AllSuited + (from x in PreflopRangeHoldem.AllOffsuitedCells(Game)
								 where x.GetPreflopRangeIndex(Game) < cell.GetPreflopRangeIndex(Game)
								 select x).Sum((Func<PreflopRangeHoldemCell, int>)GetOffsuitedVsAllSOffsuitedResults);
	}

	public static int ComputeCellVsCellIndex(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.IsPair)
		{
			return cell.GetPairCellStartIndex() + cell.GetPairCellResultsUntilCell(other);
		}
		if (cell.IsSuited)
		{
			return cell.GetSuitedCellStartIndex() + cell.GetSuitedCellResultsUntilCell(other);
		}
		return cell.GetOffsuitedCellStartIndex() + cell.GetOffsuitedCellResultsUntilCell(other);
	}

	private static int GetPairCellResultsUntilCell(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (other.IsPair)
		{
			return PairVsAllSuited + PairVsAllOffsuited + ((!other.IsAA()) ? PreflopRangeHoldem.AllPairCells(Game).SumResults(cell, other.GetPreviousPairCell(Game)) : 0);
		}
		if (other.IsSuited)
		{
			if (!other.IsAKs())
			{
				return PreflopRangeHoldem.AllSuitedCells(Game).SumResults(cell, other.GetPreviousSuitedCell(Game));
			}
			return 0;
		}
		return PairVsAllSuited + ((!other.IsAKo()) ? PreflopRangeHoldem.AllOffsuitedCells(Game).SumResults(cell, other.GetPreviousOffsuitedCell(Game)) : 0);
	}

	private static int GetSuitedCellResultsUntilCell(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (other.IsSuited)
		{
			return SuitedVsAllOffsuited + ((!other.IsAKs()) ? PreflopRangeHoldem.AllSuitedCells(Game).SumResults(cell, other.GetPreviousSuitedCell(Game)) : 0);
		}
		if (!other.IsAKo())
		{
			return PreflopRangeHoldem.AllOffsuitedCells(Game).SumResults(cell, other.GetPreviousOffsuitedCell(Game));
		}
		return 0;
	}

	private static int GetOffsuitedCellResultsUntilCell(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (!other.IsAKo())
		{
			return PreflopRangeHoldem.FullRangeTexasHoldem.OffsuitedCellsWithoutPairs.SumResults(cell, other.GetPreviousOffsuitedCell(Game));
		}
		return 0;
	}

	private static int GetPairVsPairResults(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (other.Column >= cell.Column)
		{
			if (other.Row != cell.Row)
			{
				return 3;
			}
			return 1;
		}
		return 0;
	}

	private static int GetPairVsSuitedResults(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (other.Row != cell.Row && other.Column != cell.Column)
		{
			return 2;
		}
		return 1;
	}

	private static int GetPairVsOffsuitedResults(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.Row != other.Row && other.Column != cell.Column)
		{
			return 4;
		}
		return 2;
	}

	private static int GetSuitedVsOffsuitedResults(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		return 3 - cell.CountEqualRanks(other);
	}

	private static int GetSuitedVsSuitedResults(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.GetPreflopRangeIndex(Game) <= other.GetPreflopRangeIndex(Game))
		{
			if (cell.Row == other.Row || cell.Column == other.Row || cell.Column == other.Column)
			{
				return 1;
			}
			return 2;
		}
		return 0;
	}

	private static int GetOffsuitedVsOffsuitedResults(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.GetPreflopRangeIndex(Game) > other.GetPreflopRangeIndex(Game))
		{
			return 0;
		}
		int num = 2;
		if (cell.Column != other.Column)
		{
			if (cell.Row != other.Row)
			{
				num++;
			}
			num++;
		}
		if (cell.Row != other.Column)
		{
			num += 2;
		}
		if (cell.Row != other.Row)
		{
			num++;
		}
		return num;
	}

	private static int SumResults(this IEnumerable<PreflopRangeHoldemCell> cells, PreflopRangeHoldemCell forCell, PreflopRangeHoldemCell toCell)
	{
		return cells.Where((PreflopRangeHoldemCell x) => x.GetPreflopRangeIndex(Game) <= toCell.GetPreflopRangeIndex(Game)).Sum(forCell.GetCountFunction(toCell));
	}

	public static Func<PreflopRangeHoldemCell, int> GetCountFunction(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell otherCell)
	{
		if (cell.IsPair)
		{
			if (otherCell.IsPair)
			{
				return (PreflopRangeHoldemCell x) => cell.GetPairVsPairResults(x);
			}
			if (otherCell.IsSuited)
			{
				return (PreflopRangeHoldemCell x) => cell.GetPairVsSuitedResults(x);
			}
			return (PreflopRangeHoldemCell x) => cell.GetPairVsOffsuitedResults(x);
		}
		if (cell.IsSuited)
		{
			if (otherCell.IsSuited)
			{
				return (PreflopRangeHoldemCell x) => cell.GetSuitedVsSuitedResults(x);
			}
			return (PreflopRangeHoldemCell x) => cell.GetSuitedVsOffsuitedResults(x);
		}
		return (PreflopRangeHoldemCell x) => cell.GetOffsuitedVsOffsuitedResults(x);
	}
}
