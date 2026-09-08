using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MemoryPools;

internal static class MemoryUnderlyingArrayAccessor<T>
{
	public static readonly Func<Memory<T>, object> GetUnderlyingArray = GetUnderlyingArrayDelegate<T>();

	private static Func<Memory<T>, object> GetUnderlyingArrayDelegate<T>()
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(Memory<T>), "memory");
		FieldInfo field = typeof(Memory<T>).GetField("_object", BindingFlags.Instance | BindingFlags.NonPublic);
		return Expression.Lambda<Func<Memory<T>, object>>(Expression.Field(parameterExpression, field), new ParameterExpression[1] { parameterExpression }).Compile();
	}
}
