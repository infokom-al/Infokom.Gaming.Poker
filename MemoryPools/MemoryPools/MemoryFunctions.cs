using System;

namespace MemoryPools;

public static class MemoryFunctions
{
	public static T[] GetUnderlyingArray<T>(this Memory<T> memory)
	{
		return MemoryUnderlyingArrayAccessor<T>.GetUnderlyingArray(memory).VerifyType<T[]>();
	}
}
