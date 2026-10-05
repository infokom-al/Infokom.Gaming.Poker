using Infokom.Numerics.Extensions;

using MediatR;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Infokom.Gaming.Poker.Texas.Estimators
{

	public partial class MonteCarloEstimator
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AccumulateOutcome(ulong winnersMask, Span<int> soleWins, Span<int> sharedWins, Span<int> topFinishes, Span<double> equityShares, ref int games)
		{
			games++;

			int winnerCount = BitOperations.PopCount(winnersMask);
			double share = 1.0 / winnerCount;
			bool shared = winnerCount > 1;

			while (winnersMask != 0)
			{
				int player = BitOperations.TrailingZeroCount(winnersMask);

				topFinishes[player]++;

				if (shared)
					sharedWins[player]++;
				else
					soleWins[player]++;

				equityShares[player] += share;

				winnersMask &= winnersMask - 1;
			}
		}



		/// <summary>
		/// Try to pick pocket cards from players preference feasible for a given set of available cards (deck)
		/// </summary>
		/// <param name="preferedPockets">Player pocket preference</param>
		/// <param name="availableCards">Available cards in the dealer deck</param>
		/// <param name="feasiblePockets">Feasible pockets</param>
		/// <param name="selectedPocket">Single randomlyu picked pocket from feasible ones</param>
		/// <returns>True if pocket extraction was successfull, false if no feasible pocket is possible</returns>
		/// <exception cref="InvalidOperationException"></exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryExtractPocket(ReadOnlySpan<CardSet> preferedPockets, CardSet availableCards, Span<CardSet> feasiblePockets, out CardSet selectedPocket)
		{
			int n = 0;

			for (int i = 0; i < preferedPockets.Length; i++)
			{
				var hand = preferedPockets[i];

				if (availableCards.IsSupersetOf(hand))
					feasiblePockets[n++] = hand;
			}

			return feasiblePockets[..n].TryGetRandom(out selectedPocket);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryOne(CardSet[][] preferedHands, Span<CardSet> pockets, out ulong winnersMask)
		{
			const int MaxSamplingAttempts = 1024;

			for (int attempt = 0; attempt < MaxSamplingAttempts; attempt++)
			{
				CardSet deck = CardSet.Ω;
				bool accepted = true;

				for (int p = 0; p < preferedHands.Length; p++)
				{
					if (!preferedHands[p].AsSpan().TryGetRandom(out var pocket) || !deck.IsSupersetOf(pocket))
					{
						accepted = false;
						break;
					}

					pockets[p] = pocket;
					deck &= ~pocket;
				}

				if (!accepted)
					continue;

				CardSet board = deck.ChooseRandom(5);

				if (board.Count != 5)
					continue;

				Hand.Ranking best = Hand.Evaluate(pockets[0] | board);
				winnersMask = 1UL;

				for (int p = 1; p < pockets.Length; p++)
				{
					Hand.Ranking handRanking = Hand.Evaluate(pockets[p] | board);

					if (handRanking > best)
					{
						best = handRanking;
						winnersMask = 1UL << p;
					}
					else if (handRanking == best)
					{
						winnersMask |= 1UL << p;
					}
				}

				return true;
			}

			winnersMask = 0;
			return false;
		}


		/// <summary>
		/// Playerwise hands
		/// </summary>
		/// <param name="hands">Plyer hands</param>
		/// <param name="trials">Number of trials</param>
		/// <param name="state"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void RunMonteCarloEstimator(CardSet[][] hands, int trials, ref MonteCarloEstimatorWorkerState state)
		{
			for (int i = 0; i < trials; i++)
			{
				if (!TryOne(hands, state.Pockets, out ulong winnersMask))
					continue;
				AccumulateOutcome(winnersMask, state.SoleWins, state.SharedWins, state.TopFinishes, state.EquityShares, ref state.Trials);
			}
		}

		private struct MonteCarloEstimatorWorkerState
		{
			public MonteCarloEstimatorWorkerState(int litigants)
			{
				Litigants = litigants;
			}


			public readonly int Litigants;
			public int Trials;

			public InlineArray10<int> SoleWins;
			public InlineArray10<int> SharedWins;
			public InlineArray10<int> TopFinishes;
			public InlineArray10<double> EquityShares;
			private fixed ulong _pockets[10];
			public unsafe Span<CardSet> Pockets => MemoryMarshal.CreateSpan(ref Unsafe.As<ulong, CardSet>(ref _pockets[0]), Litigants);
		}

		private sealed class LocalAccumulator
		{
			public LocalAccumulator(int playerCount)
			{
				SoleWins = new long[playerCount];
				SharedWins = new long[playerCount];
				TopFinishes = new long[playerCount];
				EquityShares = new double[playerCount];
			}

			public long[] SoleWins { get; }
			public long[] SharedWins { get; }
			public long[] TopFinishes { get; }
			public double[] EquityShares { get; }
			public long Games;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Response Handle(Request request)
		{
			ArgumentNullException.ThrowIfNull(request, nameof(request));

			var id = request.Id;
			var ranges = request.Players;
			var trials = request.Trials;

			//m plyers, n hands
			int m = ranges.Length;


			var hands = new CardSet[m][];//hands[i]: hands of i-th player


			// playerwise range sampling
			for (int i = 0; i < m; i++)
			{
				var cells = ranges[i].Cells.ToArray();// expansion of i-th player preference


				var handBuffer = new List<CardSet>(128);
				foreach (var cell in cells)
				{
					handBuffer.AddRange(cell.Hands);
				}

				hands[i] = [.. handBuffer];

			}

			int workerCount = Math.Min((int)Math.Ceiling(Environment.ProcessorCount/2.0), trials);
			var workerStates = new MonteCarloEstimatorWorkerState[workerCount];
			
			_ = Parallel.For(0, workerCount, worker =>
			{
				int start = worker * trials / workerCount;
				int end = (worker + 1) * trials / workerCount;
				int count = end - start;

				workerStates[worker] = new MonteCarloEstimatorWorkerState(m);

				RunMonteCarloEstimator(hands, count, ref workerStates[worker]);
			});

			var soleWins = new int[m];
			var sharedWins = new int[m];
			var topFinishes = new int[m];
			var equityShares = new double[m];
			int games = 0;


			for (int i = 0; i < workerStates.Length; i++)
			{
				var local = workerStates[i];
				games += local.Trials;

				for (int p = 0; p < m; p++)
				{
					soleWins[p] += local.SoleWins[p];
					sharedWins[p] += local.SharedWins[p];
					topFinishes[p] += local.TopFinishes[p];
					equityShares[p] += local.EquityShares[p];
				}
			}

			return new Response(request.Id, m, soleWins, sharedWins, topFinishes, equityShares, games);
		}
	}
}