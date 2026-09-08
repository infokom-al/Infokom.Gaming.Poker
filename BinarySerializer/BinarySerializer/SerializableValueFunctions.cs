using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace BinarySerializer;

public static class SerializableValueFunctions
{
	public static Dictionary<Type, MethodBase?> Constructors { get; } = new Dictionary<Type, MethodBase>();

	public static Dictionary<Type, MethodInfo?> DeserializationConverters { get; } = new Dictionary<Type, MethodInfo>();

	public static Dictionary<Type, MethodInfo?> SerializationConverters { get; } = new Dictionary<Type, MethodInfo>();

	public static ISerializableValue GetSerializableValue<T>(this T value)
	{
		return value.GetSerializableValue(typeof(T));
	}

	public static ISerializableValue GetSerializableValue(this object value, Type declaredType)
	{
		if (value.TryGetConvertedSerializableValue(out ISerializableValue result))
		{
			return result;
		}
		if (declaredType.IsNullableType(out Type nullableValueType) && value != null)
		{
			return value.GetSerializableValue(nullableValueType);
		}
		if (value.TryGetPrimitiveSerializableValue(out result))
		{
			return result;
		}
		if (value.TryGetDictionarySerializableValue(out DictionarySerializableValue result2))
		{
			return result2;
		}
		if (value.TryGetCollectionSerializableValue<ISerializableValue>(out CollectionSerializableValue result3))
		{
			return result3;
		}
		return new ObjectSerializableValue(value.GetTypeTag(declaredType), value.GetObjectMapProperties().ToDictionary());
	}

	public static IEnumerable<(int tag, ISerializableValue value)> GetObjectMapProperties(this object @object)
	{
		foreach (var (item, member) in @object.GetType().GetSerializableProperies())
		{
			if (member.ShouldSerialize(@object))
			{
				ISerializableValue serializableValue = member.GetPropertyValue(@object).GetSerializableValue(member.GetPropertyType());
				yield return (tag: item, value: serializableValue);
			}
		}
	}

	public static bool TryGetConvertedSerializableValue(this object @object, out ISerializableValue result)
	{
		if (@object.GetType().TryGetSerializationConverter(out MethodInfo result2))
		{
			result = result2.Invoke(null, new object[1] { @object }).GetSerializableValue(result2.ReturnType);
			return true;
		}
		result = null;
		return false;
	}

	public static bool TryGetPrimitiveSerializableValue(this object @object, out ISerializableValue result)
	{
		ISerializableValue serializableValue = ((@object is bool flag) ? new LongSerializableValue(flag ? 1 : 0) : ((@object is byte b) ? new LongSerializableValue(b) : ((@object is ushort num) ? new LongSerializableValue(num) : ((@object is short num2) ? new LongSerializableValue(num2) : ((@object is int num3) ? new LongSerializableValue(num3) : ((@object is uint num4) ? new LongSerializableValue(num4) : ((@object is double value) ? new DoubleSerializableValue(value) : ((@object is string value2) ? new StringSerializableValue(value2) : ((@object is long value3) ? new LongSerializableValue(value3) : ((@object is ulong value4) ? new ULongSerializableValue(value4) : ((@object is float value5) ? ((ISerializableValue)new FloatSerializableValue(value5)) : ((ISerializableValue)((@object is DateTime dateTime) ? new LongSerializableValue(dateTime.Ticks) : ((!(@object is TimeSpan timeSpan)) ? (@object.GetType().IsEnum ? new LongSerializableValue(@object.ConvertEnumToLong()) : null) : new LongSerializableValue(timeSpan.Ticks)))))))))))))));
		result = serializableValue;
		return result != null;
	}

	public static bool TryGetDictionarySerializableValue(this object @object, out DictionarySerializableValue result)
	{
		result = null;
		IDictionary dictionary = @object as IDictionary;
		if (dictionary == null)
		{
			return false;
		}
		var (keyType, valueType) = @object.GetType().GetDictionaryGenericArguments();
		if (keyType.IsCollection())
		{
			throw new NotImplementedException("Collection key is not supported in type " + @object.GetType().Name);
		}
		result = new DictionarySerializableValue(dictionary.Keys.MapToImmutableList((object key) => (Key: key, Value: dictionary[key]).GetSerializableValue(keyType, valueType)));
		return true;
	}

	public static ObjectSerializableValue GetSerializableValue(this (object Key, object Value) keyValuePair, Type keyType, Type valueType)
	{
		return new ObjectSerializableValue(0, new Dictionary<int, ISerializableValue>
		{
			{
				1,
				keyValuePair.Key.GetSerializableValue(keyType)
			},
			{
				2,
				keyValuePair.Value.GetSerializableValue(valueType)
			}
		});
	}

	public static bool TryGetCollectionSerializableValue<T>(this object @object, out CollectionSerializableValue result)
	{
		result = null;
		if (!(@object is IEnumerable enumerable) || @object.ContainsAttribute<BinarySerializableAttribute>() || !@object.GetType().TryGetCollectionArgumentType(out Type argumentType))
		{
			return false;
		}
		bool writeDefaultValues = ShouldWriteDefaultValue();
		result = new CollectionSerializableValue(enumerable.MapToImmutableList((object item) => (writeDefaultValues || !item.IsDefaultValue(argumentType)) ? item.GetSerializableValue(argumentType) : DefaultSerializableValue.Instance));
		return true;
		bool ShouldWriteDefaultValue()
		{
			if (argumentType.IsTupleType())
			{
				return true;
			}
			if (argumentType.IsEnum)
			{
				return false;
			}
			if (argumentType.IsStruct())
			{
				return true;
			}
			return false;
		}
	}

	public static bool ShouldSerialize(this MemberInfo member, object memberParentValue)
	{
		Type propertyType = member.GetPropertyType();
		if (propertyType.IsInlineArray())
		{
			return true;
		}
		return !member.GetPropertyValue(memberParentValue).IsDefaultValue(propertyType);
	}

	public static bool IsDefaultValue(this object? memberValue, Type type)
	{
		if (type.IsNullableType())
		{
			return memberValue == null;
		}
		if (memberValue == null)
		{
			return true;
		}
		if (memberValue is bool flag)
		{
			return !flag;
		}
		if (memberValue is string text && text == null)
		{
			return true;
		}
		if (memberValue is ICollection { Count: 0 })
		{
			return true;
		}
		if (memberValue is byte && (byte)memberValue == 0)
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
		if (memberValue is decimal num2 && num2 == 0m)
		{
			return true;
		}
		if (memberValue is double num3)
		{
			return num3 == 0.0;
		}
		if (memberValue is DateTime dateTime && dateTime == default(DateTime))
		{
			return true;
		}
		if (memberValue is TimeSpan timeSpan && timeSpan == default(TimeSpan))
		{
			return true;
		}
		if (memberValue.GetType().IsEnum && memberValue.IsDefaultEnumValue())
		{
			return true;
		}
		if (memberValue is Guid guid)
		{
			return object.Equals(guid, Guid.Empty);
		}
		if (type.IsStruct() && memberValue.Equals(Activator.CreateInstance(type)))
		{
			return true;
		}
		return false;
	}

	public static bool IsDefaultEnumValue(this object value)
	{
		return Convert.ToInt64(value) == 0;
	}

	public static StringSerializableValue ToStringValue(this string @string)
	{
		return new StringSerializableValue(@string);
	}

	public static DoubleSerializableValue ToDoubleValue(this double value)
	{
		return new DoubleSerializableValue(value);
	}

	public static long ConvertEnumToLong(this object value)
	{
		return Convert.ToInt64(value);
	}

	public static bool TryGetPropertyTag(this MemberInfo property, out int result)
	{
		if (property.TryGetAttribute<TagAttribute>(out var result2))
		{
			result = result2.Value;
			return true;
		}
		result = 0;
		Type declaringType = property.DeclaringType;
		if (declaringType == null)
		{
			return false;
		}
		if (declaringType.IsTupleType() || declaringType.IsAnonymousType())
		{
			result = declaringType.GetPropertyMembers().ToList().IndexOf(property) + 1;
			return true;
		}
		if (declaringType.IsRecordType() && declaringType.TryGetMainConstructor(out ConstructorInfo result3))
		{
			if (!result3.TryGetMatchingParameter(property, out ParameterInfo result4))
			{
				return false;
			}
			if (!((object)result4).TryGetAttribute(out result2))
			{
				return false;
			}
			result = result2.Value;
			return true;
		}
		return false;
	}

	internal static object? GetObjectOfType(this ISerializableValue value, Type expectedType)
	{
		if (value is DefaultSerializableValue)
		{
			if (!expectedType.IsValueType)
			{
				return null;
			}
			return Activator.CreateInstance(expectedType);
		}
		if (expectedType.TryGetDeserializationConverter(out MethodInfo result))
		{
			return result.Invoke(null, new object[1] { value.GetObjectOfType(result.GetParameters()[0].ParameterType) });
		}
		if (expectedType.IsNullableType(out Type nullableValueType))
		{
			object objectOfType = value.GetObjectOfType(nullableValueType);
			return Activator.CreateInstance(expectedType, objectOfType);
		}
		if (!(value is StringSerializableValue stringSerializableValue))
		{
			if (!(value is LongSerializableValue longSerializableValue))
			{
				if (!(value is ULongSerializableValue uLongSerializableValue))
				{
					if (!(value is DoubleSerializableValue doubleSerializableValue))
					{
						if (value is ObjectSerializableValue serializableObject)
						{
							return serializableObject.GetObjectOfType(expectedType);
						}
						if (value is CollectionSerializableValue collectionMap)
						{
							return collectionMap.GetCollectionOfType(expectedType);
						}
						if (value is DictionarySerializableValue dictionaryValue)
						{
							return dictionaryValue.GetDictionaryOfType(expectedType);
						}
					}
					else
					{
						if (expectedType == typeof(double) || expectedType == typeof(object))
						{
							return doubleSerializableValue.Value;
						}
						if (expectedType == typeof(decimal) || expectedType == typeof(object))
						{
							return (decimal)doubleSerializableValue.Value;
						}
						if (expectedType == typeof(float) || expectedType == typeof(object))
						{
							return (float)doubleSerializableValue.Value;
						}
					}
				}
				else if (expectedType == typeof(ulong))
				{
					return uLongSerializableValue.Value;
				}
			}
			else
			{
				if (expectedType == typeof(int) || expectedType == typeof(object))
				{
					return (int)longSerializableValue.Value;
				}
				if (expectedType == typeof(uint) || expectedType == typeof(object))
				{
					return (uint)longSerializableValue.Value;
				}
				if (expectedType == typeof(long))
				{
					return longSerializableValue.Value;
				}
				if (expectedType == typeof(bool))
				{
					return longSerializableValue.Value != 0;
				}
				if (expectedType == typeof(double))
				{
					return (double)longSerializableValue.Value;
				}
				if (expectedType == typeof(ushort))
				{
					return (ushort)longSerializableValue.Value;
				}
				if (expectedType == typeof(short))
				{
					return (short)longSerializableValue.Value;
				}
				if (expectedType == typeof(byte))
				{
					return (byte)longSerializableValue.Value;
				}
				if (expectedType.IsEnum)
				{
					return Enum.ToObject(expectedType, longSerializableValue.Value);
				}
				if (expectedType == typeof(DateTime))
				{
					return new DateTime(longSerializableValue.Value);
				}
				if (expectedType == typeof(TimeSpan))
				{
					return new TimeSpan(longSerializableValue.Value);
				}
			}
		}
		else if (expectedType == typeof(string) || expectedType == typeof(object))
		{
			return stringSerializableValue.Value;
		}
		return false;
	}

	public static object? GetObjectOfType(this ObjectSerializableValue serializableObject, Type expectedType)
	{
		if (expectedType.IsTupleType())
		{
			return serializableObject.GetTupleOfType(expectedType);
		}
		if (expectedType.IsAnonymousType())
		{
			return serializableObject.GetAnonymousObject(expectedType);
		}
		if (expectedType.IsInterface)
		{
			if (!expectedType.GetInterfaceSubTypes().TryGet<(int, Type)>(((int tag, Type type) tagType) => tagType.tag == serializableObject.TypeTag, out var result))
			{
				throw new InvalidOperationException();
			}
			expectedType = result.Item2;
		}
		if (expectedType.TryGetBinaryDeserializationConstructor(out MethodBase result2))
		{
			object[] arguments = result2.GetParameters().GetMethodArguments(expectedType, serializableObject).ToArray();
			return result2.InvokeBinaryDeserializationConstructor(arguments);
		}
		Dictionary<int, ISerializableValue> properties = serializableObject.Properties;
		if (!expectedType.GetConstructorsIncludingProtected().TryGet((ConstructorInfo constructor) => constructor.GetParameters().Length == 0, out ConstructorInfo result3))
		{
			throw new InvalidOperationException($"Empty constructor for type {expectedType} not found");
		}
		object obj = result3.Invoke(null);
		foreach (var (key, member) in expectedType.GetSerializableProperies())
		{
			if (properties.TryGetValue(key, out var value))
			{
				member.SetPropertyValue(obj, value.GetObjectOfType(member.GetPropertyType()));
			}
			else
			{
				member.SetPropertyValue(obj, member.GetPropertyType().GetDefaultValue());
			}
		}
		return obj;
	}

	public static object GetDictionaryOfType(this DictionarySerializableValue dictionaryValue, Type expectedType)
	{
		var (keyType, valueType) = expectedType.GetDictionaryGenericArguments();
		return dictionaryValue.KeyValues.Select((ObjectSerializableValue keyValue) => (key: keyValue.Properties[1].GetObjectOfType(keyType), value: keyValue.Properties[2].GetObjectOfType(valueType))).GetDictionaryOfType(expectedType);
	}

	public static object GetCollectionOfType(this CollectionSerializableValue collectionMap, Type expectedType)
	{
		Type argumentType = expectedType.GetCollectionArgumentType();
		return collectionMap.Values.MapToList((ISerializableValue itemValue) => itemValue.GetObjectOfType(argumentType)).CreateListOfType(expectedType);
	}

	public static object GetTupleOfType(this ObjectSerializableValue serializableObject, Type expectedType)
	{
		MethodInfo valueTupleConstructor = (from field in expectedType.GetFields()
			select field.FieldType).ToArray().GetValueTupleConstructor();
		object[] parameters = valueTupleConstructor.GetParameters().GetMethodArguments(expectedType, serializableObject).ToArray();
		return valueTupleConstructor.Invoke(null, parameters) ?? throw new InvalidOperationException("Can't create an object from " + expectedType.Name);
	}

	public static object GetAnonymousObject(this ObjectSerializableValue serializableObject, Type expectedType)
	{
		if (!expectedType.IsAnonymousType())
		{
			throw new ArgumentException($"{expectedType} is not an anonymous type");
		}
		ConstructorInfo constructorInfo = expectedType.GetConstructors().First();
		object[] parameters = constructorInfo.GetParameters().GetMethodArguments(expectedType, serializableObject).ToArray();
		return constructorInfo.Invoke(parameters);
	}

	public static IEnumerable<object?> GetMethodArguments(this ParameterInfo[] methodParameters, Type declaringType, ObjectSerializableValue serializableObject)
	{
		List<(int tag, MemberInfo memberInfo)> declaredProperties = declaringType.GetSerializableProperies();
		foreach (ParameterInfo parameter in methodParameters)
		{
			if (parameter.Name == null)
			{
				throw new InvalidOperationException("Null parameter names aren't expected");
			}
			if (declaredProperties.TryGet<(int, MemberInfo)>(((int tag, MemberInfo memberInfo) property) => parameter.Name.ContructorParameterMatchesPropertyName(property.memberInfo.Name, declaringType), out var result) && serializableObject.TryGetPropertyValue(result.Item1, out ISerializableValue result2))
			{
				yield return result2.GetObjectOfType(parameter.ParameterType);
			}
			else if (parameter.IsOptional)
			{
				yield return null;
			}
			else
			{
				yield return parameter.ParameterType.GetDefaultValue();
			}
		}
	}

	public static int GetTypeTag(this object value, Type declaredType)
	{
		Type type = value.GetType();
		if (type == declaredType)
		{
			return 0;
		}
		if (!declaredType.GetCustomAttributes<TypeTagAttribute>().TryGet((TypeTagAttribute attribute) => attribute.Type == type, out TypeTagAttribute result) || result == null)
		{
			throw new InvalidOperationException($"{value.GetType()} can not be deserialized as {declaredType.Name} without {"TypeTagAttribute"}");
		}
		return result.Tag;
	}

	public static CollectionSerializableValue ToCollectionSerializableValue(this IEnumerable<ISerializableValue> values)
	{
		return new CollectionSerializableValue(values.ToImmutableList());
	}

	public static DictionarySerializableValue ToDictionarySerializableValue(this IEnumerable<ObjectSerializableValue> keyValues)
	{
		return new DictionarySerializableValue(keyValues.ToImmutableList());
	}

	public static bool TryGetSerializationConverter(this Type type, out MethodInfo result)
	{
		if (SerializationConverters.TryGetValue(type, out result))
		{
			return result != null;
		}
		foreach (MethodInfo item in type.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetAllStaticFunctions()))
		{
			if (item.ContainsAttribute<BinarySerializationConverterAttribute>() && item.GetParameters().Length == 1)
			{
				ParameterInfo parameterInfo = item.GetParameters()[0];
				if (parameterInfo.ParameterType == type)
				{
					result = item;
					break;
				}
				if (type.IsGenericInstanceOf(parameterInfo.ParameterType))
				{
					result = item.MakeGenericMethod(type.GenericTypeArguments);
					break;
				}
			}
		}
		lock (SerializationConverters)
		{
			SerializationConverters[type] = result;
		}
		return result != null;
	}

	public static bool TryGetDeserializationConverter(this Type type, out MethodInfo result)
	{
		if (DeserializationConverters.TryGetValue(type, out result))
		{
			return result != null;
		}
		foreach (MethodInfo item in type.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetAllStaticFunctions()))
		{
			if (item.ContainsAttribute<BinaryDeserializationConverterAttribute>() && item.GetParameters().Length == 1)
			{
				if (item.ReturnType == type)
				{
					result = item;
					break;
				}
				if (type.IsGenericInstanceOf(item.ReturnType))
				{
					result = item.MakeGenericMethod(type.GenericTypeArguments);
					break;
				}
			}
		}
		lock (DeserializationConverters)
		{
			DeserializationConverters[type] = result;
		}
		return result != null;
	}

	public static IEnumerable<Assembly> GetAssemblies(this Type type)
	{
		if (!type.Name.StartsWith("System."))
		{
			yield return type.Assembly;
		}
		yield return typeof(SerializableValueFunctions).Assembly;
	}

	public static bool IsBinarySerialiazableType(this Type type)
	{
		return type.CustomAttributes.Any((CustomAttributeData attribute) => attribute.AttributeType == typeof(BinarySerializableAttribute) || attribute.AttributeType == typeof(TypeTagAttribute));
	}

	public static bool IsBinarySerializableMember(this MemberInfo member)
	{
		return member.CustomAttributes.Any((CustomAttributeData attribute) => attribute.AttributeType == typeof(TagAttribute));
	}

	public static bool TryGetBinaryDeserializationConstructor(this Type type, out MethodBase result)
	{
		if (Constructors.TryGetValue(type, out result))
		{
			return result != null;
		}
		lock (Constructors)
		{
			if (type.TryGetBinaryDeserializationConstructorSlow(out result))
			{
				Constructors[type] = result;
				return true;
			}
			Constructors[type] = null;
		}
		return false;
	}

	public static bool TryGetBinaryDeserializationConstructorSlow(this Type type, out MethodBase result)
	{
		result = null;
		MethodInfo[] methods = type.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.IsStatic && methodInfo.HasAttribute<BinaryDeserializationConstructorAttribute>())
			{
				result = methodInfo;
				return true;
			}
		}
		List<(int tag, MemberInfo memberInfo)> serializableProperties = type.GetSerializableProperies().ToList();
		List<ConstructorInfo> list = (from constuctor in type.GetConstructorsIncludingProtected()
			where constuctor.GetParameters().Length == serializableProperties.Count
			select constuctor).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (ConstructorInfo item in list)
		{
			bool flag = true;
			ParameterInfo[] parameters = item.GetParameters();
			foreach (ParameterInfo parameter in parameters)
			{
				if (!serializableProperties.Any(((int tag, MemberInfo memberInfo) property) => parameter.Name.ContructorParameterMatchesPropertyName(property.memberInfo.Name, type)))
				{
					if (list.Count == 1)
					{
						throw new InvalidOperationException($"Matching property for the constructor parameter {type.Name}.{parameter.Name} not found.");
					}
					flag = false;
					break;
				}
			}
			if (flag)
			{
				result = item;
				return true;
			}
		}
		return false;
	}

	public static object InvokeBinaryDeserializationConstructor(this MethodBase method, params object[] arguments)
	{
		if (method is MethodInfo methodInfo)
		{
			if (!methodInfo.IsStatic)
			{
				throw new InvalidOperationException("Method must be static to be a binary deserialization constructor");
			}
			return methodInfo.Invoke(null, arguments);
		}
		return method.VerifyType<ConstructorInfo>().Invoke(arguments);
	}

	public static void VerifyBinarySerializable(this object @object)
	{
		Type type = @object.GetType();
		if (!type.IsSystemType() && !type.IsEnum && !type.IsCollection() && !type.IsDictionaryType() && !type.HasAttribute<BinarySerializableAttribute>() && !type.IsAnonymousType())
		{
			throw new InvalidOperationException("Type " + type.Name.Quoted() + " doesn't have a BinarySerializableAttribute");
		}
	}
}
