using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public class PreflopRangeConverter : JsonConverter<IPreflopRange>
{
	public override IPreflopRange? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!JsonDocument.TryParseValue(ref reader, out JsonDocument document))
		{
			throw new JsonException();
		}
		JsonElement value = document.RootElement;
		string rawText = value.GetRawText();
		if (document.RootElement.TryGetProperty("Cards", out value))
		{
			return JsonSerializer.Deserialize<PocketRangeHoldem>(rawText);
		}
		if (document.RootElement.TryGetProperty("Cells", out value))
		{
			return JsonSerializer.Deserialize<PreflopRangeHoldem>(rawText);
		}
		if (document.RootElement.TryGetProperty("Range", out value))
		{
			return JsonSerializer.Deserialize<PocketsRangeWeightedHoldem>(rawText);
		}
		throw new JsonException();
	}

	public override void Write(Utf8JsonWriter writer, IPreflopRange value, JsonSerializerOptions options)
	{
		if (!(value is PreflopRangeHoldem preflopRangeHoldem))
		{
			if (!(value is PocketRangeHoldem pocketRangeHoldem))
			{
				if (value is PocketsRangeWeightedHoldem pocketsRangeWeightedHoldem)
				{
					JsonSerializer.Serialize(writer, pocketsRangeWeightedHoldem, pocketsRangeWeightedHoldem.GetType(), options);
				}
			}
			else
			{
				JsonSerializer.Serialize(writer, pocketRangeHoldem, pocketRangeHoldem.GetType(), options);
			}
		}
		else
		{
			JsonSerializer.Serialize(writer, preflopRangeHoldem, preflopRangeHoldem.GetType(), options);
		}
	}
}
