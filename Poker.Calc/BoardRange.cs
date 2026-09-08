using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[JsonConverter(typeof(BoardRangeConverter))]
public class BoardRange
{
	public static readonly BoardRange AnyFlopHoldem = new BoardRange(ImmutableList.Create<ImmutableList<Card>>(Cards.AllCards, Cards.AllCards, Cards.AllCards));

	public static readonly BoardRange AnyTurnHoldem = new BoardRange(ImmutableList.Create<ImmutableList<Card>>(Cards.AllCards, Cards.AllCards, Cards.AllCards, Cards.AllCards));

	public static readonly BoardRange AnyFlopShortDeck = new BoardRange(ImmutableList.Create<ImmutableList<Card>>(Cards.AllCardsShortDeck, Cards.AllCardsShortDeck, Cards.AllCardsShortDeck));

	public static readonly BoardRange AnyTurnShortDeck = new BoardRange(ImmutableList.Create<ImmutableList<Card>>(Cards.AllCardsShortDeck, Cards.AllCardsShortDeck, Cards.AllCardsShortDeck, Cards.AllCardsShortDeck));

	public ImmutableList<ImmutableList<Card>> BoardCardRanges { get; }

	public Streets Street => BoardCardRanges.Count.CardsCountToStreet();

	public BoardRange(ImmutableList<ImmutableList<Card>> boardCardRanges)
	{
		BoardCardRanges = boardCardRanges;
		if (boardCardRanges.Count > 5 || boardCardRanges.Count < 3)
		{
			throw new ArgumentException($"Can't be {boardCardRanges.Count} cards on a board");
		}
		foreach (ImmutableList<Card> boardCardRange in boardCardRanges)
		{
			boardCardRange.VerifyCollectionNotEmpty();
			boardCardRange.VerifyDistinct();
		}
		(from x in BoardCardRanges
		 where x.Count == 1
		 select x[0]).ToList().VerifyDistinct();
	}

	public static BoardRange FromBoard(Board board)
	{
		return new BoardRange(board.Cards.MapToImmutableList((Card card) => card.ToSingleImmutableList()));
	}

	public bool IsSingleBoard(out Board singleBoard)
	{
		if (BoardCardRanges.All((ImmutableList<Card> x) => x.Count == 1))
		{
			singleBoard = new Board(BoardCardRanges.Select((ImmutableList<Card> x) => x[0]).ToInlineList());
			return true;
		}
		singleBoard = null;
		return false;
	}
}
