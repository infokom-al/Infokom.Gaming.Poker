namespace Poker.Calc;

internal readonly struct Line
{
	public Point Start { get; }

	public Point End { get; }

	public bool IsHorizontalLine => Start.Y == End.Y;

	public bool IsVerticalLine => Start.X == End.X;

	public int Height => (Start.Y - End.Y).Abs() + 1;

	public int Width => (Start.X - End.X).Abs() + 1;

	public int Length => Math.Max(Width, Height);

	public bool IsDiagonal => Height == Width;

	public bool IsSinglePoint => Start == End;

	public Point TopLeft => new Point(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y));

	public Point BottomRight => new Point(Math.Max(Start.X, End.X), Math.Max(Start.Y, End.Y));

	public Line(Point start, Point end)
	{
		Start = start;
		End = end;
		if (!IsVerticalLine && !IsHorizontalLine && !IsDiagonal)
		{
			throw new ArgumentException($"Only vertical, horizontal and diagonal lines are supported but was {this}");
		}
	}

	public IEnumerable<Point> GetAllPoints()
	{
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				if (!IsDiagonal || i == j)
				{
					yield return new Point(i, j).Shift(TopLeft);
				}
			}
		}
	}

	public override string ToString()
	{
		return $"{Start}; {End}";
	}
}
