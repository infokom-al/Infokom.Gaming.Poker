using System;
using System.Collections;

namespace CSharpSerializer;

internal static class Verification
{
	public static T VerifyType<T>(this object? @object)
	{
		if (@object is T)
		{
			return (T)@object;
		}
		throw new InvalidOperationException("Expected object of type " + typeof(T).Name + " but was " + @object.GetType().Name);
	}

	public static void VerifyCollectionNotEmpty(this ICollection collection, string? message = "Collection is empty")
	{
		if (collection.Count == 0)
		{
			throw new InvalidOperationException(message);
		}
	}
}
