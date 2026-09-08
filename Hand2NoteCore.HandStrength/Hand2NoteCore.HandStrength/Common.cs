using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand2NoteCore.HandStrength;

internal static class Common
{
	public static void VerifyArgumentTrue(this bool value, string argumentName)
	{
		if (!value)
		{
			throw new ArgumentException("Expecting value to be true", "argumentName");
		}
	}

	public static void VerifyTrue(this bool value, string? name = null)
	{
		if (!value)
		{
			throw new InvalidOperationException("Expecting value " + name + " to be true");
		}
	}

	public static int VerifyArgumentPositive(this int value, string argumentName)
	{
		if (value <= 0)
		{
			throw new ArgumentException($"Expecting positive value but was {value}", argumentName);
		}
		return value;
	}

	public static T VerifyArgumentNotEqual<T>(this T @object, T otherObject, string argumentName)
	{
		if (object.Equals(@object, otherObject))
		{
			throw new ArgumentException($"Expecting an object not to be equal to other object {otherObject} but was {@object}", argumentName);
		}
		return @object;
	}

	public static int VerifyPositive(this int value, string? name = null)
	{
		if (value <= 0)
		{
			throw new InvalidOperationException($"Value {name} must be positive but was {value}");
		}
		return value;
	}

	public static List<TOut> MapToList<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> convert)
	{
		return enumerable.Select(convert).ToList();
	}

	public static TOut[] MapToArray<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> convert)
	{
		return items.Select(convert).ToArray();
	}
}
