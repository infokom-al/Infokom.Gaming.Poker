using System;

namespace MemoryPools;

public struct ArrayList<T>
{
	public T[] Array { get; }

	public int Count { get; private set; }

	public T Last => Array[Count - 1];

	public ArrayList(int size)
	{
		Count = 0;
		Array = ArrayPool<T>.ThreadShared.GetArray(size);
	}

	public ArrayList<T> Add(T item)
	{
		Array[Count] = item;
		Count++;
		return this;
	}

	public static implicit operator T[](ArrayList<T> list)
	{
		return list.ToArray();
	}

	public T[] ToArray()
	{
		if (Count != Array.Length)
		{
			return Array.AsSpan().Slice(0, Count).ToArrayFromPool();
		}
		return Array;
	}
}
