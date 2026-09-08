using MemoryPools;

namespace Poker.Calc;

internal class SelectorComparer<T, TComparable> : IComparer<T> where TComparable : IComparable<TComparable>
{
	private Func<T, TComparable> _keySelector;

	public static SelectorComparer<T, TComparable> Create(Func<T, TComparable> keySelector)
	{
		SelectorComparer<T, TComparable> selectorComparer = ObjectPool<SelectorComparer<T, TComparable>>.ThreadShared.RentObject();
		selectorComparer._keySelector = keySelector;
		return selectorComparer;
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
		return val.CompareTo(other);
	}
}
