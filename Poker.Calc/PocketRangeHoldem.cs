using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[Games(PokerGames.ShortDeckFamily | PokerGames.TexasHoldem)]
[JsonConverter(typeof(PocketsRangeHoldemConverter))]
public class PocketRangeHoldem : IPreflopRange
{
	public static readonly PocketRangeHoldem Empty = new PocketRangeHoldem(ImmutableArray<PocketCardsHoldem>.Empty);

	public ImmutableArray<PocketCardsHoldem> Cards { get; }

	public bool IsEmpty => Cards.IsEmpty;

	public int Combos => Cards.Length;

	public PocketRangeHoldem(ImmutableArray<PocketCardsHoldem> cards)
	{
		Cards = cards;
	}

	public override string ToString()
	{
		return Cards.AggregateToString(" ");
	}
}
