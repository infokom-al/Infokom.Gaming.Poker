namespace Poker.Calc;

public static class PreflopRangeOmahaGeneration
{
	public static CardRanks[] AllCardRanks = Enum.GetValues<CardRanks>().Reversed().ToArray();

	public static int[] CardRanksPrimes = new int[13]
	{
		2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
		31, 37, 41
	}.Reversed().ToArray();

	public static IEnumerable<InlineList<Card>> GetAllOmahaCombos(this int cardCount)
	{
		InlineList<Card> result = default(InlineList<Card>);
		for (int i = 0; i < cardCount; i++)
		{
			foreach (Card item in result.GenerateNextCard())
			{
				result.Add(item);
				if (result.Count == cardCount)
				{
					yield return result;
					result.Clear();
				}
			}
		}
	}

	public static IEnumerable<Card> GenerateNextCard(this InlineList<Card> currentCards)
	{
		CardRanks[] allCardRanks = AllCardRanks;
		foreach (CardRanks rank in allCardRanks)
		{
			if (currentCards.Count > 0)
			{
				if (currentCards[currentCards.Length - 1].Rank < rank)
				{
					continue;
				}
			}
			int num = currentCards.Count((Card card, CardRanks cardRanks) => card.Rank == cardRanks, rank);
			if (num == 4)
			{
				continue;
			}
			foreach (Suits item in Card.AllSuits.AsEnumerable().Skip(num))
			{
				yield return new Card(rank, item);
			}
		}
	}

	private static IEnumerable<InlineList<CardRanks>> GenerateRanks(this PokerGames game)
	{
		return game switch
		{
			PokerGames.Omaha => GenerateRanksOmaha(),
			PokerGames.OmahaFive => GenerateRanksOmahaFive(),
			PokerGames.OmahaSix => GenerateRanksOmahaSix(),
			_ => throw new NotImplementedException($"Game {game} is not supported for Omaha generation"),
		};
	}

	private static IEnumerable<InlineList<CardRanks>> GenerateRanksOmaha()
	{
		HashSet<long> returnedMasks = new HashSet<long>();
		CardRanks[] allCardRanks = AllCardRanks;
		foreach (CardRanks firstRank in allCardRanks)
		{
			CardRanks[] allCardRanks2 = AllCardRanks;
			foreach (CardRanks secondRank in allCardRanks2)
			{
				CardRanks[] allCardRanks3 = AllCardRanks;
				foreach (CardRanks thirdRank in allCardRanks3)
				{
					CardRanks[] allCardRanks4 = AllCardRanks;
					foreach (CardRanks seat in allCardRanks4)
					{
						InlineList<CardRanks> inlineList = default(InlineList<CardRanks>);
						inlineList.Add(firstRank);
						inlineList.Add(secondRank);
						inlineList.Add(thirdRank);
						inlineList.Add(seat);
						long mask = inlineList.GetMask();
						if (!returnedMasks.Contains(mask))
						{
							yield return inlineList;
							returnedMasks.Add(mask);
						}
					}
				}
			}
		}
	}

	private static IEnumerable<InlineList<CardRanks>> GenerateRanksOmahaFive()
	{
		HashSet<long> returnedMasks = new HashSet<long>();
		CardRanks[] allCardRanks = AllCardRanks;
		foreach (CardRanks firstRank in allCardRanks)
		{
			CardRanks[] allCardRanks2 = AllCardRanks;
			foreach (CardRanks secondRank in allCardRanks2)
			{
				CardRanks[] allCardRanks3 = AllCardRanks;
				foreach (CardRanks thirdRank in allCardRanks3)
				{
					CardRanks[] allCardRanks4 = AllCardRanks;
					foreach (CardRanks fourthRank in allCardRanks4)
					{
						CardRanks[] allCardRanks5 = AllCardRanks;
						foreach (CardRanks cardRanks in allCardRanks5)
						{
							InlineList<CardRanks> items = default(InlineList<CardRanks>);
							items.Add(firstRank);
							items.Add(secondRank);
							items.Add(thirdRank);
							items.Add(fourthRank);
							if (items.Count(cardRanks) != 4)
							{
								items.Add(cardRanks);
								long mask = items.GetMask();
								if (!returnedMasks.Contains(mask))
								{
									yield return items;
									returnedMasks.Add(mask);
								}
							}
						}
					}
				}
			}
		}
	}

	private static IEnumerable<InlineList<CardRanks>> GenerateRanksOmahaSix()
	{
		HashSet<long> returnedMasks = new HashSet<long>();
		CardRanks[] allCardRanks = AllCardRanks;
		foreach (CardRanks firstRank in allCardRanks)
		{
			CardRanks[] allCardRanks2 = AllCardRanks;
			foreach (CardRanks secondRank in allCardRanks2)
			{
				CardRanks[] allCardRanks3 = AllCardRanks;
				foreach (CardRanks thirdRank in allCardRanks3)
				{
					CardRanks[] allCardRanks4 = AllCardRanks;
					foreach (CardRanks fourthRank in allCardRanks4)
					{
						CardRanks[] allCardRanks5 = AllCardRanks;
						foreach (CardRanks fifthRank in allCardRanks5)
						{
							CardRanks[] allCardRanks6 = AllCardRanks;
							foreach (CardRanks cardRanks in allCardRanks6)
							{
								InlineList<CardRanks> items = default(InlineList<CardRanks>);
								items.Add(firstRank);
								items.Add(secondRank);
								items.Add(thirdRank);
								items.Add(fourthRank);
								if (items.Count(fifthRank) == 4)
								{
									continue;
								}
								items.Add(fifthRank);
								if (items.Count(cardRanks) < 4)
								{
									items.Add(cardRanks);
									long mask = items.GetMask();
									if (!returnedMasks.Contains(mask))
									{
										yield return items;
										returnedMasks.Add(mask);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static long GetRankMask(this InlineList<Card> cards)
	{
		long num = 0L;
		InlineList<Card>.Enumerator enumerator = cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Card current = enumerator.Current;
			int num2 = CardRanksPrimes[(int)current.Rank];
			num = ((num != 0L) ? (num * num2) : num2);
		}
		return num;
	}

	public static long GetMask(this InlineList<CardRanks> ranks)
	{
		long num = 0L;
		InlineList<CardRanks>.Enumerator enumerator = ranks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CardRanks current = enumerator.Current;
			int num2 = CardRanksPrimes[(int)current];
			num = ((num != 0L) ? (num * num2) : num2);
		}
		return num;
	}

	public static IEnumerable<InlineList<Card>> GenerateOmahaPockets(this PokerGames game)
	{
		HashSet<long> returnedMasks = new HashSet<long>();
		foreach (InlineList<CardRanks> item in game.GenerateRanks())
		{
			InlineList<CardRanks> orderedRanks = item.OrderByDescending();
			foreach (InlineList<Suits> item2 in game.GenerateSuits())
			{
				InlineList<Card> inlineList = orderedRanks.ZipCards(item2).OrderBySuitness();
				if (inlineList.IsDistinct())
				{
					long omahaPocketMask = inlineList.GetOmahaPocketMask();
					if (!returnedMasks.Contains(omahaPocketMask))
					{
						returnedMasks.Add(omahaPocketMask);
						yield return inlineList;
					}
				}
			}
		}
	}

	private static bool IsDistinct(this InlineList<Card> cards)
	{
		for (int i = 1; i < cards.Count; i++)
		{
			if (cards[i] == cards[i - 1])
			{
				return false;
			}
		}
		return true;
	}

	public static InlineList<CardRanks> OrderByDescending(this InlineList<CardRanks> ranks)
	{
		return ranks.OrderByDescending((CardRanks rank) => (int)rank);
	}

	public static InlineList<Card> ZipCards(this InlineList<CardRanks> ranks, InlineList<Suits> suits)
	{
		InlineList<Card> result = default(InlineList<Card>);
		for (int i = 0; i < ranks.Count; i++)
		{
			result.Add(new Card(ranks[i], suits[i]));
		}
		return result;
	}

	public static IEnumerable<InlineList<Suits>> GenerateSuits(this PokerGames game)
	{
		return game switch
		{
			PokerGames.Omaha => GenerateSuitsOmaha(),
			PokerGames.OmahaFive => GenerateSuitsOmahaFive(),
			PokerGames.OmahaSix => GenerateSuitsOmahaSix(),
			_ => throw new NotImplementedException($"Game {game} is not supported for Omaha generation"),
		};
	}

	public static IEnumerable<InlineList<Suits>> GenerateSuitsOmaha()
	{
		Suits[] allSuits = Card.AllSuits;
		foreach (Suits firstSuit in allSuits)
		{
			Suits[] allSuits2 = Card.AllSuits;
			foreach (Suits second in allSuits2)
			{
				Suits[] allSuits3 = Card.AllSuits;
				foreach (Suits third in allSuits3)
				{
					Suits[] allSuits4 = Card.AllSuits;
					foreach (Suits seat in allSuits4)
					{
						InlineList<Suits> inlineList = default(InlineList<Suits>);
						inlineList.Add(firstSuit);
						inlineList.Add(second);
						inlineList.Add(third);
						inlineList.Add(seat);
						yield return inlineList;
					}
				}
			}
		}
	}

	public static IEnumerable<InlineList<Suits>> GenerateSuitsOmahaFive()
	{
		Suits[] allSuits = Card.AllSuits;
		foreach (Suits firstSuit in allSuits)
		{
			Suits[] allSuits2 = Card.AllSuits;
			foreach (Suits second in allSuits2)
			{
				Suits[] allSuits3 = Card.AllSuits;
				foreach (Suits third in allSuits3)
				{
					Suits[] allSuits4 = Card.AllSuits;
					foreach (Suits forth in allSuits4)
					{
						Suits[] allSuits5 = Card.AllSuits;
						foreach (Suits seat in allSuits5)
						{
							InlineList<Suits> inlineList = default(InlineList<Suits>);
							inlineList.Add(firstSuit);
							inlineList.Add(second);
							inlineList.Add(third);
							inlineList.Add(forth);
							inlineList.Add(seat);
							yield return inlineList;
						}
					}
				}
			}
		}
	}

	public static IEnumerable<InlineList<Suits>> GenerateSuitsOmahaSix()
	{
		Suits[] allSuits = Card.AllSuits;
		foreach (Suits firstSuit in allSuits)
		{
			Suits[] allSuits2 = Card.AllSuits;
			foreach (Suits second in allSuits2)
			{
				Suits[] allSuits3 = Card.AllSuits;
				foreach (Suits third in allSuits3)
				{
					Suits[] allSuits4 = Card.AllSuits;
					foreach (Suits forth in allSuits4)
					{
						Suits[] allSuits5 = Card.AllSuits;
						foreach (Suits fifth in allSuits5)
						{
							Suits[] allSuits6 = Card.AllSuits;
							foreach (Suits seat in allSuits6)
							{
								InlineList<Suits> inlineList = default(InlineList<Suits>);
								inlineList.Add(firstSuit);
								inlineList.Add(second);
								inlineList.Add(third);
								inlineList.Add(forth);
								inlineList.Add(fifth);
								inlineList.Add(seat);
								yield return inlineList;
							}
						}
					}
				}
			}
		}
	}

	public static PocketCardsOmaha GetPocketCardsOmahaSampleFromMask(this long mask, PokerGames game)
	{
		InlineList<Card> cards = default(InlineList<Card>);
		for (int i = 0; i < game.PocketCardsCount(); i++)
		{
			long num = (mask >> i * 6) & 0x3F;
			CardRanks rank = (CardRanks)(num >> 2);
			Suits suit = (Suits)(num & 3);
			cards.Add(new Card(rank, suit));
		}
		return PocketCardsOmaha.Create(cards);
	}

	public static long GetOmahaPocketMask(this InlineList<Card> cards)
	{
		InlineList<(Card, int)> inlineList = cards.OrderMaskCards().WithSuitIndex();
		long num = 0L;
		for (int i = 0; i < cards.Count; i++)
		{
			(Card, int) tuple = inlineList[i];
			Card item = tuple.Item1;
			int item2 = tuple.Item2;
			num |= (((long)item.Rank << 2) | item2) << i * 6;
		}
		return num;
	}

	public static InlineList<(Card card, int suitIndex)> WithSuitIndex(this InlineList<Card> cards)
	{
		int num = 0;
		int num2 = -1;
		InlineList<(Card, int)> result = default(InlineList<(Card, int)>);
		InlineList<Card>.Enumerator enumerator = cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Card current = enumerator.Current;
			Suits suit = current.Suit;
			int num3 = 1 << (int)suit;
			if ((num & num3) == 0)
			{
				num2++;
				result.Add((current, num2));
				num |= num3;
			}
			else
			{
				result.Add((current, num2));
			}
		}
		return result;
	}

	public static InlineList<Card> OrderMaskCards(this InlineList<Card> cards)
	{
		return cards.OrderBySuitness().SplitBySuit().OrderByDescending((InlineList<Card> inlineList) => inlineList.Length, (InlineList<Card> cards2) => cards2.GetRankMask())
			.Flatten();
	}
}
