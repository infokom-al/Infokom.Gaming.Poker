namespace Poker.Calc;

public class SuitsBoards
{
	public Suits Suit { get; }

	public int Count { get; }

	public List<long> Boards { get; }

	public SuitsBoards(Suits suit, int count, List<long> boards)
	{
		Suit = suit;
		Count = count;
		Boards = boards;
	}

	public void Add(long board)
	{
		Boards.Add(board);
	}
}
