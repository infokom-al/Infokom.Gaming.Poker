namespace Poker.Calc;

internal static class Doubles
{
	public static int DefaultDoubleDeviationDigitsCount = (int)Math.Log(1E-06, 0.1);

	public const double DefaultDoubleDeviation = 1E-06;

	public const float DefaultFloatDeviation = 1E-06f;

	public static bool IsZero(this double value, double deviation = 1E-06)
	{
		return Math.Abs(value) < deviation;
	}

	public static bool IsZero(this float value, float deviation = 1E-06f)
	{
		return Math.Abs(value) < deviation;
	}

	public static bool IsNotZero(this double value, double deviation = 1E-06)
	{
		return !value.IsZero(deviation);
	}

	public static bool IsEqual(this double value, double other, double deviation = 1E-06)
	{
		return Math.Abs(value - other) < deviation;
	}

	public static bool IsNotEqual(this double value, double other, double deviation = 1E-06)
	{
		return !value.IsEqual(other, deviation);
	}

	public static bool IsGreater(this double value, double other, double deviation = 1E-06)
	{
		return value - other > deviation;
	}

	public static bool IsLess(this double value, double other, double deviation = 1E-06)
	{
		return other - value > deviation;
	}

	public static bool IsLessMaxValue(this double value)
	{
		return value.IsLess(double.MaxValue);
	}

	public static bool IsGreaterOrEqual(this double value, double other, double deviation = 1E-06)
	{
		return value - other > 0.0 - deviation;
	}

	public static bool IsLessOrEqual(this double value, double other, double deviation = 1E-06)
	{
		return other - value > 0.0 - deviation;
	}

	public static bool IsLessOrEqual(this float value, float other, double deviation = 1E-06)
	{
		return (double)(other - value) > 0.0 - deviation;
	}

	public static bool IsLessOrEqualZero(this double value, double deviation = 1E-06)
	{
		return value.IsLessOrEqual(0.0, deviation);
	}

	public static bool IsGreaterZero(this double value, double deviation = 1E-06)
	{
		return value > deviation;
	}

	public static bool IsLessZero(this double value, double deviation = 1E-06)
	{
		return value < 0.0 - deviation;
	}

	public static bool IsMaxValue(this double value)
	{
		return value.IsEqual(double.MaxValue);
	}

	public static double VerifyPositive(this double value, string? name = null)
	{
		if (value.IsGreaterZero())
		{
			return value;
		}
		throw new InvalidOperationException($"Expecting positive value {name} but was {value}");
	}

	public static double VerifyArgumentPositive(this double value, string argumentName)
	{
		if (value.IsLessOrEqualZero(value))
		{
			throw new ArgumentException($"Expecting a positive value but was {value}", "argumentName");
		}
		return value;
	}

	public static double VerifyGreaterOrEqualZero(this double value, string? name = null)
	{
		if (value.IsLessZero())
		{
			throw new InvalidOperationException($"Expecting value {name} to be greater or equal zero but was {value}");
		}
		return value;
	}

	public static double RoundDeviation(this double value)
	{
		return Math.Round(value, DefaultDoubleDeviationDigitsCount + 1);
	}

	public static double Rounded(this double value, int digits = 0)
	{
		return Math.Round(value, digits);
	}

	public static bool IsMinValue(this double value)
	{
		if (!double.IsNegativeInfinity(value))
		{
			return value == double.MinValue;
		}
		return true;
	}
}
