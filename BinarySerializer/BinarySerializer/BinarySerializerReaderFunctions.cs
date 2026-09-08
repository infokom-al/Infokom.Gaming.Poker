using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using MemoryPools;

namespace BinarySerializer;

public static class BinarySerializerReaderFunctions
{
	private static readonly UTF8Encoding UTF8 = new UTF8Encoding();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadInt32(this BinarySerializerReader reader)
	{
		return BinaryPrimitives.ReadInt32LittleEndian(reader.ReadBytes(4));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long ReadInt64(this BinarySerializerReader reader)
	{
		return BinaryPrimitives.ReadInt64LittleEndian(reader.ReadBytes(8));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong ReadUInt64(this BinarySerializerReader reader)
	{
		return BinaryPrimitives.ReadUInt64LittleEndian(reader.ReadBytes(8));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double ReadDouble(this BinarySerializerReader reader)
	{
		return BinaryPrimitives.ReadDoubleLittleEndian(reader.ReadBytes(8));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ReadFloat(this BinarySerializerReader reader)
	{
		return BinaryPrimitives.ReadSingleLittleEndian(reader.ReadBytes(4));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ReadStringLengthPrefixed(this BinarySerializerReader reader)
	{
		int num = reader.ReadLength();
		string result = StringPool.GetString(new ReadOnlySpan<byte>(reader.Buffer, reader.Position, num), Encoding.UTF8);
		reader.AdvancePosition(num);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ReadStringTillEnd(this BinarySerializerReader reader)
	{
		return StringPool.GetString(reader.ReadBytesTillEnd(), Encoding.UTF8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal ReadDecimal(this BinarySerializerReader reader)
	{
		return new decimal(new int[4]
		{
			reader.ReadInt32(),
			reader.ReadInt32(),
			reader.ReadInt32(),
			reader.ReadInt32()
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ReadLengthDelimited<T>(this BinarySerializerReader reader, Func<BinarySerializerReader, T> read)
	{
		long length = reader.Length;
		reader.Length = reader.ReadLength() + reader.Position;
		T result = read(reader);
		reader.Length = length;
		return result;
	}
}
