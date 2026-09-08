using BinarySerializer;

namespace Poker.Calc;

[BinarySerializable]
public readonly struct PreflopRangeHoldemCompact
{
	[Tag(1)]
	public long BitsOne { get; }

	[Tag(2)]
	public long BitsTwo { get; }

	[Tag(3)]
	public long BitsThree { get; }

	public bool IsEmpty
	{
		get
		{
			if (BitsOne == 0L && BitsTwo == 0L)
			{
				return BitsThree == 0;
			}
			return false;
		}
	}

	public bool IsNotEmpty => !IsEmpty;

	public static PreflopRangeHoldemCompact Empty => new PreflopRangeHoldemCompact(0L, 0L, 0L);

	public PreflopRangeHoldemCompact(long bitsOne, long bitsTwo, long bitsThree)
	{
		BitsOne = bitsOne;
		BitsTwo = bitsTwo;
		BitsThree = bitsThree;
	}

	public bool Intersects(PreflopRangeHoldemCompact other)
	{
		if (!BitsOne.Intersects(other.BitsOne) && !BitsTwo.Intersects(other.BitsTwo))
		{
			return BitsThree.Intersects(other.BitsThree);
		}
		return true;
	}

	public bool ContainsCell(int cellIndex)
	{
		return GetBits(GetBitNumber(cellIndex)).GetFlag(GetBitOffset(cellIndex));
	}

	public int GetBitNumber(int cellIndex)
	{
		return cellIndex / 64 + 1;
	}

	public int GetBitOffset(int cellIndex)
	{
		return cellIndex % 64;
	}

	public IEnumerable<int> GetCellIndices(PokerGames game)
	{
		if (game == PokerGames.TexasHoldem)
		{
			for (int cellIndex = 0; cellIndex < 169; cellIndex++)
			{
				if (ContainsCell(cellIndex))
				{
					yield return cellIndex;
				}
			}
		}
		else
		{
			if (!game.IsShortDeckFamily())
			{
				yield break;
			}
			for (int cellIndex = 0; cellIndex < 169; cellIndex++)
			{
				if (ContainsCell(cellIndex) && cellIndex.IsShortDeckHoldemCellIndex())
				{
					yield return cellIndex;
				}
			}
		}
	}

	public PreflopRangeHoldemCompact Merge(PreflopRangeHoldemCompact other)
	{
		return new PreflopRangeHoldemCompact(BitsOne | other.BitsOne, BitsTwo | other.BitsTwo, BitsThree | other.BitsThree);
	}

	public PreflopRangeHoldemCompact AddCell(int cellIndex)
	{
		int bitNumber = GetBitNumber(cellIndex);
		int bitOffset = GetBitOffset(cellIndex);
		return bitNumber switch
		{
			1 => new PreflopRangeHoldemCompact(BitsOne.SetBit(bitOffset), BitsTwo, BitsThree),
			2 => new PreflopRangeHoldemCompact(BitsOne, BitsTwo.SetBit(bitOffset), BitsThree),
			3 => new PreflopRangeHoldemCompact(BitsOne, BitsTwo, BitsThree.SetBit(bitOffset)),
			_ => throw new InvalidOperationException("Wrong cell index " + cellIndex.Quoted()),
		};
	}

	private long GetBits(int bitNumber)
	{
		return bitNumber switch
		{
			1 => BitsOne,
			2 => BitsTwo,
			3 => BitsThree,
			_ => throw new InvalidOperationException("bitNumber was out of range (" + bitNumber.Quoted() + ")"),
		};
	}
}
