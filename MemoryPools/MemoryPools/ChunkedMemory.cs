using System;
using System.Collections.Generic;

namespace MemoryPools;

public readonly struct ChunkedMemory<T>
{
	public static ChunkedMemory<T> Empty = new ChunkedMemory<T>(MemoryBufferCursor<Memory<T>>.Empty);

	public MemoryBufferCursor<Memory<T>> Chunks { get; }

	public int Length
	{
		get
		{
			if (ChunkCount != 1)
			{
				return ChunkSize * ChunkCount;
			}
			return FirstChunk.Length;
		}
	}

	public int ChunkCount => Chunks.Cursor;

	public bool IsEmpty => Length == 0;

	public bool IsNotEmpty => Length != 0;

	public int Count => Length;

	public Memory<T> FirstChunk => GetChunk(0);

	public static int ChunkSize => MemoryPool<T>.ChunkSize;

	public T this[int index]
	{
		get
		{
			if (index >= Length)
			{
				throw new InvalidOperationException($"Index ({index}) is out of range ({Length})");
			}
			return GetChunk(GetChunkIndex(index)).Span[GetIndexInChunk(index)];
		}
	}

	public ChunkedMemory(MemoryBufferCursor<Memory<T>> chunks)
	{
		Chunks = chunks;
	}

	public Memory<T> GetChunk(int index)
	{
		return Chunks[index];
	}

	public Memory<T> GetPositionChunk(int position)
	{
		return Chunks[GetChunkIndex(position)];
	}

	public int GetChunkIndex(int position)
	{
		return position / ChunkSize;
	}

	public int GetIndexInChunk(int position)
	{
		return position % ChunkSize;
	}

	public void SetItem(int index, T value)
	{
		Chunks[GetChunkIndex(index)].Span[GetIndexInChunk(index)] = value;
	}

	public ChunkedMemory<T> Expand(int minSize)
	{
		if ((ChunkCount == 0 || FirstChunk.IsEmpty) && minSize <= 1)
		{
			return new ChunkedMemory<T>(Chunks.AddItem(MemoryPool<T>.ThreadShared.GetMemory(1)));
		}
		if (ChunkCount == 0 && minSize < ChunkSize)
		{
			int size = Math.Min(ChunkSize, minSize.GetPowerOfTwoGreaterOfEqual());
			return new ChunkedMemory<T>(Chunks.AddItem(MemoryPool<T>.ThreadShared.GetMemory(size)));
		}
		if (ChunkCount == 1 && FirstChunk.Length < ChunkSize && minSize <= ChunkSize)
		{
			Memory<T> firstChunk = FirstChunk;
			int newSize = Math.Min(ChunkSize, Math.Max(minSize.GetPowerOfTwoNumberGreaterOrEqualOrZero(), firstChunk.Length * 2));
			return new ChunkedMemory<T>(Chunks.SetItem(0, firstChunk.ReplaceFromPool(newSize)));
		}
		if (Length + ChunkSize >= minSize)
		{
			return new ChunkedMemory<T>(Chunks.AddItem(MemoryPool<T>.ThreadShared.GetMemory(ChunkSize)));
		}
		MemoryBufferCursor<Memory<T>> chunks = Chunks;
		for (int i = 0; i < (minSize - Length).GetNumberOfBuckets(ChunkSize); i++)
		{
			chunks = chunks.AddItem(MemoryPool<T>.ThreadShared.GetMemory(ChunkSize));
		}
		return new ChunkedMemory<T>(chunks);
	}

	public ChunkedMemory<T> ExpandUntil(int targetLength)
	{
		if (targetLength < Length)
		{
			return this;
		}
		return Expand(targetLength);
	}

	public ChunkedMemory<T> CopyFrom(ChunkedMemory<T> other, int startPosition, int length)
	{
		if (other.Length < length)
		{
			throw new InvalidOperationException($"Given chunked memory is shorten (other.length={other.Length}) than length to copy ({length}).");
		}
		int targetLength = length + startPosition;
		ChunkedMemory<T> result = ExpandUntil(targetLength);
		int num = length;
		int num2 = startPosition;
		Span<Memory<T>> span = other.Chunks.Buffer.Span;
		for (int i = 0; i < span.Length; i++)
		{
			Memory<T> memory = span[i];
			int num3 = Math.Min(memory.Length, num);
			result.CopyItemsFrom(memory.Span.GetHead(num3), num2);
			num2 += num3;
			num -= num3;
			if (num == 0)
			{
				return result;
			}
		}
		throw new InvalidOperationException("Should not reach here");
	}

	public ChunkedMemory<T> CopyFromSpan(Span<T> span, int startPosition)
	{
		ChunkedMemory<T> result = ExpandUntil(startPosition + span.Length);
		result.CopyItemsFrom(span, startPosition);
		return result;
	}

	private void CopyItemsFrom(Span<T> items, int startPosition)
	{
		int chunkIndex = GetChunkIndex(startPosition);
		Memory<T> memory = Chunks[chunkIndex];
		int indexInChunk = GetIndexInChunk(startPosition);
		int num = Math.Min(items.Length, memory.Length - indexInChunk);
		int num2 = items.Length - num;
		items.GetHead(num).CopyTo(memory.Span.CutHead(indexInChunk));
		if (num2 > 0)
		{
			items.GetTail(num2).CopyTo(Chunks[chunkIndex + 1].Span);
		}
	}

	public ChunkedMemory<T> Clone()
	{
		List<Memory<T>> list = new List<Memory<T>>();
		for (int i = 0; i < Chunks.Buffer.Length; i++)
		{
			Memory<T> item = Chunks[i].CloneFromPool();
			list.Add(item);
		}
		return new ChunkedMemory<T>(new MemoryBufferCursor<Memory<T>>(list.ToArray().AsMemory(), Chunks.Cursor));
	}

	public void ReturnToThreadMemoryPool()
	{
		Memory<Memory<T>> buffer = Chunks.Buffer;
		Span<Memory<T>> span = buffer.Span;
		int num = 0;
		while (true)
		{
			int num2 = num;
			buffer = Chunks.Buffer;
			if (num2 < buffer.Length)
			{
				span[num].ReturnToThreadMemoryPool();
				num++;
				continue;
			}
			break;
		}
	}
}
