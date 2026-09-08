namespace Poker.Calc;

public static class RandomHelper
{
	public static T GetRandomItem<T>(this IList<T> items, FastRandom random)
	{
		return items[random.Next(items.Count)];
	}

	public static T GetRandomItem<T>(this InlineList<T> items, FastRandom random)
	{
		return items[random.Next(items.Count)];
	}

	public static bool GetRandomBoolean(this double unitIntervalProbability, FastRandom random)
	{
		return random.NextDouble() <= unitIntervalProbability;
	}

	public static T GetRandomItem<T>(this T[] items, double[] unitIntervalProbabilities, FastRandom random)
	{
		int num;
		do
		{
			num = random.Next(items.Length);
		}
		while (!unitIntervalProbabilities[num].GetRandomBoolean(random));
		return items[num];
	}
}
