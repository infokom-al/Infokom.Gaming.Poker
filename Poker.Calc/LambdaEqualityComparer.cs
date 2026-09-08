namespace Poker.Calc;

public class LambdaEqualityComparer<T> : IEqualityComparer<T>
{
	public Func<T, T, bool> Comparer { get; }

	public Func<T, int> HashCode { get; }

	public LambdaEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashCode)
	{
		Comparer = comparer;
		HashCode = hashCode;
	}

	public bool Equals(T x, T y)
	{
		return Comparer(x, y);
	}

	public int GetHashCode(T x)
	{
		return HashCode(x);
	}
}
