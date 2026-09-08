using System.Text;

namespace Poker.Calc;

internal static class StringsHelper
{
	internal static string Quoted(this string s)
	{
		return "\"" + s + "\"";
	}

	internal static string RemoveSpaces(this string s)
	{
		return s.Replace(" ", string.Empty);
	}

	public static string JoinStrings(this IEnumerable<string> collection, string separator = " ")
	{
		return string.Join(separator, collection);
	}

	public static string AggregateToString<T, TString>(this IEnumerable<T> items, Func<T, TString> selector, string? separator = null)
	{
		return items.Select(selector).AggregateToString(separator);
	}

	public static string AggregateToString<T>(this IEnumerable<T> items, string? separator = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (T item in items)
		{
			if (stringBuilder.Length != 0 && separator != null)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}

	public static string Remove(this string @string, string substring)
	{
		return @string.Replace(substring, "");
	}

	public static string AddWord(this string @string, string word)
	{
		return @string = @string.Trim() + " " + word.Trim();
	}

	public static string AddWithComma(this string @string, string word)
	{
		return @string = @string.Trim() + ", " + word.Trim();
	}

	public static string AddNameValue(this string @string, string name, object value)
	{
		return @string.AddWithComma($"{name}={value}");
	}

	public static string AddSpacesToMatchWidth(this string @string, int minWidth)
	{
		if (@string.Length >= minWidth)
		{
			return @string;
		}
		return @string.AddSpaces(minWidth - @string.Length);
	}

	public static string AddSpaces(this string @string, int count)
	{
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text += " ";
		}
		return @string + text;
	}

	internal static string Quoted(this object @object)
	{
		return "\"" + @object.ToString() + "\"";
	}

	public static string TrimEnd(this string @string, string suffix)
	{
		if (@string.Trim().EndsWith(suffix))
		{
			return @string.Substring(0, @string.Length - suffix.Length);
		}
		return @string;
	}

	public static string CamelToPascalCase(this string @string)
	{
		return $"{@string[0]}".ToUpper() + @string.Substring(1, @string.Length - 1);
	}

	public static string SeparateWords(this string @string)
	{
		if (string.IsNullOrWhiteSpace(@string))
		{
			return @string;
		}
		if (@string.Length == 1)
		{
			return @string.ToLower();
		}
		return string.Concat(@string.Select((char x, int i) => (i <= 0 || !char.IsUpper(x)) ? x.ToString() : (" " + x)));
	}

	public static string ToSentenceCase(this string @string)
	{
		return char.ToUpper(@string[0]) + @string.Substring(1).ToLower();
	}
}
