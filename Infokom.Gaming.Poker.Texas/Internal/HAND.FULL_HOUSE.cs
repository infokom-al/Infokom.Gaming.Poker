
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		public static class FULL_HOUSE
		{
			private const int r = 156, n = 3744;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = FOUR_OF_A_KIND.CUMULATIVE_PROBABILITY + p; // Filtri F_W4
			public const float ODDS_AGAINST = 1 / p - 1;

			/// <summary>
			/// MIN DISTINCT RANKS
			/// </summary>
			public const int RC = 2;

			/// <summary>
			/// MIN DISTINCT SUITS
			/// </summary>
			public const int SC = 3;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryDetect(Ranks S, Ranks D, Ranks C, Ranks H, out Rank α, out Rank β)
			{
				var R = ((S & D & C) | (S & D & H) | (S & C & H) | (D & C & H));

				if (R is not 0)
				{
					α = R.Upmost();

					R = ((S & D) |	(S & C) |	(S & H) |	(D & C) |	(D & H) | (C & H)).Exclude(α);

					if (R is not 0)
					{
						β = R.Upmost();
						return true;
					}
				}

				α = β = 0;
				return false;
			}
		}
	}
}
