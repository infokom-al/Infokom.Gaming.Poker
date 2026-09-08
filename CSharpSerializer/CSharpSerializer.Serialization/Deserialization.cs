using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using CSharpSerializer.Common;

namespace CSharpSerializer.Serialization;

public static class Deserialization
{
	public const string typeFieldName = "__typename";

	public static ConcurrentDictionary<IList<Assembly>, Deserializer> Deserializers { get; } = new ConcurrentDictionary<IList<Assembly>, Deserializer>();

	internal static T Deserialize<T>(this IObjectMapValue value, Deserializer deserializer)
	{
		return value.Deserialize(typeof(T), deserializer).VerifyType<T>();
	}

	internal static T DeserializeObjectMap<T>(this ObjectMap value, Deserializer deserializer)
	{
		return (T)value.DeserializeObjectMap(deserializer, typeof(T));
	}

	internal static object DeserializeObjectMap(this ObjectMap value, Deserializer deserializer, Type expectedType)
	{
		if (value.TryGetExplicitType(deserializer, out Type result))
		{
			if (result.IsPrimimiteObjectMapValueType())
			{
				return value.GetPropertyValue("value").Deserialize(result, deserializer);
			}
			if (result.IsEnumerableType())
			{
				return value.GetPropertyValue("values").Deserialize(result, deserializer);
			}
		}
		else
		{
			result = expectedType;
			if (expectedType == typeof(object))
			{
				throw new InvalidOperationException("Properties with declared type of object should be serialized with explicitly marked type");
			}
		}
		if (expectedType.IsPrimitiveOrEnum())
		{
			throw new InvalidOperationException($"Can't deserialize primitive type {expectedType.Name.Quoted()} from an object map {value})");
		}
		if (expectedType.IsDictionaryType() || expectedType.IsImmutableDictionaryType())
		{
			return value.DeserializeDictionaryValue(expectedType, deserializer);
		}
		if (result.TryGetMainConstructor(out IJsonConstructor result2))
		{
			object obj = result2.Invoke(result2.GetConstructorArguments(value, deserializer).ToArray());
			{
				foreach (var property in value.Properties)
				{
					var (propertyName, value2) = property;
					if (!result2.GetParameters().Any((ParameterInfo parameter) => parameter.Name.ConstructorParameterNameEquals(propertyName)) && (from member in result.GetPropertyMembers()
						where member.HasPublicSetter()
						select member).TryGet((MemberInfo member) => member.Name.Equals(propertyName), out MemberInfo result3))
					{
						obj.SetPropertyValue(result3, value2.Deserialize(result3.GetPropertyType(), deserializer));
					}
				}
				return obj;
			}
		}
		if (value.Properties.Count == 0 || value.Properties[0].name == "__typename")
		{
			return Activator.CreateInstance(result);
		}
		throw new NotImplementedException("Deserialization without constructor is not implemented yet. The type was " + result.Name);
	}

	internal static IEnumerable<object> GetConstructorArguments(this IJsonConstructor constructor, ObjectMap objectMap, Deserializer deserializer)
	{
		foreach (ParameterInfo parameter in constructor.GetParameters())
		{
			if (!objectMap.TryGetParameterValue(parameter, deserializer, out object result) && !parameter.TryGetDefaultValue(out result))
			{
				throw new InvalidOperationException("Failed to deserialize value for the parameter " + parameter.Name);
			}
			yield return result;
		}
	}

	internal static bool TryGetParameterValue(this ObjectMap objectMap, ParameterInfo parameter, Deserializer deserializer, out object result)
	{
		result = null;
		if (parameter.Name == null)
		{
			throw new InvalidOperationException("Nameless parameters in constructor aren't supported");
		}
		if (!objectMap.Properties.TryGet<(string, IObjectMapValue)>(((string name, IObjectMapValue value) property) => parameter.Name.ConstructorParameterNameEquals(property.name), out var result2))
		{
			return false;
		}
		return result2.Item2.TryDeserialize(parameter.ParameterType.GetNonByRefType(), deserializer, out result);
	}

	internal static object Deserialize(this IObjectMapValue value, Type expectedType, Deserializer deserializer)
	{
		if (!value.TryDeserialize(expectedType, deserializer, out object result))
		{
			throw new InvalidOperationException("Failed to covert " + value.ToString().Quoted() + " to object of type " + expectedType.Name.Quoted());
		}
		return result;
	}

	internal static bool TryDeserialize(this IObjectMapValue value, Type expectedType, Deserializer deserializer, out object result)
	{
		result = null;
		if (!(value is NullMapValue))
		{
			if (!(value is StringMapValue stringMapValue))
			{
				if (!(value is IntMapValue intMapValue))
				{
					if (!(value is LongMapValue longMapValue))
					{
						if (!(value is DoubleMapValue doubleMapValue))
						{
							if (!(value is ObjectMap value2))
							{
								if (!(value is CollectionMap value3))
								{
									if (!(value is DictionaryMap value4))
									{
										if (value is BoolMapValue boolMapValue)
										{
											result = boolMapValue.Value;
											return true;
										}
										return false;
									}
									result = value4.DeserializeDictionaryValue(expectedType, deserializer);
									return true;
								}
								result = value3.DeserializeCollectionValue(expectedType, deserializer);
								return true;
							}
							result = value2.DeserializeObjectMap(deserializer, expectedType);
							return true;
						}
						if (expectedType == typeof(double) || expectedType == typeof(object) || expectedType == typeof(double?))
						{
							result = doubleMapValue.Value;
							return true;
						}
						if (expectedType == typeof(decimal) || expectedType == typeof(object) || expectedType == typeof(decimal?))
						{
							result = doubleMapValue.Value.ToDecimal();
							return true;
						}
						return false;
					}
					if (expectedType == typeof(long) || expectedType == typeof(object) || expectedType == typeof(long?))
					{
						result = longMapValue.Value;
						return true;
					}
					if (expectedType == typeof(DateTime))
					{
						result = new DateTime(longMapValue.Value);
						return true;
					}
					if (expectedType == typeof(int) || expectedType == typeof(int?))
					{
						result = (int)longMapValue.Value;
						return true;
					}
					if (expectedType.IsEnum)
					{
						result = Enum.ToObject(expectedType, longMapValue.Value);
						return true;
					}
					return false;
				}
				if (expectedType == typeof(int) || expectedType == typeof(object) || expectedType == typeof(int?))
				{
					result = intMapValue.Value;
					return true;
				}
				if (expectedType == typeof(long) || expectedType == typeof(long?))
				{
					result = (long)intMapValue.Value;
					return true;
				}
				if (expectedType == typeof(bool) || expectedType == typeof(bool?))
				{
					result = intMapValue.Value != 0;
					return true;
				}
				if (expectedType == typeof(double) || expectedType == typeof(double?))
				{
					result = (double)intMapValue.Value;
					return true;
				}
				if (expectedType == typeof(decimal) || expectedType == typeof(decimal?))
				{
					result = (decimal)intMapValue.Value;
					return true;
				}
				if (expectedType == typeof(byte))
				{
					result = (byte)intMapValue.Value;
					return true;
				}
				if (expectedType == typeof(short))
				{
					result = (short)intMapValue.Value;
					return true;
				}
				if (expectedType == typeof(ushort))
				{
					result = (ushort)intMapValue.Value;
					return true;
				}
				if (expectedType == typeof(string))
				{
					result = intMapValue.Value.ToString();
					return true;
				}
				if (expectedType.IsEnum)
				{
					result = Enum.ToObject(expectedType, intMapValue.Value);
					return true;
				}
				if (expectedType == typeof(string))
				{
					result = intMapValue.Value.ToString();
					return true;
				}
				return false;
			}
			if (expectedType == typeof(DateTime) && DateTime.TryParse(stringMapValue.Value, out var result2))
			{
				result = result2;
				return true;
			}
			if (expectedType == typeof(Type) && deserializer.AllDeserializableTypes.TryGetValue(stringMapValue.Value, out Type value5))
			{
				result = value5;
				return true;
			}
			result = stringMapValue.Value;
			if (!(expectedType == typeof(string)))
			{
				return expectedType == typeof(object);
			}
			return true;
		}
		result = null;
		return true;
	}

	internal static object DeserializeCollectionValue(this CollectionMap value, Type expectedType, Deserializer deserializer)
	{
		if (expectedType.HasSerializeAsCollectionAttribute())
		{
			return value.DeserializeAsCollection(expectedType, deserializer);
		}
		if (expectedType.IsDictionaryType())
		{
			if (value.Values.Count != 0)
			{
				throw new InvalidOperationException("Only empty collectoin can be casted to dictionary");
			}
			return expectedType.CreateEmptyDictionary();
		}
		if (expectedType.IsImmutableDictionaryType())
		{
			if (value.Values.Count != 0)
			{
				throw new InvalidOperationException("Only empty collectoin can be casted to dictionary");
			}
			return expectedType.CreateEmptyImmutableDictionary();
		}
		if (!expectedType.TryGetSingleGenericArgument(out Type genericType))
		{
			throw new NotImplementedException("Non-generic collections are not implemented yet");
		}
		return value.Values.Select((IObjectMapValue itemValue) => itemValue.Deserialize(genericType, deserializer)).CreateListOfType(expectedType);
	}

	public static object DeserializeAsCollection(this CollectionMap value, Type expectedType, Deserializer deserializer)
	{
		if (!expectedType.TryGetMainConstructor(out IJsonConstructor result))
		{
			throw new InvalidOperationException("Can't deserialize collection to type without constructor");
		}
		List<ParameterInfo> list = result.GetParameters().ToList();
		if (list.Count != 1)
		{
			throw new InvalidOperationException("Expecting a single parameter in the collection serializable constructor");
		}
		Type parameterType = list[0].ParameterType;
		if (!parameterType.IsEnumerableType())
		{
			throw new InvalidOperationException("Expecting an enumerable parameter type but was " + parameterType.Name.Quoted());
		}
		if (!parameterType.TryGetSingleGenericArgument(out Type itemType))
		{
			throw new InvalidOperationException("Only single generic argument collection types are supported");
		}
		IEnumerable<object> values = value.Values.Select((IObjectMapValue itemValue) => itemValue.Deserialize(itemType, deserializer));
		return result.Invoke(new _003C_003Ez__ReadOnlySingleElementList<object>(values.CreateListOfType(parameterType)));
	}

	internal static object DeserializeDictionaryValue(this ObjectMap value, Type expectedType, Deserializer deserializer)
	{
		var (keyType, valueType) = expectedType.GetDictionaryGenericArguments();
		return value.Properties.Select<(string, IObjectMapValue), KeyValuePair<object, object>>(((string name, IObjectMapValue value) property) => new KeyValuePair<object, object>(property.name.ParsePrimitiveValue(keyType), property.value.Deserialize(valueType, deserializer))).CreateDictionaryOfType(expectedType);
	}

	public static object ParsePrimitiveValue(this string @string, Type expectedType)
	{
		if (expectedType == typeof(int))
		{
			return int.Parse(@string);
		}
		if (expectedType == typeof(long))
		{
			return long.Parse(@string);
		}
		if (expectedType == typeof(byte))
		{
			return byte.Parse(@string);
		}
		if (expectedType == typeof(DateTime))
		{
			return new DateTime(long.Parse(@string));
		}
		if (expectedType.IsEnum)
		{
			return Enum.Parse(expectedType, @string);
		}
		if (expectedType == typeof(string))
		{
			return @string;
		}
		throw new NotImplementedException("Failed to parse " + expectedType.Name + " from " + @string.Quoted());
	}

	internal static object DeserializeDictionaryValue(this DictionaryMap value, Type expectedType, Deserializer deserializer)
	{
		var (keyType, valueType) = expectedType.GetDictionaryGenericArguments();
		return value.KeyValues.Select<(IObjectMapValue, IObjectMapValue), KeyValuePair<object, object>>(((IObjectMapValue key, IObjectMapValue value) keyValue) => new KeyValuePair<object, object>(keyValue.key.Deserialize(keyType, deserializer), keyValue.value.Deserialize(valueType, deserializer))).CreateDictionaryOfType(expectedType);
	}

	internal static bool TryGetDefaultValue(this ParameterInfo parameter, out object result)
	{
		if (parameter.ParameterType == typeof(string))
		{
			result = null;
			return true;
		}
		if (parameter.ParameterType == typeof(bool))
		{
			result = false;
			return true;
		}
		if (parameter.ParameterType == typeof(double))
		{
			result = 0.0;
			return true;
		}
		if (parameter.ParameterType == typeof(int))
		{
			result = 0;
			return true;
		}
		if (parameter.ParameterType == typeof(long))
		{
			result = 0L;
			return true;
		}
		if (parameter.ParameterType == typeof(decimal))
		{
			result = 0m;
			return true;
		}
		if (parameter.ParameterType.IsEnum)
		{
			0.ToEnumValue(parameter.ParameterType);
			result = true;
		}
		if (parameter.ParameterType.IsImmutableHashSetType())
		{
			result = parameter.ParameterType.GetSingleGenericArgument().CreateEmptyImmutableHashSet();
			return true;
		}
		if (parameter.ParameterType.IsImmutableListType())
		{
			result = parameter.ParameterType.GetSingleGenericArgument().CreateEmptyImmutableList();
			return true;
		}
		if (parameter.ParameterType.IsListType())
		{
			result = parameter.ParameterType.GetSingleGenericArgument().CreateEmptyList();
			return true;
		}
		if (parameter.ParameterType.IsImmutableDictionaryType())
		{
			result = parameter.ParameterType.CreateEmptyImmutableDictionary();
			return true;
		}
		if (parameter.ParameterType.IsDictionaryType())
		{
			result = parameter.ParameterType.CreateEmptyDictionary();
			return true;
		}
		result = null;
		return true;
	}

	internal static bool TryGetExplicitType(this ObjectMap value, Deserializer deserializer, out Type result)
	{
		result = null;
		if (!value.TryGetPropertyValue("__typename", out StringMapValue value2))
		{
			return false;
		}
		string typeNameWithoutNamespace = value2.Value.GetTypeNameWithoutNamespace();
		result = typeNameWithoutNamespace.ParseType(deserializer);
		return true;
	}

	private static string GetTypeNameWithoutNamespace(this string fullTypeName)
	{
		return fullTypeName.Split('.').Last();
	}

	internal static Deserializer GetDeserializer(this IList<Assembly> assemblies)
	{
		if (Deserializers.TryGetValue(assemblies, out Deserializer value))
		{
			return value;
		}
		Deserializer deserializerSlow = assemblies.GetDeserializerSlow();
		Deserializers[assemblies] = deserializerSlow;
		return deserializerSlow;
	}

	internal static Deserializer GetDeserializerSlow(this IList<Assembly> assemblies)
	{
		return new Deserializer(assemblies.ToImmutableList().AddIfNotContains(typeof(string).Assembly).AddIfNotContains(typeof(ImmutableList<>).Assembly)
			.AddIfNotContains(typeof(List<>).Assembly)
			.SelectMany((Assembly assembly) => assembly.GetTypes())
			.DistinctBy((Type type) => type.Name)
			.ToDictionary((Type type) => type.Name, (Type type) => type));
	}

	internal static CollectionMap ToCollectionValue(this IEnumerable<IObjectMapValue> values)
	{
		return new CollectionMap(values.ToImmutableList());
	}

	public static Type ParseType(this string typeString, Deserializer deserializer)
	{
		if (typeString.Contains("<"))
		{
			return typeString.ParseGenericType(deserializer);
		}
		if (!deserializer.AllDeserializableTypes.TryGetValue(typeString, out Type value))
		{
			throw new InvalidOperationException("Type " + typeString.Quoted() + " not found in the given assemblies");
		}
		return value;
	}

	public static Type ParseGenericType(this string fullTypeName, Deserializer deserializer)
	{
		if (!fullTypeName.Contains("<"))
		{
			throw new InvalidOperationException(fullTypeName.Quoted() + " is not a generic type name");
		}
		FluentParser fluentParser = fullTypeName.ToFluentParser();
		string typeName = fluentParser.ReadUntil("<");
		List<Type> genericArguments = fluentParser.Skip("<".Length).ReadUntilLast('>').ParseTypeArguments(deserializer)
			.ToList();
		return typeName.ConstructGenericeType(genericArguments, deserializer);
	}

	public static IEnumerable<Type> ParseTypeArguments(this string @string, Deserializer deserializer)
	{
		FluentParser reader = @string.ToFluentParser().SkipSpaces();
		while (reader.HasNext)
		{
			string text = reader.ReadUntilAnyOf(',', '<');
			if (reader.HasNext && reader.NextChar == '<')
			{
				string fullTypeName = text + "<" + reader.SkipOne().ReadUntilLast('>') + ">";
				yield return fullTypeName.ParseGenericType(deserializer);
			}
			else
			{
				yield return text.ParseType(deserializer);
			}
			if (reader.HasNext)
			{
				reader.SkipOne().SkipSpaces();
				continue;
			}
			break;
		}
	}

	public static Type ConstructGenericeType(this string typeName, IList<Type> genericArguments, Deserializer deserializer)
	{
		string text = $"{typeName}`{genericArguments.Count}";
		if (!deserializer.AllDeserializableTypes.TryGetValue(text, out Type value))
		{
			throw new InvalidOperationException("Type " + text.Quoted() + " not found in the given assemblies");
		}
		return value.MakeGenericType(genericArguments.ToArray());
	}

	public static object Invoke(this IJsonConstructor constructor, IEnumerable<object> parameters)
	{
		if (!(constructor is DefaultJsonConstructor defaultJsonConstructor))
		{
			if (constructor is MethodJsonConstructor methodJsonConstructor)
			{
				return methodJsonConstructor.Method.Invoke(null, parameters.ToArray());
			}
			
			throw new InvalidOperationException($"Unsupported constructor {constructor}");
		}
		return defaultJsonConstructor.Constructor.Invoke(parameters.ToArray());
	}
}
