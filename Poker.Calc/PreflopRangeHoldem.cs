using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[JsonConverter(typeof(PreflopRangeHoldemConverter))]
[Games(PokerGames.ShortDeckFamily | PokerGames.TexasHoldem)]
public class PreflopRangeHoldem : IPreflopRange, IEquatable<PreflopRangeHoldem>
{
	public static readonly PreflopRangeHoldem Empty = new PreflopRangeHoldem(ImmutableList<PreflopRangeHoldemCell>.Empty);

	public ImmutableList<PreflopRangeHoldemCell> Cells { get; }

	public bool IsEmpty => Cells.Count == 0;

	public bool IsNotEmpty => !IsEmpty;

	public IEnumerable<PreflopRangeHoldemCell> SuitedCells => Cells.Where((PreflopRangeHoldemCell x) => x.IsSuited);

	public IEnumerable<PreflopRangeHoldemCell> OffsuitedCells => Cells.Where((PreflopRangeHoldemCell x) => x.IsOffsuited);

	public IEnumerable<PreflopRangeHoldemCell> OffsuitedCellsWithoutPairs => Cells.Where((PreflopRangeHoldemCell x) => x.IsOffsuited && !x.IsPair);

	public IEnumerable<PreflopRangeHoldemCell> PairCells => Cells.Where((PreflopRangeHoldemCell x) => x.IsPair);

	public static PreflopRangeHoldem FullRangeTexasHoldem => PokerGames.TexasHoldem.GetFullPreflopRangeCells().ToPreflopRangeHoldem();

	public static PreflopRangeHoldem FullRangeShortDeck => PokerGames.ShortDeck.GetFullPreflopRangeCells().ToPreflopRangeHoldem();

	public PreflopRangeHoldem(ImmutableList<PreflopRangeHoldemCell> cells)
	{
		Cells = cells;
	}

	public bool Equals(PreflopRangeHoldem other)
	{
		if (Cells.Count != other.Cells.Count)
		{
			return false;
		}
		foreach (PreflopRangeHoldemCell cell in Cells)
		{
			if (!other.Cells.Contains(cell))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((PreflopRangeHoldem)obj);
	}

	public override string ToString()
	{
		return Cells.AggregateToString((PreflopRangeHoldemCell x) => x.Abbreviation, " ");
	}

	public static IEnumerable<PreflopRangeHoldemCell> AllCells(PokerGames game)
	{
		return game.GetFullPreflopRangeCells();
	}

	public static IEnumerable<PreflopRangeHoldemCell> AllSuitedCells(PokerGames game)
	{
		return from cell in game.GetFullPreflopRangeCells()
			  where cell.IsSuited
			  select cell;
	}

	public static IEnumerable<PreflopRangeHoldemCell> AllPairCells(PokerGames game)
	{
		return from cell in game.GetFullPreflopRangeCells()
			  where cell.IsPair
			  select cell;
	}

	public static IEnumerable<PreflopRangeHoldemCell> AllOffsuitedCells(PokerGames game)
	{
		return from cell in game.GetFullPreflopRangeCells()
			  where cell.IsOffsuited && !cell.IsPair
			  select cell;
	}

	public static PreflopRangeHoldemCell LastCell(PokerGames game)
	{
		if (!game.IsShortDeckFamily())
		{
			return new PreflopRangeHoldemCell(CardRanks.Deuce, CardRanks.Deuce, Suitness.Offsuited);
		}
		return new PreflopRangeHoldemCell(CardRanks.Six, CardRanks.Six, Suitness.Offsuited);
	}

	public override int GetHashCode()
	{
		return Cells.Aggregate(0, (current, cell) => current ^ cell.GetHashCode());
	}
}
