using System.Collections.Immutable;

namespace Poker.Calc;

public static class PocketsRangeHoldemHelper
{
	extension(IPocketCards? cards)
	{
		public bool IsPocketAces => cards.IsPocketPair(CardRanks.Ace);

		public bool IsPocketKings => cards.IsPocketPair(CardRanks.King);

		public bool IsPocketQueens => cards.IsPocketPair(CardRanks.Queen);

		public bool IsAceKing
		{
			get
			{
				if (cards is PocketCardsHoldem { HighCard: { Rank: CardRanks.Ace }, LowCard: var lowCard })
				{
					return lowCard.Rank == CardRanks.King;
				}
				return false;
			}
		}

		public bool Is72o
		{
			get
			{
				if (cards is PocketCardsHoldem { HighCard: { Rank: CardRanks.Seven }, LowCard: { Rank: CardRanks.Deuce } } pocketCardsHoldem)
				{
					return pocketCardsHoldem.IsOffsuited;
				}
				return false;
			}
		}

		public bool Is82o
		{
			get
			{
				if (cards is PocketCardsHoldem { HighCard: { Rank: CardRanks.Eight }, LowCard: { Rank: CardRanks.Deuce } } pocketCardsHoldem)
				{
					return pocketCardsHoldem.IsOffsuited;
				}
				return false;
			}
		}

		public bool Is83o
		{
			get
			{
				if (cards is PocketCardsHoldem { HighCard: { Rank: CardRanks.Eight }, LowCard: { Rank: CardRanks.Three } } pocketCardsHoldem)
				{
					return pocketCardsHoldem.IsOffsuited;
				}
				return false;
			}
		}

		public bool Is92o
		{
			get
			{
				if (cards is PocketCardsHoldem { HighCard: { Rank: CardRanks.Nine }, LowCard: { Rank: CardRanks.Deuce } } pocketCardsHoldem)
				{
					return pocketCardsHoldem.IsOffsuited;
				}
				return false;
			}
		}

		public bool Is62o
		{
			get
			{
				if (cards is PocketCardsHoldem { HighCard: { Rank: CardRanks.Six }, LowCard: { Rank: CardRanks.Deuce } } pocketCardsHoldem)
				{
					return pocketCardsHoldem.IsOffsuited;
				}
				return false;
			}
		}
	}

	public static LambdaEqualityComparer<(PocketCardsHoldem, double)> PocketCardsWeightedEqualityComparer = new LambdaEqualityComparer<(PocketCardsHoldem, double)>(((PocketCardsHoldem pocket, double weight) first, (PocketCardsHoldem pocket, double weight) second) => first.pocket.Equals(second.pocket), ((PocketCardsHoldem pocket, double weight) x) => x.pocket.GetHashCode());

	public static bool IsPocketPair(this IPocketCards? cards, CardRanks pairRank)
	{
		if (cards is PocketCardsHoldem { IsPair: not false, HighCard: var highCard })
		{
			return highCard.Rank == pairRank;
		}
		return false;
	}

	public static bool IsPocketPair(this IPocketCards? cards, out CardRanks pairRank)
	{
		if (cards is PocketCardsHoldem { IsPair: not false } pocketCardsHoldem)
		{
			pairRank = pocketCardsHoldem.HighCard.Rank;
			return true;
		}
		pairRank = CardRanks.Deuce;
		return false;
	}

	public static bool IsSet(this IPocketCards? pockets, Board board)
	{
		if (pockets is PocketCardsHoldem && pockets.IsPocketPair(out var pairRank))
		{
			return board.Slice(Streets.Flop).Cards.Count((Card card) => card.Rank == pairRank) == 1;
		}
		return false;
	}

	public static PocketsRangeWeightedHoldem ToPocketsRangeWeightedHoldem(this PocketRangeHoldem pocketsRangeHoldem)
	{
		return pocketsRangeHoldem.Cards.ToPocketsRangeWeightedHoldem();
	}

	public static double[] GetWeights(this PocketsRangeWeightedHoldem rangeWeighted)
	{
		return rangeWeighted.Range.Select<(PocketCardsHoldem, double), double>(((PocketCardsHoldem pocket, double weightUnitInterval) item) => item.weightUnitInterval).ToArray();
	}

	public static PocketRangeHoldem ToPocketsRangeHoldem(this PocketsRangeWeightedHoldem weightedRange)
	{
		return weightedRange.Range.Select<(PocketCardsHoldem, double), PocketCardsHoldem>(((PocketCardsHoldem pocket, double weightUnitInterval) item) => item.pocket).ToPocketsRangeHoldem();
	}

	public static PocketsRangeWeightedHoldem ToPocketsRangeWeightedHoldem(this PocketRangeHoldem pocketsRangeHoldem, double weight)
	{
		return pocketsRangeHoldem.Cards.ToPocketsRangeWeightedHoldem(weight);
	}

	public static PocketsRangeWeightedHoldem ToPocketsRangeWeightedHoldemHoldem(this IEnumerable<(PocketCardsHoldem, double)> range)
	{
		return new PocketsRangeWeightedHoldem(range.ToImmutableArray());
	}

	public static PocketsRangeWeightedHoldem Concat(this PocketsRangeWeightedHoldem range, PocketsRangeWeightedHoldem otherRange)
	{
		return otherRange.Range.Union<(PocketCardsHoldem, double)>(range.Range, PocketCardsWeightedEqualityComparer).ToPocketsRangeWeightedHoldemHoldem();
	}

	public static PocketsRangeWeightedHoldem Except(this PocketsRangeWeightedHoldem range, PocketsRangeWeightedHoldem otherRange)
	{
		return range.Range.Except<(PocketCardsHoldem, double)>(otherRange.Range, PocketCardsWeightedEqualityComparer).ToPocketsRangeWeightedHoldemHoldem();
	}

	public static PocketsRangeWeightedHoldem ToPocketsRangeWeightedHoldem(this IEnumerable<PocketCardsHoldem> range, double weightUnitInterval)
	{
		return new PocketsRangeWeightedHoldem(range.Select((PocketCardsHoldem pocket) => (pocket: pocket, weightUnitInterval: weightUnitInterval)).ToImmutableArray());
	}

	public static PocketsRangeWeightedHoldem ToPocketsRangeWeightedHoldem(this IEnumerable<PocketCardsHoldem> range)
	{
		return new PocketsRangeWeightedHoldem(range.Select((PocketCardsHoldem pocket) => (pocket: pocket, 1.0)).ToImmutableArray());
	}

	public static PocketRangeHoldem ToPocketsRangeHoldem(this IEnumerable<PocketCardsHoldem> pocketCards)
	{
		return new PocketRangeHoldem(pocketCards.Distinct().ToImmutableArray());
	}

	public static IPreflopRange ExcludeDeadCards(this IPreflopRange range, InlineList<Card> deadCards)
	{
		if (!(range is PocketsRangeWeightedHoldem range2))
		{
			if (range is PocketRangeHoldem range3)
			{
				return range3.ExcludeDeadCards(deadCards);
			}
			throw new NotImplementedException();
		}
		return range2.ExcludeDeadCards(deadCards);
	}

	public static PocketRangeHoldem ExcludeDeadCards(this PocketRangeHoldem range, InlineList<Card> deadCards)
	{
		if (deadCards.Count == 0)
		{
			return range;
		}
		return new PocketRangeHoldem(range.Cards.Where((PocketCardsHoldem x) => !x.IsDead(deadCards)).ToImmutableArray());
	}

	public static PocketsRangeWeightedHoldem ExcludeDeadCards(this PocketsRangeWeightedHoldem range, InlineList<Card> deadCards)
	{
		if (deadCards.Count == 0)
		{
			return range;
		}
		return new PocketsRangeWeightedHoldem(range.Range.Where<(PocketCardsHoldem, double)>(((PocketCardsHoldem pocket, double weightUnitInterval) x) => !x.pocket.IsDead(deadCards)).ToImmutableArray());
	}

	public static bool IsDead(this PocketRangeHoldem range, Span<Card> deadCards)
	{
		ImmutableArray<PocketCardsHoldem>.Enumerator enumerator = range.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			InlineList<Card>.Enumerator enumerator2 = enumerator.Current.Cards.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				if (enumerator2.Current.IsDead(deadCards))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool ContainsDeadCards(this PocketRangeHoldem range, Span<Card> deadCards)
	{
		foreach (Card possibleCard in range.GetPossibleCards())
		{
			if (possibleCard.IsDead(deadCards))
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainsDeadCards(this IEnumerable<PocketCardsHoldem> range, Span<Card> deadCards)
	{
		foreach (PocketCardsHoldem item in range)
		{
			InlineList<Card>.Enumerator enumerator2 = item.Cards.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				if (enumerator2.Current.IsDead(deadCards))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static PocketRangeHoldem Concat(this IEnumerable<PocketRangeHoldem> ranges)
	{
		return ranges.Aggregate((PocketRangeHoldem x, PocketRangeHoldem y) => x.Concat(y));
	}

	public static PocketRangeHoldem Concat(this PocketRangeHoldem first, PocketRangeHoldem second)
	{
		return new PocketRangeHoldem(first.Cards.Concat(second.Cards).Distinct().ToImmutableArray());
	}

	public static PocketRangeHoldem Except(this PocketRangeHoldem first, PocketRangeHoldem second)
	{
		return new PocketRangeHoldem(first.Cards.Except(second.Cards).ToImmutableArray());
	}

	public static long[][] GetCardMasks(this IEnumerable<PocketRangeHoldem> ranges)
	{
		return ranges.MapToArray((PocketRangeHoldem range) => range.GetCardMasks());
	}

	public static long[] GetCardMasks(this PocketRangeHoldem range)
	{
		return range.Cards.MapToArray((PocketCardsHoldem x) => x.Cards.ToCardsMask());
	}

	public static PocketRangeHoldem VerifyNotEmpty(this PocketRangeHoldem range)
	{
		if (!range.IsEmpty)
		{
			return range;
		}
		throw new InvalidOperationException("Expecting not empty PocketRangeHoldem");
	}

	public static PocketRangeHoldem VerifyShortDeck(this PocketRangeHoldem range)
	{
		ImmutableArray<PocketCardsHoldem>.Enumerator enumerator = range.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PocketCardsHoldem current = enumerator.Current;
			if (!current.IsShortDeck())
			{
				throw new InvalidOperationException($"Expecting short deck pockets, but was - {current}");
			}
		}
		return range;
	}

	public static IEnumerable<PocketCardsHoldem> GetPocketCardsHoldem(this IPreflopRange preflopRange)
	{
		ImmutableArray<PocketCardsHoldem> cards;
		if (!(preflopRange is PocketRangeHoldem pocketRangeHoldem))
		{
			if (!(preflopRange is PocketsRangeWeightedHoldem weightedRange))
			{
				if (!(preflopRange is PreflopRangeHoldem range))
				{
					throw new NotImplementedException();
				}
				cards = range.ToPocketRangeHoldem().Cards;
			}
			else
			{
				cards = weightedRange.ToPocketsRangeHoldem().Cards;
			}
		}
		else
		{
			cards = pocketRangeHoldem.Cards;
		}
		return cards;
	}

	public static IPreflopRange VerifyShortDeck(this IPreflopRange preflopRange)
	{
		preflopRange.GetPocketCardsHoldem().ForEach(delegate (PocketCardsHoldem pocket)
		{
			if (!pocket.IsShortDeck())
			{
				throw new InvalidOperationException($"Expecting short deck pockets, but was - {pocket}");
			}
		});
		return preflopRange;
	}

	public static bool IsShortDeck(this PocketRangeHoldem range)
	{
		return Enumerable.All(range.Cards, (PocketCardsHoldem pockets) => pockets.IsShortDeck());
	}

	public static PreflopRangeHoldemCompact ToPreflopRangeHoldemCompact(this PocketCardsHoldem pocketCards)
	{
		return PreflopRangeHoldemCompact.Empty.AddCell(pocketCards.GetCellIndex());
	}

	public static PreflopRangeHoldemCompact ToPreflopRangeHoldemCompact(this int cellIndex)
	{
		return PreflopRangeHoldemCompact.Empty.AddCell(cellIndex);
	}

	public static PreflopRangeHoldemCompact ToPreflopRangeHoldemCompact(this IPocketCards pockets)
	{
		if (!(pockets is PocketCardsHoldem pocketCards))
		{
			if (pockets is PocketCardsOmaha)
			{
				return PreflopRangeHoldemCompact.Empty;
			}
			throw new NotImplementedException(pockets.GetType().Name);
		}
		return pocketCards.ToPreflopRangeHoldemCompact();
	}

	public static PreflopRangeHoldemCompact ParsePreflopRangeHoldemCompact(this string rangeAJs)
	{
		PreflopRangeHoldemCompact result = PreflopRangeHoldemCompact.Empty;
		string[] array = rangeAJs.Split(',');
		foreach (string text in array)
		{
			result = result.AddCell(text.Trim().ParsePreflopRangeHoldemCell().GetIndex());
		}
		return result;
	}

	public static PreflopRangeHoldemCompact ToPreflopRangeHoldemCompact(this IEnumerable<int> cellIndices)
	{
		PreflopRangeHoldemCompact result = PreflopRangeHoldemCompact.Empty;
		foreach (int cellIndex in cellIndices)
		{
			result = result.AddCell(cellIndex);
		}
		return result;
	}

	public static bool IsShortDeckHoldemCellIndex(this int cellIndex)
	{
		return cellIndex.ToPreflopRangeHoldemCell().LowRank >= CardRanks.Six;
	}

	public static string GetGtoWizardString(this PocketsRangeWeightedHoldem range)
	{
		return range.Range.AggregateToString<(PocketCardsHoldem, double), string>(((PocketCardsHoldem pocket, double weightUnitInterval) cell) => $"{cell.pocket.AbbreviationAhJs}: {cell.weightUnitInterval.Rounded(4)}", ",");
	}
}
