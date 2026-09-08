using System.Text.Json.Serialization;

namespace Poker.Calc;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
internal class JsonInterfaceConverterAttribute : JsonConverterAttribute
{
	public JsonInterfaceConverterAttribute(Type converterType)
		: base(converterType)
	{
	}
}
