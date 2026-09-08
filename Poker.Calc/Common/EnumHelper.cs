namespace Poker.Calc.Common;

public static class EnumHelper
{
	public static bool IsSingleFlag<T>(this T value)
	{
		return Convert.ToInt64(value).IsPowerOfTwo();
	}

	public static IEnumerable<T> GetValues<T>()
	{
		foreach (object value in Enum.GetValues(typeof(T)))
		{
			yield return (T)value;
		}
	}

	public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum
	{
		foreach (T value in Enum.GetValues(input.GetType()))
		{
			if (input.HasFlag(value) && value.IsSingleFlag())
			{
				yield return value;
			}
		}
	}

	public static IEnumerable<string> GetFlagsNames<T>(this T input) where T : Enum
	{
		return from flag in input.GetFlags()
			  select flag.GetEnumName();
	}

	public static IEnumerable<string> GetFlagsNamesOrSentenceCase<T>(this T input) where T : Enum
	{
		return from flag in input.GetFlags()
			  select flag.GetEnumNameOrSentenceCase();
	}

	public static IEnumerable<T> GetSingleFlags<T>() where T : Enum
	{
		foreach (object value in Enum.GetValues(typeof(T)))
		{
			if (value.IsSingleFlag())
			{
				yield return (T)value;
			}
		}
	}

	public static bool HasAnyFlag<TEnum>(this TEnum value, TEnum from, TEnum to) where TEnum : Enum
	{
		long num = Convert.ToInt64(value);
		long num2 = Convert.ToInt64(from);
		long num3 = Convert.ToInt64(to) - num2 + num2;
		return (num & num3) == num3;
	}

	public static bool HasFlag<TEnum>(this TEnum value, TEnum from, TEnum to) where TEnum : Enum
	{
		return Convert.ToInt64(value.Slice(from, to)) != 0;
	}

	public static TEnum Slice<TEnum>(this TEnum value, TEnum from, TEnum to) where TEnum : Enum
	{
		long num = Convert.ToInt64(value);
		long num2 = Convert.ToInt64(from);
		long num3 = Convert.ToInt64(to);
		long num4 = num2;
		long num5 = 0L;
		while (num4 <= num3)
		{
			if ((num & num4) != 0L)
			{
				num5 |= num4;
			}
			num4 <<= 1;
		}
		return (TEnum)(object)num5;
	}
}
