using MemoryPools;

namespace Poker.Calc;

internal class SelectorDescendingComparer<T, TComparable> : IComparer<T> where TComparable : IComparable<TComparable>
{
	private Func<T, TComparable> _keySelector;

	public static SelectorDescendingComparer<T, TComparable> Create(Func<T, TComparable> keySelector)
	{
		SelectorDescendingComparer<T, TComparable> selectorDescendingComparer = ObjectPool<SelectorDescendingComparer<T, TComparable>>.ThreadShared.RentObject();
		selectorDescendingComparer._keySelector = keySelector;
		return selectorDescendingComparer;
	}

	public int Compare(T x, T y)
	{
		if (x == null)
		{
			if (y != null)
			{
				return -1;
			}
			return 0;
		}
		if (y == null)
		{
			return 1;
		}
		TComparable val = _keySelector(x);
		TComparable other = _keySelector(y);
		return -val.CompareTo(other);
	}
}
