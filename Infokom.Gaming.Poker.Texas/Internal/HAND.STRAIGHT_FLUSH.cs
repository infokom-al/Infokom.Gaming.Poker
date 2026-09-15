using Infokom.Numerics.Atomics;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		public static class STRAIGHT_FLUSH
		{
			private const int r = 9, n = 36;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = ROYAL_FLUSH.CUMULATIVE_PROBABILITY + PROBABILITY;
			public const float ODDS_AGAINST = 1 / p - 1;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryDetect(RankSet s, RankSet d, RankSet c, RankSet h, out Suit suit, out Rank rank)
			{
				(suit, rank) = (default, default);

				var (ns, nd, nc, nh) = (s.Count, d.Count, c.Count, h.Count);

				if (ns >= 5 && HAND.STRAIGHT.TryDetect(s, out var spadeRank)/* && spadeRank > rank*/)
				{
					(suit, rank) = (Suit.Spade, spadeRank);
				}

				if (nd >= 5 && HAND.STRAIGHT.TryDetect(d, out var diamondRank) && diamondRank > rank)
				{
					(suit, rank) = (Suit.Diamond, diamondRank);
				}

				if (nc >= 5 && HAND.STRAIGHT.TryDetect(c, out var clubRank) && clubRank > rank)
				{
					(suit, rank) = (Suit.Club, clubRank);
				}

				if (nh >= 5 && HAND.STRAIGHT.TryDetect(h, out var heartRank) && heartRank > rank)
				{
					(suit, rank) = (Suit.Heart, heartRank);
				}

				return (suit, rank) != default;
			}
		}
	}
}
