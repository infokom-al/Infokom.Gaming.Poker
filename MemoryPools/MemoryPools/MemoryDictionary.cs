using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemoryPools;

public struct MemoryDictionary<TKey, TValue> : IEquatable<MemoryDictionary<TKey, TValue>> where TKey : IEquatable<TKey>
{
	public struct Entry
	{
		public uint HashCode { get; set; }

		public int Next { get; set; }

		public TKey Key { get; set; }

		public TValue Value { get; set; }

		public Entry(uint hashCode, int next, TKey key, TValue value)
		{
			HashCode = hashCode;
			Next = next;
			Key = key;
			Value = value;
		}
	}

	public struct Enumerator(MemoryDictionary<TKey, TValue> dictionary) : IEnumerator
	{
		private readonly MemoryDictionary<TKey, TValue> _dictionary = dictionary;

		private int _cursor = -1;

		public (TKey Key, TValue Value) Current
		{
			get
			{
				ref Entry refAt = ref _dictionary.Entries.GetRefAt(_cursor);
				return (Key: refAt.Key, Value: refAt.Value);
			}
		}

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			_cursor++;
			return _cursor < _dictionary.Count;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}

	private const uint CollisionThreshold = 200u;

	public static int[] ZeroBytes;

	public MemoryList<int> Buckets { get; }

	public MemoryList<Entry> Entries { get; }

	public int Count => Entries.Count;

	public bool IsEmpty => Count == 0;

	public IEnumerable<TKey> Keys => AsEnumerable.Select(((TKey Key, TValue Value) item) => item.Key);

	public IEnumerable<TValue> Values => AsEnumerable.Select(((TKey Key, TValue Value) item) => item.Value);

	public IEnumerable<(TKey Key, TValue Value)> AsEnumerable
	{
		get
		{
			Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (item, item2) = enumerator.Current;
				yield return (Key: item, Value: item2);
			}
		}
	}

	public MemoryDictionary(MemoryList<int> buckets, MemoryList<Entry> entries)
	{
		Buckets = buckets;
		Entries = entries;
	}

	public MemoryDictionary()
	{
		Buckets = default(MemoryList<int>);
		Entries = default(MemoryList<Entry>);
	}

	public MemoryDictionary(int capacity)
	{
		int greaterOrEqualPrime = capacity.GetGreaterOrEqualPrime();
		Buckets = new MemoryList<int>(new ExponentialMemory<int>().Expand(greaterOrEqualPrime, initializeToZeros: true), greaterOrEqualPrime);
		Entries = new MemoryList<Entry>(greaterOrEqualPrime);
	}

	private ref int GetBucket(uint hashCode)
	{
		return ref Buckets.GetRefAt((int)(hashCode % Buckets.Count));
	}

	public bool ContainsKey(TKey key)
	{
		if (IsEmpty)
		{
			return false;
		}
		TValue result;
		return TryGetValue(key, out result);
	}

	public MemoryDictionary<TKey, TValue> Add(TKey key, TValue value)
	{
		MemoryDictionary<TKey, TValue> result = this;
		if (Count == Buckets.Count)
		{
			result = result.Resize();
		}
		uint hashCode = (uint)key.GetHashCode();
		ref int bucket = ref result.GetBucket(hashCode);
		int num = bucket - 1;
		int count = Count;
		result = new MemoryDictionary<TKey, TValue>(entries: result.Entries.Add(new Entry(hashCode, (num < Count) ? num : (-1), key, value)), buckets: result.Buckets);
		bucket = count + 1;
		return result;
	}

	public MemoryDictionary<TKey, TValue> SetItem(TKey key, TValue value)
	{
		if (IsEmpty)
		{
			return Add(key, value);
		}
		uint hashCode = (uint)key.GetHashCode();
		ref int bucket = ref GetBucket(hashCode);
		int num = bucket - 1;
		uint num2 = 0u;
		if (bucket != 0)
		{
			while ((uint)num < (uint)Entries.Count)
			{
				if (Entries[num].HashCode == hashCode && Entries[num].Key.Equals(key))
				{
					Entries.GetRefAt(num).Value = value;
					return this;
				}
				num = Entries[num].Next;
				num2++;
				VerifyCollisionCount(num2);
			}
		}
		return Add(key, value);
	}

	public TValue GetValue(TKey key)
	{
		if (!TryGetValue(key, out var result))
		{
			throw new InvalidOperationException("Value with the key " + key.Quoted() + " not found)");
		}
		return result;
	}

	public bool TryGetValue(TKey key, out TValue result)
	{
		result = default(TValue);
		if (IsEmpty)
		{
			return false;
		}
		uint hashCode = (uint)key.GetHashCode();
		ref int bucket = ref GetBucket(hashCode);
		if (bucket == 0)
		{
			return false;
		}
		int num = bucket - 1;
		uint num2 = 0u;
		MemoryList<Entry> entries = Entries;
		while ((uint)num < (uint)Entries.Count)
		{
			if (entries[num].HashCode == hashCode && entries[num].Key.Equals(key))
			{
				result = entries[num].Value;
				return true;
			}
			num = entries[num].Next;
			num2++;
			VerifyCollisionCount(num2);
		}
		return false;
	}

	private void VerifyCollisionCount(uint collisionCount)
	{
		if (collisionCount > (uint)Entries.Count)
		{
			throw new InvalidOperationException($"Collision count ({collisionCount}) can't be more than entries length ({Entries.Count}).");
		}
		if (collisionCount == 200)
		{
			throw new NotImplementedException($"Handling case with more than {200u} collisions isn't implemented");
		}
	}

	private MemoryDictionary<TKey, TValue> Resize(int minSize = 0)
	{
		int greaterOrEqualPrime = Math.Max(minSize, Math.Max(1, Buckets.Count * 2)).GetGreaterOrEqualPrime();
		int value = greaterOrEqualPrime - Buckets.Count;
		MemoryList<int> buckets = Buckets.EnsureAllocated(greaterOrEqualPrime).Clear();
		int num = greaterOrEqualPrime;
		for (int i = 0; i < value.GetNumberOfBuckets(4096); i++)
		{
			buckets = buckets.AddRange(ZeroBytes.AsSpan().Slice(0, Math.Min(4096, num)));
			num -= 4096;
		}
		MemoryDictionary<TKey, TValue> memoryDictionary = new MemoryDictionary<TKey, TValue>(buckets, Entries.EnsureAllocated(greaterOrEqualPrime));
		for (int j = 0; j < Count; j++)
		{
			if (memoryDictionary.Entries[j].Next >= -1)
			{
				ref int bucket = ref memoryDictionary.GetBucket(memoryDictionary.Entries[j].HashCode);
				if (bucket - 1 == j)
				{
					throw new InvalidOperationException("Updated next item index (" + (bucket - 1).Quoted() + ") refers to the same item in the entries.");
				}
				memoryDictionary.Entries.GetRefAt(j).Next = bucket - 1;
				bucket = j + 1;
			}
		}
		return new MemoryDictionary<TKey, TValue>(buckets, memoryDictionary.Entries);
	}

	public MemoryDictionary<TKey, TValue> Clone()
	{
		return new MemoryDictionary<TKey, TValue>(Buckets.Clone(), Entries.Clone());
	}

	public readonly Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public bool Equals(MemoryDictionary<TKey, TValue> other)
	{
		if (Count == 0)
		{
			return other.Count == 0;
		}
		return false;
	}

	static MemoryDictionary()
	{
		ZeroBytes = new int[4096];
	}
}
