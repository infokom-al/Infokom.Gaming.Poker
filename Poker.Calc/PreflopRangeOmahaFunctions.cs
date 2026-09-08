using Poker.Calc.Common;

namespace Poker.Calc;

public static class PreflopRangeOmahaFunctions
{
	public static bool HitsRange(this PocketCardsOmaha hand, OmahaPreflopRange range, Range topRange)
	{
		if (hand.HitsRange(range))
		{
			return hand.HitsTopRange(topRange);
		}
		return false;
	}

	public static bool HitsRange(this PocketCardsOmaha hand, OmahaPreflopRange range)
	{
		if (range.IsEmpty)
		{
			return false;
		}
		return hand.GetOmahaPreflopRange().Intersects(range);
	}

	public static bool HitsTopRange(this PocketCardsOmaha hand, Range range)
	{
		if (range.ContainsZeroToHundred)
		{
			return true;
		}
		int num = hand.GetTopRangePosition() / hand.Cards.Count.GetTopRangeHandCount();
		return range.Contains(num);
	}

	public static double GetRangeShareProbabalistic(this OmahaPreflopRange range, PokerGames game, Range allowedTopRange, int trials = 10000)
	{
		if (range.IsEmpty)
		{
			return 0.0;
		}
		int cardsInDeck = game.CardsInDeck();
		FastRandom random = FastRandomPool.ThreadShared.GetRandom();
		int num = 0;
		int cardsCount = game.PocketCardsCount();
		for (int i = 0; i < trials; i++)
		{
			PocketCardsOmaha pocketCardsOmaha = random.GetRandomCardsInlineList(cardsCount, cardsInDeck, 0L).ToPocketCardsOmaha();
			if (pocketCardsOmaha.HitsTopRange(allowedTopRange) && pocketCardsOmaha.GetOmahaPreflopRange().Intersects(range))
			{
				num++;
			}
		}
		return (double)num / (double)trials;
	}

	public static double GetRangeShare(this OmahaPreflopRange range, PokerGames game, Range allowedTopRange)
	{
		if (allowedTopRange.ContainsZeroToHundred)
		{
			return range.GetRangeShare(game);
		}
		if (range.IsEmpty)
		{
			return allowedTopRange.Length;
		}
		if (game != PokerGames.Omaha)
		{
			return range.GetRangeShareProbabalistic(game, allowedTopRange);
		}
		HashSet<long> hashSet = range.GetHands(game).ToHashSet();
		IEnumerable<long> topRangeHands = allowedTopRange.GetTopRangeHands(game);
		int num = 0;
		foreach (long item in topRangeHands)
		{
			if (hashSet.Contains(item))
			{
				num++;
			}
		}
		return (double)num / (double)game.GetTopRangeHandCount();
	}

	public static double GetRangeShare(this OmahaPreflopRange range, PokerGames game)
	{
		if (range.IsEmpty)
		{
			return 0.0;
		}
		if (game != PokerGames.Omaha)
		{
			return range.GetRangeShareProbabalistic(game, Range.ZeroToHundred);
		}
		return (double)range.GetHands(game).Count() / (double)game.GetTopRangeHandCount();
	}

	public static IEnumerable<long> GetHands(this OmahaPreflopRange range, PokerGames game)
	{
		if (game == PokerGames.Omaha)
		{
			Dictionary<long, int>.KeyCollection keys = OmahaTopRange.Hands.Keys;
			Dictionary<long, int>.KeyCollection keyCollection = keys;
			if (range.IsEmpty)
			{
				yield break;
			}
			foreach (long item in keyCollection)
			{
				if (item.GetPocketCardsOmahaSampleFromMask(game).GetOmahaPreflopRange().Intersects(range))
				{
					yield return item;
				}
			}
			yield break;
		}
		throw new InvalidOperationException($"Invalid game {game}");
	}

	public static IEnumerable<long> GetTopRangeHands(this Range range, PokerGames game)
	{
		Dictionary<long, int> dictionary = game switch
		{
			PokerGames.Omaha => OmahaTopRange.Hands,
			PokerGames.OmahaFive => OmahaFiveTopRange.Hands,
			PokerGames.OmahaSix => OmahaSixTopRange.Hands,
			_ => throw new InvalidOperationException($"Invalid game {game}"),
		};
		if (range.ContainsZeroToHundred)
		{
			foreach (long key in dictionary.Keys)
			{
				yield return key;
			}
			yield break;
		}
		int minRank = (int)(range.Left / 100.0 * (double)dictionary.Count);
		int maxRank = (int)(range.Right / 100.0 * (double)dictionary.Count);
		foreach (var (num3, num4) in dictionary)
		{
			if (minRank <= num4 && num4 <= maxRank)
			{
				yield return num3;
			}
		}
	}

	public static double GetTopRangePositionInPercent(this PocketCardsOmaha hand)
	{
		return (double)hand.GetTopRangePosition() / (double)hand.Cards.Count.GetTopRangeHandCount() * 100.0;
	}

	public static int GetTopRangePosition(this PocketCardsOmaha hand)
	{
		return hand.Cards.Count switch
		{
			4 => OmahaTopRange.Hands[hand.Cards.GetOmahaPocketMask()],
			5 => OmahaFiveTopRange.Hands[hand.Cards.GetOmahaPocketMask()],
			6 => OmahaSixTopRange.Hands[hand.Cards.GetOmahaPocketMask()],
			_ => throw new InvalidOperationException($"Invalid pocket cards count {hand.Cards.Count}"),
		};
	}

	public static int GetTopRangeHandCount(this PokerGames game)
	{
		return game.PocketCardsCount().GetTopRangeHandCount();
	}

	private static int GetTopRangeHandCount(this int cardCount)
	{
		return cardCount switch
		{
			4 => 16432,
			5 => 134459,
			6 => 962988,
			_ => throw new InvalidOperationException($"Invalid card count {cardCount}"),
		};
	}

	public static bool Intersects(this OmahaPreflopRange range, OmahaPreflopRange other)
	{
		if ((range.Pairs & other.Pairs) == OmahaPreflopRangePairs.None && (range.Cards & other.Cards) == OmahaPreflopRangeCards.None && (range.Suiteness & other.Suiteness) == OmahaPreflopRangeSuiteness.None && (range.Straightness & other.Straightness) == OmahaPreflopRangeStraightness.None && (range.SixCards & other.SixCards) == OmahaSixPreflopRangeCards.None)
		{
			return (range.Other & other.Other) != 0;
		}
		return true;
	}

	public static OmahaPreflopRange GetOmahaPreflopRange(this PocketCardsOmaha pocketCards)
	{
		return pocketCards.Cards.GetOmahaPreflopRange();
	}

	public static OmahaPreflopRange GetOmahaPreflopRange(this InlineList<Card> cards)
	{
		return new OmahaPreflopRange(cards.GetOmahaPreflopRangePairs(), cards.GetOmahaPreflopRangeCards(), cards.GetOmahaPreflopRangeSuiteness(), cards.GetOmahaPreflopRangeStraightness(), cards.GetOmahaSixPreflopRangeCards(), cards.GetOmahaPreflopRangeOther());
	}

	public static OmahaPreflopRangePairs GetOmahaPreflopRangePairs(this InlineList<Card> cards)
	{
		OmahaPreflopRangePairs omahaPreflopRangePairs = OmahaPreflopRangePairs.None;
		InlineList<CardRanks> inlineList = cards.GetRepeatedRanks().Except(((CardRanks rank, int repeationCount) item) => item.repeationCount > 2).Map(((CardRanks rank, int repeationCount) item) => item.rank);
		if (inlineList.Count == 0)
		{
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.Unpaired;
		}
		if (inlineList.Count == 1)
		{
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.SinglePair;
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.TopPairRankAce.GetShiftedRank(inlineList[0]);
		}
		else if (inlineList.Count == 2)
		{
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.DoublePair;
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.TopPairRankAce.GetShiftedRank(inlineList[0]);
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.SecondPairRankKing.GetShiftedRankSinceKing(inlineList[1]);
		}
		else if (inlineList.Count == 3)
		{
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.TriplePair;
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.TopPairRankAce.GetShiftedRank(inlineList[0]);
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.SecondPairRankKing.GetShiftedRankSinceKing(inlineList[1]);
			omahaPreflopRangePairs |= OmahaPreflopRangePairs.ThirdPairRankQueen.GetShiftedRankSinceQueen(inlineList[2]);
		}
		return omahaPreflopRangePairs;
	}

	public static OmahaPreflopRangeSuiteness GetOmahaPreflopRangeSuiteness(this InlineList<Card> cards)
	{
		InlineList<InlineList<Card>> inlineList = from inlineList2 in cards.OrderBySuitness().GetSuitedCards()
										  orderby (int)inlineList2[0].Rank descending
										  select inlineList2;
		OmahaPreflopRangeSuiteness omahaPreflopRangeSuiteness = OmahaPreflopRangeSuiteness.None;
		if (inlineList.Count == 0)
		{
			return OmahaPreflopRangeSuiteness.Rainbow;
		}
		if (inlineList.Count == 1)
		{
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.SingleSuited;
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.TopSuitRankAce.GetShiftedRank(inlineList[0][0].Rank);
		}
		else if (inlineList.Count == 2)
		{
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.DoubleSuited;
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.TopSuitRankAce.GetShiftedRank(inlineList[0][0].Rank);
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.SecondSuitRankAce.GetShiftedRank(inlineList[1][0].Rank);
		}
		else if (inlineList.Count == 3)
		{
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.TripleSuited;
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.TopSuitRankAce.GetShiftedRank(inlineList[0][0].Rank);
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.SecondSuitRankAce.GetShiftedRank(inlineList[1][0].Rank);
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.ThirdSuitRankAce.GetShiftedRank(inlineList[2][0].Rank);
		}
		bool flag = false;
		InlineList<InlineList<Card>>.Enumerator enumerator = inlineList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			InlineList<Card> current = enumerator.Current;
			if (current.Count == cards.Count)
			{
				omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.Monotone;
			}
			if (current.Count == 2)
			{
				omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.TwoCardsOfOneSuit;
			}
			else if (current.Count == 3)
			{
				omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.ThreeCardsOfOneSuit;
			}
			else if (current.Count == 4)
			{
				omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.FourCardsOfOneSuit;
			}
			else if (current.Count == 5)
			{
				omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.FiveCardsOfOneSuit;
			}
			if (current.Count > 2)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			omahaPreflopRangeSuiteness |= OmahaPreflopRangeSuiteness.NoMoreThanTwoCardsOfOneSuit;
		}
		return omahaPreflopRangeSuiteness;
	}

	public static OmahaPreflopRangeStraightness GetOmahaPreflopRangeStraightness(this InlineList<Card> cards)
	{
		OmahaPreflopRangeStraightness omahaPreflopRangeStraightness = OmahaPreflopRangeStraightness.None;
		InlineList<InlineList<CardRanks>> omahaRundowns = cards.GetOmahaRundowns();
		if (omahaRundowns.Count == 2)
		{
			omahaPreflopRangeStraightness |= OmahaPreflopRangeStraightness.DoubleRundown;
		}
		if (omahaRundowns.Count >= 1)
		{
			omahaPreflopRangeStraightness |= OmahaPreflopRangeStraightness.TopRundownRankAce.GetShiftedRank(omahaRundowns[0][0]);
		}
		if (omahaRundowns.Count >= 2)
		{
			omahaPreflopRangeStraightness |= OmahaPreflopRangeStraightness.SecondRundownRankKing.GetShiftedRankSinceKing(omahaRundowns[1][0]);
		}
		InlineList<InlineList<CardRanks>>.Enumerator enumerator = omahaRundowns.GetEnumerator();
		while (enumerator.MoveNext())
		{
			InlineList<CardRanks> current = enumerator.Current;
			if (current.Count == 3)
			{
				omahaPreflopRangeStraightness |= OmahaPreflopRangeStraightness.ThreeCardRundown;
			}
			else if (current.Count == 4)
			{
				omahaPreflopRangeStraightness |= OmahaPreflopRangeStraightness.FourCardRundown;
			}
			else if (current.Count == 5)
			{
				omahaPreflopRangeStraightness |= OmahaPreflopRangeStraightness.FiveCardRundown;
			}
			else if (current.Count == 6)
			{
				omahaPreflopRangeStraightness |= OmahaPreflopRangeStraightness.SixCardRundown;
			}
			RundownKind rundownKind = current.GetRundownKind();
			omahaPreflopRangeStraightness |= rundownKind.GetStraightness();
		}
		return omahaPreflopRangeStraightness;
	}

	public static OmahaPreflopRangeCards GetOmahaPreflopRangeCards(this InlineList<Card> cards)
	{
		return OmahaPreflopRangeCards.None | OmahaPreflopRangeCards.TopCardRankAce.GetShiftedRank(cards[0].Rank) | OmahaPreflopRangeCards.SecondCardRankAce.GetShiftedRank(cards[1].Rank) | OmahaPreflopRangeCards.ThirdCardRankAce.GetShiftedRank(cards[2].Rank) | OmahaPreflopRangeCards.FourthCardRankAce.GetShiftedRank(cards[3].Rank);
	}

	public static OmahaSixPreflopRangeCards GetOmahaSixPreflopRangeCards(this InlineList<Card> cards)
	{
		if (cards.Count < 5)
		{
			return OmahaSixPreflopRangeCards.None;
		}
		OmahaSixPreflopRangeCards omahaSixPreflopRangeCards = OmahaSixPreflopRangeCards.FifthCardRankAce.GetShiftedRank(cards[4].Rank);
		if (cards.Count > 5)
		{
			omahaSixPreflopRangeCards |= OmahaSixPreflopRangeCards.SixthCardRankAce.GetShiftedRank(cards[5].Rank);
		}
		return omahaSixPreflopRangeCards;
	}

	public static OmahaPreflopRangeOther GetOmahaPreflopRangeOther(this InlineList<Card> cards)
	{
		OmahaPreflopRangeOther omahaPreflopRangeOther = OmahaPreflopRangeOther.None;
		InlineList<Card> danglers = cards.GetDanglers();
		if (danglers.Count == 0)
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.NoDanglers;
		}
		else if (danglers.Count == 1)
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.OneDangler;
		}
		else if (danglers.Count == 2)
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.TwoDanglers;
		}
		else if (danglers.Count >= 3)
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.ThreeDanglersOrMore;
		}
		_ = cards.Count;
		_ = danglers.Count;
		if (cards.ContainsQuads())
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.Quads;
		}
		if (cards.ContainsTrips())
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.Trips;
		}
		if ((omahaPreflopRangeOther & (OmahaPreflopRangeOther.Quads | OmahaPreflopRangeOther.Trips)) == OmahaPreflopRangeOther.None)
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.NoTripsOrQuads;
		}
		if (danglers.Count > 0)
		{
			omahaPreflopRangeOther |= OmahaPreflopRangeOther.TopDanglerRankAce.GetShiftedRank(danglers[0].Rank);
		}
		return omahaPreflopRangeOther;
	}

	public static InlineList<Card> GetDanglers(this InlineList<Card> cards)
	{
		InlineList<InlineList<Card>> items = cards.GetRepeatedCards();
		InlineList<InlineList<CardRanks>> items2 = cards.GetOmahaRundowns();
		InlineList<InlineList<Card>> items3 = cards.OrderBySuitness().GetSuitedCards();
		InlineList<Card> items4 = default(InlineList<Card>);
		InlineList<InlineList<Card>>.Enumerator enumerator = items.Where((InlineList<Card> pair) => pair.Count > 2).GetEnumerator();
		InlineList<Card>.Enumerator enumerator2;
		while (enumerator.MoveNext())
		{
			InlineList<Card> current = enumerator.Current;
			int count = current.Count - 2;
			InlineList<Card> items5 = default(InlineList<Card>);
			enumerator2 = current.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Card current2 = enumerator2.Current;
				if (!items2.Any((InlineList<CardRanks> rundown, Card card) => rundown.Contains(card.Rank), current2) && !items3.Any((InlineList<Card> suitedCards, Card card) => InlineListHelper.Take(in suitedCards, 2).Contains(card), current2))
				{
					items5.Add(current2);
				}
			}
			items4 = items4.AddRange(InlineListHelper.Take(in items5, count));
		}
		enumerator2 = cards.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			Card current3 = enumerator2.Current;
			if (!items4.Contains(current3) && !items.Any((InlineList<Card> pair, Card card) => pair.Contains(card), current3) && !items2.Any((InlineList<CardRanks> rundown, Card card) => rundown.Contains(card.Rank), current3) && !items3.Any((InlineList<Card> suitedCards, Card card) => InlineListHelper.Take(in suitedCards, 2).Contains(card), current3))
			{
				items4.Add(current3);
			}
		}
		return items4;
	}

	public static OmahaSixPreflopRangeCards GetShiftedRank(this OmahaSixPreflopRangeCards value, CardRanks rank)
	{
		return (OmahaSixPreflopRangeCards)((long)value << (int)(12 - rank));
	}

	public static OmahaPreflopRangeCards GetShiftedRank(this OmahaPreflopRangeCards value, CardRanks rank)
	{
		return (OmahaPreflopRangeCards)((long)value << (int)(12 - rank));
	}

	public static InlineList<InlineList<Card>> GetSuitedCards(this InlineList<Card> cards)
	{
		return from inlineList in cards.SplitBySuit()
			  where inlineList.Length > 1
			  select inlineList;
	}

	public static InlineList<InlineList<Card>> SplitBySuit(this InlineList<Card> cards)
	{
		InlineList<InlineList<Card>> result = default(InlineList<InlineList<Card>>);
		CardReader threadShared = CardReader.GetThreadShared(cards);
		while (threadShared.HasNext)
		{
			InlineList<Card> seat = threadShared.ReadNextSuitedCards();
			result.Add(seat);
		}
		return result;
	}

	public static InlineList<Card> OrderBySuitness(this InlineList<Card> cards)
	{
		InlineList<Card> other = default(InlineList<Card>);
		InlineList<Card> other2 = default(InlineList<Card>);
		InlineList<Card> other3 = default(InlineList<Card>);
		InlineList<Card> other4 = default(InlineList<Card>);
		InlineList<Card>.Enumerator enumerator = cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Card current = enumerator.Current;
			switch (current.Suit)
			{
				case Suits.Hearts:
					other.Add(current);
					break;
				case Suits.Diamonds:
					other2.Add(current);
					break;
				case Suits.Clubs:
					other3.Add(current);
					break;
				case Suits.Spades:
					other4.Add(current);
					break;
			}
		}
		return InlineListHelper.AddRange(default(InlineList<Card>), other).AddRange(other2).AddRange(other3)
			.AddRange(other4);
	}

	public static bool ContainsQuads(this InlineList<Card> cards)
	{
		CardReader threadShared = CardReader.GetThreadShared(cards);
		while (threadShared.HasNext)
		{
			if (threadShared.TryReadRepeatedRank(out var _, out var count) && count == 4)
			{
				return true;
			}
			if (threadShared.HasNext)
			{
				threadShared.SkipOne();
			}
		}
		return false;
	}

	public static bool ContainsTrips(this InlineList<Card> cards)
	{
		CardReader threadShared = CardReader.GetThreadShared(cards);
		while (threadShared.HasNext)
		{
			if (threadShared.TryReadRepeatedRank(out var _, out var count) && count == 3)
			{
				return true;
			}
			if (threadShared.HasNext)
			{
				threadShared.SkipOne();
			}
		}
		return false;
	}

	public static OmahaPreflopRangePairs GetShiftedRank(this OmahaPreflopRangePairs value, CardRanks rank)
	{
		return (OmahaPreflopRangePairs)((long)value).GetShiftedRank(rank);
	}

	public static OmahaPreflopRangePairs GetShiftedRankSinceKing(this OmahaPreflopRangePairs value, CardRanks rank)
	{
		return (OmahaPreflopRangePairs)((long)value << (int)(11 - rank));
	}

	public static OmahaPreflopRangePairs GetShiftedRankSinceQueen(this OmahaPreflopRangePairs value, CardRanks rank)
	{
		return (OmahaPreflopRangePairs)((long)value << (int)(10 - rank));
	}

	public static long GetShiftedRank(this long value, CardRanks rank)
	{
		return value << (int)(12 - rank);
	}

	public static InlineList<(CardRanks rank, int repeationCount)> GetRepeatedRanks(this InlineList<Card> cards)
	{
		CardReader threadShared = CardReader.GetThreadShared(cards);
		InlineList<(CardRanks, int)> result = default(InlineList<(CardRanks, int)>);
		while (threadShared.HasNext)
		{
			if (threadShared.TryReadRepeatedRank(out var rank, out var count))
			{
				result.Add((rank, count));
			}
			else
			{
				threadShared.SkipOne();
			}
		}
		return result;
	}

	public static InlineList<InlineList<Card>> GetRepeatedCards(this InlineList<Card> cards)
	{
		CardReader threadShared = CardReader.GetThreadShared(cards);
		InlineList<InlineList<Card>> result = default(InlineList<InlineList<Card>>);
		while (threadShared.HasNext)
		{
			if (threadShared.TryReadRepeatedCards(out var result2))
			{
				result.Add(result2);
			}
			else
			{
				threadShared.SkipOne();
			}
		}
		return result;
	}

	public static OmahaPreflopRangeSuiteness GetShiftedRank(this OmahaPreflopRangeSuiteness value, CardRanks rank)
	{
		return (OmahaPreflopRangeSuiteness)((long)value).GetShiftedRank(rank);
	}

	public static OmahaPreflopRangeStraightness GetShiftedRank(this OmahaPreflopRangeStraightness value, CardRanks rank)
	{
		return (OmahaPreflopRangeStraightness)((long)value).GetShiftedRank(rank);
	}

	public static OmahaPreflopRangeStraightness GetShiftedRankSinceKing(this OmahaPreflopRangeStraightness value, CardRanks rank)
	{
		return (OmahaPreflopRangeStraightness)((long)value << (int)(11 - rank));
	}

	public static bool IsRundown(this InlineList<CardRanks> ranks)
	{
		return ranks.GetRundownKind() != RundownKind.None;
	}

	public static OmahaPreflopRangeStraightness GetStraightness(this RundownKind kind)
	{
		return kind switch
		{
			RundownKind.NoGap => OmahaPreflopRangeStraightness.RundownWithNoGap,
			RundownKind.OneGap => OmahaPreflopRangeStraightness.RundownWithOneGap,
			RundownKind.TwoGaps => OmahaPreflopRangeStraightness.RundownWithTwoGaps,
			RundownKind.DoubleGap => OmahaPreflopRangeStraightness.RundownWithDoubleGap,
			_ => throw new InvalidOperationException($"Invalid {"RundownKind"}: {kind}"),
		};
	}

	public static RundownKind GetRundownKind(this InlineList<CardRanks> cards)
	{
		if (cards.Count < 2)
		{
			return RundownKind.None;
		}
		int num = 0;
		int num2 = 0;
		InlineList<(int?, CardRanks)>.Enumerator enumerator = cards.WithGaps().GetEnumerator();
		while (enumerator.MoveNext())
		{
			int? item = enumerator.Current.Item1;
			if (!item.HasValue)
			{
				continue;
			}
			if (item > 2)
			{
				return RundownKind.None;
			}
			if (item == 2)
			{
				if (num > 0 || num2 > 0)
				{
					return RundownKind.None;
				}
				num2++;
			}
			if (item == 1)
			{
				num++;
			}
		}
		if (num == 0 && num2 == 0)
		{
			return RundownKind.NoGap;
		}
		if (cards.Count == 2)
		{
			return RundownKind.None;
		}
		if (num == 1 && num2 == 0)
		{
			return RundownKind.OneGap;
		}
		if (num == 2 && num2 == 0)
		{
			return RundownKind.TwoGaps;
		}
		if (num2 == 1 && num == 0)
		{
			return RundownKind.DoubleGap;
		}
		return RundownKind.None;
	}

	public static InlineList<InlineList<CardRanks>> GetOmahaRundowns(this InlineList<Card> cards)
	{
		InlineList<InlineList<CardRanks>> result = default(InlineList<InlineList<CardRanks>>);
		CardReader threadShared = CardReader.GetThreadShared(cards);
		while (threadShared.HasNext)
		{
			if (threadShared.TryReadOmahaRundown(out var result2))
			{
				result.Add(result2);
			}
			else
			{
				threadShared.SkipOne();
			}
		}
		return result;
	}

	public static OmahaPreflopRangeOther GetShiftedRank(this OmahaPreflopRangeOther value, CardRanks rank)
	{
		return (OmahaPreflopRangeOther)((long)value << (int)(12 - rank));
	}

	public static OmahaPreflopRangePairs Toggle(this OmahaPreflopRangePairs flags, OmahaPreflopRangePairs flag)
	{
		if (!flags.HasFlag(flag))
		{
			return flags | flag;
		}
		return flags & ~flag;
	}

	public static IEnumerable<CardRanks> GetTopPairRanks(this OmahaPreflopRangePairs value)
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			if (value.HasFlag(OmahaPreflopRangePairs.TopPairRankAce.GetShiftedRank(item)))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<CardRanks> GetSecondPairRanks(this OmahaPreflopRangePairs value)
	{
		foreach (CardRanks item in Cards.RanksDecending.Where((CardRanks rank) => rank <= CardRanks.King))
		{
			if (value.HasFlag(OmahaPreflopRangePairs.SecondPairRankKing.GetShiftedRankSinceKing(item)))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<CardRanks> GetThirdPairRanks(this OmahaPreflopRangePairs value)
	{
		foreach (CardRanks item in Cards.RanksDecending.Where((CardRanks rank) => rank <= CardRanks.Queen))
		{
			if (value.HasFlag(OmahaPreflopRangePairs.ThirdPairRankQueen.GetShiftedRankSinceQueen(item)))
			{
				yield return item;
			}
		}
	}

	public static OmahaPreflopRangePairs WithTopPairRanks(this OmahaPreflopRangePairs value, IList<CardRanks> ranks)
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			value = ((!ranks.Contains(item)) ? (value & ~OmahaPreflopRangePairs.TopPairRankAce.GetShiftedRank(item)) : (value | OmahaPreflopRangePairs.TopPairRankAce.GetShiftedRank(item)));
		}
		return value;
	}

	public static OmahaPreflopRangePairs WithSecondPairRanks(this OmahaPreflopRangePairs value, IList<CardRanks> ranks)
	{
		foreach (CardRanks item in Cards.RanksDecending.Where((CardRanks rank) => rank <= CardRanks.King))
		{
			value = ((!ranks.Contains(item)) ? (value & ~OmahaPreflopRangePairs.SecondPairRankKing.GetShiftedRankSinceKing(item)) : (value | OmahaPreflopRangePairs.SecondPairRankKing.GetShiftedRankSinceKing(item)));
		}
		return value;
	}

	public static OmahaPreflopRangePairs WithThirdPairRanks(this OmahaPreflopRangePairs value, IList<CardRanks> ranks)
	{
		foreach (CardRanks item in Cards.RanksDecending.Where((CardRanks rank) => rank <= CardRanks.Queen))
		{
			value = ((!ranks.Contains(item)) ? (value & ~OmahaPreflopRangePairs.ThirdPairRankQueen.GetShiftedRankSinceQueen(item)) : (value | OmahaPreflopRangePairs.ThirdPairRankQueen.GetShiftedRankSinceQueen(item)));
		}
		return value;
	}

	public static TEnum WithCardRanks<TEnum>(this TEnum cards, TEnum from, IEnumerable<CardRanks> ranks) where TEnum : Enum
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			cards = ((!ranks.Contains(item)) ? ((TEnum)(object)((long)(object)cards & ~(long)(object)from.GetShiftedRank(item))) : ((TEnum)(object)((long)(object)cards | (long)(object)from.GetShiftedRank(item))));
		}
		return cards;
	}

	public static TEnum WithCardRanksSinceKing<TEnum>(this TEnum cards, TEnum from, IEnumerable<CardRanks> ranks) where TEnum : Enum
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			if (item != CardRanks.Ace)
			{
				cards = ((!ranks.Contains(item)) ? ((TEnum)(object)((long)(object)cards & ~(long)(object)from.GetShiftedRankSinceKing(item))) : ((TEnum)(object)((long)(object)cards | (long)(object)from.GetShiftedRankSinceKing(item))));
			}
		}
		return cards;
	}

	public static IEnumerable<CardRanks> GetCardRanks<TEnum>(this TEnum cards, TEnum from) where TEnum : Enum
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			if (cards.HasFlag(from.GetShiftedRank(item)))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<CardRanks> GetCardRanks<TEnum>(this TEnum cards, TEnum from, TEnum to) where TEnum : Enum
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			TEnum flag = from.GetShiftedRank(item);
			if (cards.HasFlag(flag))
			{
				yield return item;
			}
			if (object.Equals(to, flag))
			{
				break;
			}
		}
	}

	public static IEnumerable<CardRanks> GetCardRanksSinceKing<TEnum>(this TEnum cards, TEnum from) where TEnum : Enum
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			if (cards.HasFlag(from.GetShiftedRankSinceKing(item)))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<CardRanks> GetCardRanksSinceKing<TEnum>(this TEnum cards, TEnum from, TEnum to) where TEnum : Enum
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			if (item != CardRanks.Ace)
			{
				TEnum flag = from.GetShiftedRankSinceKing(item);
				if (cards.HasFlag(flag))
				{
					yield return item;
				}
				if (object.Equals(to, flag))
				{
					break;
				}
			}
		}
	}

	public static TEnum GetShiftedRank<TEnum>(this TEnum value, CardRanks rank) where TEnum : Enum
	{
		return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) << (int)(12 - rank));
	}

	public static TEnum GetShiftedRankSinceKing<TEnum>(this TEnum value, CardRanks rank) where TEnum : Enum
	{
		if (rank == CardRanks.Ace)
		{
			return default(TEnum);
		}
		return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) << (int)(11 - rank));
	}

	public static string GetName(this OmahaPreflopRangeSuiteness value)
	{
		return value.GetEnumName();
	}

	public static string GetName(this OmahaPreflopRangeStraightness value)
	{
		return value.GetEnumName();
	}

	public static bool HasRankFlag<TEnum>(this TEnum value, TEnum from) where TEnum : Enum
	{
		foreach (CardRanks item in Cards.RanksDecending)
		{
			if (value.HasFlag(from.GetShiftedRank(item)))
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<string> GetFlagNamesOrSentenceCase(this OmahaPreflopRange range)
	{
		return range.Pairs.GetFlagsNamesOrSentenceCase().Concat(range.Cards.GetFlagsNamesOrSentenceCase()).Concat(range.Suiteness.GetFlagsNamesOrSentenceCase())
			.Concat(range.Straightness.GetFlagsNamesOrSentenceCase())
			.Concat(range.SixCards.GetFlagsNamesOrSentenceCase())
			.Concat(range.Other.GetFlagsNamesOrSentenceCase());
	}
}
