using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpSerializer.Common;

internal static class Strings
{
	internal static string Quoted(this object @object)
	{
		return "\"" + @object.ToString() + "\"";
	}

	public static string PascalToCamelCase(this string @string)
	{
		if (char.IsLower(@string[0]))
		{
			return @string;
		}
		return char.ToLower(@string[0]) + @string.Substring(1, @string.Length - 1);
	}

	public static string CamelToPascalCase(this string @string)
	{
		return $"{@string[0]}".ToUpper() + @string.Substring(1, @string.Length - 1);
	}

	internal static string Join<T, TString>(this IEnumerable<T> items, Func<T, TString> selector, string? separator = null)
	{
		return items.Select(selector).Join(separator);
	}

	public static string Join<T>(this IEnumerable<T> strings, string separator)
	{
		return string.Join(separator, strings);
	}
}
