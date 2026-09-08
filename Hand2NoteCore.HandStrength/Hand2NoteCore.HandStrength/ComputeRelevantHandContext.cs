using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

internal readonly struct ComputeRelevantHandContext(PokerGames pokerGame, CardsMaskU pocketsMask, CardsMaskU boardMask, HandValue handValue, HandValue boardHandValue, Streets street, CardsMaskU pocketsInHandMask, CardsMaskU flopBoardMask)
{
	public PokerGames PokerGame { get; } = pokerGame;

	internal CardsMaskU PocketsMask { get; } = pocketsMask;

	internal CardsMaskU BoardMask { get; } = boardMask;

	internal CardsMaskU FlopBoardMask { get; } = flopBoardMask;

	public HandValue HandValue { get; } = handValue;

	public HandValue BoardHandValue { get; } = boardHandValue;

	public Streets Street { get; } = street;

	internal CardsMaskU PocketsInHandMask { get; } = pocketsInHandMask;

	public bool IsBestHandOnBoard => HandValue.Rank == BoardHandValue.Rank;

	public PokerHands HandType => HandValue.Type;

	public PokerHands BoardHandType => BoardHandValue.Type;

	public bool IsShortDeckFamily { get; } = pokerGame.IsShortDeckFamily();
}
