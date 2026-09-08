using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public static class JsonHelper
{
	public static void VerifyToken(this ref Utf8JsonReader reader, JsonTokenType tokenType)
	{
		if (reader.TokenType != tokenType)
		{
			throw new JsonException();
		}
	}

	public static void ReadToken(this ref Utf8JsonReader reader, JsonTokenType tokenType)
	{
		reader.Read();
		reader.VerifyToken(tokenType);
	}

	public static JsonSerializerOptions AddConverters(this JsonSerializerOptions options, List<JsonConverter> converters)
	{
		foreach (JsonConverter converter in converters)
		{
			options.Converters.Add(converter);
		}
		return options;
	}
}
