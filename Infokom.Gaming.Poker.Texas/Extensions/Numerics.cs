using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Holdem.Core.Extensions
{
	internal static class Numerics
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Max(int x1, int x2) => x1 >= x2 ? x1 : x2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(int x1, int x2) => x1 <= x2 ? x1 : x2;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Max(uint x1, uint x2) => x1 >= x2 ? x1 : x2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Min(uint x1, uint x2) => x1 <= x2 ? x1 : x2;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Max(long x1, long x2) => x1 >= x2 ? x1 : x2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Min(long x1, long x2) => x1 <= x2 ? x1 : x2;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Max(ulong x1, ulong x2) => x1 >= x2 ? x1 : x2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Min(ulong x1, ulong x2) => x1 <= x2 ? x1 : x2;
	}
}
