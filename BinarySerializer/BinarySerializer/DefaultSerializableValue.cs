namespace BinarySerializer;

public record DefaultSerializableValue : ISerializableValue
{
	public static DefaultSerializableValue Instance => new DefaultSerializableValue();
}
