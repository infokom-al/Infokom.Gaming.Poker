namespace Poker.Calc;

public class GameBoards
{
	public List<SuitsBoards> Boards { get; }

	public GameBoards(List<SuitsBoards> boards)
	{
		Boards = boards;
	}

	public IEnumerable<long> GetAllBoards()
	{
		foreach (SuitsBoards board in Boards)
		{
			foreach (long board2 in board.Boards)
			{
				yield return board2;
			}
		}
	}
}
