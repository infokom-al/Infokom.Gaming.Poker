using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public class PocketsRangeHoldemConverter : JsonConverter<PocketRangeHoldem>
{
	public override PocketRangeHoldem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		reader.VerifyToken(JsonTokenType.StartObject);
		reader.ReadToken(JsonTokenType.PropertyName);
		reader.ReadToken(JsonTokenType.StartArray);
		List<PocketCardsHoldem> list = new List<PocketCardsHoldem>();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				reader.ReadToken(JsonTokenType.EndObject);
				return list.ToPocketsRangeHoldem();
			}
			list.Add(reader.GetString()?.ParsePocketCardsHoldem() ?? throw new JsonException());
		}
		throw new JsonException();
	}

	public override void Write(Utf8JsonWriter writer, PocketRangeHoldem value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteStartArray("Cards");
		value.Cards.ForEach(delegate (PocketCardsHoldem pocketCards)
		{
			writer.WriteStringValue(pocketCards.AbbreviationAhJs);
		});
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
