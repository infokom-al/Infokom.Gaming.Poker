using System;

namespace BinarySerializer;

internal static class Numbers
{
	public static int VerifyInt(this long value)
	{
		if (value < int.MinValue || value > int.MaxValue)
		{
			throw new InvalidOperationException($"Value {value} is out of ushort range");
		}
		return (int)value;
	}

	public static int GetVarIntBytes(this int value)
	{
		int num = 0;
		while ((value >>= 7) != 0)
		{
			num++;
		}
		return num;
	}

	public static int VerifyNotNegative(this int value, string? name = null)
	{
		if (value < 0)
		{
			throw new InvalidOperationException($"Value {name} can't be negative but was {value}");
		}
		return value;
	}
}
