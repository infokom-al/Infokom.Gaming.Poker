using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryPools;

public class MemoryPool
{
	public Dictionary<Type, IMemoryPool> MemoryPoolsByType { get; private set; } = new Dictionary<Type, IMemoryPool>();

	public IEnumerable<IMemoryPool> MemoryPools => MemoryPoolsByType.Values;

	public int GetAllocatedMegabytes()
	{
		return MemoryPools.Sum((IMemoryPool pool) => pool.GetAllocatedMegabytes());
	}

	public int GetUsedMegabytes()
	{
		return MemoryPools.Sum((IMemoryPool pool) => pool.GetUsedMegabytes());
	}

	public void ClearAll()
	{
		MemoryPools.ClearAll();
	}

	public MemoryPool<T> GetMemoryPool<T>()
	{
		if (MemoryPoolsByType.TryGetValue(typeof(T), out IMemoryPool value))
		{
			return (MemoryPool<T>)value;
		}
		MemoryPool<T> memoryPool = new MemoryPool<T>();
		MemoryPoolsByType.Add(typeof(T), memoryPool);
		return memoryPool;
	}
}
public class MemoryPool<T> : IMemoryPool
{
	private static int _chunkSize;

	public ChunkedArray<T> Buffer { get; private set; }

	public static MemoryPool<T> ThreadShared => MemoryPooling.ThreadMemoryPool.GetMemoryPool<T>();

	public Dictionary<int, Stack<Memory<T>>> ReturnedMemory { get; private set; } = new Dictionary<int, Stack<Memory<T>>>();

	public long UsedItemCount { get; private set; }

	public long AllocatedItemCount => Buffer.AllocatedLength;

	public long FreeItemCount => AllocatedItemCount - UsedItemCount;

	public int CurrentChunkFreeItems => Buffer.ChunkSize - Buffer.GetIndexInChunk(UsedItemCount);

	public long AllocatedSize => AllocatedItemCount * ItemSize;

	public long UsedSize => UsedItemCount * ItemSize;

	public static int ChunkSizeExponent { get; set; }

	public static int ChunkSize
	{
		get
		{
			if (_chunkSize != 0)
			{
				return _chunkSize;
			}
			_chunkSize = MemoryPooling.GetChunkSize<T>().VerifyIsPowerOfTwo();
			ChunkSizeExponent = _chunkSize.GetPowerOfTwoExponent();
			return _chunkSize;
		}
		set
		{
			if (_chunkSize != 0 && _chunkSize != value)
			{
				throw new InvalidOperationException("Can't set chunk size twice");
			}
			_chunkSize = value.VerifyIsPowerOfTwo();
			ChunkSizeExponent = _chunkSize.GetPowerOfTwoExponent();
		}
	}

	public int ItemSize => MemoryPoolingFunctions.GetSizeOf<T>();

	public static bool AllowNonPowerOfTwoChunks { get; set; }

	public MemoryPool()
	{
		Buffer = new ChunkedArray<T>(ChunkSize);
	}

	public MemoryPoolStats GetPoolStats()
	{
		return this.GetMemoryPoolStats();
	}

	public int GetAllocatedMegabytes()
	{
		return (AllocatedItemCount * Buffer.ItemSize / 1024 / 1024).VerifyInt();
	}

	public int GetUsedMegabytes()
	{
		return (int)(UsedItemCount * Buffer.ItemSize / 1024 / 1024);
	}

	public void Reclaim()
	{
		ReturnedMemory.Clear();
		ReturnedMemory = new Dictionary<int, Stack<Memory<T>>>();
		UsedItemCount = 0L;
	}

	public void DeallocatePartial(double remainingPart)
	{
		double value = (double)UsedItemCount / (double)AllocatedItemCount;
		if (value.IsGreater(remainingPart))
		{
			throw new InvalidOperationException($"Remaining part ({remainingPart}) can't be smaller than used part ({value})");
		}
		Buffer.DeallocatePartial(remainingPart);
	}

	public void Clear()
	{
		Buffer.Clear();
		ReturnedMemory.Clear();
		ReturnedMemory = new Dictionary<int, Stack<Memory<T>>>();
		Buffer = new ChunkedArray<T>(ChunkSize);
		UsedItemCount = 0L;
	}

	public Memory<T> GetMemory(int size, bool clear)
	{
		if (!MemoryPooling.EnableThreadMemoryPooling)
		{
			return new T[size].AsMemory();
		}
		if (!clear)
		{
			return GetMemory(size);
		}
		Memory<T> memory = GetMemory(size);
		memory.Span.Clear();
		return memory;
	}

	public Memory<T> GetMemory(int size)
	{
		if (!MemoryPooling.EnableThreadMemoryPooling)
		{
			return new T[size].AsMemory();
		}
		if (size > ChunkSize)
		{
			throw new InvalidOperationException($"Requested size ({size}) is greater than chunk size ({ChunkSize})");
		}
		if (size <= 0)
		{
			throw new ArgumentException($"Size can't be {size}", "size");
		}
		if (!MemoryPooling.EnableThreadMemoryPooling)
		{
			return new T[size];
		}
		if (TryGetReturnedMemory(size, out var result))
		{
			return result;
		}
		EnsureAllocated(size);
		result = Buffer.GetMemory(UsedItemCount, size);
		UsedItemCount += size;
		return result;
	}

	public Memory<T> GetCleanMemory(int size)
	{
		if (!MemoryPooling.EnableThreadMemoryPooling)
		{
			return new T[size].AsMemory();
		}
		Memory<T> memory = GetMemory(size);
		memory.Span.Clear();
		return memory;
	}

	public bool TryGetReturnedMemory(int size, out Memory<T> result)
	{
		if (!ReturnedMemory.TryGetValue(size, out Stack<Memory<T>> value))
		{
			result = default(Memory<T>);
			return false;
		}
		return value.TryPop(out result);
	}

	private void EnsureAllocated(int size)
	{
		if (FreeItemCount < size)
		{
			Buffer.Expand();
		}
		if (CurrentChunkFreeItems < size)
		{
			UsedItemCount += CurrentChunkFreeItems;
		}
	}

	public void Return(Memory<T> memory)
	{
		if (MemoryPooling.EnableThreadMemoryPooling && Buffer.ContainsMemory(memory))
		{
			int length = memory.Length;
			if (!ReturnedMemory.TryGetValue(length, out Stack<Memory<T>> value))
			{
				value = (ReturnedMemory[length] = new Stack<Memory<T>>());
			}
			value.Push(memory);
		}
	}

	public override string ToString()
	{
		return $"chunkSize={ChunkSize}".AddNameValue("itemSize", ItemSize, "bytes").AddNameValue("allocatedSize", AllocatedSize.GetMemorySizeString()).AddNameValue("usedSize", UsedSize.GetMemorySizeString())
			.AddNameValue("allocatedChunks", Buffer.ChunkCount.Shorten())
			.AddNameValue("allocatedItems", Buffer.AllocatedLength.Shorten())
			.AddNameValue("usedItems", UsedItemCount.Shorten())
			.AddNameValue("returnedItems", ReturnedMemory.Values.Sum((Stack<Memory<T>> stack) => stack.Count))
			.AddNameValue("returnedItemSizes", ReturnedMemory.AggregateToString<KeyValuePair<int, Stack<Memory<T>>>, string>((KeyValuePair<int, Stack<Memory<T>>> item) => $"{item.Key}:{item.Value.Count}", ", "));
	}
}
