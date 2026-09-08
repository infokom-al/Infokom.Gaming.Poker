using System;
using MemoryPools;

namespace BinarySerializer;

public class BinarySerializerReader
{
	public byte[] Buffer { get; set; }

	public int Position { get; set; }

	public long Length { get; set; }

	public long BytesLeft => Length - Position;

	public bool HasNext => Position < Length;

	public byte CurrentByte => Buffer[Position];

	public bool IsEmpty => Length == 0;

	public static BinarySerializerReader Create(byte[] payload)
	{
		return Create(payload, payload.Length);
	}

	public static BinarySerializerReader Create(byte[] payloadBuffer, int length)
	{
		BinarySerializerReader binarySerializerReader = ObjectPool<BinarySerializerReader>.ThreadShared.RentObject();
		binarySerializerReader.Buffer = payloadBuffer;
		binarySerializerReader.Length = length;
		binarySerializerReader.Position = 0;
		return binarySerializerReader;
	}

	public Span<byte> ReadBytes(int length)
	{
		Span<byte> result = Buffer.AsSpan(Position, length);
		AdvancePosition(length);
		return result;
	}

	public Span<byte> ReadBytesTillEnd()
	{
		return ReadBytes((int)BytesLeft);
	}

	public int ReadByte()
	{
		byte currentByte = CurrentByte;
		AdvancePosition();
		return currentByte;
	}

	public void AdvancePosition(int count = 1)
	{
		Position += count;
	}

	public void Skip(int bytes)
	{
		Position += bytes;
	}
}
