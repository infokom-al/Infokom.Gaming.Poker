using System;
using System.Buffers.Binary;
using System.Text;

namespace BinarySerializer;

public static class BinarySerializerWriterFunctions
{
	internal static readonly UTF8Encoding UTF8 = new UTF8Encoding();

	public static BinarySerializerWriter WriteVarInt(this BinarySerializerWriter writer, long value)
	{
		if (value < 0)
		{
			if (value == long.MinValue)
			{
				throw new InvalidOperationException("Number is too large, use fixed64 instead)");
			}
			value *= -1;
		}
		do
		{
			byte b = (byte)(value & 0x7F);
			value >>= 7;
			if (value != 0L)
			{
				b |= 0x80;
			}
			writer.WriteByte(b);
		}
		while (value != 0L);
		return writer;
	}

	public static BinarySerializerWriter WriteString(this BinarySerializerWriter writer, string value, int expectedBytes)
	{
		int bytes = UTF8.GetBytes(value, writer.GetNextSpan(expectedBytes));
		writer.AdvancePosition(bytes);
		return writer;
	}

	public static BinarySerializerWriter WriteUint32(this BinarySerializerWriter writer, uint value)
	{
		BinaryPrimitives.WriteUInt32LittleEndian(writer.GetNextSpan(4), value);
		return writer.AdvancePosition(4);
	}

	public static BinarySerializerWriter WriteInt32(this BinarySerializerWriter writer, int value)
	{
		BinaryPrimitives.WriteInt32LittleEndian(writer.GetNextSpan(4), value);
		return writer.AdvancePosition(4);
	}

	public static BinarySerializerWriter WriteUlong(this BinarySerializerWriter writer, ulong value)
	{
		BinaryPrimitives.WriteUInt64LittleEndian(writer.GetNextSpan(8), value);
		return writer.AdvancePosition(8);
	}

	public static BinarySerializerWriter WriteInt64(this BinarySerializerWriter writer, long value)
	{
		BinaryPrimitives.WriteInt64LittleEndian(writer.GetNextSpan(8), value);
		return writer.AdvancePosition(8);
	}

	public static BinarySerializerWriter WriteFloat(this BinarySerializerWriter writer, float value)
	{
		BinaryPrimitives.WriteSingleLittleEndian(writer.GetNextSpan(4), value);
		writer.AdvancePosition(4);
		return writer;
	}

	public static BinarySerializerWriter WriteDouble(this BinarySerializerWriter writer, double value)
	{
		BinaryPrimitives.WriteDoubleLittleEndian(writer.GetNextSpan(8), value);
		writer.AdvancePosition(8);
		return writer;
	}

	public static BinarySerializerWriter WriteDecimal(this BinarySerializerWriter writer, decimal value)
	{
		int[] bits = decimal.GetBits(value);
		writer.WriteInt32(bits[0]).WriteInt32(bits[1]).WriteInt32(bits[2])
			.WriteInt32(bits[3]);
		return writer;
	}

	public static BinarySerializerWriter WriteWithLengthPrefix<T>(this BinarySerializerWriter writer, T value, Action<BinarySerializerWriter, T> writeValue)
	{
		int position = writer.Position;
		int num = position + 1;
		writer.AdvancePosition();
		writeValue(writer, value);
		int num2 = writer.Position - num;
		if (num2 < 0)
		{
			throw new InvalidOperationException("Object with empty properties should not be serialized");
		}
		int varIntSize = num2.GetVarIntSize();
		if (varIntSize == 1)
		{
			writer.Buffer[position] = (byte)(num2 & 0x7F);
		}
		else
		{
			writer.AllocateSpace(varIntSize);
			Array.Copy(writer.Buffer, num, writer.Buffer, position + varIntSize, num2);
			writer.MoveTo(position).WriteVarInt(num2).MoveTo(position + varIntSize + num2);
		}
		return writer;
	}
}
