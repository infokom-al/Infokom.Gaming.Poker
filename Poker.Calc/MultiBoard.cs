using BinarySerializer;

namespace Poker.Calc;

[BinarySerializable]
public class MultiBoard : IBoard, IEquatable<MultiBoard>
{
	[Tag(1)]
	public BoardMap<Board> Boards { get; }

	public Board FirstBoard
	{
		get
		{
			if (Boards.Count <= 0)
			{
				return Board.Preflop;
			}
			return Boards.First;
		}
	}

	public Board LastBoard
	{
		get
		{
			if (Boards.Count <= 0)
			{
				return Board.Preflop;
			}
			return Boards.Last;
		}
	}

	public Streets Street => LastBoard.Street;

	public int BoardCount => Boards.Count;

	public int LastBoardNumber => Boards.LastBoardNumber;

	public MultiBoard(BoardMap<Board> boards)
	{
		Boards = boards;
	}

	public MultiBoard()
	{
		Boards = default(BoardMap<Board>);
	}

	public Board GetFirstBoardOnStreet(Streets street)
	{
		return Boards.First.GetBoardOnStreet(street);
	}

	public InlineList<Board> GetBoardsOnStreet(Streets street)
	{
		return Boards.Values.Map((Board board) => board.GetBoardOnStreet(street));
	}

	public Board GetCommonBoard()
	{
		if (Boards.Count == 0)
		{
			return Board.Preflop;
		}
		for (int i = 0; i < FirstBoard.Cards.Length; i++)
		{
			if (!IsCardCommon(i, this))
			{
				InlineList<Card> cards = default(InlineList<Card>);
				for (int j = 0; j < i; j++)
				{
					cards.Add(FirstBoard.Cards[j]);
				}
				return Board.Create(cards);
			}
		}
		return FirstBoard;
		static bool IsCardCommon(int index, MultiBoard multiBoard)
		{
			InlineList<Board>.Enumerator enumerator = multiBoard.Boards.Values.Skip<Board>(1).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.Cards[index].Equals(multiBoard.FirstBoard.Cards[index]))
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool Equals(MultiBoard other)
	{
		return Boards.Values.SequenceEqual(other.Boards.Values);
	}

	public override int GetHashCode()
	{
		return Boards.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((MultiBoard)obj);
	}
}
