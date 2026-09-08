using BinarySerializer;

using MemoryPools;

using System.Text.Json.Serialization;

namespace Poker.Calc;

[BinarySerializable]
public class Board : IEquatable<Board>, IBoard
{
	public const int FirstBoardNumber = 1;

	public static Board Preflop = new Board(default(InlineList<Card>));

	[Tag(1)]
	public InlineList<Card> Cards { get; private set; }

	public Streets Street { get; private set; }

	public bool IsPreflop => Street == Streets.Preflop;

	public bool IsRiver => Street == Streets.River;

	[JsonIgnore]
	public InlineList<Card> LastStreetCards
	{
		get
		{
			if (Street != Streets.Preflop)
			{
				if (Street != Streets.Flop)
				{
					return Cards.Last.ToSingleInlineList();
				}
				return Cards;
			}
			return default(InlineList<Card>);
		}
	}

	public InlineList<Card> FlopCards
	{
		get
		{
			if (Street < Streets.Flop)
			{
				return default(InlineList<Card>);
			}
			return Cards.Take<Card>(3);
		}
	}

	[JsonConstructor]
	public Board(InlineList<Card> cards)
	{
		Cards = cards;
		Street = cards.Count.CardsCountToStreet();
	}

	[BinaryDeserializationConstructor]
	public static Board Create(InlineList<Card> cards)
	{
		Board board = ObjectPool<Board>.ThreadShared.RentObject();
		board.Cards = cards;
		board.Street = cards.Length.CardsCountToStreet();
		return board;
	}

	public Board()
	{
	}

	public override string ToString()
	{
		return Cards.ToAbbreviation();
	}

	public bool Equals(Board? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return Cards.SameCards(other.Cards);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((Board)obj);
	}

	public override int GetHashCode()
	{
		return (Cards.GetHashCode() * 397) ^ (int)Street;
	}
}
