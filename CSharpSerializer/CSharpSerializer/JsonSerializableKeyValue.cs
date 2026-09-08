namespace CSharpSerializer;

internal class JsonSerializableKeyValue
{
	public object Key { get; }

	public object Value { get; }

	public JsonSerializableKeyValue(object key, object value)
	{
		Key = key;
		Value = value;
	}
}
