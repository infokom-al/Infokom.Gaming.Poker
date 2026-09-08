using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

public class HandStrengthExpressionOptimized : IEquatable<HandStrengthExpressionOptimized>
{
	public HandStrengthExpressionType Type { get; }

	public HandStrengthExpressionOptimized LeftOperand { get; }

	public HandStrengthExpressionOptimizedOperator Operator { get; }

	public HandStrengthExpressionOptimized RightOperand { get; }

	public HandStrengthValueExpression? ExpressionValue { get; }

	public bool HasFlopBoardFilter { get; }

	public bool HasTurnBoardFilter { get; }

	public bool HasRiverBoardFilter { get; }

	public bool HasBoardFilter
	{
		get
		{
			if (!HasFlopBoardFilter && !HasTurnBoardFilter)
			{
				return HasRiverBoardFilter;
			}
			return true;
		}
	}

	public bool IsBinaryExpression => Type == HandStrengthExpressionType.Binary;

	public bool IsNotExpression => Type == HandStrengthExpressionType.Not;

	public bool IsValueExpression => Type == HandStrengthExpressionType.Value;

	[JsonIgnore]
	public IEnumerable<HandStrengthValueExpression> HandStrengthFilters => Type switch
	{
		HandStrengthExpressionType.Binary => LeftOperand.HandStrengthFilters.Concat(RightOperand.HandStrengthFilters), 
		HandStrengthExpressionType.Value => ExpressionValue.ToSingleIEnumerable(), 
		HandStrengthExpressionType.Not => Enumerable.Empty<HandStrengthValueExpression>(), 
		_ => throw new NotImplementedException(), 
	};

	public Streets GetLastHandValueStreet()
	{
		return ExpressionValue.GetLastHandValueStreet();
	}

	public Streets GetLastHandBoardStreet()
	{
		return ExpressionValue.GetLastHandBoardStreet();
	}

	public HandStrengthExpressionOptimized(HandStrengthExpressionType type, bool hasFlopBoardFilter, bool hasTurnBoardFilter, bool hasRiverBoardFilter, HandStrengthExpressionOptimized? leftOperand = null, HandStrengthExpressionOptimizedOperator? @operator = null, HandStrengthExpressionOptimized? rightOperand = null, HandStrengthValueExpression? expressionValue = null)
	{
		Type = type;
		LeftOperand = leftOperand;
		Operator = @operator.GetValueOrDefault();
		RightOperand = rightOperand;
		ExpressionValue = expressionValue;
		HasFlopBoardFilter = hasFlopBoardFilter;
		HasTurnBoardFilter = hasTurnBoardFilter;
		HasRiverBoardFilter = hasRiverBoardFilter;
	}

	public bool Evaluate(RelevantHandBoards boards)
	{
		return Type switch
		{
			HandStrengthExpressionType.Binary => EvaluateBinary(boards), 
			HandStrengthExpressionType.Value => EvaluateValue(boards), 
			HandStrengthExpressionType.Not => EvaluateNot(boards), 
			_ => throw new NotImplementedException(), 
		};
	}

	public bool Evaluate(HandStrengthValueExpression hand)
	{
		return Type switch
		{
			HandStrengthExpressionType.Binary => EvaluateBinary(hand), 
			HandStrengthExpressionType.Value => EvaluateValue(hand), 
			HandStrengthExpressionType.Not => EvaluateNot(hand), 
			_ => throw new NotImplementedException(), 
		};
	}

	private bool EvaluateBinary(RelevantHandBoards boards)
	{
		return Operator switch
		{
			HandStrengthExpressionOptimizedOperator.Or => LeftOperand.Evaluate(boards) || RightOperand.Evaluate(boards), 
			HandStrengthExpressionOptimizedOperator.And => LeftOperand.Evaluate(boards) && RightOperand.Evaluate(boards), 
			HandStrengthExpressionOptimizedOperator.AndNot => LeftOperand.Evaluate(boards) && !RightOperand.Evaluate(boards), 
			HandStrengthExpressionOptimizedOperator.OrNot => LeftOperand.Evaluate(boards) || !RightOperand.Evaluate(boards), 
			HandStrengthExpressionOptimizedOperator.Not => throw new InvalidOperationException("Not is not a binary operator. Use Not expression instead"), 
			_ => throw new NotImplementedException(), 
		};
	}

	private bool EvaluateBinary(HandStrengthValueExpression hand)
	{
		return Operator switch
		{
			HandStrengthExpressionOptimizedOperator.Or => LeftOperand.Evaluate(hand) || RightOperand.Evaluate(hand), 
			HandStrengthExpressionOptimizedOperator.And => LeftOperand.Evaluate(hand) && RightOperand.Evaluate(hand), 
			HandStrengthExpressionOptimizedOperator.AndNot => LeftOperand.Evaluate(hand) && !RightOperand.Evaluate(hand), 
			HandStrengthExpressionOptimizedOperator.OrNot => LeftOperand.Evaluate(hand) || !RightOperand.Evaluate(hand), 
			HandStrengthExpressionOptimizedOperator.Not => throw new InvalidOperationException("Not is not a binary operator. Use Not expression instead"), 
			_ => throw new NotImplementedException(), 
		};
	}

	private bool EvaluateNot(RelevantHandBoards boards)
	{
		return !LeftOperand.Evaluate(boards);
	}

	private bool EvaluateNot(HandStrengthValueExpression hand)
	{
		return !LeftOperand.Evaluate(hand);
	}

	private bool EvaluateValue(RelevantHandBoards boards)
	{
		return ExpressionValue.Evaluate(boards);
	}

	private bool EvaluateValue(HandStrengthValueExpression hand)
	{
		return ExpressionValue.Evaluate(hand);
	}

	public override string ToString()
	{
		return Type switch
		{
			HandStrengthExpressionType.Binary => ToStringBinary(), 
			HandStrengthExpressionType.Value => ToStringValue(), 
			HandStrengthExpressionType.Not => ToStringNot(), 
			_ => throw new NotImplementedException(), 
		};
	}

	private string ToStringBinary()
	{
		return $"({LeftOperand} {Operator} {RightOperand})";
	}

	private string ToStringNot()
	{
		return $"(Not {LeftOperand})";
	}

	private string ToStringValue()
	{
		return ExpressionValue.ToString();
	}

	public bool Equals(HandStrengthExpressionOptimized? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (Type == other.Type && LeftOperand.Equals((object?)other.LeftOperand) && Operator == other.Operator && RightOperand.Equals((object?)other.RightOperand) && ExpressionValue.Equals(other.ExpressionValue) && HasFlopBoardFilter == other.HasFlopBoardFilter && HasTurnBoardFilter == other.HasTurnBoardFilter)
		{
			return HasRiverBoardFilter == other.HasRiverBoardFilter;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((HandStrengthExpressionOptimized)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine((int)Type, LeftOperand, (int)Operator, RightOperand, ExpressionValue, HasFlopBoardFilter, HasTurnBoardFilter, HasRiverBoardFilter);
	}
}
