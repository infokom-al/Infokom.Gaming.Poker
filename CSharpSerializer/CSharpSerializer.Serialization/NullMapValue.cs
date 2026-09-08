namespace CSharpSerializer.Serialization;

public record NullMapValue : IObjectMapValue
{
	public static NullMapValue Instance = new NullMapValue();
}
