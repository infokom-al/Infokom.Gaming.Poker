namespace Poker.Calc;

internal readonly struct Rectangle
{
	public int Left { get; }

	public int Top { get; }

	public int Width { get; }

	public int Height { get; }

	public int Area => Width * Height;

	public Point TopLeftCorner => new Point(Left, Top);

	public Point BottomRightCorner => new Point(Right, Bottom);

	public int Right => Left + Width - 1;

	public int Bottom => Top + Height - 1;

	public Rectangle(int left, int top, int width, int height)
	{
		Left = left;
		Top = top;
		Width = width;
		Height = height;
	}
}
