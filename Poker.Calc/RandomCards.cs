namespace Poker.Calc;

public static class RandomCards
{
	public static InlineList<int> GetRandomCardsMasksCactus(this FastRandom random, long deadCardsIndexesMask, int cardsCount, int cardsInDeck)
	{
		InlineList<int> result = default(InlineList<int>);
		for (int i = 0; i < cardsCount; i++)
		{
			int num = GetRandomCardMaskLocal(deadCardsIndexesMask);
			deadCardsIndexesMask |= 1L << num;
			result.Add(num.CardIndexToCardMaskCactus());
		}
		return result;
		int GetRandomCardMaskLocal(long deadCardsMask)
		{
			int randomCardIndex;
			do
			{
				randomCardIndex = random.GetRandomCardIndex(cardsInDeck);
			}
			while ((deadCardsMask & (1L << randomCardIndex)) != 0L);
			return randomCardIndex;
		}
	}

	public static long GetRandomCardsMasks(this FastRandom random, long deadCardsMask, int cardsCount, int cardsInDeck, long[] outCardsSeparate)
	{
		long num = 0L;
		for (int i = 0; i < cardsCount; i++)
		{
			long randomCardMask = random.GetRandomCardMask(deadCardsMask, cardsInDeck);
			deadCardsMask |= randomCardMask;
			num |= randomCardMask;
			outCardsSeparate[i] = randomCardMask;
		}
		return num;
	}

	public static long GetRandomCardsMasks(this FastRandom random, long deadCardsMask, int cardsCount, int cardsInDeck)
	{
		long num = 0L;
		for (int i = 0; i < cardsCount; i++)
		{
			long randomCardMask = random.GetRandomCardMask(deadCardsMask, cardsInDeck);
			deadCardsMask |= randomCardMask;
			num |= randomCardMask;
		}
		return num;
	}

	public static long GetRandomCardMask(this FastRandom random, long deadCardsMask, int cardsInDeck)
	{
		if (deadCardsMask == 0L)
		{
			return random.GetRandomCardIndex(cardsInDeck).CardIndexToCardMaskPeval();
		}
		long num;
		do
		{
			num = random.GetRandomCardIndex(cardsInDeck).CardIndexToCardMaskPeval();
		}
		while ((num & deadCardsMask) != 0L);
		return num;
	}

	public static IEnumerable<Card> GetRandomCardsSlow(this FastRandom random, int cardsCount, PokerGames game)
	{
		return random.GetRandomCards(cardsCount, game.CardsInDeck());
	}

	public static IEnumerable<Card> GetRandomCards(this FastRandom random, int cardsCount, int cardsInDeck)
	{
		Card[] array = new Card[cardsCount];
		long num = 0L;
		for (int i = 0; i < cardsCount; i++)
		{
			Card card = (array[i] = random.GetRandomCard(num, cardsInDeck));
			num |= 1L << card.Index;
		}
		return array;
	}

	public static long GetIndexMask(this InlineList<Card> cards)
	{
		long num = 0L;
		InlineList<Card>.Enumerator enumerator = cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num |= 1L << enumerator.Current.Index;
		}
		return num;
	}

	public static InlineList<Card> GetRandomCardsInlineList(this FastRandom random, int cardsCount, int cardsInDeck, long deadCardIndexMask)
	{
		InlineList<Card> result = default(InlineList<Card>);
		for (int i = 0; i < cardsCount; i++)
		{
			Card randomCard = random.GetRandomCard(deadCardIndexMask, cardsInDeck);
			result.Add(randomCard);
			deadCardIndexMask |= 1L << randomCard.Index;
		}
		return result;
	}

	private static Card GetRandomCard(this FastRandom random, long deadCardsMask, int cardsInDeck)
	{
		if (deadCardsMask == 0L)
		{
			return random.GetRandomCardIndex(cardsInDeck).CardIndexToCard();
		}
		int randomCardIndex;
		do
		{
			randomCardIndex = random.GetRandomCardIndex(cardsInDeck);
		}
		while (((1L << randomCardIndex) & deadCardsMask) != 0L);
		return randomCardIndex.CardIndexToCard();
	}

	public static long GetRandomCardIndicesMask(this FastRandom random, int cardsCount, int cardsInDeck)
	{
		InlineList<int> randomCardIndices = random.GetRandomCardIndices(cardsCount, cardsInDeck);
		long num = 0L;
		InlineList<int>.Enumerator enumerator = randomCardIndices.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			num |= 1L << current;
		}
		return num;
	}

	public static InlineList<int> GetRandomCardIndices(this FastRandom random, int cardsCount, int cardsInDeck)
	{
		if (cardsCount > 10)
		{
			throw new InvalidOperationException("This call only for small numbers of cards");
		}
		InlineList<int> result = new InlineList<int>(cardsCount);
		for (int i = 0; i < cardsCount; i++)
		{
			int randomCardIndex = random.GetRandomCardIndex(cardsInDeck);
			result.Add(randomCardIndex);
		}
		return result;
	}

	private static int GetRandomCardIndex(this FastRandom random, int cardsInDeck)
	{
		if (cardsInDeck == 52)
		{
			return random.Next(52);
		}
		int num;
		do
		{
			num = random.Next(52);
		}
		while (num % 13 < 4);
		return num;
	}

	public static InlineList<int> GetRandomCardsMaskCactus(int cardsCount)
	{
		return FastRandomPool.ThreadShared.GetRandom().GetRandomCardsMaskCactus(cardsCount);
	}

	public static InlineList<int> GetRandomCardsMaskCactus(this FastRandom random, int cardsCount)
	{
		InlineList<int> result = new InlineList<int>(cardsCount);
		long num = 0L;
		for (int i = 0; i < cardsCount; i++)
		{
			int randomCardMaskCactus = random.GetRandomCardMaskCactus(num);
			num |= (uint)randomCardMaskCactus;
			result[i] = randomCardMaskCactus;
		}
		return result;
	}

	public static int GetRandomCardMaskCactus(this FastRandom random, long deadCards)
	{
		if (deadCards == 0L)
		{
			return random.Next(52).CardIndexToCardMaskCactus();
		}
		int num = random.Next(52).CardIndexToCardMaskCactus();
		if ((num & deadCards) == 0L)
		{
			return num;
		}
		do
		{
			num = random.Next(52).CardIndexToCardMaskCactus();
		}
		while ((num & deadCards) != 0L);
		return num;
	}
}
