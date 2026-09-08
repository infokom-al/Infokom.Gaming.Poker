using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using MemoryPools;

namespace BinarySerializer;

public static class Serialization
{
	private const int MaxEncodedFieldNumber = 536870911;

	public static long MaxLongAsVarInt = 562949953421311L;

	public static int MaxIntAsVarInt = 2097151;

	internal static readonly UTF8Encoding UTF8 = new UTF8Encoding();

	public static void BinarySerializeWithLengthPrefix<T>(this T @object, Stream stream)
	{
		BinarySerializerWriter binarySerializerWriter = BinarySerializerWriter.ThreadShared(stream);
		binarySerializerWriter.WriteWithLengthPrefix(@object, delegate(BinarySerializerWriter writer, T val)
		{
			writer.WriteSerializableValueWithoutHeader(val.GetSerializableValue(typeof(T)), 0);
		});
		binarySerializerWriter.Flush();
	}

	public static byte[] BinarySerializeWithLengthPrefixToBytes<T>(this T @object)
	{
		using MemoryStream stream = new MemoryStream();
		@object.BinarySerializeWithLengthPrefix(stream);
		return stream.Reset().ToArray();
	}

	public static T BinarySerialize<T>(this T @object, string file)
	{
		return @object.BinarySerialize(file, flushToDisk: false);
	}

	public static T BinarySerialize<T>(this T @object, string file, bool flushToDisk)
	{
		using FileStream fileStream = file.OpenFileOverwrite();
		@object.BinarySerialize(fileStream);
		fileStream.Flush(flushToDisk);
		return @object;
	}

	public static byte[] BinarySerializeToBytes<T>(this T @object)
	{
		return @object.BinarySerializeToBytes(typeof(T));
	}

	public static byte[] BinarySerializeToBytes<T>(this T @object, MemoryStream bufferStream)
	{
		return @object.BinarySerializeToBytes(typeof(T), bufferStream);
	}

	public static byte[] BinarySerializeToBytes(this object @object, Type type)
	{
		return @object.BinarySerializeToBytes(type, new MemoryStream());
	}

	public static byte[] BinarySerializeToBytes(this object @object, Type type, MemoryStream bufferStream)
	{
		if (bufferStream.Position != 0L)
		{
			bufferStream.SetLength(0L);
		}
		BinarySerializerWriter binarySerializerWriter = BinarySerializerWriter.ThreadShared(bufferStream);
		binarySerializerWriter.WriteSerializableValueWithoutHeader(@object.GetSerializableValue(type), 0).Flush();
		binarySerializerWriter.Flush();
		return bufferStream.ToArray();
	}

	public static void BinarySerialize<T>(this T @object, Stream stream)
	{
		BinarySerializerWriter binarySerializerWriter = BinarySerializerWriter.ThreadShared(stream);
		binarySerializerWriter.Serialize(@object);
		binarySerializerWriter.Flush();
	}

	public static BinarySerializerWriter Serialize<T>(this BinarySerializerWriter stream, T @object)
	{
		return stream.WriteSerializableValueWithoutHeader(@object.GetSerializableValue(), 0).Flush();
	}

	public static BinarySerializerWriter WriteSerializableValueWithoutHeader(this BinarySerializerWriter writer, ISerializableValue value, int tag)
	{
		if (!(value is ObjectSerializableValue objectSerializableValue))
		{
			if (!(value is CollectionSerializableValue collectionValue))
			{
				if (!(value is DictionarySerializableValue dictionary))
				{
					if (!(value is StringSerializableValue stringSerializableValue))
					{
						if (!(value is ULongSerializableValue uLongSerializableValue))
						{
							if (!(value is DoubleSerializableValue doubleSerializableValue))
							{
								if (!(value is FloatSerializableValue floatSerializableValue))
								{
									if (!(value is LongSerializableValue longSerializableValue))
									{
										if (value is DefaultSerializableValue)
										{
											return writer;
										}
										throw new NotImplementedException(value.GetType().Name);
									}
									return writer.WriteLong(longSerializableValue.Value);
								}
								return writer.WriteFloat(floatSerializableValue.Value);
							}
							return writer.WriteDouble(doubleSerializableValue.Value);
						}
						return writer.WriteUlong(uLongSerializableValue.Value);
					}
					return writer.WriteString(stringSerializableValue.Value);
				}
				return writer.WriteDictionary(dictionary, tag);
			}
			return writer.WriteCollection(collectionValue, tag);
		}
		return writer.WriteObjectWithoutHeader(objectSerializableValue);
	}

	public static BinarySerializerWriter WriteSerializableValueWithHeader(this BinarySerializerWriter writer, int tag, ISerializableValue value)
	{
		if ((value is CollectionSerializableValue || value is DictionarySerializableValue) ? true : false)
		{
			return writer.WriteSerializableValueWithoutHeader(value, tag);
		}
		WireType wireType = value.GetWireType();
		writer.WriteTagWireType(tag, wireType);
		if (value is StringSerializableValue stringSerializableValue)
		{
			return writer.WriteString(stringSerializableValue.Value);
		}
		if (wireType == WireType.LengthDelimited)
		{
			return writer.WriteWithLengthPrefix(value, delegate(BinarySerializerWriter writer2, ISerializableValue value2)
			{
				writer2.WriteSerializableValueWithoutHeader(value2, tag);
			});
		}
		return writer.WriteSerializableValueWithoutHeader(value, tag);
	}

	private static BinarySerializerWriter WriteObjectWithoutHeader(this BinarySerializerWriter writer, ObjectSerializableValue @object)
	{
		if (@object.TypeTag != 0)
		{
			writer.WriteVarInt(@object.TypeTag);
		}
		foreach (var (tag, value) in @object.Properties)
		{
			writer.WriteSerializableValueWithHeader(tag, value);
		}
		return writer;
	}

	public static WireType GetWireType(this long value)
	{
		if (value == long.MinValue)
		{
			return WireType.Fixed64;
		}
		WireType result;
		long num2;
		if (value < 0)
		{
			long num = value * -1;
			result = WireType.VarNegativeInt;
			num2 = num;
		}
		else
		{
			result = WireType.VarInt;
			num2 = value;
		}
		if (num2 <= MaxIntAsVarInt)
		{
			return result;
		}
		if (num2 <= int.MaxValue)
		{
			return WireType.Fixed32;
		}
		if (num2 <= MaxLongAsVarInt)
		{
			return result;
		}
		return WireType.Fixed64;
	}

	public static WireType GetWireType(this byte value)
	{
		return ((long)value).GetWireType();
	}

	public static WireType GetWireType(this int value)
	{
		return ((long)value).GetWireType();
	}

	public static WireType GetWireType(this short value)
	{
		return ((long)value).GetWireType();
	}

	public static WireType GetWireType(this uint value)
	{
		return ((long)value).GetWireType();
	}

	public static BinarySerializerWriter WriteLongWithHeader(this BinarySerializerWriter writer, long value, int tag)
	{
		WireType wireType = value.GetWireType();
		writer.WriteTagWireType(tag, wireType);
		writer.WriteLong(value, wireType);
		return writer;
	}

	public static BinarySerializerWriter WriteDoubleWithHeader(this BinarySerializerWriter writer, double value, int tag)
	{
		return writer.WriteTagWireType(tag, WireType.Fixed64).WriteDouble(value);
	}

	public static BinarySerializerWriter WriteFloatWithHeader(this BinarySerializerWriter writer, float value, int tag)
	{
		return writer.WriteTagWireType(tag, WireType.Fixed32).WriteFloat(value);
	}

	public static BinarySerializerWriter WriteBoolWithHeader(this BinarySerializerWriter writer, bool value, int tag)
	{
		return writer.WriteLongWithHeader(value ? 1 : 0, tag);
	}

	public static BinarySerializerWriter WriteLong(this BinarySerializerWriter writer, long value)
	{
		return writer.WriteLong(value, value.GetWireType());
	}

	public static BinarySerializerWriter WriteLong(this BinarySerializerWriter writer, long value, WireType wireType)
	{
		return wireType switch
		{
			WireType.VarInt => writer.WriteVarInt(value), 
			WireType.VarNegativeInt => writer.WriteVarNegativeInt(value), 
			WireType.Fixed32 => writer.WriteInt32((int)value), 
			WireType.Fixed64 => writer.WriteInt64(value), 
			WireType.LengthDelimited => throw new InvalidOperationException("Number can't be length delimited"), 
			_ => throw new NotImplementedException(wireType.ToString()), 
		};
	}

	public static BinarySerializerWriter WriteVarNegativeInt(this BinarySerializerWriter writer, long value)
	{
		return writer.WriteVarInt(value * -1);
	}

	public static BinarySerializerWriter WriteCollection(this BinarySerializerWriter writer, CollectionSerializableValue collectionValue, int tag)
	{
		foreach (ISerializableValue value2 in collectionValue.Values)
		{
			ISerializableValue value = value2;
			writer.WriteSerializableValueWithHeader(tag, value);
		}
		return writer;
	}

	public static BinarySerializerWriter WriteDictionary(this BinarySerializerWriter writer, DictionarySerializableValue dictionary, int tag)
	{
		foreach (ObjectSerializableValue keyValue in dictionary.KeyValues)
		{
			writer.WriteSerializableValueWithHeader(tag, keyValue);
		}
		return writer;
	}

	public static BinarySerializerWriter WriteTagWireType(this BinarySerializerWriter writer, int tag, WireType wireType)
	{
		return writer.WriteVarInt(tag.EncodeTagWireType(wireType));
	}

	public static BinarySerializerWriter WritePlainBytesTagWireType(this BinarySerializerWriter writer, int tag)
	{
		return writer.WriteTagWireType(tag, WireType.Default);
	}

	public static int EncodeTagWireType(this int tag, WireType wireType)
	{
		if ((tag < 0 || tag > 536870911) ? true : false)
		{
			throw new ArgumentOutOfRangeException("tag", "Field number must be between 0 and 536,870,911 (inclusive).");
		}
		if ((wireType < WireType.VarInt || wireType > (WireType)7) ? true : false)
		{
			throw new ArgumentOutOfRangeException("typeCode", "Invalid wire type.");
		}
		return (tag << 3) | (int)wireType;
	}

	public static BinarySerializerWriter WriteString(this BinarySerializerWriter writer, string @string, int tag)
	{
		writer.WriteTagWireType(tag, WireType.LengthDelimited);
		writer.WriteString(@string);
		return writer;
	}

	public static BinarySerializerWriter WriteString(this BinarySerializerWriter writer, string @string)
	{
		int byteCount = UTF8.GetByteCount(@string);
		return BinarySerializerWriterFunctions.WriteString(writer.WriteVarInt(byteCount), @string, byteCount);
	}

	public static BinarySerializerWriter WriteWithLengthPrefix<T>(this BinarySerializerWriter writer, T value, Action<BinarySerializerWriter, T> writeValue, int tag)
	{
		return writer.WriteTagWireType(tag, WireType.LengthDelimited).WriteWithLengthPrefix(value, writeValue);
	}

	public static WireType GetWireType(this ISerializableValue value)
	{
		if (!(value is LongSerializableValue longSerializableValue))
		{
			if (!(value is ULongSerializableValue) && !(value is DoubleSerializableValue))
			{
				if (!(value is FloatSerializableValue))
				{
					if (!(value is ObjectSerializableValue) && !(value is CollectionSerializableValue) && !(value is DictionarySerializableValue) && !(value is StringSerializableValue))
					{
						if (value is DefaultSerializableValue)
						{
							return WireType.Default;
						}
						throw new NotImplementedException(value.GetType().Name);
					}
					return WireType.LengthDelimited;
				}
				return WireType.Fixed32;
			}
			return WireType.Fixed64;
		}
		return longSerializableValue.Value.GetWireType();
	}

	[BinarySerializationConverter]
	public static GuidSerializable GetGuidSerializable(this Guid guid)
	{
		byte[] value = guid.ToByteArray();
		long higherBits = BitConverter.ToInt64(value, 0);
		long lowerBits = BitConverter.ToInt64(value, 8);
		return new GuidSerializable(higherBits, lowerBits);
	}

	[BinaryDeserializationConverter]
	public static Guid GetGuid(this GuidSerializable bytes)
	{
		byte[] array = new byte[16];
		BitConverter.GetBytes(bytes.HigherBits).CopyTo(array, 0);
		BitConverter.GetBytes(bytes.LowerBits).CopyTo(array, 8);
		return new Guid(array);
	}

	public static void WriteDateTime(this BinarySerializerWriter writer, DateTime value)
	{
		writer.WriteLongWithHeader(value.Ticks, 1);
	}

	public static void WriteTimeSpan(this BinarySerializerWriter writer, TimeSpan value)
	{
		writer.WriteLongWithHeader(value.Ticks, 1);
	}

	public static void WriteGuid(this BinarySerializerWriter writer, Guid value)
	{
		GuidSerializable guidSerializable = value.GetGuidSerializable();
		if (guidSerializable.HigherBits != 0L)
		{
			writer.WriteLongWithHeader(guidSerializable.HigherBits, 1);
		}
		if (guidSerializable.LowerBits != 0L)
		{
			writer.WriteLongWithHeader(guidSerializable.LowerBits, 2);
		}
	}

	public static int GetVarIntSize(this int value)
	{
		return ((long)value).GetVarIntSize();
	}

	public static int GetVarIntSize(this long value)
	{
		if (value <= 127)
		{
			return 1;
		}
		value >>= 7;
		int num = 2;
		while ((value >>= 7) != 0L)
		{
			num++;
		}
		return num;
	}

	public static void WriteAsPlainBytes<TKey, TValue>(this BinarySerializerWriter writer, MemoryDictionary<TKey, TValue> dictionary, int tag) where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		writer.WritePlainBytesTagWireType(tag);
		writer.WritePlainMemoryList(dictionary.Buckets);
		writer.WritePlainMemoryList(dictionary.Entries);
	}

	public static MemoryDictionary<TKey, TValue> ReadPlainMemoryDictionary<TKey, TValue>(this BinarySerializerReader reader) where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		MemoryList<int> buckets = reader.ReadPlainMemoryList<int>();
		MemoryList<MemoryDictionary<TKey, TValue>.Entry> entries = reader.ReadPlainMemoryList<MemoryDictionary<TKey, TValue>.Entry>();
		return new MemoryDictionary<TKey, TValue>(buckets, entries);
	}

	public static void WriteAsPlainBytes<T>(this BinarySerializerWriter writer, MemoryList<T> list, int tag) where T : struct
	{
		writer.WritePlainBytesTagWireType(tag);
		writer.WritePlainMemoryList(list);
	}

	public static void WritePlainMemoryList<T>(this BinarySerializerWriter writer, MemoryList<T> list) where T : struct
	{
		writer.WriteVarInt(list.Count);
		int num = list.Count;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < list.Buffer.AllocatedBucketCount; i++)
		{
			Span<T> span = list.Buffer.GetBucketAt(i).Span;
			writer.WriteBytes(span.Slice(0, Math.Min(num, span.Length)).CastToBytes());
			num -= span.Length;
			if (num <= 0)
			{
				break;
			}
		}
	}

	public static MemoryList<T> ReadPlainMemoryList<T>(this BinarySerializerReader reader) where T : struct
	{
		int num = reader.ReadVarInt().VerifyInt();
		if (num == 0)
		{
			return default(MemoryList<T>);
		}
		return new MemoryList<T>(reader.ReadBytes(num * Unsafe.SizeOf<T>()).CastMemory<T>());
	}
}
