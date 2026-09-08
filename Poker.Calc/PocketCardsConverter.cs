using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public class PocketCardsConverter : JsonConverter<IPocketCards>
{
	public override IPocketCards? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		reader.VerifyToken(JsonTokenType.StartObject);
		reader.ReadToken(JsonTokenType.PropertyName);
		reader.ReadToken(JsonTokenType.StartArray);
		string text = "";
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				reader.ReadToken(JsonTokenType.EndObject);
				return text.ParseCards().ToPocketCards();
			}
			text += reader.GetString() ?? throw new JsonException();
		}
		throw new JsonException();
	}

	public override void Write(Utf8JsonWriter writer, IPocketCards value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteStartArray("Cards");
		InlineList<Card>.Enumerator enumerator = value.Cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			writer.WriteStringValue(enumerator.Current.Abbreviation);
		}
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
