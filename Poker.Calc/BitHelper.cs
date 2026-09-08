namespace Poker.Calc;

public static class BitHelper 
{
	public static int BitsCount(this int value)
	{
		return Tables.BitsCountTable[value];
	}

	public static int BitsCountInt(this int value)
	{
		return Tables.BitsCountTable[value & 0xFF] + Tables.BitsCountTable[(value >> 8) & 0xFF] + Tables.BitsCountTable[(value >> 16) & 0xFF] + Tables.BitsCountTable[(value >> 24) & 0xFF];
	}

	public static int BitsCount(this ushort value)
	{
		return Tables.BitsCountTable[value];
	}

	public static int GetTopFiveRanks(this int ranksMask)
	{
		return Tables.TopFiveCardsTable[ranksMask];
	}

	public static int GetTopRank(this int ranksMask)
	{
		return Tables.TopCardRankTable[ranksMask];
	}

	public static int GetTopRankMask(this int ranksMask)
	{
		return Tables.TopRanksMaskTable[ranksMask];
	}

	internal static int GetTopTwoRanksMask(this int ranksMask)
	{
		return Tables.TopTwoRanksMaskTable[ranksMask];
	}

	public static int GetLowestBitNumber(this int value)
	{
		if (value == 0)
		{
			return 0;
		}
		if (value < 1024 && value > 0)
		{
			return Tables.LowestBitTable[value];
		}
		return value.GetLowestBitNumberSlow();
	}

	public static int GetHighestBitNumber(this int value)
	{
		if (value == 0)
		{
			return 0;
		}
		if (value < 1024 && value > 0)
		{
			return Tables.HighestBitTable[value];
		}
		return value.GetHighestBitNumberSlow();
	}

	public static int GetLowestBitNumberSlow(this int value)
	{
		if (value == 0)
		{
			return 0;
		}
		for (int i = 0; i < 32; i++)
		{
			if ((value & (1 << i)) != 0)
			{
				return i + 1;
			}
		}
		throw new InvalidOperationException("Should not reach here");
	}

	public static int GetHighestBitNumberSlow(this int value)
	{
		if (value == 0)
		{
			return 0;
		}
		for (int i = 0; i < 32; i++)
		{
			if (value == 1 << i)
			{
				return i + 1;
			}
			if (value < 1 << i)
			{
				return i;
			}
		}
		throw new InvalidOperationException("Should not reach here");
	}

	public static bool ContainsBit(this int value, int bitNumber)
	{
		return (value & (1 << bitNumber - 1)) != 0;
	}

	public static InlineList<int> GetBitNumbers(this int value)
	{
		InlineList<int> result = default(InlineList<int>);
		for (int i = 0; i < 32; i++)
		{
			if ((value & (1 << i)) != 0)
			{
				result.Add(i + 1);
			}
			if (1 << i >= value)
			{
				return result;
			}
		}
		return result;
	}

	public static int SetBit(this int value, int bitNumber)
	{
		if (bitNumber <= 0)
		{
			throw new ArgumentException($"Bit number can't be {bitNumber}", "bitNumber");
		}
		return value | (1 << bitNumber - 1);
	}

	public static long SetBit(this long value, int offset)
	{
		if (offset < 0)
		{
			throw new ArgumentException($"Bit number can't be {offset}", "offset");
		}
		return value | (1L << offset);
	}

	public static int RemoveBit(this int value, int bitNumber)
	{
		if (bitNumber <= 0)
		{
			throw new ArgumentException($"Bit number can't be {bitNumber}", "bitNumber");
		}
		return value & ~(1 << bitNumber - 1);
	}

	public static bool GetFlag(this long bitData, int shift)
	{
		return (bitData & (1L << shift)) != 0;
	}

	public static bool Intersects(this long bits, long other)
	{
		return (bits & other) != 0;
	}
}
