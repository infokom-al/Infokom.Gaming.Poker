using BinarySerializer;
using ProtoBuf;

namespace Hand2NoteCore.HandStrength;

[TypeTag(1, typeof(HandStrengthValueExpression))]
[TypeTag(2, typeof(HandStrengthBinaryExpression))]
[TypeTag(3, typeof(HandStrengthNotExpression))]
[ProtoContract]
[ProtoInclude(100, typeof(HandStrengthValueExpression))]
[ProtoInclude(101, typeof(HandStrengthBinaryExpression))]
[ProtoInclude(102, typeof(HandStrengthNotExpression))]
public interface IHandStrengthExpression
{
	bool IsBinaryExpression => this is HandStrengthBinaryExpression;

	bool IsNotExpression => this is HandStrengthNotExpression;

	bool IsValueExpression => this is HandStrengthValueExpression;

	bool IsGroupExpression => !IsValueExpression;
}
