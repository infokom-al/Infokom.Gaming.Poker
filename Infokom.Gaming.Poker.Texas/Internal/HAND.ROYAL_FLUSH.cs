using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

using CardSet = ulong;
using RankSet = ushort;

using Suit = int;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		public static class ROYAL_FLUSH
		{
			private const int r = 1, n = 4;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = p;
			public const float ODDS_AGAINST = 1/p - 1;


			private const ushort TARGET = 0x7C00;



			/// <summary>
			/// Try to find a straight flush.
			/// </summary>
			/// <param name="source">card set bit mask</param>
			/// <param name="suit">offset of the suit of the straight flush</param>
			/// <param name="target">rank set bit mask of the isolated straight</param>
			/// <returns>True if ant straight flush was found, false otherwise</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryDetect(RankSet s, RankSet d, RankSet c, RankSet h)
			{
				return (ushort)s is TARGET || (ushort)d is TARGET || (ushort)c is TARGET || (ushort)h is TARGET;
			}
		}
	}
}
