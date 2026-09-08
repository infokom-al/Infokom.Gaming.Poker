using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public class PocketsRangeWeightedHoldemConverter : JsonConverter<PocketsRangeWeightedHoldem>
{
	public override PocketsRangeWeightedHoldem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		reader.VerifyToken(JsonTokenType.StartObject);
		reader.ReadToken(JsonTokenType.PropertyName);
		reader.ReadToken(JsonTokenType.StartArray);
		List<(PocketCardsHoldem, double)> list = new List<(PocketCardsHoldem, double)>();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				reader.ReadToken(JsonTokenType.EndObject);
				return list.ToPocketsRangeWeightedHoldemHoldem();
			}
			list.Add(ParsePocketWithWeight(reader.GetString() ?? throw new JsonException()));
		}
		throw new JsonException();
		static (PocketCardsHoldem pocket, double weightUnitInterval) ParsePocketWithWeight(string @string)
		{
			string[] array = @string.Split(' ');
			return (pocket: array[0].ParsePocketCardsHoldem(), weightUnitInterval: double.Parse(array[1]));
		}
	}

	public override void Write(Utf8JsonWriter writer, PocketsRangeWeightedHoldem value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteStartArray("Range");
		value.Range.ForEach<(PocketCardsHoldem, double)>(delegate ((PocketCardsHoldem pocket, double weightUnitInterval) item)
		{
			writer.WriteStringValue($"{item.pocket} {item.weightUnitInterval}");
		});
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
