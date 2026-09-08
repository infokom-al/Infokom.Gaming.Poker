using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using CSharpSerializer.Common;
using CSharpSerializer.Serialization;

namespace CSharpSerializer;

public static class SerializationHelper
{
	public static IObjectMapValue GetObjectMapValue<T>(this T value)
	{
		return value.GetObjectMapValue(typeof(T));
	}

	public static IObjectMapValue GetObjectMapValue(this object value, Type declaredType)
	{
		if (declaredType.ShouldIncludeExplicitType(value))
		{
			(string, StringMapValue) tuple = ("__typename", value.GetType().GetTypeFieldValue().ToStringValue());
			(string, StringMapValue) tuple2;
			if (value.TryConvertToPrimitiveObjectMapValue(out IObjectMapValue result))
			{
				_003C_003Ey__InlineArray2<(string, IObjectMapValue)> buffer = default(_003C_003Ey__InlineArray2<(string, IObjectMapValue)>);
				ref(string, IObjectMapValue) reference = ref buffer[0];
				tuple2 = tuple;
				reference = (tuple2.Item1, tuple2.Item2);
				buffer[1] = ("value", result);
				return new ObjectMap(ImmutableList.Create<(string, IObjectMapValue)>(buffer));
			}
			if (value.TryConvertToDictionaryValue(out DictionaryMap result2))
			{
				_003C_003Ey__InlineArray2<(string, IObjectMapValue)> buffer2 = default(_003C_003Ey__InlineArray2<(string, IObjectMapValue)>);
				ref(string, IObjectMapValue) reference2 = ref buffer2[0];
				tuple2 = tuple;
				reference2 = (tuple2.Item1, tuple2.Item2);
				buffer2[1] = ("values", result2);
				return new ObjectMap(ImmutableList.Create<(string, IObjectMapValue)>(buffer2));
			}
			if (value.TryConvertToCollectionValue(out CollectionMap result3))
			{
				_003C_003Ey__InlineArray2<(string, IObjectMapValue)> buffer3 = default(_003C_003Ey__InlineArray2<(string, IObjectMapValue)>);
				ref(string, IObjectMapValue) reference3 = ref buffer3[0];
				tuple2 = tuple;
				reference3 = (tuple2.Item1, tuple2.Item2);
				buffer3[1] = ("values", result3);
				return new ObjectMap(ImmutableList.Create<(string, IObjectMapValue)>(buffer3));
			}
			List<(string, IObjectMapValue)> list = new List<(string, IObjectMapValue)>();
			tuple2 = tuple;
			list.Add((tuple2.Item1, tuple2.Item2));
			list.AddRange(GetProperties());
			return new ObjectMap(ImmutableList.Create(new ReadOnlySpan<(string, IObjectMapValue)>(list.ToArray())));
		}
		if (value.TryConvertToPrimitiveObjectMapValue(out IObjectMapValue result4))
		{
			return result4;
		}
		if (value.TryConvertToDictionaryValue(out DictionaryMap result5))
		{
			return result5;
		}
		if (value.TryConvertToCollectionValue(out CollectionMap result6))
		{
			return result6;
		}
		return new ObjectMap(GetProperties().ToImmutableList());
		IEnumerable<(string name, IObjectMapValue value)> GetProperties()
		{
			foreach (MemberInfo propertyMember in value.GetPropertyMembers())
			{
				if (propertyMember.ShouldSerialize(value))
				{
					object propertyValue = propertyMember.GetPropertyValue(value);
					yield return (name: propertyMember.Name, value: propertyValue.GetObjectMapValue(propertyMember.GetPropertyType()));
				}
			}
		}
	}

	public static bool TryConvertToPrimitiveObjectMapValue(this object @object, out IObjectMapValue result)
	{
		IObjectMapValue objectMapValue = ((@object is byte value) ? new IntMapValue(value) : ((@object is ushort value2) ? new IntMapValue(value2) : ((@object is short value3) ? new IntMapValue(value3) : ((@object is int value4) ? new IntMapValue(value4) : ((@object is double value5) ? new DoubleMapValue(value5) : ((@object is decimal value6) ? new DecimalMapValue(value6) : ((@object is float value7) ? new FloatMapValue(value7) : ((@object is string value8) ? new StringMapValue(value8) : ((@object is long value9) ? new LongMapValue(value9) : ((@object is bool value10) ? new BoolMapValue(value10) : ((!(@object is DateTime dateTime)) ? ((IObjectMapValue)(@object.GetType().IsEnum ? new IntMapValue(Convert.ToInt32(@object)) : null)) : ((IObjectMapValue)new LongMapValue(dateTime.Ticks)))))))))))));
		result = objectMapValue;
		return result != null;
	}

	public static bool IsPrimimiteObjectMapValueType(this Type type)
	{
		if (type.IsEnum)
		{
			return true;
		}
		if (!(type == typeof(byte)) && !(type == typeof(ushort)) && !(type == typeof(short)) && !(type == typeof(int)) && !(type == typeof(double)) && !(type == typeof(decimal)) && !(type == typeof(float)) && !(type == typeof(string)) && !(type == typeof(long)) && !(type == typeof(bool)) && !(type == typeof(DateTime)))
		{
			return type.IsEnum;
		}
		return true;
	}

	internal static bool TryConvertToDictionaryValue(this object @object, out DictionaryMap result)
	{
		result = null;
		IDictionary dictionary = @object as IDictionary;
		if (dictionary == null)
		{
			return false;
		}
		(Type, Type) dictionaryGenericArguments = @object.GetType().GetDictionaryGenericArguments();
		Type keyType = dictionaryGenericArguments.Item1;
		Type valueType = dictionaryGenericArguments.Item2;
		result = new DictionaryMap(dictionary.Keys.MapToImmutableList((object key) => (key: key.GetObjectMapValue(keyType), value: dictionary[key].GetObjectMapValue(valueType))));
		return true;
	}

	internal static bool TryConvertToCollectionValue(this object @object, out CollectionMap result)
	{
		result = null;
		if (!(@object is IEnumerable enumerable))
		{
			if (@object.ShouldSerializeAsCollection())
			{
				result = @object.SerializeAsCollection();
				return true;
			}
			return false;
		}
		if (!@object.GetType().HasGenericArguments() && !@object.GetType().IsArray)
		{
			throw new NotImplementedException("Non-generic collections aren't implemented yet");
		}
		if (!@object.GetType().TryGetSingleGenericArgument(out Type argumentType))
		{
			return false;
		}
		result = new CollectionMap(enumerable.MapToImmutableList((object item) => item.GetObjectMapValue(argumentType)));
		return true;
	}

	internal static CollectionMap SerializeAsCollection(this object @object)
	{
		if (!@object.TryGetEnumerator(out IEnumerator result))
		{
			throw new InvalidOperationException(@object.GetType().Name.Quoted() + " doesn't support enumeration");
		}
		ImmutableList<IObjectMapValue>.Builder builder = ImmutableList.CreateBuilder<IObjectMapValue>();
		while (result.MoveNext())
		{
			object current = result.Current;
			if (current != null)
			{
				builder.Add(current.GetObjectMapValue(current.GetType()));
			}
		}
		return new CollectionMap(builder.ToImmutable());
	}

	internal static bool ShouldIncludeExplicitType(this Type propertyType, object propertyValue)
	{
		if (propertyType.IsNullableType(out Type argumentType))
		{
			return argumentType != propertyValue.GetType();
		}
		return propertyType != propertyValue.GetType();
	}

	public static bool ShouldSerializeAsCollection(this object value)
	{
		if (!(value is IEnumerable))
		{
			return value.GetType().HasSerializeAsCollectionAttribute();
		}
		return true;
	}

	public static bool HasSerializeAsCollectionAttribute(this Type type)
	{
		return type.HasAttribute<JsonSerializeAsCollectionAttribute>();
	}

	internal static bool ShouldSerialize(this MemberInfo member, object memberParentValue)
	{
		if (member.HasAttribute<SerializationIgnoreAttribute>() || member.HasAttribute<JsonIgnoreAttribute>())
		{
			return false;
		}
		if (member is FieldInfo)
		{
			return true;
		}
		if (member is PropertyInfo property && property.HasPublicSetter())
		{
			return ShouldSerializeValue();
		}
		if (!memberParentValue.TryGetMainConstructor(out IJsonConstructor result))
		{
			return false;
		}
		if (!result.TryGetMatchingParameter(member, out ParameterInfo _))
		{
			return false;
		}
		return ShouldSerializeValue();
		bool ShouldSerializeValue()
		{
			object propertyValue;
			try
			{
				propertyValue = member.GetPropertyValue(memberParentValue);
			}
			catch
			{
				return false;
			}
			Type propertyType = member.GetPropertyType();
			return !propertyValue.IsDefaultValue(propertyType);
		}
	}

	internal static bool TryGetMainConstructor(this object @object, out IJsonConstructor result)
	{
		return @object.GetType().TryGetMainConstructor(out result);
	}

	internal static bool TryGetMainConstructor(this Type type, out IJsonConstructor result)
	{
		result = null;
		if (type.TryGetJsonAttributeConstructor(out ConstructorInfo result2))
		{
			result = result2.ToDefaultJsonConstructor();
			return true;
		}
		List<IJsonConstructor> list = (from constructor in type.GetJsonConstructors()
			where constructor.HasParameters()
			select constructor).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		result = list.WithMaxValue((IJsonConstructor constructor) => constructor.GetParameters().Count()).First();
		return true;
	}

	internal static bool TryGetJsonAttributeConstructor(this Type type, out ConstructorInfo result)
	{
		result = null;
		List<ConstructorInfo> list = (from constructor in type.GetConstructors()
			where constructor.GetParameters().Length != 0
			select constructor).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		if (list.TryGet((ConstructorInfo constructor) => constructor.GetCustomAttribute<JsonConstructorAttribute>() != null, out ConstructorInfo result2))
		{
			result = result2;
			return true;
		}
		return false;
	}

	internal static bool IsDefaultValue(this object? memberValue, Type memberType)
	{
		if (memberType.IsNullableType())
		{
			return memberValue == null;
		}
		if (memberValue == null)
		{
			return true;
		}
		if (memberValue is string text && text == null)
		{
			return true;
		}
		if (memberValue is ICollection { Count: 0 })
		{
			return true;
		}
		if (memberValue is int && (int)memberValue == 0)
		{
			return true;
		}
		if (memberValue is long num && num == 0L)
		{
			return true;
		}
		if (memberValue is float num2 && num2 == 0f)
		{
			return true;
		}
		if (memberValue is decimal num3 && num3 == 0m)
		{
			return true;
		}
		if (memberValue is double value && value.IsZero())
		{
			return true;
		}
		if (memberValue is DateTime dateTime && dateTime == default(DateTime))
		{
			return true;
		}
		if (memberValue.GetType().IsEnum && Convert.ToInt32(memberValue) == 0)
		{
			return true;
		}
		if (memberValue.IsDefaultStructValue())
		{
			return true;
		}
		return false;
	}

	internal static bool IsDefaultStructValue(this object? value)
	{
		if (value == null)
		{
			return false;
		}
		Type type = value.GetType();
		if (!type.IsValueType || type.IsEnum || type.IsPrimitive)
		{
			return false;
		}
		return value.Equals(Activator.CreateInstance(type));
	}

	internal static StringMapValue ToStringValue(this string @string)
	{
		return new StringMapValue(@string);
	}

	internal static IntMapValue ToIntValue(this int value)
	{
		return new IntMapValue(value);
	}

	internal static LongMapValue ToLongValue(this long value)
	{
		return new LongMapValue(value);
	}

	internal static DoubleMapValue ToDoubleValue(this double value)
	{
		return new DoubleMapValue(value);
	}

	internal static BoolMapValue ToBoolValue(this bool value)
	{
		return new BoolMapValue(value);
	}

	internal static bool HasPublicSetter(this PropertyInfo property)
	{
		MethodInfo setMethod = property.GetSetMethod();
		if (setMethod == null)
		{
			return false;
		}
		return setMethod.IsPublic;
	}

	internal static bool TryGetMatchingParameter(this IJsonConstructor constructor, MemberInfo member, out ParameterInfo result)
	{
		if (constructor.GetParameters().TryGet<ParameterInfo>((ParameterInfo parameter) => (parameter.Name ?? throw new InvalidOperationException("Nameless constructor parameters aren't suported")).ConstructorParameterNameEquals(member.Name), out result))
		{
			return true;
		}
		return false;
	}

	internal static bool ConstructorParameterNameEquals(this string constructorParameterName, string otherName)
	{
		if (constructorParameterName == otherName)
		{
			return true;
		}
		if (constructorParameterName.Length != otherName.Length)
		{
			return false;
		}
		if (constructorParameterName == otherName.PascalToCamelCase())
		{
			return true;
		}
		if (constructorParameterName.CamelToPascalCase() == otherName)
		{
			return true;
		}
		return false;
	}

	internal static bool IsPrimitiveType(this IObjectMapValue value)
	{
		if (!(value is CollectionMap) && !(value is DictionaryMap))
		{
			return !(value is ObjectMap);
		}
		return false;
	}

	public static string GetTypeFieldValue(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetDeclatedTypeName() + "<" + type.GetGenericArguments().Join((Type argument) => argument.GetTypeFieldValue(), ", ") + ">";
		}
		return type.GetDeclatedTypeName();
	}

	public static string GetDeclatedTypeName(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.Name.Substring(0, type.Name.IndexOf('`'));
		}
		return type.Name;
	}

	public static IEnumerable<IJsonConstructor> GetJsonConstructors(this Type type)
	{
		if (type.TryGetSingleJsonConstructorMethod(out MethodInfo result))
		{
			yield return result.ToMethodJsonConstructor();
			yield break;
		}
		foreach (MethodInfo jsonConstructorMethod in type.GetJsonConstructorMethods())
		{
			yield return jsonConstructorMethod.ToMethodJsonConstructor();
		}
		ConstructorInfo[] constructors = type.GetConstructors();
		foreach (ConstructorInfo constructor in constructors)
		{
			yield return constructor.ToDefaultJsonConstructor();
		}
	}

	public static bool TryGetSingleJsonConstructorMethod(this Type type, out MethodInfo result)
	{
		List<MethodInfo> list = type.GetJsonConstructorMethods().ToList();
		if (list.Count == 1)
		{
			result = list[0];
			return true;
		}
		result = null;
		return false;
	}

	public static IEnumerable<MethodInfo> GetJsonConstructorMethods(this Type type)
	{
		return type.GetMethods().Where(IsJsonConstructorMethod);
	}

	public static bool IsJsonConstructorMethod(this MethodInfo method)
	{
		return method.GetCustomAttributes().Any((Attribute attribute) => attribute.GetType().Name == "BinaryDeserializationConstructorAttribute");
	}

	public static DefaultJsonConstructor ToDefaultJsonConstructor(this ConstructorInfo constructor)
	{
		return new DefaultJsonConstructor(constructor);
	}

	public static MethodJsonConstructor ToMethodJsonConstructor(this MethodInfo method)
	{
		return new MethodJsonConstructor(method);
	}

	public static IEnumerable<ParameterInfo> GetParameters(this IJsonConstructor constructor)
	{
		if (constructor is DefaultJsonConstructor defaultJsonConstructor)
		{
			return defaultJsonConstructor.Constructor.GetParameters();
		}
		if (constructor is MethodJsonConstructor methodJsonConstructor)
		{
			return methodJsonConstructor.Method.GetParameters();
		}
		throw new InvalidOperationException("Unknown IJsonConstructor type");
	}

	public static bool HasParameters(this IJsonConstructor constructor)
	{
		if (!(constructor is DefaultJsonConstructor defaultJsonConstructor) || defaultJsonConstructor.Constructor.GetParameters().Length == 0)
		{
			if (constructor is MethodJsonConstructor methodJsonConstructor)
			{
				return methodJsonConstructor.Method.GetParameters().Length != 0;
			}
			return false;
		}
		return true;
	}
}
