using MemoryPools;

namespace Poker.Calc;

public static class BoardHelper
{
	extension(IBoard board)
	{
		public bool IsPostflop => board.GetStreet() >= Streets.Flop;
	}

	public static BoardMap<Board> GetBoardMap(this IBoard board)
	{
		if (!(board is Board item))
		{
			if (board is MultiBoard multiBoard)
			{
				return multiBoard.Boards;
			}
			throw new NotImplementedException(board.GetType().Name);
		}
		return (boardNumber: 1, value: item).ToSingleBoardMap();
	}

	public static InlineList<Board> GetBoards(this IBoard board)
	{
		return board.GetBoardMap().Values;
	}

	public static Board GetCommonBoard(this IBoard board)
	{
		if (!(board is Board result))
		{
			if (board is MultiBoard multiBoard)
			{
				return multiBoard.GetCommonBoard();
			}
			throw new InvalidOperationException(board.GetType().Name + " has no common board");
		}
		return result;
	}

	public static bool TryGetCommonBoard(this IBoard board, out Board commonBoard)
	{
		commonBoard = board.GetCommonBoard();
		return commonBoard.Cards.Length > 0;
	}

	public static Board GetBoardOnStreet(this IBoard board, Streets street)
	{
		if (!(board is Board board2))
		{
			if (board is MultiBoard multiBoard)
			{
				return multiBoard.GetFirstBoardOnStreet(street);
			}
			throw new NotImplementedException(board.GetType().Name);
		}
		return board2.GetBoardOnStreet(street);
	}

	public static InlineList<Board> GetBoardsOnStreet(this IBoard board, Streets street)
	{
		if (!(board is Board board2))
		{
			if (board is MultiBoard multiBoard)
			{
				return multiBoard.GetBoardsOnStreet(street);
			}
			throw new NotImplementedException(board.GetType().Name);
		}
		return board2.GetBoardOnStreet(street).ToSingleInlineList();
	}

	public static InlineList<Board> GetDistinctBoardsOnStreet(this IBoard board, Streets street)
	{
		if (street.IsPreflop())
		{
			return Board.Preflop.ToSingleInlineList();
		}
		if (board is Board board2)
		{
			return board2.GetBoardOnStreet(street).ToSingleInlineList();
		}
		InlineList<Board> boardsOnStreet = board.VerifyType<MultiBoard>().GetBoardsOnStreet(street);
		if (boardsOnStreet.Count == 0 || boardsOnStreet[0].Cards.Length == 0)
		{
			return boardsOnStreet;
		}
		if (boardsOnStreet[0].Cards.Last == boardsOnStreet[1].Cards.Last)
		{
			return boardsOnStreet[0].ToSingleInlineList();
		}
		return boardsOnStreet;
	}

	public static Streets GetStreet(this IBoard board)
	{
		if (!(board is Board { Street: var street }))
		{
			if (!(board is MultiBoard { Street: var street2 }))
			{
				throw new NotImplementedException(board.GetType().Name);
			}
			return street2;
		}
		return street;
	}

	public static IEnumerable<Card> GetAllCards(this IBoard board)
	{
		if (!(board is Board board2))
		{
			if (board is MultiBoard multiBoard)
			{
				return multiBoard.Boards.Values.SelectMany((Board board3) => board3.Cards).Distinct();
			}
			throw new NotImplementedException(board.GetType().Name);
		}
		return board2.Cards.AsEnumerable();
	}

	public static Board GetBoard(this IBoard board, int boardNumber)
	{
		if (board is Board result)
		{
			if (boardNumber != 1)
			{
				throw new InvalidOperationException("Failed to get board #" + boardNumber.Quoted() + " from a single board");
			}
			return result;
		}
		BoardMap<Board>.Enumerator enumerator = ((board as MultiBoard) ?? throw new NotImplementedException(board.GetType().Name)).Boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int, Board) current = enumerator.Current;
			if (current.Item1 == boardNumber)
			{
				return current.Item2;
			}
		}
		throw new InvalidOperationException($"Failed to get border #{boardNumber} from a single board.");
	}

	public static Board GetFirstBoard(this IBoard board)
	{
		return board.GetBoard(1);
	}

	public static Board GetLastBoard(this IBoard board)
	{
		if (!(board is Board result))
		{
			if (board is MultiBoard multiBoard)
			{
				return multiBoard.LastBoard;
			}
			throw new NotImplementedException(board.GetType().Name);
		}
		return result;
	}

	public static bool IsPreflop(this IBoard board)
	{
		return board.GetStreet().IsPreflop();
	}

	public static long ToBoardCardsMask(this Board board)
	{
		return board.Cards.ToCardsMask();
	}

	public static long ToCardsMask(this Board board, int cardsToTake)
	{
		return board.Cards.ToCardsMask(cardsToTake);
	}

	public static long ToBoardCardsMask(this Board board, out long[] separateCards)
	{
		return board.Cards.ToCardsMask(out separateCards);
	}

	public static Board ToBoard(this InlineList<Card> cards)
	{
		return new Board(cards);
	}

	public static Board ParseBoard(this string? cards)
	{
		if (!string.IsNullOrWhiteSpace(cards))
		{
			return cards.ParseCards().ToBoard();
		}
		return Board.Preflop;
	}

	public static IBoard Slice(this IBoard board, Streets street)
	{
		if (board is Board board2)
		{
			return board2.Slice(street);
		}
		MultiBoard multiBoard = board.VerifyType<MultiBoard>();
		if (multiBoard.TryGetCommonBoard(out Board commonBoard) && commonBoard.Street >= street)
		{
			return commonBoard.Slice(street);
		}
		return new MultiBoard(multiBoard.Boards.Map((Board singleBoard) => singleBoard.Slice(street)));
	}

	public static Board Slice(this Board board, Streets street)
	{
		if (street == Streets.Preflop)
		{
			return Board.Preflop;
		}
		if (street == board.Street)
		{
			return board;
		}
		if (street >= board.Street)
		{
			throw new InvalidOperationException($"Can't slice {board} to {street}");
		}
		int num = street.CardsCount();
		InlineList<Card> cards = default(InlineList<Card>);
		for (int i = 0; i < num; i++)
		{
			cards.Add(board.Cards[i]);
		}
		return Board.Create(cards);
	}

	public static Board Next(this Board board, InlineList<Card> cards)
	{
		if (cards.Count == 0)
		{
			throw new ArgumentException("Next street cards can't be empty", "cards");
		}
		if (board.Cards.Length > 0 && board.Cards.GetIntersection(cards).Count > 0)
		{
			throw new InvalidOperationException("Cards " + InlineListHelper.AggregateToString(in cards) + " already has been dealt on board");
		}
		if (board.Street >= Streets.Flop)
		{
			if (cards.Count != 1)
			{
				throw new ArgumentException("Expecting a single card on the next street", "cards");
			}
		}
		else if (cards.Count != 3)
		{
			throw new ArgumentException("Expecting three cards on the next street board", "cards");
		}
		return new Board(board.Cards.Concat(cards));
	}

	public static Board GetBoardOnStreet(this Board board, Streets street)
	{
		if (street > board.Street)
		{
			throw new InvalidOperationException($"{street} is later than boards street {board.Street}");
		}
		return new Board(board.Cards.GetRange<Card>(0, street.CardsCount()));
	}

	public static int GetBoardCount(this IBoard board)
	{
		if (!(board is MultiBoard multiBoard))
		{
			return 1;
		}
		return multiBoard.BoardCount;
	}

	public static Board VerifyArgumentBoardMinStreet(this Board board, Streets street, string argumentName)
	{
		if (board.Street < street)
		{
			throw new ArgumentException($"Expecting at least {street} board but was a board on {board.Street}", argumentName);
		}
		return board;
	}

	public static InlineList<Card> GetStreetCards(this Board board, Streets street)
	{
		if (board.Street < street)
		{
			throw new ArgumentException($"Can't get {street} cards of the {board.Street} board");
		}
		return street switch
		{
			Streets.Preflop => default(InlineList<Card>),
			Streets.Flop => board.Cards.Take<Card>(3),
			Streets.Turn => board.Cards.Skip<Card>(3).Take<Card>(1),
			_ => board.Cards.Skip<Card>(4).Take<Card>(1),
		};
	}

	public static bool SameCards(this InlineList<Card> cards, InlineList<Card> other)
	{
		if (cards.Count != other.Count)
		{
			return false;
		}
		return cards.ToCardsMask() == other.ToCardsMask();
	}

	public static bool Any<T>(this in BoardMap<T> boards, Func<T, bool> predicate)
	{
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Any<T>(this in BoardMap<T> boards, Func<int, T, bool> predicate)
	{
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (arg, arg2) = enumerator.Current;
			if (predicate(arg, arg2))
			{
				return true;
			}
		}
		return false;
	}

	public static bool All<T>(this in BoardMap<T> boards, Func<int, T, bool> predicate)
	{
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (arg, arg2) = enumerator.Current;
			if (!predicate(arg, arg2))
			{
				return false;
			}
		}
		return true;
	}

	public static BoardMap<TOut> Map<TIn, TOut>(this in BoardMap<TIn> boards, Func<TIn, TOut> selector)
	{
		BoardMap<TOut> result = BoardMap<TOut>.Empty;
		BoardMap<TIn>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int boardNumber, TIn value) current = enumerator.Current;
			int item = current.boardNumber;
			TIn item2 = current.value;
			result = result.With(item, selector(item2));
		}
		return result;
	}

	public static BoardMap<TOut> Map<TIn, TArgumentOne, TOut>(this in BoardMap<TIn> boards, Func<TIn, TArgumentOne, TOut> selector, TArgumentOne argumentOne)
	{
		BoardMap<TOut> result = BoardMap<TOut>.Empty;
		BoardMap<TIn>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int boardNumber, TIn value) current = enumerator.Current;
			int item = current.boardNumber;
			TIn item2 = current.value;
			result = result.With(item, selector(item2, argumentOne));
		}
		return result;
	}

	public static BoardMap<TOut> Map<TIn, TArgumentOne, TOut>(this in BoardMap<TIn> boards, Func<int, TIn, TArgumentOne, TOut> selector, TArgumentOne argumentOne)
	{
		BoardMap<TOut> result = BoardMap<TOut>.Empty;
		BoardMap<TIn>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int boardNumber, TIn value) current = enumerator.Current;
			int item = current.boardNumber;
			TIn item2 = current.value;
			result = result.With(item, selector(item, item2, argumentOne));
		}
		return result;
	}

	public static double Sum<T>(this in BoardMap<T> boards, Func<T, double> selector)
	{
		double num = 0.0;
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			num += selector(item);
		}
		return num;
	}

	public static double Sum<T, TArgumentOne>(this in BoardMap<T> boards, Func<T, TArgumentOne, double> selector, TArgumentOne argumentOne)
	{
		double num = 0.0;
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			num += selector(item, argumentOne);
		}
		return num;
	}

	public static bool TryGet<T>(this in BoardMap<T> boards, Func<T, bool> predicate, out T result)
	{
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			if (predicate(item))
			{
				result = item;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static BoardMap<T> ToSingleBoardMap<T>(this (int boardNumber, T value) boardValue)
	{
		return BoardMap<T>.Empty.With(boardValue.boardNumber, boardValue.value);
	}

	public static BoardMap<T> ToSingleBoardMap<T>(this T value)
	{
		return BoardMap<T>.Empty.With(1, value);
	}

	public static (int boardNumber, T value)[] ToArray<T>(this in BoardMap<T> boards)
	{
		(int, T)[] array = new (int, T)[boards.Count];
		int num = 0;
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (item, item2) = enumerator.Current;
			array[num] = (item, item2);
			num++;
		}
		return array;
	}

	public static BoardMap<T> Where<T>(this in BoardMap<T> boards, Func<T, bool> predicate)
	{
		BoardMap<T> result = default(BoardMap<T>);
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (boardNumber, val) = enumerator.Current;
			if (predicate(val))
			{
				result.Set(boardNumber, val);
			}
		}
		return result;
	}

	public static BoardMap<T> Where<T, TArgumentOne>(this in BoardMap<T> boards, Func<T, TArgumentOne, bool> predicate, TArgumentOne argumentOne)
	{
		BoardMap<T> result = default(BoardMap<T>);
		BoardMap<T>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (boardNumber, val) = enumerator.Current;
			if (predicate(val, argumentOne))
			{
				result.Set(boardNumber, val);
			}
		}
		return result;
	}

	public static BoardMap<T> WithRemoved<T>(this BoardMap<T> boards, int boardNumber)
	{
		boards.Remove(boardNumber);
		return boards;
	}

	public static int VerifyBoardNumber(this int boardNumber)
	{
		if (boardNumber < 1 || boardNumber > 10)
		{
			throw new InvalidOperationException($"Invalid board number {boardNumber}.");
		}
		return boardNumber;
	}

	public static bool HasSameCards(this BoardMap<Board> boardMap, BoardMap<Board> other)
	{
		BoardMap<Board>.Enumerator enumerator = boardMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (boardNumber, board) = enumerator.Current;
			if (!other.TryGet(boardNumber, out Board result))
			{
				return false;
			}
			if (!board.HasSameCards(result))
			{
				return false;
			}
		}
		return true;
	}

	public static bool HasSameCards(this Board board, Board other)
	{
		if (board.IsPreflop)
		{
			return other.IsPreflop;
		}
		if (!other.GetBoardOnStreet(Streets.Flop).Cards.SameCards(board.GetBoardOnStreet(Streets.Flop).Cards))
		{
			return false;
		}
		return other.Cards.Skip<Card>(3).SequenceEqual(board.Cards.Skip<Card>(3));
	}

	public static Board VerifySingleBoard(this IBoard board)
	{
		return board.VerifyType<Board>();
	}

	public static BoardMap<T> ToBoardMap<T>(this IEnumerable<T> boards)
	{
		int num = 1;
		BoardMap<T> result = default(BoardMap<T>);
		foreach (T board in boards)
		{
			result.Set(num, board);
			num++;
		}
		return result;
	}

	public static BoardMap<TOut> ToBoardMap<TIn, TOut>(this IEnumerable<TIn> boards, Func<TIn, TOut> selector)
	{
		int num = 1;
		BoardMap<TOut> result = default(BoardMap<TOut>);
		foreach (TIn board in boards)
		{
			result.Set(num, selector(board));
			num++;
		}
		return result;
	}

	public static BoardMap<T[]> Flatten<T>(this BoardMap<StreetMap<T[]>> boards)
	{
		BoardMap<T[]> result = default(BoardMap<T[]>);
		BoardMap<StreetMap<T[]>>.Enumerator enumerator = boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(int boardNumber, StreetMap<T[]> value) current = enumerator.Current;
			int item = current.boardNumber;
			StreetMap<T[]> item2 = current.value;
			T[] array = ArrayPool<T>.ThreadShared.GetArray(item2.Values.Sum((T[] value) => value.Length));
			int num = 0;
			InlineList<T[]>.Enumerator enumerator2 = item2.Values.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				T[] current2 = enumerator2.Current;
				foreach (T val in current2)
				{
					array[num] = val;
					num++;
				}
			}
			result.Set(item, array);
		}
		return result;
	}

	public static SeatMap<double> GetAveragePerSeat(this BoardMap<SeatMap<double>> boardMap)
	{
		SeatMap<(double, int)> seats = default(SeatMap<(double, int)>);
		InlineList<SeatMap<double>>.Enumerator enumerator = boardMap.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SeatMap<double>.Enumerator enumerator2 = enumerator.Current.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				var (seatNumber, num) = enumerator2.Current;
				if (seats.TryGet(seatNumber, out var result))
				{
					seats.Set(seatNumber, (result.Item1 + num, result.Item2 + 1));
				}
				else
				{
					seats.Set(seatNumber, (num, 1));
				}
			}
		}
		return seats.Map<(double, int), double>(((double value, int number) value) => (value.number != 0) ? (value.value / (double)value.number) : 0.0);
	}

	public static bool IsDoubleBoard(this IBoard board)
	{
		if (board.Street != Streets.Preflop && board is MultiBoard { BoardCount: 2 } multiBoard)
		{
			return multiBoard.GetCommonBoard().Street == Streets.Preflop;
		}
		return false;
	}

	public static bool IsTripleBoard(this IBoard board)
	{
		if (board.Street != Streets.Preflop && board is MultiBoard { BoardCount: 3 } multiBoard)
		{
			return multiBoard.GetCommonBoard().Street == Streets.Preflop;
		}
		return false;
	}
}
