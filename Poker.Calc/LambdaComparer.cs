namespace Poker.Calc;

internal class LambdaComparer<T> : IComparer<T>
{
	public Func<T, T, int> _compare;

	public LambdaComparer(Func<T, T, int> compare)
	{
		_compare = compare;
	}

	public int Compare(T x, T y)
	{
		return _compare(x, y);
	}
}
