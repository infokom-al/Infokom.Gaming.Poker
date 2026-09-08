using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		public static class FLUSH
		{
			private const int r = 1277, n = 5108;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = FULL_HOUSE.CUMULATIVE_PROBABILITY + p; // Filtri F_W5
			public const float ODDS_AGAINST = 1 / p - 1;


			/// <summary>
			/// MIN DISTINCT RANKS
			/// </summary>
			public const int RC = 5;

			/// <summary>
			/// MIN DISTINCT SUITS
			/// </summary>
			public const int SC = 1;


		

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool TryDetect(Ranks s, Ranks d, Ranks c, Ranks h, out Rank ρ1, out Rank ρ2, out Rank ρ3, out Rank ρ4, out Rank ρ5)
			{
				var R = s.Count >= 5 ? s : d.Count >= 5 ? d : c.Count >= 5 ? c : h.Count >= 5 ? h : default;
				
				if(R is not 0)
				{				
					ρ1 = (R					).Upmost();
					ρ2 = (R = R.Exclude(ρ1)	).Upmost();
					ρ3 = (R = R.Exclude(ρ2)	).Upmost();
					ρ4 = (R = R.Exclude(ρ3)	).Upmost();
					ρ5 = (    R.Exclude(ρ4)	).Upmost();

					return true;
				}

				ρ1 = ρ2 = ρ3 = ρ4 = ρ5 = default;
				return false;
			}
		}
	}
}
