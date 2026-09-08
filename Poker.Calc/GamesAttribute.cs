namespace Poker.Calc;

public class GamesAttribute : Attribute
{
	public PokerGames Value { get; }

	public GamesAttribute(PokerGames value)
	{
		Value = value;
	}
}
