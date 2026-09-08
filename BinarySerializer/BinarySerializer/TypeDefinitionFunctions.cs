using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace BinarySerializer;

internal static class TypeDefinitionFunctions
{
	public static Dictionary<Type, ITypeDefinition> TypeDefinitions = new Dictionary<Type, ITypeDefinition>();

	public static ITypeDefinition GetTypeDefinition(this Type type)
	{
		if (TypeDefinitions.TryGetValue(type, out ITypeDefinition value))
		{
			return value;
		}
		lock (TypeDefinitions)
		{
			return TypeDefinitions[type] = type.GetTypeDefinitionSlow();
		}
	}

	private static ITypeDefinition GetTypeDefinitionSlow(this Type type)
	{
		if (TypeDefinitions.TryGetValue(type, out ITypeDefinition value))
		{
			return value;
		}
		if (type.IsDictionaryType() || type.IsImmutableDictionaryType())
		{
			var (type2, type3) = type.GetDictionaryGenericArguments();
			return new DictionaryTypeDefinition(type, new ObjectTypeDefinition((type2, type3).GetType(), ImmutableDictionary<int, ITypeDefinition>.Empty.Add(1, type2.GetTypeDefinition()).Add(2, type3.GetTypeDefinition())));
		}
		if (type.IsCollection())
		{
			return new CollectionTypeDefinition(type, type.GetCollectionArgumentType().GetTypeDefinition());
		}
		if (type == typeof(string))
		{
			return StringTypeDefinition.Instance;
		}
		if (type.IsInterface)
		{
			return new InterfaceTypeDefinition(type, type.GetInterfaceSubTypes().ToImmutableDictionary<(int, Type), int, Type>(((int tag, Type type) item) => item.tag, ((int tag, Type type) item) => item.type));
		}
		if (type.TryGetConvertedTypeDefinition(out ConvertedTypeDefinition result))
		{
			return result;
		}
		if (type.IsNullableType(out Type nullableValueType))
		{
			return new NullableTypeDefinition(nullableValueType.GetTypeDefinition());
		}
		if (type.IsPrimitive || type.IsEnum || type == typeof(DateTime) || type == typeof(TimeSpan))
		{
			return new NumericTypeDefinition(type);
		}
		return new ObjectTypeDefinition(type, type.GetSerializableProperies().ToImmutableDictionary<(int, MemberInfo), int, ITypeDefinition>(((int tag, MemberInfo memberInfo) memberId) => memberId.tag, ((int tag, MemberInfo memberInfo) memberId) => memberId.memberInfo.GetPropertyType().GetTypeDefinition()));
	}

	public static bool TryGetConvertedTypeDefinition(this Type type, out ConvertedTypeDefinition result)
	{
		if (type.TryGetSerializationConverter(out MethodInfo result2))
		{
			Type returnType = result2.ReturnType;
			result = new ConvertedTypeDefinition(returnType.GetTypeDefinition());
			return true;
		}
		result = null;
		return false;
	}

	public static IEnumerable<(int tag, ObjectTypeDefinition definition)> GetInterfaceSubTypeDefinitions(this Type type)
	{
		return from tagType in type.GetInterfaceSubTypes()
			select (tag: tagType.tag, tagType.type.GetTypeDefinition().VerifyType<ObjectTypeDefinition>());
	}

	public static IEnumerable<(int tag, Type type)> GetInterfaceSubTypes(this Type type)
	{
		return from attribute in type.GetCustomAttributes<TypeTagAttribute>().DistinctBy((TypeTagAttribute attribute) => attribute.Tag)
			select (Tag: attribute.Tag, Type: attribute.Type);
	}

	public static string GetNameWithGenerics(this Type type)
	{
		if (!type.IsGenericType)
		{
			return type.Name;
		}
		string name = type.GetGenericTypeDefinition().Name;
		name = name.Substring(0, name.IndexOf('`'));
		string text = type.GetGenericArguments().Select(GetNameWithGenerics).Join(", ");
		return name + "<" + text + ">";
	}
}
