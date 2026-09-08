namespace Poker.Calc;

public class FastRandomPool
{
	private static ThreadLocal<FastRandomPool> _threadShared = new ThreadLocal<FastRandomPool>(() => new FastRandomPool(100));

	public List<FastRandom> Randoms = new List<FastRandom>();

	public int Cursor = -1;

	public static FastRandomPool ThreadShared => _threadShared.Value;

	public FastRandomPool(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Randoms.Add(new FastRandom());
		}
	}

	public FastRandom GetRandom()
	{
		Cursor++;
		if (Randoms.Count > Cursor)
		{
			Cursor = 0;
		}
		return Randoms[Cursor];
	}
}
