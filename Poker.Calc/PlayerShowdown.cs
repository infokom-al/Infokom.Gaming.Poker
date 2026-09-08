namespace Poker.Calc;

public readonly struct PlayerShowdown
{
	public int SeatNumber { get; }

	public IPocketCards? PocketCards { get; }

	public bool IsFolded { get; }

	public PlayerShowdown(int seatNumber, IPocketCards? pocketCards, bool isFolded)
	{
		SeatNumber = seatNumber;
		PocketCards = pocketCards;
		IsFolded = isFolded;
	}
}
