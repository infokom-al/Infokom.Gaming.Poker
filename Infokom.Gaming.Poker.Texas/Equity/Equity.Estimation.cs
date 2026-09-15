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

				public long Score => _owner._scoreDistribution[_seatIndex];

				public double WinRate => _owner._scoreDistribution[_seatIndex] / (double)_owner._simulationCount;
			}

			private readonly long[] _scoreDistribution;
			private readonly long _scoreSum;
			private readonly long _simulationCount;

			private Estimation(ulong id, long[] scoreDistribution, long count)
			{
				Id = id;
				_scoreDistribution = scoreDistribution;
				_scoreSum = scoreDistribution.Sum();
				_simulationCount = count;
			}

			public ulong Id { get; }

			public int PlayerCount => _scoreDistribution.Length;

			public long[] Wins => _scoreDistribution;

			public long Size => _simulationCount;

			public double WinRateOf(int player) => _scoreDistribution[player] / (double)_scoreSum;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Estimation Estimate(Request request, bool monteCarlo = true)
			{
				if(monteCarlo)
					return EstimateMonteCarlo(request);

				int playerCount = request.Players.Length;
				var wins = new long[playerCount];
				long games = 0;

				Deck deck = Deck.Factory;
				CardSet available = deck.State;

				var pockets = new CardSet[playerCount]; // preallocate once
				EstimatePlayers(request.Players, 0, available, pockets, wins, ref games);

				return new Estimation(request.Id, wins, games);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void EstimatePlayers(ImmutableArray<GTO.Range> ranges, int player, CardSet available, CardSet[] pockets, long[] wins, ref long games)
			{
				if (player == ranges.Length)
				{
					// simple threshold; tune with profiling
					if (available.Count >= 24) EstimateBoardsParallel(pockets, available, wins, ref games);
					else EstimateBoards(pockets, available, wins, ref games);
					return;
				}

				foreach (var cell in ranges[player].Cells)
				{
					foreach (CardSet pocket in cell.Hands)
					{
						if (!available.IsSupersetOf(pocket))
							continue;

						pockets[player] = pocket;
						EstimatePlayers(ranges, player + 1, available & ~pocket, pockets, wins, ref games);
					}
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void EstimateBoards(CardSet[] pockets, CardSet available, long[] wins, ref long games)
			{
				var combinations = available.Choose(5);

				foreach (CardSet board in combinations)
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

					games++;

					while (winners != 0)
					{
						int player =
						    BitOperations.TrailingZeroCount(winners);

						wins[player]++;

						winners &= winners - 1;
					}
				}
			}

			private static void EstimateBoardsParallel(CardSet[] pockets, CardSet available, long[] wins, ref long games)
			{
				int playerCount = pockets.Length;
				object sync = new();
				long totalGames = 0;

				_ = System.Threading.Tasks.Parallel.ForEach(
					available.Choose(5),
					() => (Games: 0L, Wins: new long[playerCount]),
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

						local.Games++;

						while (winners != 0)
						{
							int player = BitOperations.TrailingZeroCount(winners);
							local.Wins[player]++;
							winners &= winners - 1;
						}

						return local;
					},
					local =>
					{
						lock (sync)
						{
							totalGames += local.Games;

							for (int i = 0; i < playerCount; i++)
								wins[i] += local.Wins[i];
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
					System.Collections.Generic.List<CardSet> list = new(256);

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

					return new Estimation(request.Id, local.Wins, local.Games);
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

				var wins = new long[playerCount];
				long games = 0;

				for (int i = 0; i < locals.Length; i++)
				{
					var local = locals[i];
					games += local.Games;

					for (int p = 0; p < playerCount; p++)
						wins[p] += local.Wins[p];
				}

				return new Estimation(request.Id, wins, games);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void RunMonteCarlo(CardSet[][] allHands, int simulations, MonteCarloLocal local)
			{
				for (int i = 0; i < simulations; i++)
				{
					if (!TryPlayOne(allHands, local.Pockets, local.Candidates, ref local.Rng, out ulong winnersMask))
						continue;

					local.Games++;

					while (winnersMask != 0)
					{
						int w = BitOperations.TrailingZeroCount(winnersMask);
						local.Wins[w]++;
						winnersMask &= winnersMask - 1;
					}
				}
			}

			private sealed class MonteCarloLocal
			{
				public MonteCarloLocal(int playerCount, int maxHands, ulong seed)
				{
					Rng = new XorShift64Star(seed);
					Wins = new long[playerCount];
					Pockets = new CardSet[playerCount];
					Candidates = new CardSet[maxHands];
				}

				public XorShift64Star Rng;
				public long[] Wins { get; }
				public long Games;
				public CardSet[] Pockets { get; }
				public CardSet[] Candidates { get; }
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