namespace Poker.Calc;

internal class ThenByComparer<TItem, TComparableFirst, TComparableSecond> : IComparer<TItem> where TComparableFirst : IComparable<TComparableFirst> where TComparableSecond : IComparable<TComparableSecond>
{
	public Func<TItem, TComparableFirst> FirstKeySelector { get; set; }

	public Func<TItem, TComparableSecond> SecondKeySelector { get; set; }

	public ThenByComparer(Func<TItem, TComparableFirst> firstKeySelector, Func<TItem, TComparableSecond> secondKeySelector)
	{
		FirstKeySelector = firstKeySelector;
		SecondKeySelector = secondKeySelector;
	}

	public int Compare(TItem x, TItem y)
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
		TComparableFirst val = FirstKeySelector(x);
		TComparableFirst other = FirstKeySelector(y);
		int num = val.CompareTo(other);
		if (num != 0)
		{
			return num;
		}
		TComparableSecond val2 = SecondKeySelector(x);
		TComparableSecond other2 = SecondKeySelector(y);
		return val2.CompareTo(other2);
	}
}
