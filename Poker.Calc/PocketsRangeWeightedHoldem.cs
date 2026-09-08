using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[JsonConverter(typeof(PocketsRangeWeightedHoldemConverter))]
[Games(PokerGames.ShortDeckFamily | PokerGames.TexasHoldem)]
public class PocketsRangeWeightedHoldem : IPreflopRange
{
	public static readonly PocketsRangeWeightedHoldem Empty = new PocketsRangeWeightedHoldem(ImmutableArray<(PocketCardsHoldem, double)>.Empty);

	public ImmutableArray<(PocketCardsHoldem pocket, double weightUnitInterval)> Range { get; }

	public bool IsEmpty => Range.IsEmpty;

	public double Combos => Range.Sum<(PocketCardsHoldem, double)>(((PocketCardsHoldem pocket, double weightUnitInterval) item) => item.weightUnitInterval);

	public PocketsRangeWeightedHoldem(ImmutableArray<(PocketCardsHoldem pocket, double weightUnitInterval)> range)
	{
		Range = range;
	}

	public override string ToString()
	{
		return Range.Select<(PocketCardsHoldem, double), string>(((PocketCardsHoldem pocket, double weightUnitInterval) item) => item.weightUnitInterval.IsEqual(1.0) ? $"{item.pocket}" : $"{item.pocket} {item.weightUnitInterval * 100.0:0.#}%").AggregateToString(" ");
	}
}
