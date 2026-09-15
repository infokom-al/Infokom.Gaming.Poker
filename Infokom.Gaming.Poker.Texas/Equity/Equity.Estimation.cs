using Infokom.Numerics.Extensions;

using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Infokom.Gaming.Poker.Texas
{


	public static partial class Equity
	{
		public class Estimation
		{
			public class Request
			{
				private readonly GTO.Range[] _players;

				private Request(params GTO.Range[] players)
				{
					Id = (ulong)Environment.TickCount64;
					Players = [.. players];
				}

				public ulong Id { get; }

				public ImmutableArray<GTO.Range> Players { get; }


				public static Request Create(GTO.Range hero, GTO.Range vill) => new(hero, vill);

				public static Request Create(params ReadOnlySpan<GTO.Range> players)
				{
					if (players.Length is < 2 or > 10)
						throw new ArgumentOutOfRangeException(nameof(players), "The number of players must be between 2 and 10.");

					return new Request(players.ToArray());
				}
			}

			public readonly struct Result
			{
				private readonly Estimation _owner;
				private readonly int _seatIndex;

				public Result(Estimation owner, int index)
				{
					_owner = owner;
					_seatIndex = index;
				}

				public long SoleWins => _owner._soleWins[_seatIndex];
				public long SharedWins => _owner._sharedWins[_seatIndex];
				public long TopFinishes => _owner._topFinishes[_seatIndex];
				public double EquityShare => _owner._equityShares[_seatIndex];

				public double SoleWinRate => _owner.SoleWinRateOf(_seatIndex);
				public double SharedWinRate => _owner.SharedWinRateOf(_seatIndex);
				public double TopFinishRate => _owner.TopFinishRateOf(_seatIndex);
				public double WinRate => _owner.WinRateOf(_seatIndex);
			}

			private readonly long[] _soleWins;
			private readonly long[] _sharedWins;
			private readonly long[] _topFinishes;
			private readonly double[] _equityShares;
			private readonly long _simulationCount;

			private Estimation(ulong id, long[] soleWins, long[] sharedWins, long[] topFinishes, double[] equityShares, long count)
			{
				Id = id;
				_soleWins = soleWins;
				_sharedWins = sharedWins;
				_topFinishes = topFinishes;
				_equityShares = equityShares;
				_simulationCount = count;
			}

			public ulong Id { get; }

			public int PlayerCount => _topFinishes.Length;

			/// <summary>
			/// Gets the number of times each player won the hand outright (without sharing the win with any other player).
			/// </summary>
			public long[] SoleWins => _soleWins;

			/// <summary>
			/// Gets the number of times each player won the hand, including shared wins (where multiple players had the same best hand).
			/// </summary>
			public long[] SharedWins => _sharedWins;

			/// <summary>
			/// Gets the number of times each player finished in the top position, regardless of whether they won outright or shared the win with other players.
			/// </summary>
			public long[] TopFinishes => _topFinishes;

			/// <inheritdoc cref="TopFinishes"/>
			public long[] Wins => _topFinishes;

			/// <summary>
			/// Gets the equity share for each player, which represents the proportion of the total simulations in which each player had the best hand (either outright or shared).
			/// </summary>
			public double[] EquityShares => _equityShares;

			public long Size => _simulationCount;

			public double WinRateOf(int player) => _simulationCount == 0 ? 0d : _equityShares[player] / _simulationCount;
			public double SoleWinRateOf(int player) => _simulationCount == 0 ? 0d : _soleWins[player] / (double)_simulationCount;
			public double SharedWinRateOf(int player) => _simulationCount == 0 ? 0d : _sharedWins[player] / (double)_simulationCount;
			public double TopFinishRateOf(int player) => _simulationCount == 0 ? 0d : _topFinishes[player] / (double)_simulationCount;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Estimation Estimate(Request request, bool monteCarlo = true)
			{
				if (monteCarlo)
					return EstimateMonteCarlo(request);

				int playerCount = request.Players.Length;
				var soleWins = new long[playerCount];
				var sharedWins = new long[playerCount];
				var topFinishes = new long[playerCount];
				var equityShares = new double[playerCount];
				long games = 0;

				Deck deck = Deck.Factory;
				CardSet available = deck.State;

				var pockets = new CardSet[playerCount];
				EstimatePlayers(request.Players, 0, available, pockets, soleWins, sharedWins, topFinishes, equityShares, ref games);

				return new Estimation(request.Id, soleWins, sharedWins, topFinishes, equityShares, games);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void EstimatePlayers(
	ImmutableArray<GTO.Range> ranges,
	int player,
	CardSet available,
	CardSet[] pockets,
	long[] soleWins,
	long[] sharedWins,
	long[] topFinishes,
	double[] equityShares,
	ref long games)
{
	if (player == ranges.Length)
	{
		if (available.Count >= 24)
			EstimateBoardsParallel(pockets, available, soleWins, sharedWins, topFinishes, equityShares, ref games);
		else
			EstimateBoards(pockets, available, soleWins, sharedWins, topFinishes, equityShares, ref games);

		return;
	}

	foreach (var cell in ranges[player].Cells)
	{
		foreach (CardSet pocket in cell.Hands)
		{
			if (!available.IsSupersetOf(pocket))
				continue;

			pockets[player] = pocket;
			EstimatePlayers(ranges, player + 1, available & ~pocket, pockets, soleWins, sharedWins, topFinishes, equityShares, ref games);
		}
	}
}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void EstimateBoards(
	CardSet[] pockets,
	CardSet available,
	long[] soleWins,
	long[] sharedWins,
	long[] topFinishes,
	double[] equityShares,
	ref long games)
{
	foreach (CardSet board in available.Choose(5))
	{
		Hand.Ranking best = Hand.Evaluate(pockets[0] | board);
		ulong winners = 1UL;

		for (int player = 1; player < pockets.Length; player++)
		{
			Hand.Ranking value = Hand.Evaluate(pockets[player] | board);

			if (value > best)
			{
				best = value;
				winners = 1UL << player;
			}
			else if (value == best)
			{
				winners |= 1UL << player;
			}
		}

		AccumulateOutcome(winners, soleWins, sharedWins, topFinishes, equityShares, ref games);
	}
}

			private static void EstimateBoardsParallel(CardSet[] pockets, CardSet available, long[] soleWins, long[] sharedWins, long[] topFinishes, double[] equityShares, ref long games)
			{
				int playerCount = pockets.Length;
				object sync = new();
				long totalGames = 0;

				_ = Parallel.ForEach(
					available.Choose(5),
					() => new LocalAccumulator(playerCount),
					(board, _, local) =>
					{
						Hand.Ranking best = Hand.Evaluate(pockets[0] | board);
						ulong winners = 1UL;

						for (int player = 1; player < playerCount; player++)
						{
							Hand.Ranking value = Hand.Evaluate(pockets[player] | board);

							if (value > best)
							{
								best = value;
								winners = 1UL << player;
							}
							else if (value == best)
							{
								winners |= 1UL << player;
							}
						}

						AccumulateOutcome(winners, local.SoleWins, local.SharedWins, local.TopFinishes, local.EquityShares, ref local.Games);
						return local;
					},
					local =>
					{
						lock (sync)
						{
							totalGames += local.Games;

							for (int i = 0; i < playerCount; i++)
							{
								soleWins[i] += local.SoleWins[i];
								sharedWins[i] += local.SharedWins[i];
								topFinishes[i] += local.TopFinishes[i];
								equityShares[i] += local.EquityShares[i];
							}
						}
					});

				games += totalGames;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Estimation EstimateMonteCarlo(Request request, int simulations = 100_000, int? seed = null, bool parallel = true)
			{
				ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(simulations, 0);

				int playerCount = request.Players.Length;

				var allHands = new CardSet[playerCount][];
				int maxHands = 0;

				for (int p = 0; p < playerCount; p++)
				{
					List<CardSet> list = new(256);

					foreach (var cell in request.Players[p].Cells)
						foreach (var hand in cell.Hands)
							list.Add(hand);

					allHands[p] = [.. list];
					maxHands = Math.Max(maxHands, allHands[p].Length);
				}

				if (!parallel || simulations == 1)
				{
					var local = new MonteCarloLocal(
						playerCount,
						maxHands,
						(ulong)(uint)(seed ?? Environment.TickCount));

					RunMonteCarlo(allHands, simulations, local);

					return new Estimation(request.Id, local.SoleWins, local.SharedWins, local.TopFinishes, local.EquityShares, local.Games);
				}

				int workerCount = Math.Min(Environment.ProcessorCount, simulations);
				var locals = new MonteCarloLocal[workerCount];
				ulong baseSeed = (ulong)(uint)(seed ?? Environment.TickCount);

				_ = Parallel.For(0, workerCount, worker =>
				{
					int start = worker * simulations / workerCount;
					int end = (worker + 1) * simulations / workerCount;
					int count = end - start;

					var local = new MonteCarloLocal(
						playerCount,
						maxHands,
						baseSeed + (ulong)(worker + 1));

					RunMonteCarlo(allHands, count, local);
					locals[worker] = local;
				});

				var soleWins = new long[playerCount];
				var sharedWins = new long[playerCount];
				var topFinishes = new long[playerCount];
				var equityShares = new double[playerCount];
				long games = 0;

				for (int i = 0; i < locals.Length; i++)
				{
					var local = locals[i];
					games += local.Games;

					for (int p = 0; p < playerCount; p++)
					{
						soleWins[p] += local.SoleWins[p];
						sharedWins[p] += local.SharedWins[p];
						topFinishes[p] += local.TopFinishes[p];
						equityShares[p] += local.EquityShares[p];
					}
				}

				return new Estimation(request.Id, soleWins, sharedWins, topFinishes, equityShares, games);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void RunMonteCarlo(CardSet[][] allHands, int simulations, MonteCarloLocal local)
			{
				for (int i = 0; i < simulations; i++)
				{
					if (!TryPlayOne(allHands, local.Pockets, local.Candidates, ref local.Rng, out ulong winnersMask))
						continue;

					AccumulateOutcome(winnersMask, local.SoleWins, local.SharedWins, local.TopFinishes, local.EquityShares, ref local.Games);
				}
			}

			private sealed class MonteCarloLocal
			{
				public MonteCarloLocal(int playerCount, int maxHands, ulong seed)
				{
					Rng = new XorShift64Star(seed);
					SoleWins = new long[playerCount];
					SharedWins = new long[playerCount];
					TopFinishes = new long[playerCount];
					EquityShares = new double[playerCount];
					Pockets = new CardSet[playerCount];
					Candidates = new CardSet[maxHands];
				}

				public XorShift64Star Rng;
				public long[] SoleWins { get; }
				public long[] SharedWins { get; }
				public long[] TopFinishes { get; }
				public double[] EquityShares { get; }
				public long Games;
				public CardSet[] Pockets { get; }
				public CardSet[] Candidates { get; }
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

			private struct XorShift64Star
			{
				private ulong _state;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public XorShift64Star(ulong seed)
				{
					_state = Mix(seed == 0 ? 0x9E3779B97F4A7C15UL : seed);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private static ulong Mix(ulong x)
				{
					x += 0x9E3779B97F4A7C15UL;
					x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
					x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
					return x ^ (x >> 31);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public ulong NextUInt64()
				{
					ulong x = _state;
					x ^= x >> 12;
					x ^= x << 25;
					x ^= x >> 27;
					_state = x;
					return x * 2685821657736338717UL;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public int NextInt32(int exclusiveMax)
				{
					ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exclusiveMax);

					uint bound = (uint)exclusiveMax;
					ulong product = (ulong)(uint)NextUInt64() * bound;
					uint low = (uint)product;

					if (low < bound)
					{
						uint threshold = (0u - bound) % bound;

						while (low < threshold)
						{
							product = (ulong)(uint)NextUInt64() * bound;
							low = (uint)product;
						}
					}

					return (int)(product >> 32);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void AccumulateOutcome(
	ulong winnersMask,
	long[] soleWins,
	long[] sharedWins,
	long[] topFinishes,
	double[] equityShares,
	ref long games)
{
	games++;

	int winnerCount = BitOperations.PopCount(winnersMask);
	double share = 1d / winnerCount;
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

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool TryPlayOne(CardSet[][] allHands, CardSet[] pockets, CardSet[] candidates, ref XorShift64Star rng, out ulong winnersMask)
			{
				CardSet available = Deck.Factory.State;

				for (int p = 0; p < allHands.Length; p++)
				{
					if (!TryPickPocket(allHands[p], available, candidates, ref rng, out var pocket))
					{
						winnersMask = 0;
						return false;
					}

					pockets[p] = pocket;
					available &= ~pocket;
				}

				CardSet board = DrawRandomCards(available, 5, ref rng);

				Hand.Ranking best = Hand.Evaluate(pockets[0] | board);
				winnersMask = 1UL;

				for (int p = 1; p < pockets.Length; p++)
				{
					Hand.Ranking value = Hand.Evaluate(pockets[p] | board);

					if (value > best)
					{
						best = value;
						winnersMask = 1UL << p;
					}
					else if (value == best)
					{
						winnersMask |= 1UL << p;
					}
				}

				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool TryPickPocket(CardSet[] hands, CardSet available, CardSet[] candidates, ref XorShift64Star rng, out CardSet pocket)
			{
				int n = 0;

				for (int i = 0; i < hands.Length; i++)
				{
					var hand = hands[i];

					if (available.IsSupersetOf(hand))
						candidates[n++] = hand;
				}

				if (n == 0)
				{
					pocket = default;
					return false;
				}

				pocket = candidates[rng.NextInt32(n)];
				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static CardSet DrawRandomCards(CardSet source, int k, ref XorShift64Star rng)
			{
				ulong bits = source;
				int remaining = BitOperations.PopCount(bits);
				ulong result = 0;

				for (int i = 0; i < k; i++)
				{
					int n = rng.NextInt32(remaining--);
					ulong picked = SelectNthSetBit(bits, n);
					result |= picked;
					bits ^= picked;
				}

				return unchecked((CardSet)result);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static ulong SelectNthSetBit(ulong bits, int n)
			{
				if (System.Runtime.Intrinsics.X86.Bmi2.X64.IsSupported)
					return System.Runtime.Intrinsics.X86.Bmi2.X64.ParallelBitDeposit(1UL << n, bits);

				while (n-- > 0)
					bits &= bits - 1;

				return bits & (0UL - bits);
			}
		}
	}
}