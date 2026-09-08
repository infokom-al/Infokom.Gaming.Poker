using System;

namespace MemoryPools;

public struct ThreadSharedArray<T> : IDisposable
{
	public T[] Value { get; set; }

	public ThreadSharedArray(int size)
	{
		Value = ArrayPool<T>.ThreadShared.GetArray(size);
	}

	public void Dispose()
	{
		Value.ReturnToThreadSharedPool();
	}

	public static implicit operator T[](ThreadSharedArray<T> threadSharedArray)
	{
		return threadSharedArray.Value;
	}

	public static implicit operator Span<T>(ThreadSharedArray<T> threadSharedArray)
	{
		return threadSharedArray.Value.AsSpan();
	}
}
