using System;
using System.Collections.Generic;
using System.Text.Json;
using CSharpSerializer.Serialization;

namespace CSharpSerializer;

public static class JsonSerialization
{
	public static string ToJsonWithTypeNames<T>(this T @object, JsonSerializerOptions options)
	{
		return @object.GetObjectMapValue(typeof(T)).ToJsonSerializable().ToSystemJson(options);
	}

	public static string ToJsonWithTypeNames<T>(this T @object)
	{
		return @object.ToJsonWithTypeNames(new JsonSerializerOptions
		{
			WriteIndented = true
		});
	}

	public static string ToJsonWithTypeNamesIndented<T>(this T @object)
	{
		return @object.ToJsonWithTypeNames(new JsonSerializerOptions
		{
			WriteIndented = true
		});
	}

	internal static Dictionary<string, object> ToJsonSerializable(this ObjectMap @object)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (var (key, value) in @object.Properties)
		{
			dictionary.Add(key, value.ToJsonSerializable());
		}
		return dictionary;
	}

	internal static object ToJsonSerializable(this IObjectMapValue value)
	{
		if (!(value is ObjectMap objectMap))
		{
			if (!(value is StringMapValue stringMapValue))
			{
				if (!(value is IntMapValue intMapValue))
				{
					if (!(value is LongMapValue longMapValue))
					{
						if (!(value is DoubleMapValue doubleMapValue))
						{
							if (!(value is DecimalMapValue decimalMapValue))
							{
								if (!(value is FloatMapValue floatMapValue))
								{
									if (!(value is BoolMapValue boolMapValue))
									{
										if (!(value is CollectionMap collectionMap))
										{
											if (value is DictionaryMap dictionaryValue)
											{
												return dictionaryValue.ToJsonSerializable();
											}
											throw new NotImplementedException(value.GetType().Name);
										}
										return collectionMap.Values.MapToList((IObjectMapValue value2) => value2.ToJsonSerializable());
									}
									return boolMapValue.Value;
								}
								return floatMapValue.Value;
							}
							return decimalMapValue.Value;
						}
						return doubleMapValue.Value;
					}
					return longMapValue.Value;
				}
				return intMapValue.Value;
			}
			return stringMapValue.Value;
		}
		return objectMap.ToJsonSerializable();
	}

	internal static object ToJsonSerializable(this DictionaryMap dictionaryValue)
	{
		if (dictionaryValue.KeyValues.Count > 0 && !dictionaryValue.KeyValues[0].key.IsPrimitiveType())
		{
			return dictionaryValue.KeyValues.MapToList<(IObjectMapValue, IObjectMapValue), JsonSerializableKeyValue>(((IObjectMapValue key, IObjectMapValue value) keyValue) => new JsonSerializableKeyValue(keyValue.key.ToJsonSerializable(), keyValue.value.ToJsonSerializable()));
		}
		return dictionaryValue.KeyValues.MapToList<(IObjectMapValue, IObjectMapValue), JsonSerializableKeyValue>(((IObjectMapValue key, IObjectMapValue value) kv) => new JsonSerializableKeyValue(kv.key.ToJsonSerializable(), kv.value.ToJsonSerializable()));
	}
}
