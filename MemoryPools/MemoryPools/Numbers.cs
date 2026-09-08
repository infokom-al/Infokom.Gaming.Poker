using System;
using System.Globalization;
using System.Numerics;

namespace MemoryPools;

internal static class Numbers
{
	private const double DefaultDoubleDeviation = 1E-06;

	public static int[] PowerOfTwoBucketIndices = new int[1024]
	{
		0, 1, 1, 2, 2, 2, 2, 3, 3, 3,
		3, 3, 3, 3, 3, 4, 4, 4, 4, 4,
		4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
		4, 5, 5, 5, 5, 5, 5, 5, 5, 5,
		5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
		5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
		5, 5, 5, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
		7, 7, 7, 7, 7, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
		9, 9, 9, 10
	};

	public static bool IsLess(this double value, double other, double deviation = 1E-06)
	{
		return other - value > deviation;
	}

	public static bool IsGreater(this double value, double other, double deviation = 1E-06)
	{
		return value - other > deviation;
	}

	public static int GetPowerOfTwoNumberGreaterOrEqualOrZero(this int value)
	{
		if (value == 0)
		{
			return 0;
		}
		return value.GetPowerOfTwoGreaterOfEqual();
	}

	public static int GetPowerOfTwoGreaterOfEqual(this int value)
	{
		value.VerifyArgumentPositive("value");
		if (value <= 2)
		{
			return value;
		}
		for (int i = 2; i < 32; i++)
		{
			int num = 1 << i;
			if (num >= value)
			{
				return num;
			}
		}
		throw new InvalidOperationException($"Failed to get next power of two number for value = {value}");
	}

	public static int VerifyArgumentPositive(this int value, string argumentName)
	{
		if (value <= 0)
		{
			throw new ArgumentException($"Expecting positive value but was {value}", argumentName);
		}
		return value;
	}

	public static int VerifyIsPowerOfTwo(this int value, string? message = null)
	{
		if (!value.IsPowerOfTwo())
		{
			throw new InvalidOperationException(message ?? $"Expecting power of two but was {value}");
		}
		return value;
	}

	public static bool IsPowerOfTwo(this int x)
	{
		if (x != 0)
		{
			return (x & (x - 1)) == 0;
		}
		return false;
	}

	public static int VerifyIsPowerOfTwoOrZero(this int value, string? message = null)
	{
		if (value != 0 && !value.IsPowerOfTwo())
		{
			throw new InvalidOperationException(message ?? $"Expecting power of two but was {value}");
		}
		return value;
	}

	public static int Abs(this int value)
	{
		return Math.Abs(value);
	}

	public static int GetNumberOfBuckets(this int value, int bucketSize)
	{
		int num = value / bucketSize;
		if (value % bucketSize == 0)
		{
			return num;
		}
		return num + 1;
	}

	public static int VerifyInt(this long value)
	{
		if (value < int.MinValue || value > int.MaxValue)
		{
			throw new InvalidOperationException($"Value {value} is out of ushort range");
		}
		return (int)value;
	}

	public static string Shorten(this int value)
	{
		return ((long)value).Shorten();
	}

	public static string Shorten(this long value)
	{
		long num = Math.Abs(value);
		if (num < 1000000)
		{
			if (num >= 1000)
			{
				if (num < 10000)
				{
					return ((double)value / 1000.0).Rounded(1).ToString(CultureInfo.InvariantCulture) + "k";
				}
				return ((double)value / 1000.0).Rounded().ToString(CultureInfo.InvariantCulture) + "k";
			}
			return value.ToString(CultureInfo.InvariantCulture);
		}
		if (num < 10000000)
		{
			return ((double)value / 1000000.0).Rounded(1).ToString(CultureInfo.InvariantCulture) + "m";
		}
		return ((double)value / 1000000.0).Rounded().ToString(CultureInfo.InvariantCulture) + "m";
	}

	public static double Rounded(this double value, int digits = 0)
	{
		return Math.Round(value, digits);
	}

	public static int GetPowerOfTwoExponent(this int number)
	{
		return BitOperations.TrailingZeroCount(number);
	}

	public static int SumPowerOfTwo(int startExponent, int endExponent)
	{
		return (1 << endExponent + 1) - (1 << startExponent);
	}

	public static int GetPowerOfTwoBucketLength(this int firstBucketLengthExponent, int bucketCount, int maxBucketSize, int maxBucketSizeExponent)
	{
		int num = firstBucketLengthExponent + bucketCount - 1;
		int endExponent = Math.Min(num, maxBucketSizeExponent);
		int num2 = SumPowerOfTwo(firstBucketLengthExponent, endExponent);
		if (num > maxBucketSizeExponent)
		{
			num2 += maxBucketSize * (num - maxBucketSizeExponent);
		}
		return num2;
	}

	public static int GetPowerOfTwoBucketIndex(this int index, int firstBucketLengthExponent, int maxBucketSize, int maxBucketSizeExponent, out int indexInBucket)
	{
		int num = 1 << firstBucketLengthExponent;
		if (index < num)
		{
			indexInBucket = index;
			return 0;
		}
		return (index + num - 1).GetPowerOfTwoBucketIndex(maxBucketSize, maxBucketSizeExponent, out indexInBucket) - firstBucketLengthExponent;
	}

	public static int GetPowerOfTwoBucketLength(this int bucketCount, int maxBucketSize, int maxBucketSizeExponent)
	{
		if (bucketCount <= maxBucketSizeExponent)
		{
			return (1 << bucketCount) - 1;
		}
		return (bucketCount - maxBucketSizeExponent + 1) * maxBucketSize - 1;
	}

	public static int GetPowerOfTwoBucketIndex(this int index, int maxBucketSize, int maxBucketSizeExponent)
	{
		if (index == 0)
		{
			return 0;
		}
		if (index < maxBucketSize && index < PowerOfTwoBucketIndices.Length)
		{
			return PowerOfTwoBucketIndices[index];
		}
		int num = BitOperations.Log2((uint)(index + 1));
		if (num <= maxBucketSizeExponent)
		{
			return num;
		}
		int value = index + 1 - (maxBucketSizeExponent + 1).GetPowerOfTwoBucketLength(maxBucketSize, maxBucketSizeExponent);
		return maxBucketSizeExponent + value.GetNumberOfBuckets(maxBucketSize);
	}

	public static int GetPowerOfTwoBucketIndex(this int index, int maxBucketSize, int maxBucketSizeExponent, out int indexInBucket)
	{
		int powerOfTwoBucketIndex = index.GetPowerOfTwoBucketIndex(maxBucketSize, maxBucketSizeExponent);
		if (powerOfTwoBucketIndex == 0)
		{
			indexInBucket = 0;
			return powerOfTwoBucketIndex;
		}
		indexInBucket = index - powerOfTwoBucketIndex.GetPowerOfTwoBucketLength(maxBucketSize, maxBucketSizeExponent);
		return powerOfTwoBucketIndex;
	}
}
