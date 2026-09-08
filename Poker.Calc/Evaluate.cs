namespace Poker.Calc;

public static class Evaluate
{
	public static SeatNumberFlags GetShowdownWinners(this SeatMap<PlayerShowdown> players, PokerGames game, IBoard board)
	{
		if (!(board is Board board2))
		{
			if (board is MultiBoard multiBoard)
			{
				return players.GetMultiBoardShowdownWinners(game, multiBoard);
			}
			throw new NotImplementedException(board.GetType().Name);
		}
		return players.GetSingleBoardShowdownWinners(game, board2);
	}

	public static SeatNumberFlags GetMultiBoardShowdownWinners(this SeatMap<PlayerShowdown> players, PokerGames game, MultiBoard multiBoard)
	{
		SeatNumberFlags result = SeatNumberFlags.Empty;
		BoardMap<Board>.Enumerator enumerator = multiBoard.Boards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Board item = enumerator.Current.value;
			result = result.Add(players.GetShowdownWinners(game, item));
		}
		return result;
	}

	public static SeatNumberFlags GetSingleBoardShowdownWinners(this SeatMap<PlayerShowdown> players, PokerGames game, Board board)
	{
		if (!board.IsRiver)
		{
			throw new ArgumentException($"Expecting a river board but was {board.Street}");
		}
		SeatMap<int> seatMap = default(SeatMap<int>);
		int num = 0;
		SeatMap<PlayerShowdown>.Enumerator enumerator = players.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (seatNumber, playerShowdown) = enumerator.Current;
			if (playerShowdown.IsFolded)
			{
				seatMap.Set(seatNumber, 0);
			}
			else if (playerShowdown.PocketCards != null)
			{
				int num2 = playerShowdown.PocketCards.ComputeHandRank(board, game);
				if (num2 > num)
				{
					num = num2;
				}
				seatMap.Set(seatNumber, num2);
			}
			else
			{
				seatMap.Set(seatNumber, 0);
			}
		}
		SeatNumberFlags result = SeatNumberFlags.Empty;
		SeatMap<int>.Enumerator enumerator2 = seatMap.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			(int seatNumber, int value) current = enumerator2.Current;
			var (seatNumber2, _) = current;
			if (current.value == num)
			{
				result = result.Add(seatNumber2);
			}
		}
		return result;
	}

	public static HandValue ComputeHandValue(this IPocketCards pockets, Board board, PokerGames game)
	{
		if (game.IsOmahaFamily())
		{
			return ((PocketCardsOmaha)pockets).ComputeHandValue(board, game);
		}
		return pockets.ToCardsWithBoard(board).ComputeHandValue(game);
	}

	public static HandValue ComputeHandValue(this InlineList<Card> cards, PokerGames game)
	{
		return cards.ComputeHandRank(game).ToHandValue(game);
	}

	public static HandValue ComputeHandValue(this PocketCardsOmaha pockets, Board board, PokerGames game)
	{
		if (!game.IsOmahaFamily())
		{
			throw new ArgumentException($"Use another overload for {game}", "game");
		}
		return Omaha.EvaluateHandRankRiver(pockets, board, game).ToHandValue(game);
	}

	public static int ComputeHandRank(this IPocketCards cards, Board board, PokerGames game)
	{
		if (board.Street != Streets.River)
		{
			throw new ArgumentException("Only river hand rank evaluation is supported", "board");
		}
		if (game.IsOmahaFamily())
		{
			return Omaha.EvaluateHandRankRiver(cards, board, game);
		}
		return cards.ToCardsWithBoard(board).ComputeHandRank(game);
	}

	public static InlineList<Card> ToCardsWithBoard(this IPocketCards cards, Board board)
	{
		return cards.Cards.AddRange(board.Cards);
	}

	public static int ComputeHandRank(this InlineList<Card> cards, PokerGames pokerGame)
	{
		switch (pokerGame)
		{
			case PokerGames.TexasHoldem:
				if (cards.Length != 5)
				{
					var (cards4, ranks3) = cards.ToCardsMaskUWithRanks();
					return TexasHoldem.EvaluateCards(cards4, ranks3, cards.Length);
				}
				if (cards.Length == 5)
				{
					return TexasHoldem.EvaluateHandRank(cards[0].ToCardMaskCactus(), cards[1].ToCardMaskCactus(), cards[2].ToCardMaskCactus(), cards[3].ToCardMaskCactus(), cards[4].ToCardMaskCactus());
				}
				throw new NotImplementedException();
			case PokerGames.ShortDeck:
				var (cards3, ranks2) = cards.ToCardsMaskUWithRanks();
				return ShortDeck.EvaluateCards(cards3, ranks2, cards.Length);
			case PokerGames.ShortDeckTbs:
				var (cards2, ranks) = cards.ToCardsMaskUWithRanks();
				return ShortDeckTbs.EvaluateCards(cards2, ranks, cards.Length);
			default:
				throw new NotImplementedException();
		}
	}




	internal static int EvaluateHandRank7Cards(long cardsMask, int ranksMask, PokerGames game)
	{
		return game switch
		{
			PokerGames.TexasHoldem => cardsMask.CardsMaskToCardsMaskU().EvaluateSevenCards(ranksMask),
			PokerGames.ShortDeck => ShortDeck.EvaluateCards(cardsMask.CardsMaskToCardsMaskU(), ranksMask, 7),
			PokerGames.ShortDeckTbs => ShortDeckTbs.EvaluateCards(cardsMask.CardsMaskToCardsMaskU(), ranksMask, 7),
			_ => throw new InvalidOperationException(),
		};
	}

	internal static int EvaluateHandRank7Cards(CardsMaskU cardsMaskU, int ranksMask, PokerGames game)
	{
		return game switch
		{
			PokerGames.TexasHoldem => cardsMaskU.EvaluateSevenCards(ranksMask),
			PokerGames.ShortDeck => ShortDeck.EvaluateCards(cardsMaskU, ranksMask, 7),
			PokerGames.ShortDeckTbs => ShortDeckTbs.EvaluateCards(cardsMaskU, ranksMask, 7),
			_ => throw new InvalidOperationException(),
		};
	}

	internal static int EvaluateHandRank7Cards(CardsMaskU cardsMaskU, int ranksMask, PokerGames game, bool isFlushPossible)
	{
		return game switch
		{
			PokerGames.TexasHoldem => cardsMaskU.EvaluateSevenCards(ranksMask, isFlushPossible),
			PokerGames.ShortDeck => ShortDeck.EvaluateCards(cardsMaskU, ranksMask, 7),
			PokerGames.ShortDeckTbs => ShortDeckTbs.EvaluateCards(cardsMaskU, ranksMask, 7),
			_ => throw new InvalidOperationException(),
		};
	}

	internal static int EvaluateHandRankRiverCactus(InlineList<int> pockets, InlineList<int> board, PokerGames game)
	{
		if (!game.IsOmahaFamily())
		{
			throw new ArgumentException($"Do not use cactus evaluation algorithm for {game}", "game");
		}
		return Omaha.EvaluateHandRankRiver(pockets, board, game);
	}

	internal static int EvaluateHandRankRiver(Span<long> pockets, long[][] pocketsSeparate, long board, Span<long> boardSeparate, PokerGames game)
	{
		if (!game.RequiresPocketsInHandCombo())
		{
			throw new InvalidOperationException($"Call another overload of {"EvaluateHandRankRiver"} for {game}");
		}
		throw new NotImplementedException();
	}

	private static (CardsMaskU mask, int ranks) ToCardsMaskUWithRanks(this InlineList<Card> cards)
	{
		long num = 0L;
		int num2 = 0;
		InlineList<Card>.Enumerator enumerator = cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Card current = enumerator.Current;
			num |= Tables.CardsMasksPeval[current.Index];
			num2 |= 1 << (int)current.Rank;
		}
		return (mask: num.CardsMaskToCardsMaskU(), ranks: num2);
	}







	public static int ComputeHoldemHandRank(Card c1, Card c2, Card c3, Card c4, Card c5, Card c6, Card c7)
	{
		InlineList<Card> cards = new([c1, c2, c3, c4, c5, c6, c7]);

		var (cardsMask, ranks) = cards.ToCardsMaskUWithRanks();

		return TexasHoldem.EvaluateCards(cardsMask, ranks, 7);
	}

	public static int ComputeHoldemHandRankBitwise(Card c1, Card c2, Card c3, Card c4, Card c5, Card c6, Card c7)
	{
		var cards = 0UL;
		cards |= 1UL << c1.Index;
		cards |= 1UL << c2.Index;
		cards |= 1UL << c3.Index;
		cards |= 1UL << c4.Index;
		cards |= 1UL << c5.Index;
		cards |= 1UL << c6.Index;
		cards |= 1UL << c7.Index;

		var ranks = 0U;
		ranks |= 1U << c1.RankIndex;
		ranks |= 1U << c2.RankIndex;
		ranks |= 1U << c3.RankIndex;
		ranks |= 1U << c4.RankIndex;
		ranks |= 1U << c5.RankIndex;
		ranks |= 1U << c6.RankIndex;
		ranks |= 1U << c7.RankIndex;

		return TexasHoldem.EvaluateCards(cards, ranks, 7);
	}
}
