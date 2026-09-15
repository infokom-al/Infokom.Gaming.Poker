using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{

		public static class THREE_OF_A_KIND
		{
			private const int r = 858, n = 54912;
			private const float p = (float)n / N;

			public const int DISTINCT_HANDS = r;
			public const int FREQUENCY = n;
			public const float PROBABILITY = p;
			public const float CUMULATIVE_PROBABILITY = STRAIGHT.CUMULATIVE_PROBABILITY + p; // Filtri F_W7
			public const float ODDS_AGAINST = 1 / p - 1;



			/// <summary>
			/// MIN DISTINCT RANKS, (1 FOR TRIP + 2 FOR KICKERS)
			/// </summary>
			public const int RC = 3;

			/// <summary>
			/// MIN DISTINCT SUITS, (TRIP RANK CARDS ARE SUIT DISJOINT)
			/// </summary>
			public const int SC = 3;




			[method: MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static ushort IsolateTrips(ushort s, ushort d, ushort c, ushort h) => (ushort)(
				(s & d & c) |
				(s & d & h) |
				(s & d & c) |
				(s & c & h) |
				(d & c & h));



			[method: MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static bool TryIsolate(ushort s, ushort d, ushort c, ushort h, out ushort target)
			{
				target = (ushort)
				(
					(s & d & c    ) | 
					(s & d     & h) | 
					(s & d & c    ) |
					(s     & c & h) |
					(    d & c & h)
				);

				return target != 0;
			}





			[method: MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static bool TryExtract(RankSet s, RankSet d, RankSet c, RankSet h, out RankSet target)
			{
				target =	(s & d & c    ) | 
						(s & d     & h) | 
						(s & d & c    ) |
						(s     & c & h) |
						(    d & c & h) ;

				return target != 0;
			}

			[method: MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static bool TryExtract(CardSet source, out RankSet target)
			{
				var (s, d, c, h) = source;

				return TryExtract(s, d, c, h, out target);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static bool TryDetect(RankSet S, RankSet D, RankSet C, RankSet H, out Rank α, out Rank β, out Rank γ)
			{
				var R =
				(
					(S & D & C    ) |
					(S & D &     H) |
					(S &     C & H) |
					(    D & C & H)
				);

				if (!R.IsEmpty)									//if there are at least the coranking cards
				{	
					α = R.Upmost();

					R = (S | D | C | H).Exclude(α);

					if (!R.IsEmpty)								//and if there are enouph cards to pick another as first kciker
					{
						β = R.Upmost();

						R = R.Exclude(β);
						if (!R.IsEmpty)							//and if there are enouph cards to pick another as second kciker
						{
							γ = R.Upmost();			
									
							return true;								//we successfully build the strongest possible three of a kind hand of 5 cards
						}
					}
				}

				α = β = γ = default;					//otherwise if wasn't possible to collect 5 cards everything will be erased
				return false;
			}

			
		}
	}
}
