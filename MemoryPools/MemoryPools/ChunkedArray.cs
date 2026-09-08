using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryPools;

public class ChunkedArray<T>
{
	private int? _itemSize;

	public int ChunkSize { get; }

	public List<T[]> Chunks { get; private set; } = new List<T[]>();

	public HashSet<T[]> ChunkHashSet { get; private set; } = new HashSet<T[]>();

	public long AllocatedLength => ChunkSize * Chunks.Count;

	public int ChunkCount => Chunks.Count;

	public int ItemSize
	{
		get
		{
			int valueOrDefault = _itemSize.GetValueOrDefault();
			if (!_itemSize.HasValue)
			{
				valueOrDefault = MemoryPooling.GetItemSize<T>();
				_itemSize = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public ChunkedArray(int chunkSize)
	{
		ChunkSize = chunkSize;
	}

	public void Expand()
	{
		T[] item = new T[ChunkSize];
		Chunks.Add(item);
		ChunkHashSet.Add(item);
	}

	public Memory<T> GetMemory(long start, int length)
	{
		GetChunkIndex(start);
		int indexInChunk = GetIndexInChunk(start);
		if (indexInChunk + length > ChunkSize)
		{
			throw new InvalidOperationException($"Request chunk memory boundary (start={start}, length={length}, indexInChunk={indexInChunk}) goes beyond the chunk size ({ChunkSize})");
		}
		return new Memory<T>(Chunks[GetChunkIndex(start)], GetIndexInChunk(start), length);
	}

	public int GetChunkIndex(long position)
	{
		return (int)(position / ChunkSize);
	}

	public int GetIndexInChunk(long position)
	{
		return (int)(position % ChunkSize);
	}

	public void Clear()
	{
		Chunks.Clear();
		ChunkHashSet.Clear();
	}

	public bool ContainsMemory(Memory<T> memory)
	{
		if (memory.Length > 0)
		{
			return ChunkHashSet.Contains(memory.GetUnderlyingArray());
		}
		return false;
	}

	public int GetAllocatedMegabytes()
	{
		return (int)((long)ChunkSize * (long)Chunks.Count * ItemSize / 1024 / 1024);
	}

	public void DeallocatePartial(double remainingPart)
	{
		int num = (int)((double)AllocatedLength * remainingPart) + 1;
		int count = GetChunkIndex(num) + 1;
		Chunks = Chunks.Take(count).ToList();
		ChunkHashSet = Chunks.ToHashSet();
	}
}
