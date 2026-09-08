using System;

namespace MemoryPools;

public readonly struct MemoryBufferCursor<T>
{
	public static MemoryBufferCursor<T> Empty = new MemoryBufferCursor<T>(Memory<T>.Empty, 0);

	public Memory<T> Buffer { get; }

	public int Cursor { get; }

	public int FreeItemCount => Buffer.Length - Cursor;

	public T this[int index] => Buffer.Span[index];

	public MemoryBufferCursor(Memory<T> buffer, int cursor)
	{
		Buffer = buffer;
		Cursor = cursor;
	}

	public MemoryBufferCursor<T> AddItem(T item)
	{
		if (FreeItemCount == 0)
		{
			return Expand().AddItem(item);
		}
		Buffer.Span[Cursor] = item;
		return new MemoryBufferCursor<T>(Buffer, Cursor + 1);
	}

	public MemoryBufferCursor<T> SetItem(int index, T item)
	{
		if (index > Cursor)
		{
			throw new ArgumentOutOfRangeException($"Index was {index} but the cursor was {Cursor}");
		}
		Buffer.Span[index] = item;
		return this;
	}

	private MemoryBufferCursor<T> Expand()
	{
		return new MemoryBufferCursor<T>(Buffer.ReplaceFromPool(Math.Max(1, Buffer.Length * 2)), Cursor);
	}
}
