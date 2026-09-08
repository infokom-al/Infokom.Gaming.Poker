namespace Poker.Calc;

internal static class FluentParserHelper
{
	public static FluentParser ToFluentParser(this string @string)
	{
		return new FluentParser(@string);
	}

	public static long ParseLongFromHexString(this string @string)
	{
		return Convert.ToInt64(@string, 16);
	}

	public static int ParseIntFromHexString(this string @string)
	{
		return Convert.ToInt32(@string, 16);
	}
}
