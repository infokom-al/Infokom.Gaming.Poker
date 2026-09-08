using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPools;

internal static class Strings
{
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

	public static string AddWithComma(this string @string, string word)
	{
		return @string = @string.Trim() + ", " + word.Trim();
	}

	public static string AddNameValue(this string @string, string name, object value)
	{
		return @string.AddWithComma($"{name}={value}");
	}

	public static string AddNameValue(this string @string, string name, object value, string units)
	{
		return @string.AddWithComma($"{name}={value}{units}");
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

	public static string Quoted(this object? @object)
	{
		return $"\"{@object}\"";
	}

	public static string GetMemorySizeString(this long size)
	{
		if (size >= 1024)
		{
			if (size < 1048576)
			{
				long value = size / 1024;
				return $"{value}KB";
			}
			if (size < 1073741824)
			{
				long value2 = size / 1024 / 1024;
				return $"{value2}MB";
			}
			double num = (double)size / 1024.0 / 1024.0 / 1024.0;
			if (!(num < 5.0))
			{
				return $"{num.Rounded()}GB";
			}
			return $"{num.Rounded(2)}GB";
		}
		return $"{size}B";
	}
}
