using System;

namespace CSharpSerializer;

public static class Numbers
{
	public static decimal ToDecimal(this double value)
	{
		double num = 7.922816251426434E+28;
		double num2 = -7.922816251426434E+28;
		if (value >= num - 0.0001)
		{
			return decimal.MaxValue;
		}
		if (value < num2 - 0.0001)
		{
			return decimal.MinValue;
		}
		return (decimal)value;
	}

	public static bool IsZero(this double value)
	{
		return Math.Abs(value) < 1E-06;
	}
}
