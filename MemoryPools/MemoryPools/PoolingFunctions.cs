using System.Collections.Generic;
using System.IO;

namespace MemoryPools;

public static class PoolingFunctions
{
	public static void ReclaimAll(this IEnumerable<IPool> pools)
	{
		foreach (IPool pool in pools)
		{
			pool.Reclaim();
		}
	}

	public static void RefillAll(this IEnumerable<IPool> pools)
	{
		foreach (IPool pool in pools)
		{
			pool.Refill();
		}
	}

	public static byte[] ToArrayFromPoolBackedByPowerOfTwo(this MemoryStream stream, ArrayPoolMap<byte> arrayPool, out int resultLength)
	{
		if (stream.Position == 0L)
		{
			resultLength = 0;
			return EmptyArray<byte>.Value;
		}
		resultLength = stream.Position.VerifyInt();
		int powerOfTwoGreaterOfEqual = resultLength.GetPowerOfTwoGreaterOfEqual();
		byte[] array = arrayPool.GetArray(powerOfTwoGreaterOfEqual);
		stream.WriteTo(new MemoryStream(array));
		return array;
	}
}
