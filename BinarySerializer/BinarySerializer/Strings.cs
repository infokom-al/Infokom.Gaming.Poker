using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer;

internal static class Strings
{
	public static Dictionary<string, string> PascalToCamel = new Dictionary<string, string>();

	public static Dictionary<string, string> CamelToPascal = new Dictionary<string, string>();

	internal static string Quoted(this object @object)
	{
		return "\"" + @object.ToString() + "\"";
	}

	public static string PascalToCamelCase(this string @string)
	{
		if (PascalToCamel.TryGetValue(@string, out string value))
		{
			return value;
		}
		lock (PascalToCamel)
		{
			return PascalToCamel[@string] = @string.PascalToCamelCaseSlow();
		}
	}

	public static string PascalToCamelCaseSlow(this string @string)
	{
		if (char.IsLower(@string[0]))
		{
			return @string;
		}
		return char.ToLower(@string[0]) + @string.Substring(1, @string.Length - 1);
	}

	public static string CamelToPascalCase(this string @string)
	{
		if (CamelToPascal.TryGetValue(@string, out string value))
		{
			return value;
		}
		lock (CamelToPascal)
		{
			return CamelToPascal[@string] = @string.CamelToPascalCaseSlow();
		}
	}

	public static string CamelToPascalCaseSlow(this string @string)
	{
		return $"{@string[0]}".ToUpper() + @string.Substring(1, @string.Length - 1);
	}

	public static string Join<T>(this IEnumerable<T> items, string separator)
	{
		return items.Select((T item) => item.ToString()).Join(separator);
	}

	public static string Join(this IEnumerable<string> strings, string separator)
	{
		string text = string.Empty;
		foreach (string @string in strings)
		{
			if (text != string.Empty)
			{
				text += separator;
			}
			text += @string;
		}
		return text;
	}

	public static string JoinWithSpace<T>(this IEnumerable<T> items)
	{
		return items.Select((T item) => item.ToString()).Join(" ");
	}
}
