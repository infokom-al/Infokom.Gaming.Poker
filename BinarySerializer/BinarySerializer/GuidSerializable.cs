namespace BinarySerializer;

[BinarySerializable]
public struct GuidSerializable
{
	[Tag(1)]
	public long HigherBits { get; }

	[Tag(2)]
	public long LowerBits { get; }

	public GuidSerializable(long higherBits, long lowerBits)
	{
		HigherBits = higherBits;
		LowerBits = lowerBits;
	}
}
