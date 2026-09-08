using Poker.Calc.Common;

using System.Collections.Immutable;

namespace Poker.Calc;

public static class PreflopRangeHoldemHelper
{
	public static readonly ImmutableList<string> HoldemHandsOrderRange = "AA\r\nKK\r\nQQ\r\nJJ\r\nTT\r\nAKs\r\nAKo\r\nAQs\r\n99\r\nAJs\r\nAQo\r\n88\r\nATs\r\nAJo\r\nKQs\r\n77\r\nKJs\r\nATo\r\nKQo\r\nA9s\r\nKTs\r\n66\r\nA8s\r\nQJs\r\nA7s\r\nKJo\r\nQTs\r\nA5s\r\nA9o\r\nA6s\r\nJTs\r\n55\r\nK9s\r\nA4s\r\nKTo\r\nA3s\r\nA8o\r\nQJo\r\nQ9s\r\nA2s\r\nK8s\r\nJ9s\r\n44\r\nK7s\r\nT9s\r\nQTo\r\nA7o\r\nK6s\r\nA5o\r\nJTo\r\nQ8s\r\nK9o\r\nA6o\r\nK5s\r\nJ8s\r\nT8s\r\nA4o\r\n33\r\n98s\r\nK4s\r\nA3o\r\nQ7s\r\nQ6s\r\nQ9o\r\nK3s\r\nK2s\r\nA2o\r\nJ7s\r\nK8o\r\nJ9o\r\nT7s\r\n87s\r\nT9o\r\nQ5s\r\n97s\r\nK7o\r\n22\r\nQ4s\r\nK6o\r\nJ6s\r\nQ8o\r\n86s\r\n76s\r\nT6s\r\nQ3s\r\nK5o\r\n96s\r\nJ8o\r\nJ5s\r\nT8o\r\nQ2s\r\n98o\r\nK4o\r\n65s\r\nJ4s\r\nQ7o\r\n75s\r\nQ6o\r\nJ3s\r\nK3o\r\nJ7o\r\n95s\r\n85s\r\nT5s\r\nT7o\r\nK2o\r\n87o\r\n54s\r\nJ2s\r\n97o\r\nQ5o\r\nT4s\r\n64s\r\nT3s\r\n74s\r\nQ4o\r\n76o\r\nJ6o\r\n84s\r\nT2s\r\n94s\r\nT6o\r\n53s\r\nQ3o\r\n86o\r\nJ5o\r\n93s\r\n96o\r\nQ2o\r\n63s\r\n92s\r\n65o\r\n43s\r\nJ4o\r\n73s\r\n75o\r\n83s\r\n52s\r\nJ3o\r\n85o\r\nT5o\r\n82s\r\n95o\r\n54o\r\nJ2o\r\n42s\r\nT4o\r\n62s\r\n64o\r\n72s\r\n32s\r\nT3o\r\n74o\r\n84o\r\nT2o\r\n53o\r\n94o\r\n93o\r\n43o\r\n63o\r\n92o\r\n73o\r\n83o\r\n52o\r\n82o\r\n42o\r\n62o\r\n72o\r\n32o".Split('\n').MapToImmutableList((string x) => x.Trim()).ToImmutableList();

	public static readonly ImmutableList<string> ShortDeckHandsOrderRange = "AA\r\nKK\r\nQQ\r\nJJ\r\nTT\r\nAKs\r\nAQs\r\nAKo\r\nAJs\r\nKQs\r\nAQo\r\nATs\r\nKJs\r\nAJo\r\nQJs\r\nKTs\r\nKQo\r\nJTs\r\nQTs\r\nATo\r\n99\r\nKJo\r\nA9s\r\nQJo\r\nKTo\r\nJTo\r\nQTo\r\nA8s\r\nA9o\r\nK9s\r\nT9s\r\nA8o\r\nQ9s\r\nJ9s\r\nA7s\r\nK9o\r\nT9o\r\nQ9o\r\nJ9o\r\nA7o\r\n88\r\nA6s\r\nK8s\r\n98s\r\nT8s\r\nQ8s\r\nJ8s\r\nK7s\r\nA6o\r\nK8o\r\n98o\r\nT8o\r\nQ8o\r\nJ8o\r\nK6s\r\nK7o\r\n97s\r\nT7s\r\nQ7s\r\nJ7s\r\n77\r\nQ6s\r\nK6o\r\n97o\r\nT7o\r\n87s\r\nQ7o\r\nJ7o\r\n96s\r\nQ6o\r\nT6s\r\nJ6s\r\n87o\r\n86s\r\n96o\r\nT6o\r\nJ6o\r\n66\r\n76s\r\n86o\r\n76o".Split('\n').MapToImmutableList((string x) => x.Trim()).ToImmutableList();

	public static readonly ImmutableArray<PreflopRangeHoldemCell> DefaultOrderedRangeHoldem = HoldemHandsOrderRange.Select(ParsePreflopRangeHoldemCell).ToImmutableArray();

	public static readonly ImmutableArray<PreflopRangeHoldemCell> DefaultOrderedRangeShortDeck = ShortDeckHandsOrderRange.Select(ParsePreflopRangeHoldemCell).ToImmutableArray();

	public static int GetPreflopRangeTotalColumns(this PokerGames game)
	{
		if (game.IsShortDeckFamily())
		{
			return 9;
		}
		if (game == PokerGames.TexasHoldem)
		{
			return 13;
		}
		throw new InvalidOperationException("Columns are not defined for preflop range of " + game);
	}

	public static PreflopRangeHoldem GetPreflopRangeHoldem(this PocketRangeHoldem pocketsRange, out PocketRangeHoldem tail)
	{
		HashSet<string> pocketCardsHashSet = pocketsRange.Cards.Select((PocketCardsHoldem x) => x.AbbreviationAhJs).ToHashSet();
		HashSet<PreflopRangeHoldemCell> hashSet = new HashSet<PreflopRangeHoldemCell>();
		List<PocketCardsHoldem> list = new List<PocketCardsHoldem>();
		ImmutableArray<PocketCardsHoldem>.Enumerator enumerator = pocketsRange.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PocketCardsHoldem current = enumerator.Current;
			PreflopRangeHoldemCell preflopRangeHoldemCell = current.GetPreflopRangeHoldemCell();
			if (!hashSet.Contains(preflopRangeHoldemCell))
			{
				if (current.GetPreflopRangeHoldemCell().GetPocketCards().All((PocketCardsHoldem x) => pocketCardsHashSet.Contains(x.AbbreviationAhJs)))
				{
					hashSet.Add(preflopRangeHoldemCell);
				}
				else
				{
					list.Add(current);
				}
			}
		}
		tail = list.ToPocketsRangeHoldem();
		return hashSet.ToPreflopRangeHoldem();
	}

	public static PreflopRangeHoldem GetPairsRange(this PreflopRangeHoldem range)
	{
		return range.Cells.Where((PreflopRangeHoldemCell x) => x.IsPair).ToPreflopRangeHoldem();
	}

	public static PreflopRangeHoldem GetSuitedRange(this PreflopRangeHoldem range)
	{
		return range.Cells.Where((PreflopRangeHoldemCell x) => x.IsSuited).ToPreflopRangeHoldem();
	}

	public static PreflopRangeHoldem GetOffsuitedRange(this PreflopRangeHoldem range)
	{
		return range.Cells.Where((PreflopRangeHoldemCell x) => x.IsOffsuited).ToPreflopRangeHoldem();
	}

	public static PocketRangeHoldem GetTopRangeInterval(this IList<PocketCardsHoldem> orderedRange, double fromPercents, double toPercents)
	{
		if (fromPercents.IsGreater(toPercents))
		{
			throw new ArgumentException($"Invalid range: from {fromPercents} to {toPercents}");
		}
		return orderedRange.Skip(fromPercents.GetSliceOf(orderedRange.Count)).Take((toPercents - fromPercents).GetSliceOf(orderedRange.Count)).ToPocketsRangeHoldem();
	}

	public static PreflopRangeHoldem GetDefaultTopRangeInterval(this PokerGames game, double fromPercents, double toPercents)
	{
		if (fromPercents.IsGreater(toPercents))
		{
			throw new ArgumentException($"Invalid range: from {fromPercents} to {toPercents}");
		}
		ImmutableArray<PreflopRangeHoldemCell> defaultOrderedRange = game.GetDefaultOrderedRange();
		int count = defaultOrderedRange.TakeCellCount(fromPercents.GetSliceOf(defaultOrderedRange.GetTotalCombos()));
		int count2 = defaultOrderedRange.TakeCellCount((toPercents - fromPercents).GetSliceOf(defaultOrderedRange.GetTotalCombos()));
		return defaultOrderedRange.Skip(count).Take(count2).ToPreflopRangeHoldem();
	}

	public static PreflopRangeHoldem GetBottomRange(this IList<PreflopRangeHoldemCell> orderedRange, double percents)
	{
		return orderedRange.Reverse().TakeCombos(percents.GetSliceOf(orderedRange.GetTotalCombos())).ToPreflopRangeHoldem();
	}

	public static ImmutableArray<PreflopRangeHoldemCell> GetDefaultOrderedRange(this PokerGames game)
	{
		if (game == PokerGames.TexasHoldem)
		{
			return DefaultOrderedRangeHoldem;
		}
		if (game.IsShortDeckFamily())
		{
			return DefaultOrderedRangeShortDeck;
		}
		throw new ArgumentException($"No default top range defined for {game}");
	}

	public static PreflopRangeHoldem GetFullPreflopRangeHoldem(this PokerGames game)
	{
		return game.VerifyIsHoldem().GetDefaultTopRange(100.0);
	}

	internal static IEnumerable<PreflopRangeHoldemCell> GetFullPreflopRangeCells(this PokerGames game)
	{
		if (game == PokerGames.TexasHoldem)
		{
			return from x in Enumerable.Range(0, 169)
				  select x.ToPreflopRangeHoldemCell();
		}
		if (game.IsShortDeckFamily())
		{
			return from x in GetShortDeckCellIndicies()
				  select x.ToPreflopRangeHoldemCell();
		}
		throw new InvalidOperationException($"Game {game} doesn't supports preflop range heat map");
	}

	internal static IEnumerable<int> GetShortDeckCellIndicies()
	{
		for (int row = 0; row < 9; row++)
		{
			for (int col = 0; col < 9; col++)
			{
				yield return row * 13 + col;
			}
		}
	}

	public static int GetFullPreflopRangeCellsCount(this PokerGames game)
	{
		if (game == PokerGames.TexasHoldem)
		{
			return 169;
		}
		if (game.IsShortDeckFamily())
		{
			return 81;
		}
		throw new InvalidOperationException($"Game {game} doesn't supports preflop range heat map");
	}

	internal static PreflopRangeHoldemCell ToPreflopRangeHoldemCell(this int cellIndex)
	{
		return cellIndex.ToPreflopRangeHoldemCell(13);
	}

	internal static PreflopRangeHoldemCell ToPreflopRangeHoldemCell(this int cellIndex, int totalColumns)
	{
		int num = cellIndex % totalColumns;
		int num2 = cellIndex / totalColumns;
		bool flag = num > num2;
		CardRanks val = (CardRanks)(12 - num2);
		CardRanks val2 = (CardRanks)(12 - num);
		return new PreflopRangeHoldemCell((CardRanks)Math.Max((int)val, (int)val2), (CardRanks)Math.Min((int)val, (int)val2), flag ? Suitness.Suited : Suitness.Offsuited);
	}

	public static PreflopRangeHoldem GetDefaultTopRange(this PokerGames game, double percents)
	{
		return game.GetDefaultOrderedRange().GetTopRange(percents);
	}

	public static PreflopRangeHoldem GetDefaultBottomRange(this PokerGames game, double percents)
	{
		return game.GetDefaultOrderedRange().GetBottomRange(percents);
	}

	public static PreflopRangeHoldem GetTopRange(this IList<PreflopRangeHoldemCell> orderedRange, double percents)
	{
		return orderedRange.TakeCombos(percents.GetSliceOf(orderedRange.GetTotalCombos())).ToPreflopRangeHoldem();
	}

	public static int GetTotalCombos(this IEnumerable<PreflopRangeHoldemCell> cells)
	{
		return cells.Sum((PreflopRangeHoldemCell x) => x.Combos);
	}

	public static IEnumerable<PreflopRangeHoldemCell> TakeCombos(this IEnumerable<PreflopRangeHoldemCell> cells, int combos)
	{
		if (combos == 0)
		{
			yield break;
		}
		int returnedCombos = 0;
		foreach (PreflopRangeHoldemCell cell in cells)
		{
			yield return cell;
			returnedCombos += cell.Combos;
			if (returnedCombos >= combos)
			{
				yield break;
			}
		}
	}

	public static int TakeCellCount(this IEnumerable<PreflopRangeHoldemCell> cells, int combos)
	{
		if (combos == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		foreach (PreflopRangeHoldemCell cell in cells)
		{
			num += cell.Combos;
			if (num >= combos)
			{
				return num2;
			}
			num2++;
		}
		throw new InvalidOperationException($"Loop ended without {num} getting bigger or equal than {combos}");
	}

	private static int GetSliceOf(this double percents, int count)
	{
		return (int)Math.Round((double)count * percents.VerifyArgumentPositive("percents") / 100.0);
	}

	public static PocketRangeHoldem ToPocketRangeHoldem(this PreflopRangeHoldem range)
	{
		return range.Cells.SelectMany((PreflopRangeHoldemCell x) => x.GetPocketCards()).ToPocketsRangeHoldem();
	}

	public static IEnumerable<PocketCardsHoldem> GetPocketCards(this PreflopRangeHoldemCell cell)
	{
		if (cell.IsPair)
		{
			CardRanks rank = cell.HighRank;
			for (int i = 0; i < 4; i++)
			{
				for (int j = i + 1; j < 4; j++)
				{
					yield return PocketCardsHoldem.Create(new Card(rank, (Suits)i), new Card(rank, (Suits)j));
				}
			}
		}
		else if (cell.IsSuited)
		{
			Suits[] allSuits = Cards.AllSuits;
			foreach (Suits suit in allSuits)
			{
				yield return PocketCardsHoldem.Create(new Card(cell.HighRank, suit), new Card(cell.LowRank, suit));
			}
		}
		else
		{
			if (!cell.IsOffsuited)
			{
				yield break;
			}
			InlineList<Card>.Enumerator enumerator = cell.HighRank.GetAllPossibleCards().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Card card1 = enumerator.Current;
				InlineList<Card>.Enumerator enumerator2 = cell.LowRank.GetAllPossibleCards().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Card current = enumerator2.Current;
					if (card1.Suit != current.Suit)
					{
						yield return PocketCardsHoldem.Create(card1, current);
					}
				}
			}
		}
	}

	public static IEnumerable<PocketCardsHoldem> GetPocketCards(this PreflopRangeHoldemCell cell, PocketCardsHoldem deadCards)
	{
		foreach (PocketCardsHoldem pocketCards in cell.GetPocketCards())
		{
			if (!deadCards.Cards.Any((Card x) => pocketCards.Cards.Any<Card>(((Card)x).Equals)))
			{
				yield return pocketCards;
			}
		}
	}

	public static PreflopRangeHoldemCell ParsePreflopRangeHoldemCell(this string abbreviationAJs)
	{
		if (!abbreviationAJs.TryParsePreflopRangeHoldemCell(out var result))
		{
			throw new ArgumentException("Failed to parse " + abbreviationAJs, "abbreviationAJs");
		}
		return result;
	}

	public static bool TryParsePreflopRangeHoldemCell(this string abbreviationAJs, out PreflopRangeHoldemCell result)
	{
		result = default(PreflopRangeHoldemCell);
		if (abbreviationAJs.Length >= 2 && abbreviationAJs[0].TryParseCardRank(out var res) && abbreviationAJs[1].TryParseCardRank(out var res2))
		{
			if (abbreviationAJs.Length == 2 && res == res2)
			{
				result = PreflopRangeHoldemCell.Pair(res2);
				return true;
			}
			if (abbreviationAJs.Length != 3 || !abbreviationAJs[2].TryParseSuitness(out var result2))
			{
				return false;
			}
			result = new PreflopRangeHoldemCell(res, res2, result2);
			return true;
		}
		return false;
	}

	private static PreflopRangeHoldemCell ToPreflopRangeHoldemCell(this InlineList<Card> cards)
	{
		if (cards.Count != 2)
		{
			throw new ArgumentException($"Expected two cards but was {cards.Count}", "cards");
		}
		return cards[0].ToPreflopRangeHoldemCell(cards[1]);
	}

	public static PreflopRangeHoldemCell ToPreflopRangeHoldemCell(this Card firstCard, Card secondCard)
	{
		return PreflopRangeHoldemCell.FromCards(firstCard, secondCard);
	}

	public static PreflopRangeHoldem ToPreflopRangeHoldem(this IEnumerable<PreflopRangeHoldemCell> cells)
	{
		return new PreflopRangeHoldem(cells.Distinct().ToImmutableList());
	}

	public static bool IsSixPlusCell(this PreflopRangeHoldemCell holdemMatrixCell)
	{
		if (holdemMatrixCell.HighRank >= CardRanks.Six)
		{
			return holdemMatrixCell.LowRank >= CardRanks.Six;
		}
		return false;
	}

	public static PreflopRangeHoldemCell VerifyIsSuited(this PreflopRangeHoldemCell cell)
	{
		if (!cell.IsSuited)
		{
			throw new InvalidOperationException($"Expecting a suited cell but was {cell}");
		}
		return cell;
	}

	public static PreflopRangeHoldemCell VerifyIsPair(this PreflopRangeHoldemCell cell)
	{
		if (!cell.IsPair)
		{
			throw new InvalidOperationException($"Expecting a pair but was {cell}");
		}
		return cell;
	}

	public static PreflopRangeHoldem VerifyNotEmpty(this PreflopRangeHoldem range)
	{
		if (range.IsEmpty)
		{
			throw new InvalidOperationException("Given preflop range is empty");
		}
		return range;
	}

	public static PreflopRangeHoldem Except(this PreflopRangeHoldem range, PreflopRangeHoldem other)
	{
		return range.Cells.Except(other.Cells.ToHashSet()).ToPreflopRangeHoldem();
	}

	public static IEnumerable<PreflopRangeHoldemCell> Except(this IEnumerable<PreflopRangeHoldemCell> cells, HashSet<PreflopRangeHoldemCell> otherCells)
	{
		return cells.Where((PreflopRangeHoldemCell x) => !otherCells.Contains(x));
	}

	public static int CompareByRanks(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.HighRank != other.HighRank)
		{
			return cell.HighRank.CompareTo(other.HighRank);
		}
		return cell.LowRank.CompareTo(other.LowRank);
	}

	public static PreflopRangeHoldemCell GetPreflopRangeHoldemCell(this PocketCardsHoldem pocketCards)
	{
		return new PreflopRangeHoldemCell(pocketCards.HighCard.Rank, pocketCards.LowCard.Rank, pocketCards.Suitness);
	}

	public static PreflopRangeHoldemCell GetSuitedPreflopRangeHoldemCell(this (CardRanks firstRank, CardRanks secondRank) pocketRanks)
	{
		return pocketRanks.GetPreflopRangeHoldemCell(Suitness.Suited);
	}

	public static PreflopRangeHoldemCell GetOffsuitedPreflopRangeHoldemCell(this (CardRanks firstRank, CardRanks secondRank) pocketRanks)
	{
		return pocketRanks.GetPreflopRangeHoldemCell(Suitness.Offsuited);
	}

	public static PreflopRangeHoldemCell GetPreflopRangeHoldemCell(this (CardRanks firstRank, CardRanks secondRank) pocketRanks, Suitness suitness)
	{
		return new PreflopRangeHoldemCell(pocketRanks.firstRank, pocketRanks.secondRank, suitness);
	}

	public static bool ContainsTexasHoldemCells(this PreflopRangeHoldem range)
	{
		return range.Cells.Any(IsTexasHoldemCell);
	}

	public static bool IsTexasHoldemCell(this PreflopRangeHoldemCell cell)
	{
		if (cell.HighRank >= CardRanks.Five)
		{
			return cell.LowRank < CardRanks.Five;
		}
		return true;
	}

	public static int GetMaxColumn(this PocketRangeHoldem range)
	{
		return range.Cards.MaxOrDefault((PocketCardsHoldem x) => x.GetPreflopRangeHoldemCell().Column);
	}

	public static int GetMaxRow(this PocketRangeHoldem range)
	{
		return range.Cards.MaxOrDefault((PocketCardsHoldem x) => x.GetPreflopRangeHoldemCell().Row);
	}

	public static Dictionary<PreflopRangeHoldemCell, double> GetCellToCombos(this PocketRangeHoldem range)
	{
		PocketRangeHoldem tail;
		Dictionary<PreflopRangeHoldemCell, double> dictionary = ((IEnumerable<PreflopRangeHoldemCell>)range.GetPreflopRangeHoldem(out tail).Cells).ToDictionary((Func<PreflopRangeHoldemCell, PreflopRangeHoldemCell>)((PreflopRangeHoldemCell x) => x), (Func<PreflopRangeHoldemCell, double>)((PreflopRangeHoldemCell x) => x.Combos));
		ImmutableArray<PocketCardsHoldem>.Enumerator enumerator = tail.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PocketCardsHoldem current = enumerator.Current;
			dictionary.IncrementOrAdd(current.GetPreflopRangeHoldemCell());
		}
		return dictionary;
	}

	public static Dictionary<PreflopRangeHoldemCell, double> GetCellToCombos(this PocketsRangeWeightedHoldem range)
	{
		return (from pocketWeight in range.Range
			   group pocketWeight by pocketWeight.pocket.GetPreflopRangeHoldemCell() into grouping
			   select (cell: grouping.Key, combos: grouping.Sum(((PocketCardsHoldem pocket, double weightUnitInterval) y) => y.weightUnitInterval))).ToDictionary(((PreflopRangeHoldemCell cell, double combos) x) => x.cell, ((PreflopRangeHoldemCell cell, double combos) x) => x.combos);
	}

	public static Dictionary<PreflopRangeHoldemCell, double> GetCellToCombos(this IPreflopRange range)
	{
		if (!(range is PocketsRangeWeightedHoldem range2))
		{
			if (!(range is PocketRangeHoldem range3))
			{
				if (range is PreflopRangeHoldem preflopRangeHoldem)
				{
					return ((IEnumerable<PreflopRangeHoldemCell>)preflopRangeHoldem.Cells).ToDictionary((Func<PreflopRangeHoldemCell, PreflopRangeHoldemCell>)((PreflopRangeHoldemCell x) => x), (Func<PreflopRangeHoldemCell, double>)((PreflopRangeHoldemCell x) => x.Combos));
				}
				throw new NotImplementedException();
			}
			return range3.GetCellToCombos();
		}
		return range2.GetCellToCombos();
	}

	public static int GetRowIndex(this PocketCardsHoldem cards)
	{
		if (!cards.IsSuited)
		{
			return 12 - cards.LowCard.RankIndex;
		}
		return 12 - cards.HighCard.RankIndex;
	}

	public static int GetColumnIndex(this PocketCardsHoldem cards)
	{
		if (!cards.IsSuited)
		{
			return 12 - cards.HighCard.RankIndex;
		}
		return 12 - cards.LowCard.RankIndex;
	}

	public static int GetCellIndex(this PocketCardsHoldem cards)
	{
		return 13 * cards.GetRowIndex() + cards.GetColumnIndex();
	}

	public static int GetPocketsColumnFromEnd(this PocketCardsHoldem cards)
	{
		return 13 - cards.GetColumnIndex();
	}

	public static int GetSuitsInRow(this int row)
	{
		return 12 - row;
	}

	public static int GetOfSuitsInRow(this int row)
	{
		return row;
	}

	public static int GetDistanceFromDiagonalInRows(this PreflopRangeHoldemCell cell)
	{
		return Math.Abs(cell.Column - cell.Row);
	}

	public static bool SuitedRowHasColumn(this int row, int column)
	{
		return row.GetSuitsInRow() >= 13 - column;
	}

	public static bool OffsuitedRowHasColumn(this int row, int column)
	{
		return row.GetOfSuitsInRow() > column;
	}

	public static bool HasEqualRanks(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		if (cell.HighRank == other.HighRank)
		{
			return cell.LowRank == other.LowRank;
		}
		return false;
	}

	public static bool HasEqualRank(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		return cell.Ranks().Intersect(other.Ranks()).Any();
	}

	public static int CountEqualRanks(this PreflopRangeHoldemCell cell, PreflopRangeHoldemCell other)
	{
		return cell.Ranks().Intersect(other.Ranks()).Count();
	}

	public static PreflopRangeHoldemCell GetPreviousSuitedCell(this PreflopRangeHoldemCell cell, PokerGames game)
	{
		int num = cell.GetPreflopRangeIndex(game);
		int preflopRangeTotalColumns = game.GetPreflopRangeTotalColumns();
		while (num != 0)
		{
			num--;
			int num2 = num % preflopRangeTotalColumns;
			int num3 = num / preflopRangeTotalColumns;
			if (num2 > num3)
			{
				return num.ToPreflopRangeHoldemCell(preflopRangeTotalColumns);
			}
		}
		throw new InvalidOperationException("Should not get here");
	}

	public static PreflopRangeHoldemCell GetPreviousOffsuitedCell(this PreflopRangeHoldemCell cell, PokerGames game)
	{
		int num = cell.GetPreflopRangeIndex(game);
		int preflopRangeTotalColumns = game.GetPreflopRangeTotalColumns();
		while (num != 0)
		{
			num--;
			int num2 = num % preflopRangeTotalColumns;
			int num3 = num / preflopRangeTotalColumns;
			if (num2 < num3)
			{
				return num.ToPreflopRangeHoldemCell(preflopRangeTotalColumns);
			}
		}
		throw new InvalidOperationException("Should not get here");
	}

	public static PreflopRangeHoldemCell GetPreviousPairCell(this PreflopRangeHoldemCell cell, PokerGames game)
	{
		int num = cell.GetPreflopRangeIndex(game);
		int preflopRangeTotalColumns = game.GetPreflopRangeTotalColumns();
		while (num != 0)
		{
			num--;
			int num2 = num % preflopRangeTotalColumns;
			int num3 = num / preflopRangeTotalColumns;
			if (num2 == num3)
			{
				return num.ToPreflopRangeHoldemCell(preflopRangeTotalColumns);
			}
		}
		throw new InvalidOperationException("Should not get here");
	}

	public static bool IsAKs(this PreflopRangeHoldemCell cell)
	{
		return cell.GetIndex() == 1;
	}

	public static bool IsAKo(this PreflopRangeHoldemCell cell)
	{
		return cell.GetIndex() == 13;
	}

	public static bool IsAA(this PreflopRangeHoldemCell cell)
	{
		return cell.GetIndex() == 0;
	}

	public static IEnumerable<PocketCardsHoldem> GetAllPossiblePocketCards(this PreflopRangeHoldem range)
	{
		return range.Cells.SelectMany((PreflopRangeHoldemCell cell) => cell.GetPocketCards());
	}
}
