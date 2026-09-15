
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		public static class TWO_PAIR
		{
			private const int r = 858, n = 123552;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = THREE_OF_A_KIND.CUMULATIVE_PROBABILITY + p; // Filtri F_W8_A
			public const float ODDS_AGAINST = 1 / p - 1;



			/// <summary>
			/// MIN DISTINCT RANKS, (1 FOR HIGH PAIR + 1 LOW PAIR + 1 FOR THE KICKER)
			/// </summary>
			public const int RC = 3;

			/// <summary>
			/// MIN DISTINCT SUITS, (MAX 5 - 3 IN CASE THE KICKER SUIT IS PRESENT IN BOTH HI AND LO PAIRS)
			/// </summary>
			public const int SC = 2;


			/// <summary>
			/// $$ \rho(\{(\rho_1, \sigma_{1,1}), (\rho_1, \sigma_{1,2}), (\rho_2, \sigma_{2,1}), (\rho_2, \sigma_{2,2}), (\rho_3, \sigma_3)\}) = \{\rho_1, \rho_2, \rho_3\} $$
			/// </summary>
			/// <param name="cards"></param>
			/// <param name="ρ1"></param>
			/// <param name="ρ2"></param>
			/// <param name="ρ3"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static bool TryDetect(RankSet S, RankSet D, RankSet C, RankSet H, out Rank ρ1, out Rank ρ2, out Rank ρ3)
			{
				var R = (S & D) | (S & C) | (S & H) | (D & C) | (D & H) | (C & H);
				if (!R.IsEmpty)
				{
					ρ1 = R.Upmost();//top pair
					if(!R.Exclude(ρ1).IsEmpty)
					{
						ρ2 = R.Upmost();//top pair
						if (!(R = (S | D | C | H).Exclude(ρ1).Exclude(ρ2)).IsEmpty)
						{
							ρ3 = R.Upmost();
							return true;
						}
					}
				}

				ρ1 = ρ2 = ρ3 = default;
				return false;
			}
		}
	}
}
