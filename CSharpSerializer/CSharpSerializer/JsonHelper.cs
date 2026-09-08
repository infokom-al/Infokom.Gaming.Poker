using System;
using System.Text.Json;

namespace CSharpSerializer;

internal static class JsonHelper
{
	public static string ToSystemJson(this object @object)
	{
		return JsonSerializer.Serialize(@object);
	}

	public static string ToSystemJson(this object @object, JsonSerializerOptions options)
	{
		return JsonSerializer.Serialize(@object, options);
	}

	public static string ToJsonIndented(this object @object)
	{
		JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
		jsonSerializerOptions.WriteIndented = true;
		return JsonSerializer.Serialize(@object, jsonSerializerOptions);
	}

	internal static T ParseJson<T>(this string json)
	{
		return JsonSerializer.Deserialize<T>(json);
	}

	internal static JsonElement VerifyValueKind(this JsonElement element, JsonValueKind expectedValueKind)
	{
		if (element.ValueKind != expectedValueKind)
		{
			throw new InvalidCastException($"Expecting json element with value kind of {expectedValueKind} but was {element.ValueKind}");
		}
		return element;
	}
}
