using BinarySerializer;

namespace Poker.Calc;

[BinarySerializable]
public struct Range : IEquatable<Range>
{
	public static Range Zero = new Range(0.0, 0.0);

	public static Range ZeroToInf = new Range(0.0, double.MaxValue);

	public static Range ZeroToHundred = new Range(0.0, 100.0);

	public static Range InfToInf = new Range(double.MinValue, double.MaxValue);

	public static Range InfToZero = new Range(double.MinValue, 0.0);

	[Tag(1)]
	public double Left { get; }

	[Tag(2)]
	public double Right { get; }

	public double Length => Right - Left;

	public bool IsZeroToInf
	{
		get
		{
			if (Left.IsZero())
			{
				return Right.IsMaxValue();
			}
			return false;
		}
	}

	public bool IsInfToInf
	{
		get
		{
			if (Left.IsMinValue())
			{
				return Right.IsMaxValue();
			}
			return false;
		}
	}

	public bool IsSinglePoint => Left.IsEqual(Right);

	public bool IsZeroToZero
	{
		get
		{
			if (Left.IsZero())
			{
				return Right.IsZero();
			}
			return false;
		}
	}

	public bool IsFullRangeInPercents
	{
		get
		{
			if (Left.IsZero())
			{
				return Right.Equals(100.0);
			}
			return false;
		}
	}

	public bool IsZeroToHundred
	{
		get
		{
			if (Left.IsZero())
			{
				return Right.Equals(100.0);
			}
			return false;
		}
	}

	public bool ContainsZeroToHundred => Contains(ZeroToHundred);

	public double MidValue => Left + (Right - Left) / 2.0;

	public Range(double left, double right)
	{
		if (left.IsGreater(right, 1E-07))
		{
			throw new ArgumentException($"Can't create a valid range from given points. left=={left}, right=={right} ");
		}
		double num = left.RoundDeviation();
		double num2 = right.RoundDeviation();
		Left = num;
		Right = num2;
	}

	public bool Contains(double value, double deviation = 1E-06)
	{
		if ((!IsInfToInf || !double.IsNaN(value)) && (!Left.IsLessOrEqual(value, deviation) || !value.IsLessOrEqual(Right, deviation)) && (!value.IsMaxValue() || !Right.IsMaxValue()))
		{
			if (value.IsMinValue())
			{
				return Left.IsMinValue();
			}
			return false;
		}
		return true;
	}

	public bool ContainsWithin(double value, double deviation = 1E-06)
	{
		if (Left.IsLess(value, deviation))
		{
			return value.IsLess(Right, deviation);
		}
		return false;
	}

	public bool Contains(Range other)
	{
		if (Left.IsLessOrEqual(other.Left))
		{
			return other.Right.IsLessOrEqual(Right);
		}
		return false;
	}

	public bool IntersectOrTouches(Range other)
	{
		if (!Contains(other.Left) && !Contains(other.Right) && !other.Contains(Left))
		{
			return other.Contains(Right);
		}
		return true;
	}

	public bool Intersects(Range other)
	{
		if (!ContainsWithin(other.Left) && !ContainsWithin(other.Right) && !other.ContainsWithin(Left) && !other.ContainsWithin(Right))
		{
			return Equals(other);
		}
		return true;
	}

	public Range GetIntersection(Range other)
	{
		if (!IntersectOrTouches(other))
		{
			throw new InvalidOperationException("Given ranges do not intersect");
		}
		return new Range(Math.Max(Left, other.Left), Math.Min(Right, other.Right));
	}

	public static Range FromPoint(double value)
	{
		return new Range(value, value);
	}

	public static Range FromLeft(double left)
	{
		return new Range(left, double.MaxValue);
	}

	public override string ToString()
	{
		if (!Right.IsMaxValue())
		{
			if (!Left.IsMinValue())
			{
				return $"[{Left.RoundDeviation()}; {Right.RoundDeviation()}]";
			}
			return $"< {Right.RoundDeviation()}";
		}
		return $"> {Left.RoundDeviation()}";
	}

	public string ToString(Range fullRange)
	{
		if (!Right.Equals(fullRange.Right))
		{
			if (!Left.Equals(fullRange.Left))
			{
				return $"{Left.RoundDeviation()}-{Right.RoundDeviation()}";
			}
			return $"< {Right.RoundDeviation()}";
		}
		return $"> {Left.RoundDeviation()}";
	}

	public static bool operator ==(Range range1, Range range2)
	{
		return range1.Equals(range2);
	}

	public static bool operator !=(Range range1, Range range2)
	{
		return !range1.Equals(range2);
	}

	public bool Equals(Range other)
	{
		if (Left.IsEqual(other.Left))
		{
			return Right.IsEqual(other.Right);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Range other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 31 + Left.RoundDeviation().GetHashCode()) * 31 + Right.RoundDeviation().GetHashCode();
	}
}
