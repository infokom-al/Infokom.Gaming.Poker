namespace Poker.Calc;

internal static class FluentParserHelperInternal
{
	public static bool IsWordCharacter(this char @char)
	{
		if (!@char.IsDigit())
		{
			return @char.IsLetter();
		}
		return true;
	}

	public static bool IsDigit(this char @char)
	{
		if ('0' <= @char)
		{
			return @char <= '9';
		}
		return false;
	}

	public static bool IsCapitalLetter(this char @char)
	{
		if ('A' <= @char)
		{
			return @char <= 'Z';
		}
		return false;
	}

	public static bool IsSmallLetter(this char @char)
	{
		if ('a' <= @char)
		{
			return @char <= 'z';
		}
		return false;
	}

	public static bool IsLetter(this char @char)
	{
		if (!@char.IsSmallLetter())
		{
			return @char.IsCapitalLetter();
		}
		return true;
	}

	public static int ToDigit(this char @char)
	{
		return @char - 48;
	}

	public static bool IsHexDigit(this char @char)
	{
		if (!@char.IsDigit() && ('A' > @char || @char > 'F'))
		{
			if ('a' <= @char)
			{
				return @char <= 'f';
			}
			return false;
		}
		return true;
	}
}
