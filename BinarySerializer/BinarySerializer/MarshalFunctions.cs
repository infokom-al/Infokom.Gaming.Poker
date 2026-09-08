using System;
using System.Runtime.InteropServices;

namespace BinarySerializer;

public static class MarshalFunctions
{
	public static Span<T> CastMemory<T>(this Span<byte> bytes) where T : struct
	{
		return MemoryMarshal.Cast<byte, T>(bytes);
	}

	public static Span<byte> CastToBytes<T>(this Span<T> span) where T : struct
	{
		return MemoryMarshal.Cast<T, byte>(span);
	}
}
