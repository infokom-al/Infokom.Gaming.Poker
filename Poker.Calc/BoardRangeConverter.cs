using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public class BoardRangeConverter : JsonConverter<BoardRange>
{
	public override BoardRange? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		reader.VerifyToken(JsonTokenType.StartObject);
		reader.ReadToken(JsonTokenType.PropertyName);
		reader.ReadToken(JsonTokenType.StartArray);
		List<string> list = new List<string>();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				reader.ReadToken(JsonTokenType.EndObject);
				return list.Select((string abbreviation) => abbreviation.ParseCards().AsEnumerable()).ToBoardRange();
			}
			list.Add(reader.GetString() ?? throw new JsonException());
		}
		throw new JsonException();
	}

	public override void Write(Utf8JsonWriter writer, BoardRange value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("BoardCardRanges");
		writer.WriteStartArray();
		value.BoardCardRanges.ForEach(delegate (ImmutableList<Card> cards)
		{
			writer.WriteStringValue(cards.ToAbbreviation());
		});
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
