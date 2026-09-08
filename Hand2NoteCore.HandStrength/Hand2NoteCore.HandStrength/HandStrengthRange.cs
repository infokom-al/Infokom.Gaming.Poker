using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

public static class HandStrengthRange
{
	public static IEnumerable<(IHandStrengthExpression expression, double frequency, PocketRangeHoldem range)> ComputeHandStrengthRange(this PocketRangeHoldem pocketsRange, BoardRange boardRange, PokerGames game, IList<IHandStrengthExpression> expressions, int trials)
	{
		trials.VerifyArgumentPositive("trials");
		Dictionary<IHandStrengthExpression, (int hitTrials, HashSet<PocketCardsHoldem> hitPocketCards)> result = new Dictionary<IHandStrengthExpression, (int, HashSet<PocketCardsHoldem>)>();
		Dictionary<HandStrengthValueExpression, IHandStrengthExpression> dictionary = new Dictionary<HandStrengthValueExpression, IHandStrengthExpression>();
		Board[] array = boardRange.GetBoardsSample(trials).ToArray();
		FastRandom random = new FastRandom();
		int num = array.Length;
		_ = pocketsRange.Cards.Length;
		for (int i = 0; i < trials; i++)
		{
			Board board = array[i % num];
			PocketCardsHoldem randomItem = pocketsRange.Cards.GetRandomItem(random);
			if (randomItem.IsDead(board.Cards))
			{
				continue;
			}
			HandStrengthValueExpression handStrengthExpressionValue = randomItem.GetHandStrengthExpressionValue(board, game);
			if (!dictionary.TryGetValue(handStrengthExpressionValue, out var value))
			{
				if (!expressions.TryGet(handStrengthExpressionValue, out value))
				{
					throw new InvalidOperationException($"No expression found for hand {randomItem} on board {board} and relevantFlopHandValue - {handStrengthExpressionValue.FlopHandValue}, relevantTurnHandValue - {handStrengthExpressionValue.TurnHandValue}, relevantRiverHandValue - {handStrengthExpressionValue.RiverHandValue}. Given expressions should be exhaustive.");
				}
				dictionary[handStrengthExpressionValue] = value;
			}
			if (!result.TryGetValue(value, out (int, HashSet<PocketCardsHoldem>) value2))
			{
				value2 = (0, new HashSet<PocketCardsHoldem>());
			}
			value2.Item2.Add(randomItem);
			result[value] = (value2.Item1 + 1, value2.Item2);
		}
		int totalTrials = result.Sum<KeyValuePair<IHandStrengthExpression, (int, HashSet<PocketCardsHoldem>)>>((KeyValuePair<IHandStrengthExpression, (int hitTrials, HashSet<PocketCardsHoldem> hitPocketCards)> x) => x.Value.hitTrials);
		if (totalTrials == 0)
		{
			throw new ImpossibleSimulationException();
		}
		foreach (IHandStrengthExpression expression in expressions)
		{
			if (result.TryGetValue(expression, out (int, HashSet<PocketCardsHoldem>) value3))
			{
				yield return (expression: expression, frequency: (double)value3.Item1 / (double)totalTrials, range: value3.Item2.ToPocketsRangeHoldem());
			}
		}
	}

	public static IEnumerable<(IHandStrengthExpression expression, PocketRangeHoldem range)> ComputeHandStrengthRange(this PocketRangeHoldem pocketsRange, Board board, PokerGames game, IList<IHandStrengthExpression> expressions)
	{
		if (pocketsRange.IsDead(board.Cards))
		{
			throw new ArgumentException("Given range is dead on the given board");
		}
		Dictionary<IHandStrengthExpression, List<PocketCardsHoldem>> result = new Dictionary<IHandStrengthExpression, List<PocketCardsHoldem>>();
		Dictionary<HandStrengthValueExpression, IHandStrengthExpression> dictionary = new Dictionary<HandStrengthValueExpression, IHandStrengthExpression>();
		ImmutableArray<PocketCardsHoldem>.Enumerator enumerator = pocketsRange.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PocketCardsHoldem current = enumerator.Current;
			if (current.IsDead(board.Cards))
			{
				continue;
			}
			HandStrengthValueExpression handStrengthExpressionValue = current.GetHandStrengthExpressionValue(board, game);
			if (!dictionary.TryGetValue(handStrengthExpressionValue, out var value))
			{
				if (!expressions.TryGet(handStrengthExpressionValue, out value))
				{
					throw new InvalidOperationException($"No expression found for hand {current} and relevantFlopHandValue - {handStrengthExpressionValue.FlopHandValue}, relevantTurnHandValue - {handStrengthExpressionValue.TurnHandValue}, relevantRiverHandValue - {handStrengthExpressionValue.RiverHandValue}. Given expressions should be exhaustive.");
				}
				dictionary[handStrengthExpressionValue] = value;
			}
			result.AddOrAddToList(value, current);
		}
		foreach (IHandStrengthExpression expression in expressions)
		{
			if (result.TryGetValue(expression, out List<PocketCardsHoldem> value2))
			{
				yield return (expression: expression, range: value2.ToPocketsRangeHoldem());
			}
		}
	}

	public static PocketRangeHoldem FilterByHandStrength(this IPreflopRange preflopRange, BoardRange boardRange, PokerGames game, IHandStrengthExpression expression, InlineList<Card> deadCards)
	{
		if (preflopRange is PocketRangeHoldem preflopRange2)
		{
			return preflopRange2.FilterByHandStrength(boardRange, game, expression, deadCards);
		}
		throw new NotImplementedException(preflopRange.GetType().Name);
	}

	public static PocketRangeHoldem FilterByHandStrength(this IPreflopRange preflopRange, Board board, PokerGames game, IHandStrengthExpression expression)
	{
		if (preflopRange is PocketRangeHoldem preflopRange2)
		{
			return preflopRange2.FilterByHandStrength(board, game, expression);
		}
		throw new NotImplementedException(preflopRange.GetType().Name);
	}

	public static PocketRangeHoldem FilterByHandStrength(this PocketRangeHoldem pocketsRange, Board board, PokerGames game, IHandStrengthExpression expression, int trials)
	{
		return pocketsRange.FilterByHandStrength(board.ToSingleBoardRange(), game, expression, trials);
	}

	public static PocketRangeHoldem FilterByHandStrength(this PocketRangeHoldem pocketsRange, BoardRange boardRange, PokerGames game, IHandStrengthExpression expression, int trials)
	{
		Dictionary<PreflopRangeHoldemCell, int> dictionary = new Dictionary<PreflopRangeHoldemCell, int>();
		foreach (Board item in boardRange.GetBoardsSample(trials))
		{
			ImmutableArray<PocketCardsHoldem>.Enumerator enumerator2 = pocketsRange.Cards.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				PocketCardsHoldem current2 = enumerator2.Current;
				if (current2.Hits(expression, item, game))
				{
					PreflopRangeHoldemCell preflopRangeHoldemCell = current2.GetPreflopRangeHoldemCell();
					dictionary.IncrementOrAdd(preflopRangeHoldemCell);
				}
			}
		}
		throw new NotImplementedException();
	}

	public static bool Hits(this PocketCardsHoldem pocketCards, IHandStrengthExpression expression, Board board, PokerGames game)
	{
		return expression.Evaluate(pocketCards.ComputeRelevantHandValue(board, game).ToHandStrengthExpressionValue(board.Street, pocketCards.ToPreflopRangeHoldemCompact()));
	}

	private static bool TryGet(this IEnumerable<IHandStrengthExpression> expressions, HandStrengthValueExpression expressionValue, out IHandStrengthExpression result)
	{
		return expressions.TryGet<IHandStrengthExpression>((IHandStrengthExpression handStrengthExpression) => handStrengthExpression.Evaluate(expressionValue), out result);
	}

	private static HandStrengthValueExpression GetHandStrengthExpressionValue(this PocketCardsHoldem pocketCards, Board board, PokerGames game)
	{
		if (board.Street == Streets.Flop)
		{
			return pocketCards.ComputeRelevantHandValue(board, game).ToHandStrengthExpressionValue(Streets.Flop, pocketCards.ToPreflopRangeHoldemCompact());
		}
		if (board.Street == Streets.Turn)
		{
			RelevantHandValue flopHandValue = pocketCards.ComputeRelevantHandValue(board.GetBoardOnStreet(Streets.Flop), game);
			RelevantHandValue turnHandValue = pocketCards.ComputeRelevantHandValue(board, game);
			return HandStrengthValueExpression.Create(flopHandValue, turnHandValue, (RelevantHandValue)0L, (RelevantFlopType)0L, (RelevantTurnType)0uL, (RelevantRiverType)0uL, pocketCards.ToPreflopRangeHoldemCompact(), OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
		}
		if (board.Street == Streets.River)
		{
			RelevantHandValue flopHandValue2 = pocketCards.ComputeRelevantHandValue(board.GetBoardOnStreet(Streets.Flop), game);
			RelevantHandValue turnHandValue2 = pocketCards.ComputeRelevantHandValue(board.GetBoardOnStreet(Streets.Turn), game);
			RelevantHandValue riverHandValue = pocketCards.ComputeRelevantHandValue(board, game);
			return HandStrengthValueExpression.Create(flopHandValue2, turnHandValue2, riverHandValue, (RelevantFlopType)0L, (RelevantTurnType)0uL, (RelevantRiverType)0uL, pocketCards.ToPreflopRangeHoldemCompact(), OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
		}
		throw new ArgumentException($"Unexpected value of {"Street"} is equal to - {board.Street}");
	}
}
