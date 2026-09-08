using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MemoryPools;

internal static class ListItemsAccessor<T>
{
	public static readonly Func<List<T>, T[]> GetInternalItems = CreateGetInternalItemsDelegate<T>();

	private static Func<List<T>, T[]> CreateGetInternalItemsDelegate<T>()
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(List<T>), "list");
		FieldInfo field = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
		return Expression.Lambda<Func<List<T>, T[]>>(Expression.Field(parameterExpression, field), new ParameterExpression[1] { parameterExpression }).Compile();
	}
}
