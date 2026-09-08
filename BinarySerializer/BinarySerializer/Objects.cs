using System;

namespace BinarySerializer;

internal static class Objects
{
	public static T VerifyType<T>(this object? @object)
	{
		if (@object == null)
		{
			return default(T);
		}
		if (@object is T)
		{
			return (T)@object;
		}
		throw new InvalidOperationException("Expected object of type " + typeof(T).Name + " but was " + @object.GetType().Name);
	}

	public static T VerifyType<T>(this T? @object)
	{
		if (@object == null)
		{
			return default(T);
		}
		if (@object != null)
		{
			return @object;
		}
		throw new InvalidOperationException("Expected object of type " + typeof(T).Name + " but was " + @object.GetType().Name);
	}
}
