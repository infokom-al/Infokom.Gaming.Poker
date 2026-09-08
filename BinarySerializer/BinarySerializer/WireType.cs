namespace BinarySerializer;

public enum WireType
{
	VarInt,
	VarNegativeInt,
	Fixed32,
	Fixed64,
	LengthDelimited,
	Default
}
