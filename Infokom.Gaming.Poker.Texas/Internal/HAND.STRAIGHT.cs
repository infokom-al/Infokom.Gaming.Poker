using Infokom.Numerics.Atomics;

using System.Collections.Specialized;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

using static Infokom.Gaming.Poker.CardExtensions;

namespace Infokom.Gaming.Poker.Texas.Internal
{
	internal static partial class HAND
	{
		public static class STRAIGHT
		{
			private const int r = 10, n = 10200;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = FLUSH.CUMULATIVE_PROBABILITY + p; // Filtri F_W6
			public const float ODDS_AGAINST = 1 / p - 1;

			const sbyte A = 14;
			const ushort KERNEL = 0b11111;
			const ushort A2345 = 0b_01000000000111100;
			const sbyte MATCH_NOT_FOUND = -1;
			const Rank NOT_FOUND = (Rank)MATCH_NOT_FOUND;

			private static Vector256<ushort> TARGETS = Vector256.Create<ushort>([
				0b_00000000000000000,
				0b_00000000000000000,
				0b_00000000000000000,
				0b_00000000000000000,
				0b_00000000000000000,
				0b_01000000000111100,// __2345________A_
				0b_00000000001111100,// __23456_________
				0b_00000000011111000,// ___34567________
				0b_00000000111110000,// ____45678_______
				0b_00000001111100000,// _____56789______
				0b_00000011111000000,// ______6789T_____
				0b_00000111110000000,// _______789TJ____
				0b_00001111100000000,// ________89TJQ___
				0b_00011111000000000,// _________9TJQK__
				0b_00111110000000000,// __________TJQKA_
				0b_00000000000000000,
			]);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static sbyte Detect(ushort source, out ushort target)
			{
				for (sbyte r = A; r >= 6; r--)
				{
					target = (ushort)(KERNEL << (r - 4));
					if ((source & target) == target)
					{
						return r;
					}
				}

				target = A2345;
				if ((source & target) == target)
					return 5;

				return MATCH_NOT_FOUND;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryDetect(Ranks source, out Rank result)
			{
				result = (Rank)Detect((ushort)source, out _);

				return result != NOT_FOUND;
			}
		}
	}
}