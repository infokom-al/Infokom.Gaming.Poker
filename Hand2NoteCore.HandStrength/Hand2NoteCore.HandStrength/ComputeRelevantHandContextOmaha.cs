using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

internal readonly struct ComputeRelevantHandContextOmaha(PokerGames pokerGame, CardsMaskU pocketsMask, CardsMaskU boardMask, CardsMaskU[] pocketsCards, CardsMaskU[] boardCards, HandValue handValue, Streets street, CardsMaskU pocketsInHandMask, CardsMaskU boardCardsInHandMask, HandValue boardHandValue, CardsMaskU flopBoardMask)
{
	public PokerGames PokerGame { get; } = pokerGame;

	internal CardsMaskU PocketsMask { get; } = pocketsMask;

	internal CardsMaskU BoardMask { get; } = boardMask;

	internal CardsMaskU[] PocketsCards { get; } = pocketsCards;

	internal CardsMaskU[] BoardCards { get; } = boardCards;

	public HandValue HandValue { get; } = handValue;

	public HandValue BoardHandValue { get; } = boardHandValue;

	public Streets Street { get; } = street;

	internal CardsMaskU FlopBoardMask { get; } = flopBoardMask;

	public PokerHands HandType => HandValue.Type;

	public CardsMaskU PocketsInHandMask { get; } = pocketsInHandMask;

	public CardsMaskU BoardCardsInHandMask { get; } = boardCardsInHandMask;

	public PokerHands BoardHandType => BoardHandValue.Type;
}
