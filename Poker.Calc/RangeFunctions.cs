namespace Poker.Calc;

public static class RangeFunctions
{
	public static Range SliceLeft(this Range range, double value)
	{
		if (value.IsGreater(range.Right))
		{
			throw new ArgumentOutOfRangeException($"Unable to slice range {range} from left at {value}", "value");
		}
		return new Range(Math.Max(range.Left, value), range.Right);
	}

	public static Range Shift(this Range range, double delta)
	{
		return new Range(range.Left + delta, range.Right + delta);
	}

	public static Range Multiply(this Range range, double multiplier)
	{
		return new Range(range.Left * multiplier, range.Right * multiplier);
	}

	public static Range Divide(this Range range, double divider)
	{
		return range.Multiply(1.0 / divider);
	}

	public static Range RoundDeviation(this Range range)
	{
		return new Range(range.Left.RoundDeviation(), range.Right.RoundDeviation());
	}

	public static double GetDistanceTo(this Range range, double point)
	{
		if (!point.IsLess(range.Left))
		{
			if (!point.IsGreater(range.Right))
			{
				return 0.0;
			}
			return point - range.Right;
		}
		return range.Left - point;
	}

	public static double GetRelativePosition(this Range range, double point)
	{
		return (point - range.Left) / range.Length;
	}

	public static double GetRelativePositionIn(this double point, Range range)
	{
		return range.GetRelativePosition(point);
	}

	public static double GetRelativePositionIn(this decimal point, Range range)
	{
		return range.GetRelativePosition((double)point);
	}

	public static Range ToSinglePointRange(this double value)
	{
		return new Range(value, value);
	}

	public static IEnumerable<Range> GetRanges(this IEnumerable<double> values)
	{
		double? num = null;
		foreach (double value in values.Order())
		{
			if (num.HasValue && !num.Value.IsEqual(value))
			{
				yield return new Range(num.Value, value);
			}
			num = value;
		}
	}

	public static Range GetMinMaxRange<T>(this IEnumerable<T> values, Func<T, double> selector)
	{
		return values.Select(selector).GetMinMaxRange();
	}

	public static Range GetMinMaxRange(this IEnumerable<double> values)
	{
		double num = double.MaxValue;
		double num2 = double.MinValue;
		bool flag = true;
		foreach (double value in values)
		{
			flag = false;
			if (value < num)
			{
				num = value;
			}
			if (value > num2)
			{
				num2 = value;
			}
		}
		if (flag)
		{
			return Range.InfToInf;
		}
		return new Range(num, num2);
	}
}
