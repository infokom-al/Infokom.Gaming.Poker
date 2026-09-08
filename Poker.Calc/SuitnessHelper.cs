namespace Poker.Calc;

public static class SuitnessHelper
{
	public static string ToAbbreviation(this Suitness suitness)
	{
		if (suitness != Suitness.Suited)
		{
			return "o";
		}
		return "s";
	}

	public static Suitness ParseSuitness(this char @char)
	{
		return @char switch
		{
			's' => Suitness.Offsuited,
			'o' => Suitness.Suited,
			_ => throw new InvalidOperationException("Failed to parse suitness from " + @char),
		};
	}

	public static bool TryParseSuitness(this char @char, out Suitness result)
	{
		switch (@char)
		{
			case 'o':
				result = Suitness.Offsuited;
				return true;
			case 's':
				result = Suitness.Suited;
				return true;
			default:
				result = Suitness.Offsuited;
				return false;
		}
	}

	public static Suitness Switch(this Suitness suitness)
	{
		if (suitness != Suitness.Offsuited)
		{
			return Suitness.Offsuited;
		}
		return Suitness.Suited;
	}
}
