using Poker.Calc.Common;

using System.Collections.Immutable;

namespace Poker.Calc;

public static class PreflopRangeHelper
{
	public static PocketCardsHoldem GetRandomPocketCardsHoldem(this IPreflopRange preflopRange, FastRandom random)
	{
		if (!(preflopRange is PocketRangeHoldem pocketRangeHoldem))
		{
			if (!(preflopRange is PocketsRangeWeightedHoldem rangeWeightedHoldem))
			{
				if (preflopRange is PreflopRangeHoldem range)
				{
					return range.ToPocketRangeHoldem().Cards.GetRandomItem(random);
				}
				throw new ArgumentException($"{preflopRange} is not supported");
			}
			return rangeWeightedHoldem.GetRandomPocketCardsHoldem(random);
		}
		return pocketRangeHoldem.Cards.GetRandomItem(random);
	}

	public static PocketCardsHoldem GetRandomPocketCardsHoldem(this PocketsRangeWeightedHoldem rangeWeightedHoldem, FastRandom random)
	{
		int index;
		do
		{
			index = random.Next(rangeWeightedHoldem.Range.Length);
		}
		while (!rangeWeightedHoldem.Range[index].weightUnitInterval.GetRandomBoolean(random));
		return rangeWeightedHoldem.Range[index].pocket;
	}

	public static PocketsRangeWeightedHoldem ToPocketsRangeWeightedHoldem(this IPreflopRange preflopRange)
	{
		if (!(preflopRange is PocketsRangeWeightedHoldem result))
		{
			if (!(preflopRange is PocketRangeHoldem pocketsRangeHoldem))
			{
				if (preflopRange is PreflopRangeHoldem range)
				{
					return range.ToPocketRangeHoldem().ToPocketsRangeWeightedHoldem();
				}
				throw new ArgumentException($"{preflopRange} is not supported.");
			}
			return pocketsRangeHoldem.ToPocketsRangeWeightedHoldem();
		}
		return result;
	}

	public static bool IsValidShortDeckRange(this IPreflopRange preflopRange)
	{
		if (!(preflopRange is PocketRangeHoldem range))
		{
			if (preflopRange is PreflopRangeHoldem range2)
			{
				return range2.ToPocketRangeHoldem().IsShortDeck();
			}
			throw new NotImplementedException();
		}
		return range.IsShortDeck();
	}

	public static double GetComboCount(this IPreflopRange preflopRange)
	{
		if (!(preflopRange is PocketRangeHoldem pocketRangeHoldem))
		{
			if (!(preflopRange is PreflopRangeHoldem range))
			{
				if (!(preflopRange is PocketsRangeWeightedHoldem { Combos: var combos }))
				{
					throw new NotImplementedException();
				}
				return combos;
			}
			return range.ToPocketRangeHoldem().Combos;
		}
		return pocketRangeHoldem.Combos;
	}

	public static bool IsEmpty(this IPreflopRange range)
	{
		if (!(range is PocketRangeHoldem { IsEmpty: var isEmpty }))
		{
			if (!(range is PreflopRangeHoldem { IsEmpty: var isEmpty2 }))
			{
				if (!(range is PocketsRangeWeightedHoldem { IsEmpty: var isEmpty3 }))
				{
					throw new NotImplementedException(range.GetType().Name);
				}
				return isEmpty3;
			}
			return isEmpty2;
		}
		return isEmpty;
	}

	public static string GetDefaultLiteral(this IPreflopRange range)
	{
		if (!(range is PocketRangeHoldem pocketsRange))
		{
			if (!(range is PreflopRangeHoldem preflopRange))
			{
				if (range is PocketsRangeWeightedHoldem rangeWeightedHoldem)
				{
					return rangeWeightedHoldem.GetDefaultLiteral();
				}
				throw new NotImplementedException(range.GetType().Name);
			}
			return preflopRange.GetDefaultLiteral();
		}
		return pocketsRange.GetDefaultLiteral();
	}

	public static bool IsDead(this IPreflopRange range, Span<Card> deadCards)
	{
		if (!(range is PocketRangeHoldem range2))
		{
			if (range is PreflopRangeHoldem range3)
			{
				return range3.ToPocketRangeHoldem().IsDead(deadCards);
			}
			throw new NotImplementedException(range.GetType().Name);
		}
		return range2.IsDead(deadCards);
	}

	public static bool MatchesGame(this IPreflopRange range, PokerGames game)
	{
		return range.GetAttribute<GamesAttribute>().Value.HasFlag(game);
	}

	public static bool TryGetGame(this IPreflopRange range, out PokerGames game)
	{
		game = range.GetAttribute<GamesAttribute>().Value;
		if (game.IsSingleFlag())
		{
			return true;
		}
		if (range is PreflopRangeHoldem range2 && range2.ContainsTexasHoldemCells())
		{
			game = PokerGames.TexasHoldem;
			return true;
		}
		return false;
	}

	public static void VerifyNotDead(this IEnumerable<IPreflopRange> ranges, InlineList<Card> deadCards)
	{
		ranges.ForEach(delegate (IPreflopRange x)
		{
			x.VerifyNotDead(deadCards);
		});
	}

	public static T VerifyNotDead<T>(this T range, InlineList<Card> deadCards) where T : IPreflopRange
	{
		if (range.GetPossibleCards().All((Card x) => x.IsDead(deadCards)))
		{
			throw new InvalidOperationException("Preflop range is dead.");
		}
		return range;
	}

	public static T VerifyNotEmpty<T>(this T range) where T : IPreflopRange
	{
		if (range is PreflopRangeHoldem range2)
		{
			range2.VerifyNotEmpty();
		}
		else if (range is PocketRangeHoldem range3)
		{
			range3.VerifyNotEmpty();
		}
		else
		{
			if (!(range is PocketsRangeWeightedHoldem weightedRange))
			{
				throw new NotImplementedException(range.GetType().Name);
			}
			weightedRange.ToPocketsRangeHoldem().VerifyNotEmpty();
		}
		return range;
	}

	public static IEnumerable<Card> GetPossibleCards(this IPreflopRange range)
	{
		if (!(range is PocketRangeHoldem pocketRangeHoldem))
		{
			if (range is PocketsRangeWeightedHoldem weightedRange)
			{
				return weightedRange.ToPocketsRangeHoldem().Cards.SelectMany((PocketCardsHoldem x) => x.Cards.AsEnumerable());
			}
			throw new NotImplementedException();
		}
		return pocketRangeHoldem.Cards.SelectMany((PocketCardsHoldem x) => x.Cards.AsEnumerable());
	}

	public static bool TryAdd(this IPreflopRange range, IPreflopRange other, out IPreflopRange result)
	{
		result = null;
		if (range.TryGetGame(out var game) && other.TryGetGame(out var game2) && game != game2)
		{
			return false;
		}
		if (!range.IsHoldemRange())
		{
			throw new NotImplementedException("Combining omaha ranges is not implemented");
		}
		result = range.Concat(other);
		return true;
	}

	public static IPreflopRange Concat(this IPreflopRange range, IPreflopRange other)
	{
		return new IPreflopRange[2] { range, other }.Concat();
	}

	public static IPreflopRange Except(this IPreflopRange range, IPreflopRange other)
	{
		if (range is PocketsRangeWeightedHoldem || other is PocketsRangeWeightedHoldem)
		{
			return range.ToPocketsRangeWeightedHoldem().Except(other.ToPocketsRangeWeightedHoldem());
		}
		return range.ToPocketsRangeHoldem().Except(other.ToPocketsRangeHoldem());
	}

	public static IPreflopRange Concat(this IList<IPreflopRange> ranges)
	{
		if (!ranges.Any())
		{
			return PocketRangeHoldem.Empty;
		}
		List<IPreflopRange> source = ranges.Where((IPreflopRange range) => !range.IsEmpty()).ToList();
		if (!source.Any())
		{
			return PocketRangeHoldem.Empty;
		}
		if (!source.Any((IPreflopRange range) => range is PocketsRangeWeightedHoldem))
		{
			return source.Aggregate((IPreflopRange x, IPreflopRange y) => x.ToPocketsRangeHoldem().Concat(y.ToPocketsRangeHoldem()));
		}
		return source.Select((IPreflopRange range) => range.ToPocketsRangeWeightedHoldem()).Aggregate((PocketsRangeWeightedHoldem x, PocketsRangeWeightedHoldem y) => x.Concat(y));
	}

	public static bool TryRemove(this IPreflopRange range, IPreflopRange other, out IPreflopRange result)
	{
		result = null;
		if (range.TryGetGame(out var game) && other.TryGetGame(out var game2) && game != game2)
		{
			return false;
		}
		if (range.IsHoldemRange())
		{
			result = range.GetPocketsRangeHoldem().Except(other.GetPocketsRangeHoldem());
			return true;
		}
		if (range.GetType() != other.GetType())
		{
			return false;
		}
		throw new NotImplementedException("Combining omaha ranges is not implemented");
	}

	public static bool IsHoldemRange(this IPreflopRange preflopRange)
	{
		PokerGames value = preflopRange.GetAttribute<GamesAttribute>().Value;
		if (!value.HasFlag(PokerGames.TexasHoldem) && !value.HasFlag(PokerGames.ShortDeck))
		{
			return value.HasFlag(PokerGames.ShortDeckTbs);
		}
		return true;
	}

	public static PocketRangeHoldem ToPocketsRangeHoldem(this IPreflopRange preflopRange)
	{
		if (!preflopRange.IsHoldemRange())
		{
			throw new ArgumentException("Expecting a holdem range but was " + preflopRange.GetType().Name);
		}
		if (preflopRange is PocketRangeHoldem result)
		{
			return result;
		}
		if (preflopRange is PocketsRangeWeightedHoldem weightedRange)
		{
			return weightedRange.ToPocketsRangeHoldem();
		}
		return preflopRange.VerifyType<PreflopRangeHoldem>().ToPocketRangeHoldem();
	}

	public static bool RangeEquals(this IPreflopRange range, IPreflopRange other)
	{
		if (!range.IsHoldemRange() || !other.IsHoldemRange())
		{
			throw new NotImplementedException("Omaha ranges comparison is not implemented yet");
		}
		if (!(range is PocketsRangeWeightedHoldem range2))
		{
			return range.ToPocketsRangeHoldem().RangeEquals(other.ToPocketsRangeHoldem());
		}
		return range2.RangeEquals(other.VerifyType<PocketsRangeWeightedHoldem>());
	}

	public static bool RangeEquals(this PocketsRangeWeightedHoldem range, PocketsRangeWeightedHoldem other)
	{
		return range.Range.AreEqual(other.Range);
	}

	public static bool RangeEquals(this PocketRangeHoldem range, PocketRangeHoldem other)
	{
		if (range.Cards.Length != other.Cards.Length)
		{
			return false;
		}
		HashSet<PocketCardsHoldem> hashSet = other.Cards.ToHashSet();
		ImmutableArray<PocketCardsHoldem>.Enumerator enumerator = range.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PocketCardsHoldem current = enumerator.Current;
			if (!hashSet.Contains(current))
			{
				return false;
			}
		}
		return true;
	}

	public static PocketRangeHoldem GetPocketsRangeHoldem(this IPreflopRange range)
	{
		if (!(range is PocketRangeHoldem result))
		{
			if (!(range is PocketsRangeWeightedHoldem weightedRange))
			{
				if (range is PreflopRangeHoldem range2)
				{
					return range2.ToPocketRangeHoldem();
				}
				throw new NotImplementedException();
			}
			return weightedRange.ToPocketsRangeHoldem();
		}
		return result;
	}
}
