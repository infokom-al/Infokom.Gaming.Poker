using BinarySerializer;

using CSharpSerializer.Serialization;

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[DebuggerDisplay("{DebugView}")]
[BinarySerializable]
[JsonSerializeAsCollection]
public struct InlineList<T> : IEquatable<InlineList<T>>
{
	public struct Enumerator(InlineList<T> list) : IEnumerator
	{
		private readonly InlineList<T> _list = list;

		private int _index = -1;

		public T Current => _list[_index];

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			_index++;
			return _index < _list.Count;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}

	[Tag(1)]
	public Items10<T> Items;

	[Tag(2)]
	public int Count { get; private set; }

	public int Length => Count;

	[JsonIgnore]
	public T First
	{
		get
		{
			if (Count <= 0)
			{
				throw new InvalidOperationException("The list is empty");
			}
			return this[0];
		}
	}

	public static InlineList<T> Empty => default(InlineList<T>);

	[JsonIgnore]
	public bool IsNotEmpty => Count != 0;

	[JsonIgnore]
	public bool IsEmpty => Count == 0;

	[JsonIgnore]
	public T this[int index]
	{
		get
		{
			Span<T> span = Items;
			if (index >= Count)
			{
				throw new InvalidOperationException($"Index ({index}) was beyond the list bounds (count={Count})");
			}
			return span[index];
		}
		set
		{
			Span<T> span = Items;
			if (index >= Count)
			{
				throw new InvalidOperationException($"Index ({index}) was beyond the list bounds (count={Count})");
			}
			span[index] = value;
		}
	}

	[JsonIgnore]
	public T Last
	{
		get
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("List is empty");
			}
			return Items[Count - 1];
		}
	}

	[JsonIgnore]
	public T BeforeLast
	{
		get
		{
			if (Count <= 1)
			{
				throw new InvalidOperationException("List doesn't contain the before last element");
			}
			return Items[Count - 2];
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public List<T> DebugView => GetDebugView();

	public InlineList(Items10<T> items, int count)
	{
		Items = items;
		Count = count;
	}

	public InlineList(int initialSize)
	{
		Items = default(Items10<T>);
		if (initialSize > 10)
		{
			throw new InvalidOperationException($"Maximimum seats length can be 10 but was {initialSize}");
		}
		Count = initialSize;
	}

	[JsonConstructor]
	public InlineList(IEnumerable<T> items)
	{
		Items = default(Items10<T>);
		Count = 0;
		foreach (T item in items)
		{
			Add(item);
		}
	}

	public InlineList<T> With(T seat)
	{
		InlineList<T> result = this;
		result.Items[Count] = seat;
		result.Count++;
		return result;
	}

	public void Add(T seat)
	{
		Items[Count] = seat;
		Count++;
	}

	public void AddRange(IEnumerable<T> items)
	{
		foreach (T item in items)
		{
			Add(item);
		}
	}

	public void Clear()
	{
		Count = 0;
	}

	public static InlineList<T> CreateFilledWithDefaultValues(int count = 10)
	{
		InlineList<T> result = default(InlineList<T>);
		for (int i = 0; i < count; i++)
		{
			result.Add(default(T));
		}
		return result;
	}

	public readonly Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public Span<T> AsSpan()
	{
		return MemoryMarshal.CreateSpan(ref Unsafe.As<Items10<T>, T>(ref Items), Count);
	}

	public static implicit operator Span<T>(InlineList<T> list)
	{
		return list.AsSpan();
	}

	public List<T> GetDebugView()
	{
		List<T> list = new List<T>();
		Span<T> span = AsSpan();
		for (int i = 0; i < span.Length; i++)
		{
			T item = span[i];
			list.Add(item);
		}
		return list;
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		Span<T> span = AsSpan();
		for (int i = 0; i < span.Length; i++)
		{
			T value = span[i];
			hashCode.Add(value);
		}
		return hashCode.ToHashCode();
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is InlineList<T> other))
		{
			return false;
		}
		return Equals(other);
	}

	public bool Equals(InlineList<T> other)
	{
		if (other.Count != Count)
		{
			return false;
		}
		if (Count == 0)
		{
			return true;
		}
		Span<T> span = AsSpan();
		Span<T> span2 = other.AsSpan();
		for (int i = 0; i < span.Length; i++)
		{
			if (!span[i].Equals(span2[i]))
			{
				return false;
			}
		}
		return true;
	}
}
