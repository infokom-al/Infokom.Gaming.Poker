
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		public static class ONE_PAIR
		{
			private const int r = 2860, n = 1098240;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = TWO_PAIR.CUMULATIVE_PROBABILITY + p; // Filtri F_W8_B
			public const float ODDS_AGAINST = 1 / p - 1;



			/// <summary>
			/// MIN DISTINCT RANKS, (1 COLLAPSED BY PAIR)
			/// </summary>
			public const int RC = 4;

			/// <summary>
			/// MIN DISTINCT SUITS, (MAX 5 - 3 IN CASE KICKERS ARE SUITED AND THAT SUIT IS PRESENT IN THE PAIR)
			/// </summary>
			public const int SC = 2;

		


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static bool TryDetect(RankSet S, RankSet D, RankSet C, RankSet H, out Rank ρ1, out Rank ρ2, out Rank ρ3, out Rank ρ4)
			{
				var R = (S & D) | (S & C) | (S & H) | (D & C) | (D & H) | (C & H);
				if (!R.IsEmpty)
				{
					ρ1 = R.Upmost();//top pair
					R = (S | D | C | H);
					if (!R.Exclude(ρ1).IsEmpty)
					{
						ρ2 = R.Upmost();
						if (!R.Exclude(ρ2).IsEmpty)
						{
							ρ3 = R.Upmost();
							if (!R.Exclude(ρ3).IsEmpty)
							{	
								ρ4 = R.Upmost();
								return true;
							}
						}
					}
				}

				ρ1 = ρ2 = ρ3 = ρ4 = default;
				return false;
			}
		}
	}
}
