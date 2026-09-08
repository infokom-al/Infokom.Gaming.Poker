using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Extensions
{
	public static class BITWISE
	{
		public static class OPERATORS<Tx, Ty, T> where Tx : unmanaged, IBitwiseOperators<Tx, Ty, T>
		{
			public readonly struct NOT
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static T Invoke(Tx x) => ~x;


				public T this[Tx x]
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => Invoke(x);
				}
			}



			public readonly struct AND
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static T Invoke(Tx x, Ty y) => x & y;

				public T this[Tx x, Ty y]
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => Invoke(x, y);
				}
			}

			public readonly struct OR
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static T Invoke(Tx x, Ty y) => x | y;

				public T this[Tx x, Ty y]
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => Invoke(x, y);
				}
			}

			public readonly struct XOR
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static T Invoke(Tx x, Ty y) => x ^ y;

				public T this[Tx x, Ty y]
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => Invoke(x, y);
				}
			}
		}
	}


	public static partial class BINARY
	{

		public static class UINT08
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(byte source) => BitOperations.Log2(source);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(byte source, out byte target)
			{
				int offset = UpmostNonZeroBit(source);

				target = (byte)(1u << offset);

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(byte source) => BitOperations.Log2(source);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(byte source, out byte target)
			{
				int offset = LowestNonZeroBit(source);

				target = (byte)(1u << offset);

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static byte IsolateUpmostNonZeroBit(byte source) => (byte)(1u << UpmostNonZeroBit(source));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static byte IsolateLowestNonZeroBit(byte source) => (byte)(1u << LowestNonZeroBit(source));
		}

		public static class UINT16
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(ushort source) => BitOperations.Log2(source);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(ushort source, out ushort target)
			{
				int offset = UpmostNonZeroBit(source);

				target = (ushort)(1u << offset);

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(ushort source) => BitOperations.Log2(source);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(ushort source, out ushort target)
			{
				int offset = LowestNonZeroBit(source);

				target = (ushort)(1u << offset);

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ushort IsolateUpmostNonZeroBit(ushort source) => (ushort)(1u << UpmostNonZeroBit(source));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ushort IsolateLowestNonZeroBit(ushort source) => (ushort)(1u << LowestNonZeroBit(source));
		}

		public static class UINT32
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(uint source) => BitOperations.Log2(source);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(uint source, out uint target)
			{
				int offset = UpmostNonZeroBit(source);

				target = 1u << offset;

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(uint source) => BitOperations.Log2(source);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(uint source, out uint target)
			{
				int offset = LowestNonZeroBit(source);

				target = 1u << offset;

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static uint IsolateUpmostNonZeroBit(uint source) => 1u << UpmostNonZeroBit(source);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static uint IsolateLowestNonZeroBit(uint source) => 1u << LowestNonZeroBit(source);
		}

		public static class UINT64
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(ulong source) => BitOperations.Log2(source);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int UpmostNonZeroBit(ulong source, out ulong target)
			{
				int offset = UpmostNonZeroBit(source);

				target = 1ul << offset;

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(ulong source) => BitOperations.Log2(source);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int LowestNonZeroBit(ulong source, out ulong target)
			{
				int offset = LowestNonZeroBit(source);

				target = 1ul << offset;

				return offset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ulong IsolateUpmostNonZeroBit(ulong source) => 1ul << UpmostNonZeroBit(source);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ulong IsolateLowestNonZeroBit(ulong source) => 1ul << LowestNonZeroBit(source);
		}
	}

}
