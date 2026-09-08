namespace Poker.Calc;

internal readonly struct Point
{
	public int X { get; }

	public int Y { get; }

	public Point(int x, int y)
	{
		X = x;
		Y = y;
	}

	public static bool operator ==(Point firstPoint, Point secondPoint)
	{
		if (firstPoint.X == secondPoint.X)
		{
			return firstPoint.Y == secondPoint.Y;
		}
		return false;
	}

	public static bool operator !=(Point firstPoint, Point secondPoint)
	{
		if (firstPoint.X == secondPoint.X)
		{
			return firstPoint.Y != secondPoint.Y;
		}
		return true;
	}

	public bool Equals(Point other)
	{
		if (X == other.X)
		{
			return Y == other.Y;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Point other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return X ^ Y;
	}

	public override string ToString()
	{
		return $"{X}, {Y}";
	}
}
