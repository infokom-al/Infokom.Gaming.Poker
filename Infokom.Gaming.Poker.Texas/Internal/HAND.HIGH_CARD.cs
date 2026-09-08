using Infokom.Numerics.Extensions;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		public static class HIGH_CARD
		{
			private const int r = 1277, n = 1302540;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = ONE_PAIR.CUMULATIVE_PROBABILITY + p; // Filtri F_W9 (Rezultati ekzaktësisht 1.0f ose 100%)
			public const float ODDS_AGAINST = 1 / p - 1;




			/// <summary>
			/// MIN DISTINCT RANKS, (AT LEAST 5 DISJOINT RANKS IN ORDER TO NOT BE PAIR+)
			/// </summary>
			public const int RC = 5;

			/// <summary>
			/// SHORTEST SEGMENT (AT LEAST ONE PATTERN 101 IN ORDER TO AVOID STRAIGHT)
			/// </summary>
			public const int RL = 6;

			/// <summary>
			/// MIN DISTINCT SUITS, (AT MOST 4 SUITED IN ORDER TO NOT BE A FLUSH)
			/// </summary>
			public const int SC = 2;


			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryExtract(ushort source, out ushort target)
			{
				target = default;

				if (BitOperations.PopCount(source) >= 5)
				{
					target |= ((ushort)(source ^ target)).IsolateUpmostNonZeroBit();
					target |= ((ushort)(source ^ target)).IsolateUpmostNonZeroBit();
					target |= ((ushort)(source ^ target)).IsolateUpmostNonZeroBit();
					target |= ((ushort)(source ^ target)).IsolateUpmostNonZeroBit();
					target |= ((ushort)(source ^ target)).IsolateUpmostNonZeroBit();

					return true;
				}

				return false;
			}


			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryExtract(Ranks source, out Ranks target)
			{
				target = default;
				
				return TryExtract((ushort)source, out Unsafe.As<Ranks, ushort>(ref target));
			}

			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryDetect(Ranks source, out Rank r1, out Rank r2, out Rank r3, out Rank r4, out Rank r5)
			{
				if(source.Count >= 5)
				{	
					r1 = (source						).Upmost();
					r2 = (source = source.Exclude(r1)	).Upmost();
					r3 = (source = source.Exclude(r2)	).Upmost();
					r4 = (source = source.Exclude(r3)	).Upmost();
					r5 = (         source.Exclude(r4)	).Upmost();

					return true;
				}

				r1 = r2 = r3 = r4 = r5 = default;
				return false;
			}
		}
	}
}
