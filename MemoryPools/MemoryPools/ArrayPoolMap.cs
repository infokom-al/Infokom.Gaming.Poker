using System;
using System.Collections.Generic;
using System.Threading;

namespace MemoryPools;

public class ArrayPoolMap<T>
{
	public int ThreadId = Thread.CurrentThread.ManagedThreadId;

	public const int LargeArraySize = 65536;

	public ArrayPool<T>?[] ArrayPools { get; private set; } = new ArrayPool<T>[32];

	public Dictionary<int, ArrayPool<T>> LargeArrayPools { get; } = new Dictionary<int, ArrayPool<T>>();

	public T[] GetArray(int size)
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadId)
		{
			throw new InvalidOperationException($"{GetType()} is accessed from different threads");
		}
		if (!Pooling.EnableThreadObjectPooling)
		{
			return new T[size];
		}
		if (size < ArrayPools.Length)
		{
			ArrayPool<T> arrayPool = ArrayPools[size];
			if (arrayPool == null)
			{
				ArrayPool<T> arrayPool2 = new ArrayPool<T>(size);
				arrayPool = (ArrayPools[size] = arrayPool2);
				if (this == ArrayPool<T>.ThreadShared)
				{
					Pooling.ThreadSharedPools.Add(arrayPool2);
				}
			}
			return arrayPool.GetArray();
		}
		if (size >= 65536)
		{
			if (!size.IsPowerOfTwo())
			{
				throw new InvalidOperationException($"We can pool large arrays only if their size is a power of two but was {size}");
			}
			if (!LargeArrayPools.TryGetValue(size, out ArrayPool<T> value))
			{
				value = (LargeArrayPools[size] = new ArrayPool<T>(size));
				if (this == ArrayPool<T>.ThreadShared)
				{
					Pooling.ThreadSharedPools.Add(value);
				}
			}
			return value.GetArray();
		}
		ArrayPool<T>[] array = new ArrayPool<T>[size + 1];
		ArrayPools.CopyTo(array, 0);
		ArrayPools = array;
		return GetArray(size);
	}

	public void Return(T[] array)
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadId)
		{
			throw new InvalidOperationException($"{GetType()} is accessed from different threads");
		}
		if (Pooling.EnableThreadObjectPooling)
		{
			ArrayPools[array.Length]?.Return(array);
		}
	}

	public void ReclaimAll()
	{
		ArrayPool<T>[] arrayPools = ArrayPools;
		for (int i = 0; i < arrayPools.Length; i++)
		{
			arrayPools[i]?.Reclaim();
		}
	}
}
