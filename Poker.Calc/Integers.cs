namespace Poker.Calc
{
	internal static class Integers
	{
		public static int VerifyArgumentPositive(this int value, string argumentName)
		{
			if (value <= 0)
			{
				throw new ArgumentException($"Expecting positive value but was {value}", argumentName);
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

		public static bool IsPowerOfTwo(this long x)
		{
			if (x != 0L)
			{
				return (x & (x - 1)) == 0;
			}
			return false;
		}

		public static int VerifyIsPowerOfTwo(this int value, string? message = null)
		{
			if (!value.IsPowerOfTwo())
			{
				throw new InvalidOperationException(message ?? $"Expecting power of two but was {value}");
			}
			return value;
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
	}
}
