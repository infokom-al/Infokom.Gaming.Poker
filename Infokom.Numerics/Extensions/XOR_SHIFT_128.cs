using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Xml.Linq;

using static Infokom.Numerics.Extensions.RANDOM;

namespace Infokom.Numerics.Extensions
{


	internal static class RANDOM
	{
		public static class XOR_SHIFT_128
		{
			private static readonly ulong UPP;
			private static readonly ulong LOW;

			private static uint X, Y, Z, W;

			static XOR_SHIFT_128()
			{
				LOW = (ulong)DateTime.UtcNow.Ticks ^ (ulong)Environment.TickCount64;
				UPP = (~((ulong)DateTime.UtcNow.Ticks)) ^ (~((ulong)Environment.TickCount64));

				X = (uint)(LOW & 0xFFFFFFFFul);
				Y = (uint)(LOW >> 32);
				Z = (uint)(UPP & 0xFFFFFFFFul);
				W = (uint)(UPP >> 32);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void Shuffle()
			{
				X ^= X << 11;
				X ^= X >> 8;
				X ^= W ^ (W >> 19);
				(X, Y, Z, W) = (Y, Z, W, X);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static uint Next()
			{
				Shuffle();

				return W;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static uint Next(uint min = uint.MinValue, uint max = uint.MaxValue) => min + (Next() % (Math.Max(min, max) - min + 1));
		}
	}

	public static partial class UInt32Extensions
	{
		extension(uint)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static uint Random(uint min = uint.MinValue, uint max = uint.MaxValue) => XOR_SHIFT_128.Next(min, max);
		}
	}

}
