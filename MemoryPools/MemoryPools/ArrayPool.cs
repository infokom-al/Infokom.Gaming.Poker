using System;
using System.Collections.Generic;
using System.Threading;

namespace MemoryPools;

public class ArrayPool<T> : IPool
{
	private T[] _firstArray;

	public static ThreadLocal<ArrayPoolMap<T>> _threadSharedMap = new ThreadLocal<ArrayPoolMap<T>>(() => new ArrayPoolMap<T>(), trackAllValues: true);

	public T[] FirstArray
	{
		get
		{
			return _firstArray ?? (_firstArray = new T[ArraySize]);
		}
		private set
		{
			_firstArray = value;
		}
	}

	public List<T[]> Arrays { get; private set; }

	public int Position { get; private set; } = -1;

	public int ArraySize { get; }

	public static ArrayPoolMap<T> ThreadShared => _threadSharedMap.Value;

	public int ThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

	public ArrayPool(int arraySize)
	{
		ArraySize = arraySize;
		Arrays = new List<T[]>();
	}

	public T[] GetArray()
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadId)
		{
			throw new InvalidOperationException($"{GetType()} is accessed from different threads");
		}
		if (!Pooling.EnableThreadObjectPooling)
		{
			return new T[ArraySize];
		}
		if (ArraySize == 0)
		{
			return FirstArray;
		}
		if (Position == -1)
		{
			Position = 0;
			return FirstArray;
		}
		Position++;
		if (Arrays.Count >= Position)
		{
			return GetArrayAtPosition(Position);
		}
		if (Pooling.ThrowIfMissingPooledObject)
		{
			throw new InvalidOperationException($"Missing array of size {ArraySize} in the pool");
		}
		T[] array = new T[ArraySize];
		Arrays.Add(array);
		return array;
	}

	public void Return(T[] array)
	{
		if (array.Length != ArraySize)
		{
			throw new InvalidOperationException($"Can't return an array of size {array.Length} to the array pool of {ArraySize} sizes");
		}
		if (Position == 0)
		{
			return;
		}
		if (GetArrayAtPosition(Position) == array)
		{
			Position--;
			return;
		}
		int arrayPosition = GetArrayPosition(array);
		if (arrayPosition != -1)
		{
			SwapArrays(array, arrayPosition, GetArrayAtPosition(Position), Position);
			Position--;
		}
	}

	public void SwapArrays(T[] firstArray, int firstArrayPosition, T[] secondArray, int secondArrayPosition)
	{
		SetArrayAtPosition(firstArray, firstArrayPosition);
		SetArrayAtPosition(secondArray, secondArrayPosition);
	}

	private void SetArrayAtPosition(T[] array, int position)
	{
		if (position == 0)
		{
			FirstArray = array;
		}
		else
		{
			Arrays[Position - 1] = array;
		}
	}

	private int GetArrayPosition(T[] array)
	{
		for (int i = 0; i <= Position; i++)
		{
			if (GetArrayAtPosition(i) == array)
			{
				return i;
			}
		}
		return -1;
	}

	private T[] GetArrayAtPosition(int position)
	{
		if (position == 0)
		{
			return FirstArray;
		}
		return Arrays[position - 1];
	}

	public void Reclaim()
	{
		Position = -1;
	}

	public void Clear()
	{
		FirstArray = null;
		Arrays = new List<T[]>();
		Position = -1;
	}

	public void Refill()
	{
		throw new NotImplementedException("We don't need refilling arrays at this moment");
	}

	public override string ToString()
	{
		return $"Pool of {typeof(T).Name}[{ArraySize}] with {Position} items";
	}
}
