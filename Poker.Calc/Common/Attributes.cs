using System.Collections.Immutable;
using System.Reflection;

namespace Poker.Calc.Common;

internal static class Attributes
{
	private static ImmutableDictionary<Type, ImmutableList<(Type attributeType, object value)>> _cache = ImmutableDictionary<Type, ImmutableList<(Type, object)>>.Empty;

	public static T GetEnumAttribute<T>(this object obj) where T : Attribute
	{
		Type type = obj.GetType();
		return (T)type.GetMember(obj.ToString()).First(x => x.DeclaringType == type).GetCustomAttributes(typeof(T), inherit: false)[0];
	}

	public static bool HasEnumAttribute<T>(this object @object) where T : Attribute => @object.TryGetEnumAttribute<T>(out T result);

	public static bool TryGetEnumAttribute<T>(this object obj, out T result) where T : class
	{
		Type type = obj.GetType();
		object[] customAttributes = type.GetMember(obj.ToString()).First((MemberInfo x) => x.DeclaringType == type || x.DeclaringType.IsSubclassOf(type)).GetCustomAttributes(typeof(T), inherit: false);
		result = (T)customAttributes.FirstOrDefault();
		return customAttributes.Length != 0;
	}

	public static T GetAttribute<T>(this object obj) where T : Attribute
	{
		return obj.GetType().FindAttribute<T>() ?? throw new InvalidOperationException("Attribute " + typeof(T).Name + " not found on object of type " + obj.GetType().Name);
	}

	public static T GetAttribute<T>(this Type type) where T : Attribute
	{
		return type.FindAttribute<T>() ?? throw new InvalidOperationException("Attribute " + typeof(T).Name + " not found on type " + type.Name);
	}

	public static T FindAttribute<T>(this Type objectType) where T : Attribute
	{
		if (_cache.TryGetValue(objectType, out ImmutableList<(Type, object)> value))
		{
			foreach (var (type, obj) in value)
			{
				if (type == typeof(T))
				{
					return (T)obj;
				}
			}
			T val = objectType.LoadAttribute<T>();
			_cache = _cache.SetItem(objectType, value.Add((typeof(T), val)));
			return val;
		}
		T val2 = objectType.LoadAttribute<T>();
		_cache = _cache.SetItem(objectType, ImmutableList<(Type, object)>.Empty.Add((typeof(T), val2)));
		return val2;
	}

	private static T LoadAttribute<T>(this Type objectType)
	{
		object[] customAttributes = objectType.GetCustomAttributes(typeof(T), inherit: true);
		if (customAttributes.Length == 0)
		{
			throw new InvalidOperationException($"No custom attribute of type {typeof(T)} found for object type {objectType}");
		}
		return (T)customAttributes[0];
	}

	public static bool TryGetAttribute<T>(this PropertyInfo property, out T result) where T : Attribute
	{
		List<T> list = property.GetCustomAttributes<T>().ToList();
		result = ((list.Count > 0) ? list[0] : null);
		return list.Count > 0;
	}

	public static string GetEnumName<T>(this T value) where T : Enum
	{
		if (!value.TryGetEnumAttribute<NameAttribute>(out var result))
		{
			return value.ToString().SeparateWords();
		}
		return result.Value;
	}

	public static string GetEnumNameOrSentenceCase<T>(this T value) where T : Enum
	{
		if (!value.TryGetEnumAttribute<NameAttribute>(out var result))
		{
			return value.ToString().SeparateWords().ToSentenceCase();
		}
		return result.Value;
	}
}
