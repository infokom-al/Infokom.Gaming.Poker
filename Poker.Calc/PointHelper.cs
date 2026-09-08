namespace Poker.Calc;

internal static class PointHelper
{
	public static Line ToLine(this Point start, Point end)
	{
		return new Line(start, end);
	}

	public static Line VerifyArgumentIsHorizontalLine(this Line line, string argumentName)
	{
		if (!line.IsHorizontalLine)
		{
			throw new ArgumentException($"Expecting a horizontal line but was {line}", argumentName);
		}
		return line;
	}

	public static Line VerifyArgumentIsVerticalLine(this Line line, string argumentName)
	{
		if (!line.IsVerticalLine)
		{
			throw new ArgumentException($"Expecting a vertical line but was {line}", argumentName);
		}
		return line;
	}

	public static Line ExtendRight(this Line line)
	{
		return line.VerifyArgumentIsHorizontalLine("line").Start.ToLine(line.End.ShiftRight());
	}

	public static Line ExtendBottom(this Line line)
	{
		return line.VerifyArgumentIsVerticalLine("line").Start.ToLine(line.End.ShiftBottom());
	}

	public static Point ShiftRight(this Point point)
	{
		return point.Shift(1, 0);
	}

	public static Point ShiftBottom(this Point point)
	{
		return point.Shift(0, 1);
	}

	public static Point Shift(this Point point, Point delta)
	{
		return point.Shift(delta.X, delta.Y);
	}

	public static Point Shift(this Point point, int deltaX, int deltaY)
	{
		return new Point(point.X + deltaX, point.Y + deltaY);
	}

	public static bool Contains(this Rectangle rectangle, Point point)
	{
		if (rectangle.Left <= point.X && point.X <= rectangle.Right && rectangle.Top <= point.Y)
		{
			return point.Y <= rectangle.Bottom;
		}
		return false;
	}

	public static Rectangle GetRectangle(this Point cornerPoint, Point otherCornerPoint)
	{
		return new Rectangle(Math.Min(cornerPoint.X, otherCornerPoint.X), Math.Min(cornerPoint.Y, otherCornerPoint.Y), Math.Abs(cornerPoint.X - otherCornerPoint.X) + 1, Math.Abs(cornerPoint.Y - otherCornerPoint.Y) + 1);
	}

	public static IEnumerable<Point> GetAllPoints(this Rectangle rectangle)
	{
		for (int i = 0; i < rectangle.Width; i++)
		{
			for (int j = 0; j < rectangle.Height; j++)
			{
				yield return new Point(rectangle.Left + i, rectangle.Top + j);
			}
		}
	}

	public static Point RelativeTo(this Point point, Point other)
	{
		return new Point(point.X - other.X, point.Y - other.Y);
	}

	public static Point VerifyNotNegative(this Point point)
	{
		if (point.X < 0 || point.Y < 0)
		{
			throw new InvalidOperationException($"Point can't be negative but was {point}");
		}
		return point;
	}

	public static Line VerifyIsDiagonal(this Line line)
	{
		if (!line.IsDiagonal)
		{
			throw new InvalidOperationException($"Expecting a diagonal but was {line}");
		}
		return line;
	}
}
