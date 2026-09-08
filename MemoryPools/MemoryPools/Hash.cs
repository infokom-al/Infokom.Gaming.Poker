using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryPools;

internal static class Hash
{
	internal static int[] Primes = new int[72]
	{
		3, 7, 11, 17, 23, 29, 37, 47, 59, 71,
		89, 107, 131, 163, 197, 239, 293, 353, 431, 521,
		631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
		4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
		25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363,
		156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
		968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559,
		5999471, 7199369
	};

	public static int GetGreaterOrEqualPrime(this int minValue)
	{
		int[] primes = Primes;
		foreach (int num in primes)
		{
			if (num >= minValue)
			{
				return num;
			}
		}
		throw new NotImplementedException("Requested prime is too high and wasn't predefined.");
	}

	public static ulong GetFastModMultiplier(uint divisor)
	{
		return ulong.MaxValue / (ulong)divisor + 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint FastMod(uint value, uint divisor, ulong multiplier)
	{
		return (uint)(((multiplier * value >> 32) + 1) * divisor >> 32);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetSpanHashCode(this Span<char> span)
	{
		return HashCode.Combine(span.CombineValues());
	}

	private static int CombineValues<T>(this Span<T> span)
	{
		ref byte bytes = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span));
		nint length = (nint)(uint)(span.Length * Unsafe.SizeOf<T>());
		return GetBytesHashCode(ref bytes, length);
	}

	private unsafe static int GetBytesHashCode(ref byte bytes, nint length)
	{
		int num = 5381;
		nint num2 = 0;
		if (Vector.IsHardwareAccelerated && length >= Vector<byte>.Count << 3)
		{
			Vector<int> left = new Vector<int>(5381);
			Vector<int> right = new Vector<int>(33);
			while (length >= Vector<byte>.Count << 3)
			{
				Vector<int> left2 = Vector.Multiply(left, right);
				nint num3 = num2;
				_ = Vector<byte>.Count;
				left = Vector.Xor(left2, Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num3 + 0)));
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2 + Vector<byte>.Count)));
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2 + Vector<byte>.Count * 2)));
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2 + Vector<byte>.Count * 3)));
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2 + Vector<byte>.Count * 4)));
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2 + Vector<byte>.Count * 5)));
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2 + Vector<byte>.Count * 6)));
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2 + Vector<byte>.Count * 7)));
				length -= Vector<byte>.Count << 3;
				num2 += Vector<byte>.Count << 3;
			}
			while (length >= Vector<byte>.Count)
			{
				left = Vector.Xor(Vector.Multiply(left, right), Unsafe.ReadUnaligned<Vector<int>>(in Unsafe.Add(ref bytes, num2)));
				length -= Vector<byte>.Count;
				num2 += Vector<byte>.Count;
			}
			for (int i = 0; i < Vector<int>.Count; i++)
			{
				num = ((num << 5) + num) ^ left[i];
			}
		}
		else
		{
			if (sizeof(nint) == 8)
			{
				while (length >= 64)
				{
					ulong num4 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 0));
					num = ((num << 5) + num) ^ (int)num4 ^ (int)(num4 >> 32);
					ulong num5 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 8));
					num = ((num << 5) + num) ^ (int)num5 ^ (int)(num5 >> 32);
					ulong num6 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 16));
					num = ((num << 5) + num) ^ (int)num6 ^ (int)(num6 >> 32);
					ulong num7 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 24));
					num = ((num << 5) + num) ^ (int)num7 ^ (int)(num7 >> 32);
					ulong num8 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 32));
					num = ((num << 5) + num) ^ (int)num8 ^ (int)(num8 >> 32);
					ulong num9 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 40));
					num = ((num << 5) + num) ^ (int)num9 ^ (int)(num9 >> 32);
					ulong num10 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 48));
					num = ((num << 5) + num) ^ (int)num10 ^ (int)(num10 >> 32);
					ulong num11 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref bytes, num2 + 56));
					num = ((num << 5) + num) ^ (int)num11 ^ (int)(num11 >> 32);
					length -= 64;
					num2 += 64;
				}
			}
			while (length >= 32)
			{
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 0));
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 4));
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 8));
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 12));
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 16));
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 20));
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 24));
				num = ((num << 5) + num) ^ (int)Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref bytes, num2 + 28));
				length -= 32;
				num2 += 32;
			}
		}
		if (length >= 16)
		{
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 0));
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 2));
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 4));
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 6));
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 8));
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 10));
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 12));
			num = ((num << 5) + num) ^ Unsafe.ReadUnaligned<ushort>(in Unsafe.Add(ref bytes, num2 + 14));
			length -= 16;
			num2 += 16;
		}
		while (length > 0)
		{
			num = ((num << 5) + num) ^ Unsafe.Add(ref bytes, num2);
			length--;
			num2++;
		}
		return num;
	}
}
