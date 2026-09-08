using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemoryPools;

public readonly struct MemoryList<T> : IEquatable<MemoryList<T>>
{
	public struct Enumerator(MemoryList<T> items) : IEnumerator
	{
		private readonly MemoryList<T> _items = items;

		private int _index = -1;

		public T Current => _items[_index];

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			_index++;
			return _index < _items.Count;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}

	public ExponentialMemory<T> Buffer { get; }

	public int Count { get; }

	public int FreeItemCount => Buffer.AllocatedLength - Count;

	public bool IsEmpty => Count == 0;

	public bool IsNotEmpty => Count != 0;

	public T this[int index] => Buffer[index];

	public IEnumerable<T> AsEnumerable => Buffer.AsEnumerable.Take(Count);

	public MemoryList(ExponentialMemory<T> buffer, int count)
	{
		Buffer = buffer;
		Count = count;
	}

	public MemoryList(int capacity)
	{
		this = new MemoryList<T>(new ExponentialMemory<T>().Expand(capacity), 0);
	}

	public MemoryList(Span<T> items)
	{
		Buffer = new ExponentialMemory<T>().Expand(items.Length);
		items.CopyTo(Buffer.FirstAllocatedBucket.Span);
		Count = items.Length;
	}

	public MemoryList<T> Clone()
	{
		return new MemoryList<T>(Buffer.Clone(), Count);
	}

	public MemoryList<T> Expand()
	{
		return new MemoryList<T>(Buffer.Expand(0), Count);
	}

	public MemoryList<T> Expand(int minSize)
	{
		return new MemoryList<T>(Buffer.Expand(minSize), Count);
	}

	public MemoryList<T> Clear()
	{
		return new MemoryList<T>(Buffer, 0);
	}

	public MemoryList<T> EnsureAllocated(int size)
	{
		if (Buffer.AllocatedLength < size)
		{
			return Expand(size);
		}
		return this;
	}

	public ref T GetRefAt(int index)
	{
		return ref Buffer.GetRefAt(index);
	}

	public MemoryList<T> Add(T item)
	{
		if (FreeItemCount == 0)
		{
			return Expand().Add(item);
		}
		Buffer.SetItem(Count, item);
		return new MemoryList<T>(Buffer, Count + 1);
	}

	public MemoryList<T> AddRange(Span<T> span)
	{
		if (span.Length == 0)
		{
			return this;
		}
		return new MemoryList<T>(Buffer.CopyFromSpan(span, Count), Count + span.Length);
	}

	public MemoryList<T> AddRange(MemoryList<T> others)
	{
		if (others.IsEmpty)
		{
			return this;
		}
		MemoryList<T> memoryList = this;
		return new MemoryList<T>(Buffer.CopyFrom(others.Buffer, Count, others.Count), memoryList.Count + others.Count);
	}

	public MemoryList<T> SetItem(int index, T value)
	{
		if (index >= Count)
		{
			throw new InvalidOperationException($"Index {index} is out of range (length={Count})");
		}
		Buffer.SetItem(index, value);
		return this;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public bool Equals(MemoryList<T> other)
	{
		if (other.Count == 0 && Count == 0)
		{
			return true;
		}
		return false;
	}

	public void ReturnToThreadMemoryPool()
	{
		Buffer.ReturnToThreadMemoryPool();
	}

	public MemoryList<T> VerifyBufferLength()
	{
		if (FreeItemCount < 0)
		{
			throw new InvalidOperationException($"Free item count can't be negative but was {FreeItemCount}");
		}
		return this;
	}
}
