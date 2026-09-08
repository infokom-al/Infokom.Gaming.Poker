using System;
using BinarySerializer;
using ProtoBuf;

namespace Hand2NoteCore.HandStrength;

[BinarySerializable]
[ProtoContract(SkipConstructor = true)]
public class HandStrengthBinaryExpression : IHandStrengthExpression
{
	[Tag(1)]
	[ProtoMember(1)]
	public HandStrengthExpressionBinaryOperator Operator { get; }

	[Tag(2)]
	[ProtoMember(2)]
	public IHandStrengthExpression LeftOperand { get; }

	[Tag(3)]
	[ProtoMember(3)]
	public IHandStrengthExpression RightOperand { get; }

	public HandStrengthBinaryExpression(HandStrengthExpressionBinaryOperator @operator, IHandStrengthExpression leftOperand, IHandStrengthExpression rightOperand)
	{
		Operator = @operator;
		LeftOperand = leftOperand;
		RightOperand = rightOperand;
	}

	public override string ToString()
	{
		return $"({LeftOperand} {Operator} {RightOperand})";
	}

	protected bool Equals(HandStrengthBinaryExpression other)
	{
		if (Operator == other.Operator && LeftOperand.Equals(other.LeftOperand))
		{
			return RightOperand.Equals(other.RightOperand);
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
		return Equals((HandStrengthBinaryExpression)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine((int)Operator, LeftOperand, RightOperand);
	}
}
