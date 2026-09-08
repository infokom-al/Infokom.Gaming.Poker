using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poker.Calc;

public class PreflopRangeHoldemConverter : JsonConverter<PreflopRangeHoldem>
{
	public override PreflopRangeHoldem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		reader.VerifyToken(JsonTokenType.StartObject);
		reader.ReadToken(JsonTokenType.PropertyName);
		reader.ReadToken(JsonTokenType.StartArray);
		List<PreflopRangeHoldemCell> list = new List<PreflopRangeHoldemCell>();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				reader.Read();
				return list.ToPreflopRangeHoldem();
			}
			list.Add(ReadCell(ref reader));
		}
		throw new JsonException();
		static PreflopRangeHoldemCell ReadCell(ref Utf8JsonReader reference)
		{
			reference.VerifyToken(JsonTokenType.Number);
			int rank = (int)reference.GetDecimal();
			reference.ReadToken(JsonTokenType.Number);
			CardRanks rank2 = (CardRanks)(int)reference.GetDecimal();
			reference.Read();
			Suitness suitness = (reference.GetBoolean() ? Suitness.Suited : Suitness.Offsuited);
			return new PreflopRangeHoldemCell((CardRanks)rank, rank2, suitness);
		}
	}

	public override void Write(Utf8JsonWriter writer, PreflopRangeHoldem value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteStartArray("Cells");
		value.Cells.ForEach(delegate (PreflopRangeHoldemCell cell)
		{
			writer.WriteNumberValue((int)cell.HighRank);
			writer.WriteNumberValue((int)cell.LowRank);
			writer.WriteBooleanValue(cell.IsSuited);
		});
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
