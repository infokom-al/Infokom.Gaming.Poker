namespace Poker.Calc;

public static class BoardsStorage
{
	public static GameBoards Boards52 { get; }

	public static GameBoards Boards36 { get; }

	static BoardsStorage()
	{
		Boards52 = GetAllBoards(PokerGames.TexasHoldem);
		Boards36 = GetAllBoards(PokerGames.ShortDeck);
	}

	public static IEnumerable<long> GetAllGameBoards(this PokerGames pokerGame)
	{
		return pokerGame.CardsInDeck() switch
		{
			36 => Boards36.GetAllBoards(),
			52 => Boards52.GetAllBoards(),
			_ => throw new NotImplementedException(),
		};
	}

	public static GameBoards GetGameBoards(this PokerGames pokerGame)
	{
		return pokerGame.CardsInDeck() switch
		{
			36 => Boards36,
			52 => Boards52,
			_ => throw new NotImplementedException(),
		};
	}

	private static GameBoards GetAllBoards(PokerGames pokerGame)
	{
		List<SuitsBoards> list = new List<SuitsBoards>();
		foreach (long board in GetBoards(0L, 0, 5))
		{
			list.AddBoard(board);
		}
		return new GameBoards(list);
		IEnumerable<long> GetBoards(long cardsMask, int cardsAdded, int boardLength, int lastAddedIndex = -1)
		{
			if (boardLength != 0)
			{
				long oldMask = cardsMask;
				for (int i = lastAddedIndex + 1; i <= 52 - (boardLength - cardsAdded); i++)
				{
					if (i.IsCardIndexHitsPokerGame(pokerGame))
					{
						long num = i.CardIndexToCardMaskPeval();
						if (!oldMask.HasCard(num))
						{
							cardsMask = oldMask | num;
							if (cardsAdded + 1 < boardLength)
							{
								foreach (long board2 in GetBoards(cardsMask, cardsAdded + 1, boardLength, i))
								{
									yield return board2;
								}
							}
							else
							{
								yield return cardsMask;
							}
						}
					}
				}
			}
		}
	}

	private static void AddBoard(this List<SuitsBoards> suitsBoards, long board)
	{
		if (!TryAddSuitBoard(5) && !TryAddSuitBoard(4) && !TryAddSuitBoard(3) && !TryAddSuitBoard(2))
		{
			TryAddSuitBoard(1);
		}
		bool TryAddSuitBoard(int suitsCount)
		{
			if (!board.GetSuitsWithCount(suitsCount, out var suits))
			{
				return false;
			}
			foreach (SuitsBoards suitsBoard in suitsBoards)
			{
				if (suitsBoard.Suit == suits && suitsBoard.Count == suitsCount)
				{
					suitsBoard.Add(board);
					return true;
				}
			}
			suitsBoards.Add(new SuitsBoards(suits, suitsCount, new List<long> { board }));
			return true;
		}
	}

	private static bool HasCard(this long boardMask, long cardMaskPeval)
	{
		return (boardMask & cardMaskPeval) != 0;
	}
}
