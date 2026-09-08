using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BinarySerializer;

public static class Deserialization
{
	public static Dictionary<Type, MethodInfo> ReadMethod { get; set; } = new Dictionary<Type, MethodInfo>();

	public static T BinaryDeserializeWithLengthPrefix<T>(this Stream stream)
	{
		int num = (int)stream.ReadVarInt();
		return BinarySerializerReader.Create(stream.GetBytes(num), num).BinaryDeserialize<T>();
	}

	public static T BinaryDeserialize<T>(this string file)
	{
		if (!file.FileExists())
		{
			return (T)typeof(T).GetDefaultValue();
		}
		using FileStream stream = File.OpenRead(file);
		return stream.BinaryDeserialize<T>();
	}

	public static T BinaryDeserialize<T>(this byte[] bytes)
	{
		return new MemoryStream(bytes).BinaryDeserialize<T>();
	}

	public static T BinaryDeserializeOptimized<T>(this byte[] bytes)
	{
		return BinarySerializerReader.Create(bytes, bytes.Length).BinaryDeserialize<T>();
	}

	public static T BinaryDeserialize<T>(this byte[] bytes, int bytesCount)
	{
		return new MemoryStream(bytes).Slice(bytesCount).BinaryDeserialize<T>();
	}

	public static T GetDefaultValue<T>()
	{
		return typeof(T).GetDefaultValue().VerifyType<T>();
	}

	private static MethodInfo GetReadMethod(this Type type)
	{
		return type.Assembly.GetTypes().First((Type type2) => type2.Name == "BinaryDeserializationGeneratedFunctions").GetMembers()
			.First((MemberInfo member) => member.Name == "Read" + type.Name)
			.VerifyType<MethodInfo>();
	}

	public static T BinaryDeserialize<T>(this Stream stream)
	{
		byte[] bytes = stream.GetBytes();
		return (T)BinarySerializerReader.Create(bytes, bytes.Length).Deserialize(typeof(T));
	}

	public static T BinaryDeserialize<T>(this BinarySerializerReader reader)
	{
		return (T)reader.Deserialize(typeof(T));
	}

	public static object? Deserialize(this BinarySerializerReader reader, Type type)
	{
		ITypeDefinition typeDefinition = type.GetTypeDefinition();
		ISerializableValue value;
		if (!(typeDefinition is ObjectTypeDefinition objectMapDefinition))
		{
			if (!(typeDefinition is InterfaceTypeDefinition typeDefinition2))
			{
				if (!(typeDefinition is CollectionTypeDefinition collectionTypeDefinition))
				{
					if (!(typeDefinition is DictionaryTypeDefinition dictionaryTypeDefinition))
					{
						throw new NotImplementedException("Can't deserialize " + type.Name);
					}
					value = reader.ReadDictionaryKeyValuesTillEnd(dictionaryTypeDefinition, 0).ToDictionarySerializableValue();
				}
				else
				{
					value = reader.ReadCollectionItems(collectionTypeDefinition, 0).ToCollectionSerializableValue();
				}
			}
			else
			{
				value = reader.ReadInterfaceTillEnd(typeDefinition2);
			}
		}
		else
		{
			value = reader.ReadObjectTillEnd(objectMapDefinition);
		}
		return value.GetObjectOfType(type);
	}

	public static ISerializableValue ReadSerializableValueAfterWireTypeTag(this BinarySerializerReader reader, WireType wireType, int tag, ITypeDefinition valueTypeDefinition)
	{
		if (wireType == WireType.Default)
		{
			return DefaultSerializableValue.Instance;
		}
		if (valueTypeDefinition is NullableTypeDefinition nullableTypeDefinition)
		{
			return reader.ReadSerializableValueAfterWireTypeTag(wireType, tag, nullableTypeDefinition.ValueTypeDefinitiion);
		}
		if (valueTypeDefinition is ConvertedTypeDefinition convertedTypeDefinition)
		{
			return reader.ReadSerializableValueAfterWireTypeTag(wireType, tag, convertedTypeDefinition.ConvertedValueTypeDefiniton);
		}
		long length = reader.Length;
		if (wireType.VerifyWireType(valueTypeDefinition) == WireType.LengthDelimited)
		{
			reader.Length = reader.ReadLength() + reader.Position;
		}
		ISerializableValue serializableValue;
		if (!(valueTypeDefinition is NumericTypeDefinition numericTypeDefinition))
		{
			if (!(valueTypeDefinition is StringTypeDefinition))
			{
				if (!(valueTypeDefinition is ObjectTypeDefinition objectMapDefinition))
				{
					if (!(valueTypeDefinition is InterfaceTypeDefinition typeDefinition))
					{
						if (!(valueTypeDefinition is CollectionTypeDefinition collectionDefinition))
						{
							if (!(valueTypeDefinition is DictionaryTypeDefinition dictionaryDefinition))
							{
								throw new NotImplementedException(valueTypeDefinition.GetType().Name);
							}
							serializableValue = reader.ReadDictionaryAfterWireTypeTag(wireType, tag, dictionaryDefinition);
						}
						else
						{
							serializableValue = reader.ReadCollectionAfterWireTypeTag(wireType, tag, collectionDefinition);
						}
					}
					else
					{
						serializableValue = reader.ReadInterfaceTillEnd(typeDefinition);
					}
				}
				else
				{
					serializableValue = reader.ReadObjectTillEnd(objectMapDefinition);
				}
			}
			else
			{
				serializableValue = new StringSerializableValue(reader.ReadStringTillEnd());
			}
		}
		else
		{
			serializableValue = reader.ReadNumericValue(numericTypeDefinition.Type, wireType);
		}
		ISerializableValue result = serializableValue;
		reader.Length = length;
		return result;
	}

	public static ObjectSerializableValue ReadObjectTillEnd(this BinarySerializerReader reader, ObjectTypeDefinition objectMapDefinition)
	{
		return new ObjectSerializableValue(0, reader.ReadObjectPropertiesTillEnd(objectMapDefinition).ToDictionary());
	}

	public static IEnumerable<(int tag, ISerializableValue value)> ReadObjectPropertiesTillEnd(this BinarySerializerReader reader, ObjectTypeDefinition objectMapDefinition)
	{
		while (reader.HasNext)
		{
			var (num, wireType) = reader.ReadTagWireType();
			if (num <= 0)
			{
				throw new InvalidPayloadException("Zero tag for object property is not expected");
			}
			if (!objectMapDefinition.TryGetPropertyType(num, out ITypeDefinition result))
			{
				reader.SkipValueSinceLengthPrefix(wireType);
				continue;
			}
			if (result is ConvertedTypeDefinition convertedTypeDefinition)
			{
				result = convertedTypeDefinition.ConvertedValueTypeDefiniton;
			}
			if (result is CollectionTypeDefinition collectionDefinition)
			{
				CollectionSerializableValue item = reader.ReadCollectionAfterWireTypeTag(wireType, num, collectionDefinition);
				yield return (tag: num, value: item);
			}
			else if (result is DictionaryTypeDefinition dictionaryDefinition)
			{
				DictionarySerializableValue item2 = reader.ReadDictionaryAfterWireTypeTag(wireType, num, dictionaryDefinition);
				yield return (tag: num, value: item2);
			}
			else
			{
				ISerializableValue item3 = reader.ReadSerializableValueAfterWireTypeTag(wireType, num, result);
				yield return (tag: num, value: item3);
			}
		}
	}

	public static ObjectSerializableValue ReadInterfaceTillEnd(this BinarySerializerReader reader, InterfaceTypeDefinition typeDefinition)
	{
		int num = reader.ReadVarInt().VerifyInt();
		if (num <= 0)
		{
			throw new InvalidPayloadException($"Type tag must be positive but was {num}");
		}
		if (!typeDefinition.TryGetSubTypeDefinition(num, out ObjectTypeDefinition result))
		{
			throw new InvalidPayloadException($"{typeDefinition.Type.Name} doesn't have sub type with id {num}");
		}
		return new ObjectSerializableValue(num, reader.ReadObjectPropertiesTillEnd(result).ToDictionary());
	}

	public static CollectionSerializableValue ReadCollectionAfterWireTypeTag(this BinarySerializerReader reader, WireType wireType, int collectionTag, CollectionTypeDefinition collectionDefinition)
	{
		return reader.ReadSerializableValueAfterWireTypeTag(wireType, collectionTag, collectionDefinition.ArgumentTypeDefinition).ToSingleIEnumerable().Concat(reader.ReadCollectionItems(collectionDefinition, collectionTag))
			.ToCollectionSerializableValue();
	}

	public static IEnumerable<ISerializableValue> ReadCollectionItems(this BinarySerializerReader reader, CollectionTypeDefinition collectionTypeDefinition, int collectionTag)
	{
		while (reader.HasNext)
		{
			int position = reader.Position;
			var (num, wireType) = reader.ReadTagWireType();
			if (num != collectionTag)
			{
				reader.Position = position;
				break;
			}
			yield return reader.ReadSerializableValueAfterWireTypeTag(wireType, collectionTag, collectionTypeDefinition.ArgumentTypeDefinition);
		}
	}

	public static DictionarySerializableValue ReadDictionaryAfterWireTypeTag(this BinarySerializerReader reader, WireType wireType, int dictionaryTag, DictionaryTypeDefinition dictionaryDefinition)
	{
		return reader.ReadKeyValueAfterKeyWireTypeTag(dictionaryTag, dictionaryDefinition.KeyValueDefinition).ToSingleIEnumerable().Concat(reader.ReadDictionaryKeyValuesTillEnd(dictionaryDefinition, dictionaryTag))
			.ToDictionarySerializableValue();
	}

	public static IEnumerable<ObjectSerializableValue> ReadDictionaryKeyValuesTillEnd(this BinarySerializerReader reader, DictionaryTypeDefinition dictionaryTypeDefinition, int dictionaryTag)
	{
		while (reader.HasNext)
		{
			int position = reader.Position;
			int item = reader.ReadTagWireType().tag;
			if (item != dictionaryTag)
			{
				reader.Position = position;
				break;
			}
			yield return reader.ReadKeyValueAfterKeyWireTypeTag(dictionaryTag, dictionaryTypeDefinition.KeyValueDefinition);
		}
	}

	public static ObjectSerializableValue ReadKeyValueAfterKeyWireTypeTag(this BinarySerializerReader reader, int tag, ITypeDefinition keyValueTypeDefinition)
	{
		return reader.ReadSerializableValueAfterWireTypeTag(WireType.LengthDelimited, tag, keyValueTypeDefinition).VerifyType<ObjectSerializableValue>();
	}

	public static ISerializableValue ReadNumericValue(this BinarySerializerReader reader, Type valueType, WireType wireType)
	{
		switch (wireType)
		{
		case WireType.VarInt:
			return new LongSerializableValue(reader.ReadVarInt());
		case WireType.VarNegativeInt:
			return new LongSerializableValue(reader.ReadVarNegativeInt());
		case WireType.Fixed32:
			if (valueType == typeof(int) || valueType.IsEnum)
			{
				return new LongSerializableValue(reader.ReadInt32());
			}
			if (valueType == typeof(float))
			{
				return new DoubleSerializableValue(reader.ReadFloat());
			}
			if (valueType == typeof(long))
			{
				return new LongSerializableValue(reader.ReadInt32());
			}
			break;
		case WireType.Fixed64:
			if (valueType == typeof(long) || valueType == typeof(DateTime) || valueType == typeof(TimeSpan))
			{
				return new LongSerializableValue(reader.ReadInt64());
			}
			if (valueType == typeof(double))
			{
				return new DoubleSerializableValue(reader.ReadDouble());
			}
			if (valueType == typeof(ulong))
			{
				return new ULongSerializableValue(reader.ReadUInt64());
			}
			break;
		case WireType.LengthDelimited:
			throw new InvalidOperationException("Expecting a numeric value type but was LengthDelimited");
		}
		throw new NotImplementedException($"wiretype={wireType}, valueType={valueType}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (int tag, WireType wireType) DecodeTagWireType(this int varIntValue)
	{
		int num = varIntValue >> 3;
		int num2 = varIntValue & 7;
		if (num < 0)
		{
			throw new ArgumentOutOfRangeException("tag", $"tag can't be negative but was {num}");
		}
		if (num2 < 0 || num2 > 7)
		{
			throw new ArgumentOutOfRangeException("typeCode", $"Invalid wire type {num2}.");
		}
		return (tag: num, wireType: (WireType)num2);
	}

	public static BinarySerializerReader SkipVarInt(this BinarySerializerReader reader)
	{
		reader.ReadVarInt();
		return reader;
	}

	public static long ReadVarNegativeInt(this BinarySerializerReader reader)
	{
		return reader.ReadVarInt() * -1;
	}

	public static long ReadVarInt(this BinarySerializerReader reader)
	{
		long num = 0L;
		int num2 = 0;
		do
		{
			int num3 = reader.ReadByte();
			if (num3 == -1)
			{
				throw new EndOfStreamException("End of stream reached without finding the end of the varint.");
			}
			num |= (long)(num3 & 0x7F) << num2;
			if ((num3 & 0x80) == 0)
			{
				return num;
			}
			num2 += 7;
		}
		while (num2 <= 63);
		throw new FormatException("Invalid varint encoding: Too many bytes.");
	}

	public static long ReadVarInt(this Stream reader)
	{
		long num = 0L;
		int num2 = 0;
		do
		{
			int num3 = reader.ReadByte();
			if (num3 == -1)
			{
				throw new EndOfStreamException("End of stream reached without finding the end of the varint.");
			}
			num |= (long)(num3 & 0x7F) << num2;
			if ((num3 & 0x80) == 0)
			{
				return num;
			}
			num2 += 7;
		}
		while (num2 <= 63);
		throw new FormatException("Invalid varint encoding: Too many bytes.");
	}

	public static long ReadLong(this BinarySerializerReader reader, WireType wireType)
	{
		return wireType switch
		{
			WireType.VarInt => reader.ReadVarInt(), 
			WireType.VarNegativeInt => reader.ReadVarNegativeInt(), 
			WireType.Fixed32 => reader.ReadInt32(), 
			WireType.Fixed64 => reader.ReadInt64(), 
			WireType.LengthDelimited => throw new InvalidOperationException("Numeric value can't have LengthDelimited wire type"), 
			_ => throw new NotImplementedException(wireType.ToString()), 
		};
	}

	public static int ReadTag(this BinarySerializerReader reader)
	{
		return reader.ReadTagWireType().tag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (int tag, WireType wireType) ReadTagWireType(this BinarySerializerReader reader)
	{
		(int, WireType) result = reader.ReadVarInt().VerifyInt().DecodeTagWireType();
		if (result.Item1 < 0)
		{
			throw new InvalidPayloadException($"Tag can't be negative but was {result.Item1}");
		}
		return result;
	}

	public static void SkipValueSinceLengthPrefix(this BinarySerializerReader reader, WireType wireType)
	{
		switch (wireType)
		{
		case WireType.VarInt:
			reader.SkipVarInt();
			break;
		case WireType.VarNegativeInt:
			reader.SkipVarInt();
			break;
		case WireType.Fixed32:
			reader.Skip(4);
			break;
		case WireType.Fixed64:
			reader.Skip(8);
			break;
		case WireType.LengthDelimited:
			reader.Skip(reader.ReadVarInt().VerifyInt());
			break;
		default:
			throw new NotImplementedException(wireType.ToString());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadLength(this BinarySerializerReader reader)
	{
		return reader.ReadVarInt().VerifyInt();
	}

	public static void VerifyIsLengthDelimited(this WireType wireType)
	{
		if (wireType != WireType.LengthDelimited)
		{
			throw new InvalidOperationException($"Expecting a LengthDelimited wire type but was {wireType}.");
		}
	}

	public static WireType VerifyWireType(this WireType wireType, ITypeDefinition typeDefinition)
	{
		if (wireType == WireType.LengthDelimited != typeDefinition.RequiresLengthDelimitedWireType())
		{
			throw new InvalidOperationException($"Wire type {wireType} is not compatable with {typeDefinition}");
		}
		return wireType;
	}

	public static bool RequiresLengthDelimitedWireType(this ITypeDefinition typeDefinition)
	{
		bool flag = ((typeDefinition is ObjectTypeDefinition || typeDefinition is InterfaceTypeDefinition || typeDefinition is StringTypeDefinition || typeDefinition is DictionaryTypeDefinition) ? true : false);
		if (!flag && (!(typeDefinition is CollectionTypeDefinition collectionTypeDefinition) || !collectionTypeDefinition.ArgumentTypeDefinition.RequiresLengthDelimitedWireType()))
		{
			if (typeDefinition is NullableTypeDefinition nullableTypeDefinition)
			{
				return nullableTypeDefinition.ValueTypeDefinitiion.RequiresLengthDelimitedWireType();
			}
			return false;
		}
		return true;
	}

	public static void VerifyTagNotZero(this int tag)
	{
		if (tag == 0)
		{
			throw new InvalidPayloadException("Zero tag is not expected here.");
		}
	}

	public static DateTime ReadDateTime(this BinarySerializerReader reader)
	{
		WireType item = reader.ReadTagWireType().wireType;
		return new DateTime(reader.ReadLong(item));
	}

	public static TimeSpan ReadTimeSpan(this BinarySerializerReader reader)
	{
		WireType item = reader.ReadTagWireType().wireType;
		return new TimeSpan(reader.ReadLong(item));
	}

	public static Guid ReadGuid(this BinarySerializerReader reader)
	{
		long num = 0L;
		long num2 = 0L;
		if (!reader.HasNext)
		{
			return new GuidSerializable(0L, 0L).GetGuid();
		}
		(int tag, WireType wireType) tuple = reader.ReadTagWireType();
		int item = tuple.tag;
		WireType item2 = tuple.wireType;
		long num3 = reader.ReadLong(item2);
		if (!reader.HasNext)
		{
			if (item != 1)
			{
				long num4 = num3;
				num2 = num4;
				num = 0L;
			}
			else
			{
				num2 = 0L;
				num = num3;
			}
			return new GuidSerializable(num, num2).GetGuid();
		}
		WireType item3 = reader.ReadTagWireType().wireType;
		long num5 = reader.ReadLong(item3);
		if (item != 1)
		{
			long num4 = num3;
			num2 = num4;
			num = num5;
		}
		else
		{
			long num4 = num5;
			num2 = num4;
			num = num3;
		}
		return new GuidSerializable(num, num2).GetGuid();
	}

	public static T ReadCollection<T>(this BinarySerializerReader reader, Func<BinarySerializerReader, int, WireType, T> readAfterTagWireType)
	{
		var (arg, arg2) = reader.ReadTagWireType();
		return readAfterTagWireType(reader, arg, arg2);
	}
}
