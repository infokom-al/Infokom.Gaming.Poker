using BinarySerializer;

namespace Poker.Calc;

[TypeTag(1, typeof(PocketCardsHoldem))]
[TypeTag(2, typeof(PocketCardsOmaha))]
[JsonInterfaceConverter(typeof(PocketCardsConverter))]
public interface IPocketCards
{
	InlineList<Card> Cards { get; }
}
