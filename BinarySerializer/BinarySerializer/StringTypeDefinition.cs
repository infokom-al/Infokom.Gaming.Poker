namespace BinarySerializer;

public record StringTypeDefinition : ITypeDefinition
{
	public static StringTypeDefinition Instance = new StringTypeDefinition();
}
