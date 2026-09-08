using System.Collections.Immutable;

namespace Poker.Calc;

public static class BoardRangeHelper
{
	public static bool ContainsDeadCards(this BoardRange range, Span<Card> deadCards)
	{
		foreach (ImmutableList<Card> boardCardRange in range.BoardCardRanges)
		{
			foreach (Card item in boardCardRange)
			{
				if (item.IsDead(deadCards))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static BoardRange GetRiverBoardsRange(this BoardRange range, Span<Card> deadCards, PokerGames game)
	{
		if (range.Street == Streets.River)
		{
			return range;
		}
		ImmutableList<Card> value = game.GetAllCards().Except(deadCards).ToImmutableList();
		if (range.Street == Streets.Flop)
		{
			return new BoardRange(range.BoardCardRanges.Add(value).Add(value));
		}
		return new BoardRange(range.BoardCardRanges.Add(value));
	}

	public static BoardRange ToBoardRange(this IEnumerable<IEnumerable<Card>> boardRange)
	{
		return new BoardRange(boardRange.Select((IEnumerable<Card> range) => range.ToImmutableList()).ToImmutableList());
	}

	public static BoardRange ToSingleBoardRange(this Board board)
	{
		return BoardRange.FromBoard(board);
	}

	public static string GetDefaultLiteral(this BoardRange range)
	{
		string text = string.Empty;
		foreach (ImmutableList<Card> boardCardRange in range.BoardCardRanges)
		{
			text = text + " " + boardCardRange.AggregateToString((Card x) => x.Abbreviation, "|");
		}
		return text.Trim();
	}

	public static IEnumerable<Board> GetBoardsSample(this BoardRange boardRange, int sampleSize)
	{
		if (boardRange.GetApproximateBoardsCount() <= sampleSize)
		{
			return boardRange.GetAllBoards();
		}
		return boardRange.GetRandomBoards(new FastRandom(), sampleSize);
	}

	public static IEnumerable<long> GetBoardMasksSample(this BoardRange boardRange, int sampleSize)
	{
		if (boardRange.GetApproximateBoardsCount() <= sampleSize)
		{
			return from x in boardRange.GetAllBoards()
				  select x.ToBoardCardsMask();
		}
		return boardRange.GetRandomBoardMasks(new FastRandom(), sampleSize);
	}

	public static IEnumerable<long> GetRandomBoardMasks(this BoardRange boardRange, FastRandom random, int sampleSize)
	{
		List<List<long>> boardRangeMask = boardRange.BoardCardRanges.MapToList((ImmutableList<Card> x) => x.MapToList((Card card) => card.ToCardMask()));
		for (int i = 0; i < sampleSize; i++)
		{
			long num = 0L;
			foreach (List<long> item in boardRangeMask)
			{
				num |= item.GetRandomCardMask(num, random);
			}
			yield return num;
		}
	}

	public static IEnumerable<Board> GetRandomBoards(this BoardRange boardRange, FastRandom random, int sampleSize)
	{
		for (int i = 0; i < sampleSize; i++)
		{
			Card[] array = new Card[boardRange.BoardCardRanges.Count];
			for (int j = 0; j < boardRange.BoardCardRanges.Count; j++)
			{
				array[j] = boardRange.BoardCardRanges[j].GetRandomCard(array, random);
			}
			yield return new Board(array.ToInlineList());
		}
	}

	public static int GetApproximateBoardsCount(this BoardRange boardRange)
	{
		int num = 1;
		foreach (ImmutableList<Card> boardCardRange in boardRange.BoardCardRanges)
		{
			num *= boardCardRange.Count;
		}
		return num;
	}

	public static bool IsDead(this BoardRange range, IList<Card> deadCards)
	{
		return range.BoardCardRanges.Any((ImmutableList<Card> cards) => cards.Intersect(deadCards).Count() == cards.Count);
	}

	public static void VerifyNotDead(this BoardRange range, IList<Card> deadCards)
	{
		if (range.IsDead(deadCards))
		{
			throw new InvalidOperationException("Board range is dead");
		}
	}

	public static BoardRange ExcludeDeadCards(this BoardRange range, IList<Card> deadCards)
	{
		return new BoardRange(range.BoardCardRanges.Select((ImmutableList<Card> cards) => cards.Except(deadCards).ToImmutableList()).ToImmutableList());
	}

	public static IEnumerable<Board> GetAllBoards(this BoardRange boardRange)
	{
		if (boardRange.Street == Streets.Flop)
		{
			for (int i = 0; i < boardRange.BoardCardRanges[0].Count; i++)
			{
				Card card1 = boardRange.BoardCardRanges[0][i];
				for (int j = 0; j < boardRange.BoardCardRanges[1].Count; j++)
				{
					Card card2 = boardRange.BoardCardRanges[1][j];
					if (card2 == card1)
					{
						continue;
					}
					for (int k = 0; k < boardRange.BoardCardRanges[2].Count; k++)
					{
						Card card3 = boardRange.BoardCardRanges[2][k];
						if (!(card3 == card1) && !(card3 == card2))
						{
							yield return new Board(new Card[3] { card1, card2, card3 }.ToInlineList());
						}
					}
				}
			}
		}
		else if (boardRange.Street == Streets.Turn)
		{
			for (int i = 0; i < boardRange.BoardCardRanges[0].Count; i++)
			{
				Card card1 = boardRange.BoardCardRanges[0][i];
				for (int j = 0; j < boardRange.BoardCardRanges[1].Count; j++)
				{
					Card card2 = boardRange.BoardCardRanges[1][j];
					if (card2 == card1)
					{
						continue;
					}
					for (int k = 0; k < boardRange.BoardCardRanges[2].Count; k++)
					{
						Card flop3 = boardRange.BoardCardRanges[2][k];
						if (flop3 == card1 || flop3 == card2)
						{
							continue;
						}
						for (int l = 0; l < boardRange.BoardCardRanges[3].Count; l++)
						{
							Card card4 = boardRange.BoardCardRanges[3][l];
							if (!(card4 == card1) && !(card4 == card2) && !(card4 == flop3))
							{
								yield return new Board(new Card[4] { card1, card2, flop3, card4 }.ToInlineList());
							}
						}
					}
				}
			}
		}
		else
		{
			if (boardRange.Street != Streets.River)
			{
				yield break;
			}
			for (int i = 0; i < boardRange.BoardCardRanges[0].Count; i++)
			{
				Card card1 = boardRange.BoardCardRanges[0][i];
				for (int j = 0; j < boardRange.BoardCardRanges[1].Count; j++)
				{
					Card card2 = boardRange.BoardCardRanges[1][j];
					if (card2 == card1)
					{
						continue;
					}
					for (int k = 0; k < boardRange.BoardCardRanges[2].Count; k++)
					{
						Card flop3 = boardRange.BoardCardRanges[2][k];
						if (flop3 == card1 || flop3 == card2)
						{
							continue;
						}
						for (int l = 0; l < boardRange.BoardCardRanges[3].Count; l++)
						{
							Card turn = boardRange.BoardCardRanges[3][l];
							if (turn == card1 || turn == card2 || turn == flop3)
							{
								continue;
							}
							for (int m = 0; m < boardRange.BoardCardRanges[4].Count; m++)
							{
								Card card5 = boardRange.BoardCardRanges[4][m];
								if (!(card5 == turn) && !(card5 == card1) && !(card5 == card2) && !(card5 == flop3))
								{
									yield return new Board(new Card[5] { card1, card2, flop3, turn, card5 }.ToInlineList());
								}
							}
						}
					}
				}
			}
		}
	}

	public static BoardRange VerifyArgumentStreet(this BoardRange range, Streets expectedStreet, string argumentName)
	{
		Streets street = range.Street;
		if (street != expectedStreet)
		{
			throw new ArgumentException($"Expected {expectedStreet} board range but was {street}", argumentName);
		}
		return range;
	}

	public static BoardRange VerifyShortDeck(this BoardRange boardRange)
	{
		foreach (ImmutableList<Card> boardCardRange in boardRange.BoardCardRanges)
		{
			foreach (Card item in boardCardRange)
			{
				if (item.Rank < CardRanks.Six)
				{
					throw new InvalidOperationException($"Expecting short deck board card, but was - {item}");
				}
			}
		}
		return boardRange;
	}
}
