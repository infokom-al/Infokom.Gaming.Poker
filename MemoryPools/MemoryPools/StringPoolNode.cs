namespace MemoryPools;

public class StringPoolNode
{
	public string Value { get; }

	public StringPoolNode? Next { get; }

	public StringPoolNode(string value, StringPoolNode? next)
	{
		Value = value;
		Next = next;
	}
}
