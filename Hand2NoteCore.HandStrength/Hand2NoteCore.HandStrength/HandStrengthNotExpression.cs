using BinarySerializer;
using MemoryPools;
using ProtoBuf;

namespace Hand2NoteCore.HandStrength;

[BinarySerializable]
[ProtoContract(SkipConstructor = true)]
public class HandStrengthNotExpression : IHandStrengthExpression
{
	[Tag(1)]
	[ProtoMember(1)]
	public IHandStrengthExpression Operand { get; set; }

	[BinaryDeserializationConstructor]
	public static HandStrengthNotExpression Create(IHandStrengthExpression operand)
	{
		HandStrengthNotExpression handStrengthNotExpression = ObjectPool<HandStrengthNotExpression>.ThreadShared.RentObject();
		handStrengthNotExpression.Operand = operand;
		return handStrengthNotExpression;
	}

	public override string ToString()
	{
		return $"(Not {Operand})";
	}

	protected bool Equals(HandStrengthNotExpression other)
	{
		return Operand.Equals(other.Operand);
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
		return Equals((HandStrengthNotExpression)obj);
	}

	public override int GetHashCode()
	{
		return Operand.GetHashCode();
	}
}
