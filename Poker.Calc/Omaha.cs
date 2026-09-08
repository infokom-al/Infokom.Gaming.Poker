namespace Poker.Calc;

public static class Omaha
{
	public static int EvaluateHandRankRiver(IPocketCards pockets, Board board, PokerGames game)
	{
		return EvaluateHandRankRiver(pockets.ToCardsMaskCactus(), board.Cards.ToCardsMasksCactus(), game);
	}

	internal static int EvaluateHandRankRiver(InlineList<int> pocketMasksCactus, InlineList<int> boardMasksCactus, PokerGames game)
	{
		if (!game.IsOmahaFamily())
		{
			throw new InvalidOperationException($"Game {game} is not Omaha family");
		}
		if (pocketMasksCactus.Length != game.PocketCardsCount())
		{
			throw new InvalidOperationException($"Invalid pocket cards count {pocketMasksCactus.Length} for game {game}");
		}
		int num = 0;
		int length = pocketMasksCactus.Length;
		int length2 = boardMasksCactus.Length;
		for (int i = 0; i < length - 1; i++)
		{
			for (int j = i + 1; j < length; j++)
			{
				for (int k = 0; k < length2 - 2; k++)
				{
					for (int l = k + 1; l < length2 - 1; l++)
					{
						for (int m = l + 1; m < length2; m++)
						{
							int num2 = TexasHoldem.EvaluateHandRank(pocketMasksCactus[i], pocketMasksCactus[j], boardMasksCactus[k], boardMasksCactus[l], boardMasksCactus[m]);
							if (num2 > num)
							{
								num = num2;
							}
						}
					}
				}
			}
		}
		return num;
	}
}
