using System;
using System.Collections.Generic;
using System.Linq;
using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

public static class HandStrengthExpressionHelper
{
	public static HandStrengthExpressionOptimized GetOptimizedExpression(this IHandStrengthExpression handStrengthExpression)
	{
		handStrengthExpression = handStrengthExpression.SimplifyExpression();
		HandStrengthExpressionType expressionType = handStrengthExpression.GetExpressionType();
		bool hasFlopBoardFilter = handStrengthExpression.GetLastBoardStreet() >= Streets.Flop;
		bool hasTurnBoardFilter = handStrengthExpression.GetLastBoardStreet() >= Streets.Turn;
		bool hasRiverBoardFilter = handStrengthExpression.GetLastBoardStreet() >= Streets.River;
		HandStrengthExpressionOptimized handStrengthExpressionOptimized;
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthNotExpression handStrengthNotExpression))
			{
				if (!(handStrengthExpression is HandStrengthValueExpression))
				{
					throw new ArgumentOutOfRangeException("handStrengthExpression");
				}
				handStrengthExpressionOptimized = null;
			}
			else
			{
				handStrengthExpressionOptimized = handStrengthNotExpression.Operand.GetOptimizedExpression();
			}
		}
		else
		{
			handStrengthExpressionOptimized = handStrengthBinaryExpression.LeftOperand.GetOptimizedExpression();
		}
		HandStrengthExpressionOptimized leftOperand = handStrengthExpressionOptimized;
		HandStrengthExpressionOptimizedOperator? handStrengthExpressionOptimizedOperator;
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression2))
		{
			if (!(handStrengthExpression is HandStrengthNotExpression))
			{
				if (!(handStrengthExpression is HandStrengthValueExpression))
				{
					throw new ArgumentOutOfRangeException("handStrengthExpression");
				}
				handStrengthExpressionOptimizedOperator = null;
			}
			else
			{
				handStrengthExpressionOptimizedOperator = HandStrengthExpressionOptimizedOperator.Not;
			}
		}
		else
		{
			handStrengthExpressionOptimizedOperator = handStrengthBinaryExpression2.Operator.ToOptimizedOperator();
		}
		HandStrengthExpressionOptimizedOperator? handStrengthExpressionOptimizedOperator2 = handStrengthExpressionOptimizedOperator;
		HandStrengthExpressionOptimized handStrengthExpressionOptimized2;
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression3))
		{
			if (!(handStrengthExpression is HandStrengthNotExpression))
			{
				if (!(handStrengthExpression is HandStrengthValueExpression))
				{
					throw new ArgumentOutOfRangeException("handStrengthExpression");
				}
				handStrengthExpressionOptimized2 = null;
			}
			else
			{
				handStrengthExpressionOptimized2 = null;
			}
		}
		else
		{
			handStrengthExpressionOptimized2 = handStrengthBinaryExpression3.RightOperand.GetOptimizedExpression();
		}
		HandStrengthExpressionOptimized rightOperand = handStrengthExpressionOptimized2;
		HandStrengthValueExpression expressionValue;
		if (!(handStrengthExpression is HandStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthNotExpression))
			{
				if (!(handStrengthExpression is HandStrengthValueExpression handStrengthValueExpression))
				{
					throw new ArgumentOutOfRangeException("handStrengthExpression");
				}
				expressionValue = handStrengthValueExpression;
			}
			else
			{
				expressionValue = null;
			}
		}
		else
		{
			expressionValue = null;
		}
		return new HandStrengthExpressionOptimized(expressionType, hasFlopBoardFilter, hasTurnBoardFilter, hasRiverBoardFilter, leftOperand, handStrengthExpressionOptimizedOperator2, rightOperand, expressionValue);
	}

	public static IHandStrengthExpression SimplifyExpression(this IHandStrengthExpression handStrengthExpression)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression expression))
		{
			if (!(handStrengthExpression is HandStrengthNotExpression expression2))
			{
				if (handStrengthExpression is HandStrengthValueExpression result)
				{
					return result;
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return expression2.SimplifyNotExpression();
		}
		return expression.SimplifyBinaryExpression();
	}

	public static IHandStrengthExpression SimplifyNotExpression(this HandStrengthNotExpression expression)
	{
		IHandStrengthExpression handStrengthExpression = expression.Operand.SimplifyExpression();
		if (handStrengthExpression.IsDefaultHandValueExpression())
		{
			return HandStrengthValueExpression.Default;
		}
		return HandStrengthNotExpression.Create(handStrengthExpression);
	}

	public static IHandStrengthExpression SimplifyBinaryExpression(this HandStrengthBinaryExpression expression)
	{
		IHandStrengthExpression handStrengthExpression = expression.LeftOperand.SimplifyExpression();
		IHandStrengthExpression handStrengthExpression2 = expression.RightOperand.SimplifyExpression();
		if (handStrengthExpression.IsDefaultHandValueExpression())
		{
			return expression.Operator switch
			{
				HandStrengthExpressionBinaryOperator.Or => HandStrengthValueExpression.Default, 
				HandStrengthExpressionBinaryOperator.And => handStrengthExpression2, 
				HandStrengthExpressionBinaryOperator.AndNot => HandStrengthNotExpression.Create(handStrengthExpression2), 
				HandStrengthExpressionBinaryOperator.OrNot => HandStrengthValueExpression.Default, 
				_ => throw new NotImplementedException(expression.Operator.ToString()), 
			};
		}
		if (handStrengthExpression2.IsDefaultHandValueExpression())
		{
			return expression.Operator switch
			{
				HandStrengthExpressionBinaryOperator.Or => HandStrengthValueExpression.Default, 
				HandStrengthExpressionBinaryOperator.And => handStrengthExpression, 
				HandStrengthExpressionBinaryOperator.AndNot => handStrengthExpression, 
				HandStrengthExpressionBinaryOperator.OrNot => handStrengthExpression, 
				_ => throw new NotImplementedException(expression.Operator.ToString()), 
			};
		}
		return new HandStrengthBinaryExpression(expression.Operator, handStrengthExpression, handStrengthExpression2);
	}

	public static bool IsDefaultHandValueExpression(this IHandStrengthExpression expression)
	{
		if (expression is HandStrengthValueExpression handStrengthValueExpression)
		{
			return handStrengthValueExpression.IsDefault;
		}
		return false;
	}

	public static HandStrengthExpressionOptimizedOperator ToOptimizedOperator(this HandStrengthExpressionBinaryOperator @operator)
	{
		return @operator switch
		{
			HandStrengthExpressionBinaryOperator.Or => HandStrengthExpressionOptimizedOperator.Or, 
			HandStrengthExpressionBinaryOperator.And => HandStrengthExpressionOptimizedOperator.And, 
			HandStrengthExpressionBinaryOperator.AndNot => HandStrengthExpressionOptimizedOperator.AndNot, 
			HandStrengthExpressionBinaryOperator.OrNot => HandStrengthExpressionOptimizedOperator.OrNot, 
			_ => throw new ArgumentOutOfRangeException("operator", @operator, null), 
		};
	}

	public static HandStrengthExpressionType GetExpressionType(this IHandStrengthExpression expression)
	{
		if (!(expression is HandStrengthBinaryExpression))
		{
			if (!(expression is HandStrengthNotExpression))
			{
				if (expression is HandStrengthValueExpression)
				{
					return HandStrengthExpressionType.Value;
				}
				throw new ArgumentOutOfRangeException("expression");
			}
			return HandStrengthExpressionType.Not;
		}
		return HandStrengthExpressionType.Binary;
	}

	public static bool Evaluate(this IHandStrengthExpression handStrengthExpression, RelevantHandBoards boards)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression expression))
		{
			if (!(handStrengthExpression is HandStrengthValueExpression handStrengthValueExpression))
			{
				if (handStrengthExpression is HandStrengthNotExpression expression2)
				{
					return expression2.EvaluateNotBoards(boards);
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return handStrengthValueExpression.Evaluate(boards);
		}
		return expression.EvaluateBinaryBoards(boards);
	}

	public static bool Evaluate(this IHandStrengthExpression handStrengthExpression, HandStrengthValueExpression hand)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression expression))
		{
			if (!(handStrengthExpression is HandStrengthValueExpression handStrengthValueExpression))
			{
				if (handStrengthExpression is HandStrengthNotExpression expression2)
				{
					return expression2.EvaluateNotHandValue(hand);
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return handStrengthValueExpression.Evaluate(hand);
		}
		return expression.EvaluateBinaryHandValue(hand);
	}

	public static bool EvaluateNotBoards(this HandStrengthNotExpression expression, RelevantHandBoards boards)
	{
		return !expression.Operand.Evaluate(boards);
	}

	public static bool EvaluateBinaryBoards(this HandStrengthBinaryExpression expression, RelevantHandBoards boards)
	{
		return expression.Operator switch
		{
			HandStrengthExpressionBinaryOperator.Or => expression.LeftOperand.Evaluate(boards) || expression.RightOperand.Evaluate(boards), 
			HandStrengthExpressionBinaryOperator.And => expression.LeftOperand.Evaluate(boards) && expression.RightOperand.Evaluate(boards), 
			HandStrengthExpressionBinaryOperator.AndNot => expression.LeftOperand.Evaluate(boards) && !expression.RightOperand.Evaluate(boards), 
			HandStrengthExpressionBinaryOperator.OrNot => expression.LeftOperand.Evaluate(boards) || !expression.RightOperand.Evaluate(boards), 
			_ => throw new NotImplementedException(), 
		};
	}

	public static bool EvaluateBinaryHandValue(this HandStrengthBinaryExpression expression, HandStrengthValueExpression hand)
	{
		return expression.Operator switch
		{
			HandStrengthExpressionBinaryOperator.Or => expression.LeftOperand.Evaluate(hand) || expression.RightOperand.Evaluate(hand), 
			HandStrengthExpressionBinaryOperator.And => expression.LeftOperand.Evaluate(hand) && expression.RightOperand.Evaluate(hand), 
			HandStrengthExpressionBinaryOperator.AndNot => expression.LeftOperand.Evaluate(hand) && !expression.RightOperand.Evaluate(hand), 
			HandStrengthExpressionBinaryOperator.OrNot => expression.LeftOperand.Evaluate(hand) || !expression.RightOperand.Evaluate(hand), 
			_ => throw new NotImplementedException(), 
		};
	}

	public static bool EvaluateNotHandValue(this HandStrengthNotExpression expression, HandStrengthValueExpression hand)
	{
		return !expression.Operand.Evaluate(hand);
	}

	public static HandStrengthValueExpression ToHandStrengthValueExpression(this PreflopRangeHoldemCompact preflopRange)
	{
		return new HandStrengthValueExpression(0L, 0L, 0L, 0L, 0L, 0L, preflopRange, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
	}

	public static HandStrengthValueExpression ToHandStrengthExpressionValue(this RelevantHandValue value, Streets street, PreflopRangeHoldemCompact preflopRange)
	{
		return street switch
		{
			Streets.Flop => HandStrengthValueExpression.Create(value, (RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantFlopType)0L, (RelevantTurnType)0uL, (RelevantRiverType)0uL, preflopRange, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred), 
			Streets.Turn => HandStrengthValueExpression.Create((RelevantHandValue)0L, value, (RelevantHandValue)0L, (RelevantFlopType)0L, (RelevantTurnType)0uL, (RelevantRiverType)0uL, preflopRange, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred), 
			Streets.River => HandStrengthValueExpression.Create((RelevantHandValue)0L, (RelevantHandValue)0L, value, (RelevantFlopType)0L, (RelevantTurnType)0uL, (RelevantRiverType)0uL, preflopRange, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred), 
			_ => throw new ArgumentException("street"), 
		};
	}

	public static bool ContainsHandValueExpression(this IHandStrengthExpression handStrengthExpression)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthValueExpression { HasHandValue: var hasHandValue }))
			{
				if (handStrengthExpression is HandStrengthNotExpression handStrengthNotExpression)
				{
					return handStrengthNotExpression.Operand.ContainsHandValueExpression();
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return hasHandValue;
		}
		return handStrengthBinaryExpression.LeftOperand.ContainsHandValueExpression() || handStrengthBinaryExpression.RightOperand.ContainsHandValueExpression();
	}

	public static bool ContainsBoardExpression(this IHandStrengthExpression handStrengthExpression)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthValueExpression { HasBoard: var hasBoard }))
			{
				if (handStrengthExpression is HandStrengthNotExpression handStrengthNotExpression)
				{
					return handStrengthNotExpression.Operand.ContainsBoardExpression();
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return hasBoard;
		}
		return handStrengthBinaryExpression.LeftOperand.ContainsBoardExpression() || handStrengthBinaryExpression.RightOperand.ContainsBoardExpression();
	}

	public static Streets GetLastHandValueStreet(this IHandStrengthExpression handStrengthExpression)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthValueExpression handStrengthValueExpression))
			{
				if (handStrengthExpression is HandStrengthNotExpression handStrengthNotExpression)
				{
					return handStrengthNotExpression.Operand.GetLastHandValueStreet();
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return handStrengthValueExpression.GetLastHandValueStreet();
		}
		return StreetsHelper.MaxStreet(handStrengthBinaryExpression.LeftOperand.GetLastHandValueStreet(), handStrengthBinaryExpression.RightOperand.GetLastHandValueStreet());
	}

	public static Streets GetLastBoardStreet(this IHandStrengthExpression handStrengthExpression)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthValueExpression { LastStreet: var lastStreet }))
			{
				if (handStrengthExpression is HandStrengthNotExpression handStrengthNotExpression)
				{
					return handStrengthNotExpression.Operand.GetLastBoardStreet();
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return lastStreet;
		}
		return StreetsHelper.MaxStreet(handStrengthBinaryExpression.LeftOperand.GetLastBoardStreet(), handStrengthBinaryExpression.RightOperand.GetLastBoardStreet());
	}

	public static IEnumerable<HandStrengthValueExpression> GetAllValueExpressions(this IHandStrengthExpression handStrengthExpression)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthValueExpression obj))
			{
				if (handStrengthExpression is HandStrengthNotExpression handStrengthNotExpression)
				{
					return handStrengthNotExpression.Operand.GetAllValueExpressions();
				}
				throw new NotImplementedException(handStrengthExpression.GetType().Name);
			}
			return obj.ToSingleIEnumerable();
		}
		return handStrengthBinaryExpression.LeftOperand.GetAllValueExpressions().Concat(handStrengthBinaryExpression.RightOperand.GetAllValueExpressions());
	}

	public static HandStrengthValueExpression ToHandStrengthExpressionValue(this RelevantFlopType value)
	{
		return HandStrengthValueExpression.Create((RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantHandValue)0L, value, (RelevantTurnType)0uL, (RelevantRiverType)0uL, PreflopRangeHoldemCompact.Empty, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
	}

	public static HandStrengthValueExpression ToHandStrengthExpression(this RelevantFlopType value)
	{
		return HandStrengthValueExpression.Create((RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantHandValue)0L, value, (RelevantTurnType)0uL, (RelevantRiverType)0uL, PreflopRangeHoldemCompact.Empty, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
	}

	public static HandStrengthValueExpression ToHandStrengthExpressionValue(this RelevantTurnType value)
	{
		return HandStrengthValueExpression.Create((RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantFlopType)0L, value, (RelevantRiverType)0uL, PreflopRangeHoldemCompact.Empty, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
	}

	public static HandStrengthValueExpression ToHandStrengthExpression(this RelevantTurnType value)
	{
		return HandStrengthValueExpression.Create((RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantFlopType)0L, value, (RelevantRiverType)0uL, PreflopRangeHoldemCompact.Empty, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
	}

	public static HandStrengthValueExpression ToHandStrengthExpressionValue(this RelevantRiverType value)
	{
		return HandStrengthValueExpression.Create((RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantFlopType)0L, (RelevantTurnType)0uL, value, PreflopRangeHoldemCompact.Empty, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
	}

	public static HandStrengthValueExpression ToHandStrengthExpression(this RelevantRiverType value)
	{
		return HandStrengthValueExpression.Create((RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantFlopType)0L, (RelevantTurnType)0uL, value, PreflopRangeHoldemCompact.Empty, OmahaPreflopRange.Empty, Poker.Calc.Range.ZeroToHundred);
	}

	public static Streets GetLastStreet(this IHandStrengthExpression handStrengthExpression)
	{
		if (!(handStrengthExpression is HandStrengthBinaryExpression handStrengthBinaryExpression))
		{
			if (!(handStrengthExpression is HandStrengthNotExpression handStrengthNotExpression))
			{
				if (!(handStrengthExpression is HandStrengthValueExpression { LastStreet: var lastStreet }))
				{
					throw new NotImplementedException();
				}
				return lastStreet;
			}
			return handStrengthNotExpression.Operand.GetLastStreet();
		}
		return StreetsHelper.MaxStreet(handStrengthBinaryExpression.LeftOperand.GetLastStreet(), handStrengthBinaryExpression.RightOperand.GetLastStreet());
	}

	public static IHandStrengthExpression AppendPreflopRange(this IHandStrengthExpression expression, PreflopRangeHoldemCompact preflopRange)
	{
		if (expression is HandStrengthValueExpression expression2)
		{
			return expression2.AppendPreflopRange(preflopRange);
		}
		return new HandStrengthBinaryExpression(HandStrengthExpressionBinaryOperator.And, preflopRange.ToHandStrengthValueExpression(), expression);
	}

	public static HandStrengthValueExpression AppendPreflopRange(this HandStrengthValueExpression expression, PreflopRangeHoldemCompact preflopRange)
	{
		return expression with
		{
			PreflopRangeHoldem = preflopRange.Merge(expression.PreflopRangeHoldem)
		};
	}

	public static bool IsPreflopExpression(this IHandStrengthExpression expression)
	{
		return expression.GetLastStreet() == Streets.Preflop;
	}

	public static bool IsBoardOnlyExpressions(this IHandStrengthExpression expression)
	{
		return expression.GetAllValueExpressions().All((HandStrengthValueExpression handStrengthValueExpression) => handStrengthValueExpression.IsBoardOnlyExpression);
	}
}
