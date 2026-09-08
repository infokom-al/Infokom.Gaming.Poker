using BinarySerializer;

using CSharpSerializer.Serialization;

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[DebuggerDisplay("{DebugView}")]
[BinarySerializable]
[JsonSerializeAsCollection]
public struct StreetMap<T> : IEquatable<StreetMap<T>>
{
	public struct Enumerator : IEnumerator
	{
		private StreetMap<T> _streets;

		public int _currentStreetIndex;

		public Streets _currentStreet => (Streets)_currentStreetIndex;

		public (Streets street, T value) Current => (street: _currentStreet, value: _streets.Get(_currentStreet));

		object IEnumerator.Current => Current;

		public Enumerator(StreetMap<T> streets)
		{
			_currentStreetIndex = -1;
			_streets = streets;
		}

		public bool MoveNext()
		{
			_currentStreetIndex++;
			if (_streets.Contains(_currentStreet))
			{
				return true;
			}
			if (_currentStreet >= Streets.River)
			{
				return false;
			}
			return MoveNext();
		}

		public void Reset()
		{
			_currentStreetIndex = -1;
		}
	}

	[Tag(1)]
	public Items4<T> InnerStreets;

	[Tag(2)]
	public int StreetNumbers;

	public int Length => Count;

	public int Count => StreetNumbers.BitsCount();

	public bool IsEmpty => Count == 0;

	public bool HasPreflop => Contains(Streets.Preflop);

	public T First => Get(FromBitNumber(StreetNumbers.GetLowestBitNumber()));

	public T Last => Get(LastStreet);

	public Streets LastStreet => FromBitNumber(StreetNumbers.GetHighestBitNumber());

	public InlineList<T> Values => this;

	public bool IsFirstStreetPreflop => HasPreflop;

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public List<(Streets street, T value)> DebugView => GetDebugView();

	public StreetMap(Items4<T> innerStreets, int streetNumbers)
	{
		InnerStreets = innerStreets;
		StreetNumbers = streetNumbers;
	}

	[JsonConstructor]
	public StreetMap(IEnumerable<(Streets street, T value)> streets)
	{
		InnerStreets = default(Items4<T>);
		StreetNumbers = 0;
		foreach (var (street, value) in streets)
		{
			Add(street, value);
		}
	}

	public bool Contains(Streets street)
	{
		return StreetNumbers.ContainsBit(street.GetStreetNumber());
	}

	public T Get(Streets street)
	{
		if (!TryGet(street, out var result))
		{
			throw new InvalidOperationException($"Street {street} not found");
		}
		return result;
	}

	public bool TryGetPreflopStreet(out T result)
	{
		return TryGet(Streets.Preflop, out result);
	}

	public bool TryGet(Streets street, out T result)
	{
		if (Contains(street))
		{
			result = InnerStreets[(int)street];
			return true;
		}
		result = default(T);
		return false;
	}

	public void Add(Streets street, T value)
	{
		InnerStreets[(int)street] = value;
		StreetNumbers = StreetNumbers.SetBit(street.GetStreetNumber());
	}

	public StreetMap<T> With<THasStreet>(THasStreet value) where THasStreet : T, IHasStreet
	{
		StreetMap<T> result = this;
		result.Add<THasStreet>(value);
		return result;
	}

	public void Add<THasStreet>(THasStreet value) where THasStreet : T, IHasStreet
	{
		Add(value.Street, (T)(object)value);
	}

	public void Remove(Streets street)
	{
		StreetNumbers = StreetNumbers.RemoveBit(street.GetStreetNumber());
		InnerStreets[(int)street] = default(T);
	}

	public static implicit operator InlineList<T>(StreetMap<T> streets)
	{
		InlineList<T> result = default(InlineList<T>);
		Enumerator enumerator = streets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T item = enumerator.Current.value;
			result.Add(item);
		}
		return result;
	}

	private static int GetBitNumber(Streets street)
	{
		return (int)(street + 1);
	}

	private static Streets FromBitNumber(int bitNumber)
	{
		return (Streets)(bitNumber - 1);
	}

	public readonly Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public List<(Streets Streets, T value)> GetDebugView()
	{
		List<(Streets, T)> list = new List<(Streets, T)>();
		Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (item, item2) = enumerator.Current;
			list.Add((item, item2));
		}
		return list;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is StreetMap<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(StreetMap<T> other)
	{
		if (Count == 0 && other.Count == 0)
		{
			return true;
		}
		if (Count != other.Count)
		{
			return false;
		}
		return Values.SequenceEqual(other.Values);
	}
}
