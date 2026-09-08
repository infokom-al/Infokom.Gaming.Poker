using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public class InlineListConverter<T> : JsonConverter<InlineList<T>>
{
	public override InlineList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		InlineList<T> result = default(InlineList<T>);
		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException("Expected a JSON array for InlineList.");
		}
		reader.Read();
		while (reader.TokenType != JsonTokenType.EndArray)
		{
			T seat = JsonSerializer.Deserialize<T>(ref reader, options);
			result.Add(seat);
			reader.Read();
		}
		return result;
	}

	public override void Write(Utf8JsonWriter writer, InlineList<T> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		InlineList<T>.Enumerator enumerator = value.GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (current != null)
			{
				JsonSerializer.Serialize(writer, current, options);
			}
			else
			{
				writer.WriteNullValue();
			}
		}
		writer.WriteEndArray();
	}
}
