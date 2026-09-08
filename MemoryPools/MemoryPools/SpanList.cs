using System;

namespace MemoryPools;

public ref struct SpanList<T>
{
	public ref struct Enumerator(SpanList<T> list)
	{
		private readonly SpanList<T> _list = list;

		private int _index = -1;

		public T Current => _list[_index];

		public bool MoveNext()
		{
			_index++;
			return _index < _list.Count;
		}
	}

	private Span<T> _buffer;

	public int Count { get; private set; }

	public Span<T> AsSpan => this;

	public T this[int index] => _buffer[index];

	public void Add(T item)
	{
		if (Count >= _buffer.Length)
		{
			int size = Math.Max(1, _buffer.Length * 2);
			Span<T> buffer = _buffer;
			_buffer = ArrayPool<T>.ThreadShared.GetArray(size).AsSpan();
			buffer.CopyTo(_buffer);
		}
		_buffer[Count] = item;
		Count++;
	}

	public SpanList(Span<T> buffer)
	{
		Count = 0;
		_buffer = buffer;
	}

	public SpanList(Span<T> buffer, int count)
	{
		_buffer = buffer;
		Count = count;
	}

	public static implicit operator SpanList<T>(Span<T> span)
	{
		return new SpanList<T>(span);
	}

	public static implicit operator Span<T>(SpanList<T> span)
	{
		return span._buffer.Slice(0, span.Count);
	}

	public ref T GetRefAt(int index)
	{
		return ref _buffer[index];
	}

	public Memory<T> ToMemoryFromPool()
	{
		return AsSpan.ToMemoryFromPool();
	}

	public void Clear()
	{
		Count = 0;
	}

	public void AddIfNotContains(T item)
	{
		if (!Contains(item))
		{
			Add(item);
		}
	}

	public bool Contains(T item)
	{
		for (int i = 0; i < Count; i++)
		{
			if (object.Equals(_buffer[i], item))
			{
				return true;
			}
		}
		return false;
	}

	public SpanList<T> Add(ReadOnlySpan<T> items)
	{
		if (items.Length <= _buffer.Length - Count)
		{
			items.CopyTo(_buffer.Slice(Count, items.Length));
			return new SpanList<T>(_buffer, Count + items.Length);
		}
		throw new NotImplementedException();
	}

	public T[] ToArray()
	{
		T[] array = ArrayPool<T>.ThreadShared.GetArray(Count);
		_buffer.Slice(0, Count).CopyTo(array);
		return array;
	}

	public readonly Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
}
