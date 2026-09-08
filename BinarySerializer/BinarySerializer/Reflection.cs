using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BinarySerializer;

internal static class Reflection
{
	public static Dictionary<Type, List<(int tag, MemberInfo memberInfO)>> SerializableProperties = new Dictionary<Type, List<(int, MemberInfo)>>();

	public static Dictionary<Type, Type> CollectionTypeArguments = new Dictionary<Type, Type>();

	public static Dictionary<Type, bool> IsAnonymousTypeCache = new Dictionary<Type, bool>();

	internal static Dictionary<Type, object> DefaultValues { get; } = new Dictionary<Type, object>();

	public static List<(int tag, MemberInfo memberInfo)> GetSerializableProperies(this Type type)
	{
		if (SerializableProperties.TryGetValue(type, out List<(int, MemberInfo)> value))
		{
			return value;
		}
		lock (SerializableProperties)
		{
			return SerializableProperties[type] = type.GetSerializableProperiesSlow();
		}
	}

	public static List<(int tag, MemberInfo memberInfo)> GetSerializableProperiesSlow(this Type type)
	{
		List<(int, MemberInfo)> list = new List<(int, MemberInfo)>();
		foreach (MemberInfo propertyMember in type.GetPropertyMembers())
		{
			if (propertyMember.TryGetPropertyTag(out var result))
			{
				list.Add((result, propertyMember));
			}
		}
		if (list.Count == 0)
		{
			list = (from property in type.GetProperties()
				where property.CanWrite && !property.IsIndexer()
				select property).WithIndex().MapToList(((int index, PropertyInfo value) indexProperty) => ((int tag, MemberInfo memberInfo))(tag: indexProperty.index + 1, memberInfo: indexProperty.value));
		}
		list.Select<(int, MemberInfo), int>(((int tag, MemberInfo memberInfo) item) => item.tag).VerifyDistinct(type.Name + " contains duplicate tag properties");
		return list;
	}

	public static bool IsIndexer(this PropertyInfo propertyInfo)
	{
		return propertyInfo.GetIndexParameters().Length != 0;
	}

	public static IEnumerable<MemberInfo> GetPropertyMembers(this Type type)
	{
		foreach (PropertyInfo item in type.GetProperties().Concat(type.GetProperties(BindingFlags.NonPublic)))
		{
			yield return item;
		}
		foreach (FieldInfo item2 in type.GetFields().Concat(type.GetFields(BindingFlags.NonPublic)))
		{
			yield return item2;
		}
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
		Type[] genericArguments = type.GetGenericArguments();
		if (genericArguments == null || genericArguments.Length != 1)
		{
			result = null;
			return false;
		}
		result = genericArguments[0];
		return true;
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

	public static object CreateListOfType(this IList<object?> values, Type listType)
	{
		if (listType.IsArray)
		{
			return listType.GetCollectionArgumentType().CreateArray(values);
		}
		if (listType.IsImmutableListType())
		{
			return CreateImmutableCollection(values, listType);
		}
		if (listType.IsImmutableHashSetType())
		{
			return CreateImmutableCollection(values, listType);
		}
		if (listType.IsImmutableArrayType())
		{
			return CreateImmutableCollection(values, listType);
		}
		if (listType.IsListType())
		{
			IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType.GetCollectionArgumentType()));
			{
				foreach (object value in values)
				{
					list.Add(value);
				}
				return list;
			}
		}
		if (listType.IsSortedSetType())
		{
			return values.GetGenericCollectionOfType(typeof(SortedSet<>), listType.GetCollectionArgumentType());
		}
		if (listType.IsHashSetType())
		{
			return values.GetGenericCollectionOfType(typeof(HashSet<>), listType.GetCollectionArgumentType());
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

	public static object GetGenericCollectionOfType(this IEnumerable<object> values, Type collectionType, Type argumentType)
	{
		Type type = collectionType.MakeGenericType(argumentType);
		object obj = Activator.CreateInstance(type);
		MethodInfo methodInfo = type.GetMethods().First((MethodInfo method) => method.Name == "Add");
		foreach (object value in values)
		{
			methodInfo.Invoke(obj, new object[1] { value });
		}
		return obj;
	}

	public static object GetDictionaryOfType(this IEnumerable<(object key, object value)> keyValues, Type dictionaryType)
	{
		if (dictionaryType.IsImmutableDictionaryType())
		{
			return keyValues.CreateImmutableDictionaryOfType(dictionaryType);
		}
		IDictionary dictionary = (IDictionary)dictionaryType.CreateEmptyDictionary();
		foreach (var (key, value) in keyValues)
		{
			dictionary.Add(key, value);
		}
		return dictionary;
	}

	public static object CreateImmutableDictionaryOfType(this IEnumerable<(object key, object value)> values, Type dictionaryType)
	{
		if (!dictionaryType.IsImmutableDictionaryType())
		{
			throw new InvalidOperationException("Expecting an immutable dictionary type but was " + dictionaryType.Name);
		}
		(Type keyType, Type valueType) dictionaryGenericArguments = dictionaryType.GetDictionaryGenericArguments();
		Type item = dictionaryGenericArguments.keyType;
		Type item2 = dictionaryGenericArguments.valueType;
		Type type = typeof(ImmutableDictionary<, >).MakeGenericType(item, item2);
		object obj = type.GetField("Empty")?.GetValue(null) ?? throw new InvalidOperationException("type " + type.Name + " doesn't have field 'Empty'");
		MethodInfo method = type.GetMethod("Add");
		foreach (var value in values)
		{
			object item3 = value.key;
			object item4 = value.value;
			obj = method?.Invoke(obj, new object[2] { item3, item4 });
		}
		if (obj == null)
		{
			throw new InvalidOperationException($"Can't create dictionary of type {dictionaryType}");
		}
		return obj;
	}

	public static object ToEnumValue(this int value, Type enumType)
	{
		return Enum.ToObject(enumType, value);
	}

	public static object CreateEmptyImmutableCollection(this Type collectionType, Type argumentType)
	{
		return collectionType.MakeGenericType(argumentType).GetField("Empty").GetValue(null);
	}

	public static object CreateEmptyCollection(this Type collectionType, Type argumentType)
	{
		return Activator.CreateInstance(collectionType.MakeGenericType(argumentType));
	}

	public static object CreateArray(this Type elementType, IList<object> elements)
	{
		Array array = Array.CreateInstance(elementType, elements.Count);
		for (int i = 0; i < elements.Count; i++)
		{
			array.SetValue(Convert.ChangeType(elements[i], elementType), i);
		}
		return array;
	}

	public static object CreateArray(this Type elementType, int size)
	{
		return Array.CreateInstance(elementType, size);
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

	public static bool IsSortedSetType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(SortedSet<>);
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

	public static bool IsImmutableArrayType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(ImmutableArray<>);
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

	public static bool TryGetAttribute<T>(this MemberInfo member, out T result) where T : Attribute
	{
		return (result = member.GetCustomAttribute<T>()) != null;
	}

	public static bool TryGetAttribute<T>(this object @object, out T result) where T : Attribute
	{
		object[] customAttributes = @object.GetCustomAttributes<T>();
		if (customAttributes.Length == 0)
		{
			result = null;
			return false;
		}
		result = (T)customAttributes[0];
		return true;
	}

	public static bool ContainsAttribute<T>(this object @object) where T : Attribute
	{
		return @object.GetCustomAttributes<T>().Length != 0;
	}

	private static object[] GetCustomAttributes<T>(this object @object)
	{
		if (!(@object is PropertyInfo propertyInfo))
		{
			if (!(@object is ParameterInfo parameterInfo))
			{
				return @object.GetType().GetCustomAttributes(typeof(T), inherit: true);
			}
			return parameterInfo.GetCustomAttributes(typeof(T), inherit: true);
		}
		return propertyInfo.GetCustomAttributes(typeof(T), inherit: true);
	}

	internal static object GetDefaultValue(this ParameterInfo parameter)
	{
		return parameter.ParameterType.GetDefaultValue();
	}

	internal static object GetDefaultValue(this Type type)
	{
		if (DefaultValues.TryGetValue(type, out object value))
		{
			return value;
		}
		lock (DefaultValues)
		{
			return DefaultValues[type] = type.GetDefaultValueSlow();
		}
	}

	internal static object GetDefaultValueSlow(this Type type)
	{
		if (type == typeof(string))
		{
			return null;
		}
		if (type == typeof(bool))
		{
			return false;
		}
		if (type == typeof(double))
		{
			return 0.0;
		}
		if (type == typeof(int))
		{
			return 0;
		}
		if (type == typeof(long))
		{
			return 0L;
		}
		if (type == typeof(decimal))
		{
			return 0.0;
		}
		if (type.IsEnum)
		{
			return 0.ToEnumValue(type);
		}
		if (type.IsImmutableListType())
		{
			return typeof(ImmutableList<>).CreateEmptyImmutableCollection(type.GetSingleGenericArgument());
		}
		if (type.IsListType())
		{
			return typeof(List<>).CreateEmptyCollection(type.GetSingleGenericArgument());
		}
		if (type.IsSortedSetType())
		{
			return typeof(SortedSet<>).CreateEmptyCollection(type.GetSingleGenericArgument());
		}
		if (type.IsImmutableDictionaryType())
		{
			return type.CreateEmptyImmutableDictionary();
		}
		if (type.IsImmutableArrayType())
		{
			return typeof(ImmutableArray<>).CreateEmptyImmutableCollection(type.GetSingleGenericArgument());
		}
		if (type.IsDictionaryType())
		{
			return type.CreateEmptyDictionary();
		}
		if (type.IsHashSetType())
		{
			return typeof(HashSet<>).CreateEmptyCollection(type.GetSingleGenericArgument());
		}
		if (type.IsImmutableHashSetType())
		{
			return typeof(ImmutableHashSet<>).CreateEmptyImmutableCollection(type.GetSingleGenericArgument());
		}
		if (type.IsArray)
		{
			return type.GetElementType().CreateArray(0);
		}
		if (type == typeof(DateTime))
		{
			return DateTime.MinValue;
		}
		if (type == typeof(TimeSpan))
		{
			return TimeSpan.Zero;
		}
		if (type == typeof(Guid))
		{
			return Guid.Empty;
		}
		if (type.TryGetEmptyConstructor(out ConstructorInfo result))
		{
			return result.Invoke(Array.Empty<object>());
		}
		return null;
	}

	public static bool TryGetEmptyConstructor(this Type type, out ConstructorInfo result)
	{
		return type.GetConstructors().TryGet<ConstructorInfo>((ConstructorInfo constructor) => constructor.GetParameters().Length == 0, out result);
	}

	public static bool TryGetMainConstructor(this Type type, out ConstructorInfo result)
	{
		result = null;
		List<ConstructorInfo> list = (from constructor in type.GetConstructors()
			where constructor.GetParameters().Length != 0
			select constructor).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		result = list.WithMaxValue((ConstructorInfo constructor) => constructor.GetParameters().Length).First();
		return true;
	}

	public static bool IsCollection(this Type type)
	{
		if (type == typeof(string))
		{
			return false;
		}
		if (type.IsDictionaryType() || type.IsImmutableDictionaryType())
		{
			return false;
		}
		if (typeof(ICollection).IsAssignableFrom(type))
		{
			return true;
		}
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(ICollection<>))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsStruct(this Type type)
	{
		if ((object)type != null && type.IsValueType)
		{
			return !type.IsPrimitive;
		}
		return false;
	}

	public static bool IsTupleType(this Type type)
	{
		if (!type.IsGenericType)
		{
			return false;
		}
		if (type.GetGenericTypeDefinition() == typeof(ValueTuple<>))
		{
			return true;
		}
		if (type.FullName != null && type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal))
		{
			return true;
		}
		return false;
	}

	public static MethodInfo GetValueTupleConstructor(this Type[] types)
	{
		string text = types.Length switch
		{
			2 => "GetValueTupleTwo", 
			3 => "GetValueTupleThree", 
			_ => throw new InvalidOperationException(), 
		};
		MethodInfo? method = typeof(Reflection).GetMethod(text, BindingFlags.Static | BindingFlags.Public);
		if (method == null)
		{
			throw new InvalidOperationException("Method " + text + " not found");
		}
		return method.MakeGenericMethod(types);
	}

	public static (T1, T2) GetValueTupleTwo<T1, T2>(T1 value1, T2 value2)
	{
		return (value1, value2);
	}

	public static (T1, T2, T3) GetValueTupleThree<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
	{
		return (value1, value2, value3);
	}

	public static bool IsRecordType(this Type type)
	{
		if (type.GetMethods().Any((MethodInfo method) => method.Name.StartsWith("<Clone>$")))
		{
			return type.GetProperty("EqualityContract", BindingFlags.Instance | BindingFlags.NonPublic) != null;
		}
		return false;
	}

	public static bool TryGetMatchingParameter(this ConstructorInfo constructor, MemberInfo member, out ParameterInfo result)
	{
		ParameterInfo[] parameters = constructor.GetParameters();
		foreach (ParameterInfo parameterInfo in parameters)
		{
			if (parameterInfo.Name == null)
			{
				throw new InvalidOperationException("Parameters with null names aren't expected");
			}
			if (parameterInfo.Name.CamelToPascalCase() == member.Name.CamelToPascalCase() || parameterInfo.Name.PascalToCamelCase() == member.Name.PascalToCamelCase())
			{
				result = parameterInfo;
				return true;
			}
		}
		result = null;
		return false;
	}

	public static Type GetCollectionArgumentType(this Type type)
	{
		if (CollectionTypeArguments.TryGetValue(type, out Type value))
		{
			return value;
		}
		lock (CollectionTypeArguments)
		{
			return CollectionTypeArguments[type] = type.GetCollectionArgumentTypeSlow();
		}
	}

	public static Type GetCollectionArgumentTypeSlow(this Type type)
	{
		if (!type.TryGetCollectionArgumentType(out Type result))
		{
			throw new InvalidOperationException("Failed to GetCollectionArgumentType from " + type.Name);
		}
		return result;
	}

	public static bool TryGetCollectionArgumentType(this Type type, out Type result)
	{
		if (type.IsArray)
		{
			result = type.GetElementType();
			return true;
		}
		if (!type.HasGenericArguments())
		{
			throw new NotImplementedException("Non-generic collections aren't implemented yet (" + type.Name + ")");
		}
		return type.TryGetSingleGenericArgument(out result);
	}

	public static bool IsAnonymousType(this Type type)
	{
		if (IsAnonymousTypeCache.TryGetValue(type, out var value))
		{
			return value;
		}
		lock (IsAnonymousTypeCache)
		{
			return IsAnonymousTypeCache[type] = type.IsAnonymousTypeSlow();
		}
	}

	public static bool IsAnonymousTypeSlow(this Type type)
	{
		bool num = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Length != 0;
		bool flag = type.FullName.Contains("AnonymousType");
		return num && flag;
	}

	public static bool ContructorParameterMatchesPropertyName(this string parameterName, string propertyName, Type declaringType)
	{
		if (!parameterName.CamelToPascalCase().Equals(propertyName.CamelToPascalCase()) && !parameterName.PascalToCamelCase().Equals(propertyName.PascalToCamelCase()))
		{
			if (declaringType.IsTupleType())
			{
				return parameterName.Replace("value", "Item").Equals(propertyName);
			}
			return false;
		}
		return true;
	}

	public static IEnumerable<MethodInfo> GetAllStaticFunctions(this Assembly assembly)
	{
		return from method in assembly.GetTypes().SelectMany((Type type) => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			where method.IsStatic
			select method;
	}

	public static bool ContainsAttribute<T>(this MethodInfo method) where T : Attribute
	{
		return method.GetCustomAttributes(typeof(T), inherit: false).Length != 0;
	}

	public static bool IsGenericInstanceOf(this Type type, Type genericType)
	{
		if (type.IsGenericType)
		{
			return type.Name == genericType.Name;
		}
		return false;
	}

	public static bool HasAttribute<T>(this MemberInfo member) where T : Attribute
	{
		return member.CustomAttributes.Any((CustomAttributeData attributeData) => attributeData.AttributeType == typeof(T));
	}

	public static bool IsNullableType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	public static bool IsNullableType(this Type type, out Type nullableValueType)
	{
		if (type.IsNullableType())
		{
			nullableValueType = type.GetGenericArguments()[0];
			return true;
		}
		nullableValueType = null;
		return false;
	}

	public static void SetPropertyValue(this MemberInfo member, object @object, object? value)
	{
		if (member is PropertyInfo propertyInfo)
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic: true);
			if (setMethod == null)
			{
				if (!propertyInfo.TryGetBackingField(out FieldInfo result))
				{
					throw new InvalidOperationException($"Property {@object.GetType()}.{propertyInfo.Name} has neigther a setter nor a backing field");
				}
				result.SetValue(@object, value);
			}
			else
			{
				setMethod.Invoke(@object, new object[1] { value });
			}
		}
		else
		{
			member.VerifyType<FieldInfo>().SetValue(@object, value);
		}
	}

	public static bool TryGetBackingField(this PropertyInfo propertyInfo, out FieldInfo result)
	{
		result = propertyInfo.DeclaringType.GetField("<" + propertyInfo.Name + ">k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
		return result != null;
	}

	public static IEnumerable<ConstructorInfo> GetConstructorsIncludingProtected(this Type type)
	{
		ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		ConstructorInfo[] array = constructors;
		foreach (ConstructorInfo constructorInfo in array)
		{
			if (!constructorInfo.IsRecordOriginalConstructor())
			{
				yield return constructorInfo;
			}
		}
	}

	public static bool IsRecordOriginalConstructor(this ConstructorInfo constructor)
	{
		if (constructor.DeclaringType.IsRecordType())
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			if (parameters.Length == 1 && parameters[0].ParameterType == constructor.DeclaringType)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsSystemType(this Type type)
	{
		return type.Namespace?.StartsWith("System") ?? false;
	}

	public static bool HasAttribute<T>(this Type type) where T : Attribute
	{
		return Attribute.IsDefined(type, typeof(T));
	}

	public static FieldInfo GetPrivateFieldOrThrow(this Type type, string fieldName)
	{
		return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"{typeof(MemoryStream)} doesn't have field {fieldName}");
	}

	public static bool IsInlineArray(this Type type)
	{
		return type.HasAttribute<InlineArrayAttribute>();
	}
}
