using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using CSharpSerializer.Common;

namespace CSharpSerializer;

internal static class Reflection
{
	public static IEnumerable<MemberInfo> GetPropertyMembers(this object @object)
	{
		if (@object is Type type)
		{
			return type.GetPropertyMembers();
		}
		return @object.GetType().GetPropertyMembers();
	}

	public static IEnumerable<MemberInfo> GetPropertyMembers(this Type type)
	{
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < properties.Length; i++)
		{
			yield return properties[i];
		}
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			yield return fields[i];
		}
	}

	public static bool HasPublicSetter(this MemberInfo memberInfo)
	{
		if (memberInfo.VerifyPropertyMember() is FieldInfo)
		{
			return true;
		}
		if (memberInfo is PropertyInfo property)
		{
			return property.HasPublicSetter();
		}
		throw new InvalidOperationException("Should not reach here");
	}

	public static object? GetPropertyValue(this MemberInfo member, object @object)
	{
		if (member is PropertyInfo propertyInfo)
		{
			return propertyInfo.GetValue(@object);
		}
		if (member is FieldInfo fieldInfo)
		{
			return fieldInfo.GetValue(@object);
		}
		throw new NotSupportedException();
	}

	public static object SetPropertyValue(this object target, MemberInfo member, object value)
	{
		if (member.VerifyPropertyMember() is FieldInfo fieldInfo)
		{
			fieldInfo.SetValue(target, value);
		}
		if (member is PropertyInfo propertyInfo)
		{
			propertyInfo.SetValue(target, value);
		}
		return target;
	}

	public static MemberInfo VerifyPropertyMember(this MemberInfo member)
	{
		if ((!(member is FieldInfo) && !(member is PropertyInfo)) || 1 == 0)
		{
			throw new InvalidOperationException($"expecting a property or field info but was {member}");
		}
		return member;
	}

	public static bool HasGenericArguments(this Type type)
	{
		return type.GetGenericArguments().Length != 0;
	}

	public static Type GetSingleGenericArgument(this Type type)
	{
		if (!type.TryGetSingleGenericArgument(out Type result))
		{
			throw new InvalidOperationException("Failed to get single generic argument of type " + type.Name);
		}
		return result;
	}

	public static bool TryGetSingleGenericArgument(this Type type, out Type result)
	{
		if (type.IsArray)
		{
			result = type.GetArrayArgumentType();
			return true;
		}
		Type[] genericArguments = type.GetGenericArguments();
		if (genericArguments == null || genericArguments.Length != 1)
		{
			result = null;
			return false;
		}
		result = genericArguments[0];
		return true;
	}

	public static Type GetArrayArgumentType(this Type arrayType)
	{
		if (!arrayType.IsArray)
		{
			throw new InvalidOperationException("Given type is not an array (" + arrayType.Name + ")");
		}
		return arrayType.GetElementType();
	}

	public static (Type keyType, Type valueType) GetDictionaryGenericArguments(this Type type)
	{
		Type[] genericArguments = type.GetGenericArguments();
		if (genericArguments == null || genericArguments.Length != 2)
		{
			throw new InvalidOperationException("Failed to GetDictionaryGenericArguments of type " + type.Name.Quoted());
		}
		return (keyType: genericArguments[0], valueType: genericArguments[1]);
	}

	public static object CreateListOfType(this IEnumerable<object> values, Type listType)
	{
		if (!listType.TryGetSingleGenericArgument(out Type result))
		{
			throw new NotImplementedException("Non-generic collections are not implemented yet");
		}
		if (listType.IsImmutableListType())
		{
			return CreateImmutableCollection(values, listType);
		}
		if (listType.IsImmutableHashSetType())
		{
			return CreateImmutableCollection(values, listType);
		}
		if (listType.IsListType())
		{
			return result.CreateGenericList(values);
		}
		if (listType.IsHashSetType())
		{
			return result.CreateGenericHashSet(values);
		}
		if (listType.IsArray)
		{
			return result.CreateArray(values);
		}
		if (listType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
		{
			return values.Cast(result);
		}
		throw new NotImplementedException("List of type " + listType.Name.Quoted() + " is not implemented");
		static object CreateImmutableCollection(IEnumerable<object> enumerable, Type collectionType)
		{
			object obj = collectionType.GetField("Empty").GetValue(null);
			MethodInfo method = collectionType.GetMethod("Add");
			foreach (object item in enumerable)
			{
				obj = method.Invoke(obj, new object[1] { item });
			}
			return obj;
		}
	}

	public static object CreateInlineArray(this IEnumerable<object> values, Type inlineArrayType)
	{
		MethodInfo? obj = inlineArrayType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public) ?? throw new InvalidOperationException("Failed to get Create method for " + inlineArrayType.Name);
		inlineArrayType.GetSingleGenericArgument();
		return obj.Invoke(null, new object[1] { values });
	}

	public static object CreateDictionaryOfType(this IEnumerable<KeyValuePair<object, object>> values, Type dictionaryType)
	{
		if (dictionaryType.IsImmutableDictionaryType())
		{
			return values.CreateImmutableDictionaryOfType(dictionaryType);
		}
		IDictionary dictionary = (IDictionary)dictionaryType.CreateEmptyDictionary();
		foreach (var (key, value) in values)
		{
			dictionary.Add(key, value);
		}
		return dictionary;
	}

	public static object CreateImmutableDictionaryOfType(this IEnumerable<KeyValuePair<object, object>> values, Type dictionaryType)
	{
		if (!dictionaryType.IsImmutableDictionaryType())
		{
			throw new InvalidOperationException("Expecting an immutable dictionary type but was " + dictionaryType.Name);
		}
		(Type keyType, Type valueType) dictionaryGenericArguments = dictionaryType.GetDictionaryGenericArguments();
		Type item = dictionaryGenericArguments.keyType;
		Type item2 = dictionaryGenericArguments.valueType;
		Type type = typeof(ImmutableDictionary<, >).MakeGenericType(item, item2);
		object obj = type.GetField("Empty").GetValue(null);
		MethodInfo method = type.GetMethod("Add");
		foreach (KeyValuePair<object, object> value in values)
		{
			obj = method.Invoke(obj, new object[2] { value.Key, value.Value });
		}
		return obj;
	}

	public static object ToEnumValue(this int value, Type enumType)
	{
		return Enum.ToObject(enumType, value);
	}

	public static object CreateGenericList(this Type genericArgumentType, IEnumerable<object> items)
	{
		IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericArgumentType));
		foreach (object item in items)
		{
			list.Add(item);
		}
		return list;
	}

	public static object CreateArray(this Type elementType, IEnumerable<object> items)
	{
		Array array = Array.CreateInstance(elementType, items.Count());
		int num = 0;
		foreach (object item in items)
		{
			array.SetValue(Convert.ChangeType(item, elementType), num++);
		}
		return array;
	}

	public static object CreateGenericHashSet(this Type genericArgumentType, IEnumerable<object> items)
	{
		Type type = typeof(HashSet<>).MakeGenericType(genericArgumentType);
		object obj = Activator.CreateInstance(type);
		MethodInfo methodInfo = type.GetMethod("Add") ?? throw new InvalidOperationException("Add method not found in " + type.Name);
		foreach (object item in items)
		{
			methodInfo.Invoke(obj, new object[1] { item });
		}
		return obj;
	}

	public static object CreateEmptyImmutableList(this Type argumentType)
	{
		return typeof(ImmutableList<>).MakeGenericType(argumentType).GetField("Empty").GetValue(null);
	}

	public static object CreateEmptyImmutableHashSet(this Type argumentType)
	{
		return typeof(ImmutableHashSet<>).MakeGenericType(argumentType).GetField("Empty").GetValue(null);
	}

	public static object CreateEmptyList(this Type argumentType)
	{
		return Activator.CreateInstance(typeof(List<>).MakeGenericType(argumentType));
	}

	public static object CreateEmptyDictionary(this Type dictionaryType)
	{
		var (type, type2) = dictionaryType.GetDictionaryGenericArguments();
		return Activator.CreateInstance(typeof(Dictionary<, >).MakeGenericType(type, type2));
	}

	public static object CreateEmptyImmutableDictionary(this Type dictionaryType)
	{
		var (type, type2) = dictionaryType.GetDictionaryGenericArguments();
		return typeof(ImmutableDictionary<, >).MakeGenericType(type, type2).GetField("Empty").GetValue(null);
	}

	public static bool IsListType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(List<>);
		}
		return false;
	}

	public static bool IsHashSetType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(HashSet<>);
		}
		return false;
	}

	public static bool IsImmutableListType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(ImmutableList<>);
		}
		return false;
	}

	public static bool IsImmutableHashSetType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(ImmutableHashSet<>);
		}
		return false;
	}

	public static bool IsDictionaryType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Dictionary<, >);
		}
		return false;
	}

	public static bool IsImmutableDictionaryType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(ImmutableDictionary<, >);
		}
		return false;
	}

	public static bool IsEnumerableType(this Type type)
	{
		if (typeof(IEnumerable).IsAssignableFrom(type))
		{
			return type != typeof(string);
		}
		return false;
	}

	public static Type GetPropertyType(this MemberInfo member)
	{
		if (member is PropertyInfo propertyInfo)
		{
			return propertyInfo.PropertyType;
		}
		if (member is FieldInfo fieldInfo)
		{
			return fieldInfo.FieldType;
		}
		throw new InvalidOperationException($"member {member} is not a property");
	}

	public static bool IsPrimitiveOrEnum(this Type type)
	{
		if (!type.IsPrimitive)
		{
			return type.IsEnum;
		}
		return true;
	}

	public static bool HasAttribute<T>(this MemberInfo member) where T : Attribute
	{
		return member.GetCustomAttribute<T>() != null;
	}

	public static bool TryGetAttribute<T>(this Type type, out T result) where T : Attribute
	{
		result = type.GetCustomAttribute<T>();
		return result != null;
	}

	public static Type GetNonByRefType(this Type type)
	{
		if (type.IsByRef)
		{
			return type.GetElementType();
		}
		return type;
	}

	public static bool TryGetEnumerator(this object @object, out IEnumerator result)
	{
		result = null;
		MethodInfo method = @object.GetType().GetMethod("GetEnumerator", BindingFlags.Instance | BindingFlags.Public);
		if (method == null)
		{
			return false;
		}
		result = method.Invoke(@object, null).VerifyType<IEnumerator>();
		return true;
	}

	public static bool IsNullableType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	public static bool IsNullableType(this Type type, out Type argumentType)
	{
		if (!type.IsNullableType())
		{
			return (argumentType = null) != null;
		}
		return (argumentType = type.GetSingleGenericArgument()) != null;
	}
}
