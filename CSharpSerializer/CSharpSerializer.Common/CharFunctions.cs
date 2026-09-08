namespace CSharpSerializer.Common;

public static class CharFunctions
{
	public static bool IsDigit(this char @char)
	{
		if ('0' <= @char)
		{
			return @char <= '9';
		}
		return false;
	}

	public static int ToDigit(this char @char)
	{
		return @char - 48;
	}
}
