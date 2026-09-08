namespace Poker.Calc;

public readonly struct PreflopRangeHoldemCell
{
	public CardRanks HighRank { get; }

	public CardRanks LowRank { get; }

	public Suitness Suitness { get; }

	public bool IsPair => HighRank == LowRank;

	public bool IsSuited => Suitness == Suitness.Suited;

	public bool IsOffsuited => Suitness == Suitness.Offsuited;

	public string Abbreviation => HighRank.ToAbbreviation() + LowRank.ToAbbreviation() + (IsPair ? null : Suitness.ToAbbreviation());

	public int Column
	{
		get
		{
			if (!IsPair)
			{
				if (!IsSuited)
				{
					return (int)(12 - HighRank);
				}
				return (int)(12 - LowRank);
			}
			return (int)(12 - HighRank);
		}
	}

	public int Row
	{
		get
		{
			if (!IsPair)
			{
				if (!IsSuited)
				{
					return (int)(12 - LowRank);
				}
				return (int)(12 - HighRank);
			}
			return (int)(12 - HighRank);
		}
	}

	public int Combos
	{
		get
		{
			if (!IsPair)
			{
				if (!IsSuited)
				{
					return 12;
				}
				return 4;
			}
			return 6;
		}
	}

	public PreflopRangeHoldemCell(CardRanks rank1, CardRanks rank2, Suitness suitness)
	{
		if (rank1 == rank2 && suitness == Suitness.Suited)
		{
			throw new ArgumentException("Pair can't be suited", "suitness");
		}
		if (rank1 >= rank2)
		{
			HighRank = rank1;
			LowRank = rank2;
		}
		else
		{
			HighRank = rank2;
			LowRank = rank1;
		}
		Suitness = suitness;
		if (rank2 > CardRanks.Ace)
		{
			throw new ArgumentException($"Card rank can't be {rank2}", "rank2");
		}
		if (rank1 > CardRanks.Ace)
		{
			throw new ArgumentException($"Card rank can't be {rank1}", "rank1");
		}
	}

	public int GetPreflopRangeIndex(PokerGames game)
	{
		return game.RanksInDeck() * Row + Column;
	}

	public int GetIndex()
	{
		return 13 * Row + Column;
	}

	public IEnumerable<CardRanks> Ranks()
	{
		yield return HighRank;
		yield return LowRank;
	}

	public static PreflopRangeHoldemCell Pair(CardRanks pairRank)
	{
		return new PreflopRangeHoldemCell(pairRank, pairRank, Suitness.Offsuited);
	}

	public static PreflopRangeHoldemCell FromCards(Card card1, Card card2)
	{
		CardRanks rank = Cards.Max(card1.Rank, card2.Rank);
		CardRanks rank2 = Cards.Min(card1.Rank, card2.Rank);
		return new PreflopRangeHoldemCell(rank, rank2, (card1.Suit == card2.Suit) ? Suitness.Suited : Suitness.Offsuited);
	}

	public override string ToString()
	{
		return Abbreviation;
	}
}
