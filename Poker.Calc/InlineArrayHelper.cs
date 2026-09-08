using BinarySerializer;

using MemoryPools;

namespace Poker.Calc;

public static class InlineArrayHelper
{
	public static Items10<T> ToItems10<T>(this SpanList<T> list)
	{
		Items10<T> result = default(Items10<T>);
		for (int i = 0; i < list.Count; i++)
		{
			result[i] = list[i];
		}
		return result;
	}

	public static Items4<T> ToItems4<T>(this SpanList<T> list)
	{
		Items4<T> result = default(Items4<T>);
		for (int i = 0; i < list.Count; i++)
		{
			result[i] = list[i];
		}
		return result;
	}

	[BinarySerializationConverter]
	public static T[] ToSerializableObject<T>(this Items10<T> array)
	{
		T[] array2 = new T[10];
		for (int i = 0; i < 10; i++)
		{
			array2[i] = array[i];
		}
		return array2;
	}

	[BinarySerializationConverter]
	public static T[] ToSerializableObject<T>(this Items4<T> array)
	{
		T[] array2 = new T[4];
		for (int i = 0; i < 4; i++)
		{
			array2[i] = array[i];
		}
		return array2;
	}

	[BinaryDeserializationConverter]
	public static Items10<T> GetItems10FromSerializableObject<T>(this T[] array)
	{
		Items10<T> result = default(Items10<T>);
		for (int i = 0; i < array.Length; i++)
		{
			result[i] = array[i];
		}
		return result;
	}

	[BinaryDeserializationConverter]
	public static Items4<T> GetItems4FromSerializableObject<T>(this T[] array)
	{
		Items4<T> result = default(Items4<T>);
		for (int i = 0; i < array.Length; i++)
		{
			result[i] = array[i];
		}
		return result;
	}
}
