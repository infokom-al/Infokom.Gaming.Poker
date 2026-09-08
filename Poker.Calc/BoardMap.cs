using BinarySerializer;

using CSharpSerializer.Serialization;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Poker.Calc;

[BinarySerializable]
[JsonSerializeAsCollection]
public struct BoardMap<T> : IEquatable<BoardMap<T>>
{
	public struct Enumerator(BoardMap<T> boards) : IEnumerator
	{
		public BoardMap<T> _boards = boards;

		public int _currentBoardNumber = 0;

		public (int boardNumber, T value) Current => (boardNumber: _currentBoardNumber, value: _boards.Get(_currentBoardNumber));

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			_currentBoardNumber++;
			if (_currentBoardNumber > 10)
			{
				return false;
			}
			if (_boards.Contains(_currentBoardNumber))
			{
				return true;
			}
			return MoveNext();
		}

		public void Reset()
		{
			_currentBoardNumber = 0;
		}
	}

	[Tag(1)]
	public Items10<T> InnerValues;

	[Tag(2)]
	public int BoardNumbers { get; private set; }

	public int Count => BoardNumbers.BitsCount();

	public int Length => Count;

	public bool IsEmpty => Length == 0;

	public T First => Get(1);

	public T Last => Get(LastBoardNumber);

	public int FirstBoardNumber => BoardNumbers.GetLowestBitNumber();

	public int LastBoardNumber => BoardNumbers.GetHighestBitNumber();

	public InlineList<T> Values => BoardNumbers.GetBitNumbers().Map((int boardNumber, BoardMap<T> boardMap) => boardMap.Get(boardNumber), this);

	public static BoardMap<T> Empty => default(BoardMap<T>);

	public BoardMap(Items10<T> innerValues, int boardNumbers)
	{
		InnerValues = innerValues;
		BoardNumbers = boardNumbers;
	}

	[JsonConstructor]
	public BoardMap(IEnumerable<(int boardNumber, T value)> boards)
	{
		InnerValues = default(Items10<T>);
		BoardNumbers = 0;
		foreach (var (boardNumber, value) in boards)
		{
			Set(boardNumber, value);
		}
	}

	public void Set(int boardNumber, T value)
	{
		InnerValues[boardNumber - 1] = value;
		BoardNumbers |= 1 << boardNumber - 1;
	}

	public bool Contains(int boardNumber)
	{
		return BoardNumbers.ContainsBit(boardNumber);
	}

	public T Get(int boardNumber)
	{
		if (!TryGet(boardNumber.VerifyArgumentPositive("boardNumber"), out var result))
		{
			throw new InvalidOperationException($"Failed to get value for the boardNumber = {boardNumber}");
		}
		return result;
	}

	public bool TryGet(int boardNumber, out T result)
	{
		if (Contains(boardNumber))
		{
			result = InnerValues[boardNumber - 1];
			return true;
		}
		result = default(T);
		return false;
	}

	public BoardMap<T> With(int boardNumber, T value)
	{
		BoardMap<T> result = this;
		result.Set(boardNumber, value);
		return result;
	}

	public BoardMap<T> Remove(int boardNumber)
	{
		if (!Contains(boardNumber))
		{
			return this;
		}
		InnerValues[boardNumber - 1] = default(T);
		BoardNumbers &= ~(1 << boardNumber - 1);
		return this;
	}

	public readonly Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BoardMap<T>)obj);
	}

	public bool Equals(BoardMap<T> other)
	{
		if (BoardNumbers == other.BoardNumbers)
		{
			return Values.SequenceEqual(other.Values);
		}
		return false;
	}
}
