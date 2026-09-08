using System;
using System.Collections.Generic;

namespace MemoryPools;

public readonly struct ExponentialMemory<T>
{
	public Memory<T> FirstBucketBuffer { get; }

	public Memory<Memory<T>> OtherBucketBuffer { get; }

	public int AllocatedBucketCount { get; }

	public int AllocatedLength
	{
		get
		{
			if (IsEmpty)
			{
				return 0;
			}
			if (AllocatedBucketCount != 1)
			{
				return FirstBucketLengthExponent.GetPowerOfTwoBucketLength(AllocatedBucketCount, MaxChunkSize, MaxChunkExponent);
			}
			return FirstAllocatedBucket.Length;
		}
	}

	public int MinimalSize => 2;

	public Memory<T> FirstAllocatedBucket => FirstBucketBuffer;

	public Memory<T> LastAllocatedBucket
	{
		get
		{
			if (AllocatedBucketCount > 1)
			{
				return OtherBucketBuffer.Span[AllocatedBucketCount - 2];
			}
			return FirstBucketBuffer;
		}
	}

	public int FirstBucketLengthExponent => FirstAllocatedBucket.Length.GetPowerOfTwoExponent();

	public int MaxChunkExponent => MemoryPool<T>.ChunkSizeExponent;

	public int MaxChunkSize => MemoryPool<T>.ChunkSize;

	public bool IsEmpty => AllocatedBucketCount == 0;

	public T this[int index] => GetItemAt(index);

	public IEnumerable<T> AsEnumerable
	{
		get
		{
			for (int i = 0; i < AllocatedBucketCount; i++)
			{
				Memory<T> bucket = GetBucketAt(i);
				for (int j = 0; j < bucket.Length; j++)
				{
					yield return bucket.Span[j];
				}
				bucket = default(Memory<T>);
			}
		}
	}

	public ExponentialMemory(Memory<T> firstBucketBuffer, Memory<Memory<T>> otherBucketBuffer, int allocatedBucketCount)
	{
		FirstBucketBuffer = firstBucketBuffer;
		OtherBucketBuffer = otherBucketBuffer;
		AllocatedBucketCount = allocatedBucketCount;
	}

	public ExponentialMemory()
	{
		FirstBucketBuffer = default(Memory<T>);
		OtherBucketBuffer = default(Memory<Memory<T>>);
		AllocatedBucketCount = 0;
	}

	public Memory<T> GetBucketAt(int index)
	{
		if (index != 0)
		{
			return OtherBucketBuffer.Span[index - 1];
		}
		return FirstAllocatedBucket;
	}

	public ref T GetRefAt(int index)
	{
		var (index2, index3) = GetIndex(index);
		return ref GetBucketAt(index2).Span[index3];
	}

	public T GetItemAt(int index)
	{
		var (index2, index3) = GetIndex(index);
		return GetBucketAt(index2).Span[index3];
	}

	public void SetItem(int index, T value)
	{
		var (index2, index3) = GetIndex(index);
		GetBucketAt(index2).Span[index3] = value;
	}

	public (int chunkIndex, int indexInChunk) GetIndex(int position)
	{
		int indexInBucket;
		return (chunkIndex: position.GetPowerOfTwoBucketIndex(FirstBucketLengthExponent, MaxChunkSize, MaxChunkExponent, out indexInBucket), indexInChunk: indexInBucket);
	}

	public ExponentialMemory<T> Expand(int minSize, bool initializeToZeros = false)
	{
		if (IsEmpty)
		{
			if (minSize == 0)
			{
				return AddMemory(MinimalSize, initializeToZeros);
			}
			if (minSize <= MaxChunkSize)
			{
				return AddMemory(minSize.GetPowerOfTwoGreaterOfEqual(), initializeToZeros);
			}
			ExponentialMemory<T> result = this;
			for (int i = 0; i < minSize.GetNumberOfBuckets(MaxChunkSize); i++)
			{
				result = result.AddMemory(MaxChunkSize, initializeToZeros);
			}
			return result;
		}
		int num = Math.Min(MaxChunkSize, Math.Max(MinimalSize, LastAllocatedBucket.Length * 2));
		if (minSize == 0 || minSize <= AllocatedLength + num)
		{
			return AddMemory(num, initializeToZeros);
		}
		int num2 = minSize - AllocatedLength;
		ExponentialMemory<T> result2 = this;
		while (num2 > 0)
		{
			result2 = result2.AddMemory(num, initializeToZeros);
			num2 -= num;
			num = Math.Min(MaxChunkSize, Math.Max(MinimalSize, num * 2));
		}
		return result2;
	}

	public ExponentialMemory<T> AddMemory(int size, bool initializeToZero)
	{
		return AddMemory(MemoryPool<T>.ThreadShared.GetMemory(size, initializeToZero));
	}

	public ExponentialMemory<T> AddMemory(Memory<T> memory)
	{
		if (AllocatedBucketCount == 0)
		{
			return new ExponentialMemory<T>(memory, OtherBucketBuffer, 1);
		}
		Memory<Memory<T>> memory2 = OtherBucketBuffer;
		if (OtherBucketBuffer.Length == AllocatedBucketCount - 1)
		{
			memory2 = memory2.ReplaceFromPool(Math.Max(1, Math.Min(OtherBucketBuffer.Length * 2, MemoryPool<Memory<T>>.ChunkSize)));
		}
		memory2.Span[AllocatedBucketCount - 1] = memory;
		return new ExponentialMemory<T>(FirstBucketBuffer, memory2, AllocatedBucketCount + 1);
	}

	public void ReturnToThreadMemoryPool()
	{
		for (int i = 0; i < AllocatedBucketCount; i++)
		{
			GetBucketAt(i).ReturnToThreadMemoryPool();
		}
	}

	public ExponentialMemory<T> Clone()
	{
		if (AllocatedBucketCount == 0)
		{
			return this;
		}
		Memory<Memory<T>> otherBucketBuffer;
		if (AllocatedBucketCount == 1)
		{
			Memory<T> firstBucketBuffer = FirstBucketBuffer.CloneFromPool();
			otherBucketBuffer = default(Memory<Memory<T>>);
			return new ExponentialMemory<T>(firstBucketBuffer, otherBucketBuffer, 1);
		}
		ArrayPoolMap<Memory<T>> threadShared = ArrayPool<Memory<T>>.ThreadShared;
		otherBucketBuffer = OtherBucketBuffer;
		SpanList<Memory<T>> spanList = new SpanList<Memory<T>>(threadShared.GetArray(otherBucketBuffer.Length));
		otherBucketBuffer = OtherBucketBuffer;
		Span<Memory<T>> span = otherBucketBuffer.Span;
		int num = 0;
		while (true)
		{
			int num2 = num;
			otherBucketBuffer = OtherBucketBuffer;
			if (num2 >= otherBucketBuffer.Length)
			{
				break;
			}
			Memory<T> item = span[num].CloneFromPool();
			spanList.Add(item);
			num++;
		}
		return new ExponentialMemory<T>(FirstBucketBuffer.CloneFromPool(), spanList.ToMemoryFromPool(), AllocatedBucketCount);
	}

	public ExponentialMemory<T> CopyFrom(ExponentialMemory<T> source, int destinationStartPosition, int length)
	{
		if (source.AllocatedLength < length)
		{
			throw new InvalidOperationException($"Given chunked memory is shorten (other.length={source.AllocatedLength}) than length to copy ({length}).");
		}
		int targetLength = length + destinationStartPosition;
		ExponentialMemory<T> result = ExpandUntil(targetLength);
		int num = length;
		int num2 = destinationStartPosition;
		for (int i = 0; i < source.AllocatedBucketCount; i++)
		{
			Memory<T> bucketAt = source.GetBucketAt(i);
			int num3 = Math.Min(bucketAt.Length, num);
			result.CopyItemsFrom(bucketAt.Span.GetHead(num3), num2);
			num2 += num3;
			num -= num3;
			if (num == 0)
			{
				return result;
			}
		}
		throw new InvalidOperationException("Should not reach here");
	}

	public ExponentialMemory<T> CopyFromSpan(Span<T> span, int startPosition)
	{
		ExponentialMemory<T> result = ExpandUntil(startPosition + span.Length);
		result.CopyItemsFrom(span, startPosition);
		return result;
	}

	private void CopyItemsFrom(Span<T> items, int destinationStartPosition)
	{
		(int chunkIndex, int indexInChunk) index = GetIndex(destinationStartPosition);
		int item = index.chunkIndex;
		int item2 = index.indexInChunk;
		Memory<T> bucketAt = GetBucketAt(item);
		int num = Math.Min(items.Length, bucketAt.Length - item2);
		items.GetHead(num).CopyTo(bucketAt.Span.CutHead(item2));
		int num2 = items.Length - num;
		if (num2 > 0)
		{
			CopyItemsFrom(items.Slice(num, num2), destinationStartPosition + num);
		}
	}

	public ExponentialMemory<T> ExpandUntil(int targetLength)
	{
		if (targetLength < AllocatedLength)
		{
			return this;
		}
		return Expand(targetLength);
	}
}
