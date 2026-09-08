using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using CSharpSerializer.Serialization;

namespace CSharpSerializer;

public static class JsonDeserialization
{
	public static T ParseJsonWithTypeNames<T>(this string json, IList<Assembly> typeSourceAssemblies)
	{
		return json.ParseJsonWithTypeNames(typeof(T), typeSourceAssemblies).VerifyType<T>();
	}

	public static object ParseJsonWithTypeNames(this string json, Type type, IList<Assembly> typeSourceAssemblies)
	{
		return json.ParseJson<JsonElement>().GetValueFromJsonElement().Deserialize(type, typeSourceAssemblies.GetDeserializer());
	}

	internal static IObjectMapValue GetValueFromJsonElement(this JsonElement element)
	{
		JsonValueKind valueKind = element.ValueKind;
		switch (valueKind)
		{
			case JsonValueKind.Object:
				return GetObjectMapValue(element);
			case JsonValueKind.Array:
				{
					IObjectMapValue result3;
					if (!element.TryGetDictionaryMap(out DictionaryMap result2))
					{
						IObjectMapValue collectionMapValue = element.GetCollectionMapValue();
						result3 = collectionMapValue;
					}
					else
					{
						IObjectMapValue collectionMapValue = result2;
						result3 = collectionMapValue;
					}
					return result3;
				}
			case JsonValueKind.String:
				return element.GetString().ToStringValue();
			case JsonValueKind.Number:
				return GetNumberMapValue(element);
			case JsonValueKind.True:
				return element.GetBoolean().ToBoolValue();
			case JsonValueKind.False:
				return element.GetBoolean().ToBoolValue();
			case JsonValueKind.Null:
				return NullMapValue.Instance;
			case JsonValueKind.Undefined:
				throw new InvalidOperationException($"Unexpected json element : {element}");
			default:
				{

					throw new InvalidOperationException($"Unexpected json value kind : {valueKind}");
				}
		}
		static IObjectMapValue GetNumberMapValue(JsonElement value)
		{
			if (value.TryGetInt32(out var value2))
			{
				return value2.ToIntValue();
			}
			if (value.TryGetInt64(out var value3))
			{
				return value3.ToLongValue();
			}
			if (value.TryGetDouble(out var value4))
			{
				return value4.ToDoubleValue();
			}
			throw new InvalidOperationException($"Failed to get number value from json element {value}");
		}
		static IObjectMapValue GetObjectMapValue(JsonElement jsonElement)
		{
			return new ObjectMap((from property in jsonElement.EnumerateObject()
							  select (name: property.Name, value: property.Value.GetValueFromJsonElement())).ToImmutableList());
		}
	}

	internal static ObjectMap GetObjectMap(this JsonElement jsonElement)
	{
		return new ObjectMap((from property in jsonElement.VerifyValueKind(JsonValueKind.Object).EnumerateObject()
						  select (name: property.Name, value: property.Value.GetValueFromJsonElement())).ToImmutableList());
	}

	internal static bool TryGetDictionaryMap(this JsonElement jsonElement, out DictionaryMap result)
	{
		result = null;
		if (jsonElement.ValueKind != JsonValueKind.Array)
		{
			return false;
		}
		ImmutableList<(IObjectMapValue, IObjectMapValue)>.Builder builder = ImmutableList.CreateBuilder<(IObjectMapValue, IObjectMapValue)>();
		foreach (JsonElement item in jsonElement.EnumerateArray())
		{
			if (item.ValueKind != JsonValueKind.Object)
			{
				return false;
			}
			List<JsonProperty> list = item.EnumerateObject().ToList();
			if (list.Count != 2)
			{
				return false;
			}
			if (list[0].Name != "Key" || list[1].Name != "Value")
			{
				return false;
			}
			builder.Add((list[0].Value.GetValueFromJsonElement(), list[1].Value.GetValueFromJsonElement()));
		}
		if (builder.Count == 0)
		{
			return false;
		}
		result = new DictionaryMap(builder.ToImmutable());
		return true;
	}

	internal static CollectionMap GetCollectionMapValue(this JsonElement jsonElement)
	{
		return (from element in jsonElement.VerifyValueKind(JsonValueKind.Array).EnumerateArray()
			   select element.GetValueFromJsonElement()).ToCollectionValue();
	}

	internal static ObjectMap GetObjectMap(this Dictionary<string, JsonElement> dictionary)
	{
		return new ObjectMap(dictionary.MapToImmutableList<KeyValuePair<string, JsonElement>, (string, IObjectMapValue)>((KeyValuePair<string, JsonElement> kv) => (name: kv.Key, value: kv.Value.GetValueFromJsonElement())));
	}
}
