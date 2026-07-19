using Holdem.Core.Extensions;

using Infokom.Gaming.Poker;
using Infokom.Gaming.Poker.Atomics;
using Infokom.Gaming.Poker.Texas;
using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System.Buffers;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.ComponentModel.DataAnnotations;

namespace Holdem.Core.Internal
{

	internal static class EQUITY
	{
		private class ThreadLocalState
		{
			private static int _threadCounter = 0;

			public readonly Random Rng;
			public readonly Pocket[] PocketsBuffer;
			public readonly float[] LocalWArray; // Zëvendësoi Vector512 për të ruajtur integritetin e memories

			public Cards GeTPocketCards() => PocketsBuffer.Select(p => (Cards)p.ID).Aggregate((c1, c2) => c1 | c2);

			public ThreadLocalState(int n)
			{
				int threadId = Interlocked.Increment(ref _threadCounter);
				int managedThreadId = Environment.CurrentManagedThreadId;
				int seed = Environment.TickCount ^ (threadId * 31337) ^ (managedThreadId * 71);
				
				Rng = new Random(seed);
				PocketsBuffer = new Pocket[n];
				LocalWArray = new float[n]; // Alokohet vetëm një herë për thread (0 alokime në loop)
			}
		}


		public static Vector512<float> Estimate(ReadOnlySpan<GTO.Range.Mask> ranges, int trials = 100000)
		{
			if (ranges.Length > 10)
				throw new ArgumentException("Number of ranges must be 10 or less");

			int n = ranges.Length;

			Pocket[][] flatCachedRanges = new Pocket[n][];
			for (int i = 0; i < n; i++)
			{
				flatCachedRanges[i] = [.. ranges[i].Cells.SelectMany(cell => cell.Combos)];
			}

			float[] globalWeightsArray = new float[16]; 
			object mergeLock = new object();

			// 3. PARALLEL.FOR ME UK ULTRA TË SIGURT
			_ = Parallel.For(0, trials, () => new ThreadLocalState(n), (t, state, localState) =>
			{
				bool hasValidPockets = false;
				ulong usedCardsMask = 0UL;

				while (!hasValidPockets)
				{
					usedCardsMask = 0UL;
					hasValidPockets = true;

					for (int i = 0; i < n; i++)
					{
						var playerPockets = flatCachedRanges[i];
						Pocket selectedPocket = playerPockets[localState.Rng.Next(playerPockets.Length)];


						ulong pocketMask = selectedPocket.ID;

						if ((usedCardsMask & pocketMask) != 0)
						{
							hasValidPockets = false;
							break;
						}

						localState.PocketsBuffer[i] = selectedPocket;
						usedCardsMask |= pocketMask;
					}
				}

				var pocketCards = localState.GeTPocketCards();
				// TËRHEQJA E BORDIT
				var deck = new Deck();
				deck.Remove(pocketCards);

				Span<Card> boardCards = stackalloc Card[5];
				deck.DrawTo(localState.Rng, boardCards, 5);

				_ = deck.TryDraw(localState.Rng, out var flop1);
				_ = deck.TryDraw(localState.Rng, out var flop2);
				_ = deck.TryDraw(localState.Rng, out var flop3);
				_ = deck.TryDraw(localState.Rng, out var turn);
				_ = deck.TryDraw(localState.Rng, out var river);

				Vector512<uint> handRank = HAND.Evaluate(localState.PocketsBuffer, boardCards[0], boardCards[1], boardCards[2], boardCards[3], boardCards[4]);
				
				Vector256<uint> vLeft = handRank.GetLower();
				Vector256<uint> vRight = handRank.GetUpper();
				Vector256<uint> vMax = Vector256.Max(vLeft, vRight);

				vMax = Vector256.Max(vMax, Vector256.Shuffle(vMax, Vector256.Create(4U, 5U, 6U, 7U, 0U, 1U, 2U, 3U)));
				vMax = Vector256.Max(vMax, Vector256.Shuffle(vMax, Vector256.Create(2U, 3U, 0U, 1U, 6U, 7U, 4U, 5U)));
				vMax = Vector256.Max(vMax, Vector256.Shuffle(vMax, Vector256.Create(1U, 0U, 3U, 2U, 5U, 4U, 7U, 6U)));

				uint maxRank = vMax.GetElement(0);

				// bonus calc N / K
				int K_winners = 0;
				for (int i = 0; i < n; i++)
				{
					if (handRank.GetElement(i) == maxRank) K_winners++;
				}

				float bonus = (float)n / K_winners;

				// bonus accumulation
				for (int i = 0; i < n; i++)
				{
					if (handRank.GetElement(i) == maxRank)
					{
						localState.LocalWArray[i] += bonus;
					}
				}

				return localState;
			},
			(finalLocalState) =>
			{
				// Bashkimi final i sigurt (Thread-Safe)
				lock (mergeLock)
				{
					for (int i = 0; i < n; i++)
					{
						globalWeightsArray[i] += finalLocalState.LocalWArray[i];
					}
				}
			});

			// results
			return Vector512.Create(globalWeightsArray);
		}



		public static float[] Estimate(ReadOnlySpan<Pocket> players, bool parallel = false)
		{
			if (parallel)
			{
				return EstimateParallel(players);
			}
			else
			{
				return EstimateSequential(players);
			}
		}

		private static unsafe float[] EstimateParallel(ReadOnlySpan<Pocket> players)
		{
			var playersCount = players.Length;

			var deck = new Deck();

			for (int i = 0; i < players.Length; i++)
			{
				var pi = players[i];
				for (int j = i + 1; j < players.Length; j++)
				{
					var pj = players[j];

					if (pi.Overlaps(pj))
					{
						throw new ArgumentException($"Players {i} and {j} have overlapping pockets.");
					}

					deck.Remove(pj.Cards);
				}

				deck.Remove(pi.Cards);
			}

			var deckSize = deck.Count;

			var cards = new Card[deckSize];
			deck.DrawTo(Random.Shared, cards, deckSize);

			var ranks = new uint[players.Length];


			float[] w = new float[players.Length];


			ref Card startRef = ref MemoryMarshal.GetReference(cards);
			ref Pocket startPocketRef = ref MemoryMarshal.GetReference(players);

			fixed (Card* pBase = &startRef)
			fixed (Pocket* pPocketBase = &startPocketRef)
			{
				Card* pCard = pBase;
				Pocket* pPocket = pPocketBase;

				int I = deckSize - 4;
				int J = deckSize - 3;
				int K = deckSize - 2;
				int L = deckSize - 1;
				int M = deckSize - 1;
				int N = playersCount;

				_ = Parallel.For(0, I, i =>
				{
					var flop1 = *(pCard + i);
					for (int j = i + 1; j < J; j++)
					{
						var flop2 = *(pCard + j);
						for (int k = j + 1; k < K; k++)
						{
							var flop3 = *(pCard + k);
							for (int l = k + 1; l < L; l++)
							{
								var turn = *(pCard + l);
								for (int m = l + 1; m < M; m++)
								{
									var river = *(pCard + m);

									var r = new uint[N];
									for (int n = 0; n < N; n++)
									{
										var pn = *(pPocket + n);


										var hand = pn.Cards | Cards.Select(flop1, flop2, flop3, turn, river);

										var rn = HAND.Evaluate(hand);

										r[n] = rn;
									}


									var mask = r == r.Max();

									int count = mask.PopCount();

									mask.ForEach((i) =>
									{
										w[i] += (float)N / count;
									});
								}
							}
						}
					}

				});
			}

			return w;
		}


		private unsafe static float[] EstimateSequential(ReadOnlySpan<Pocket> players)
		{
			var playersCount = players.Length;
			var deck = new Deck();

			var deckSize = deck.Count;
			Card* cards = stackalloc Card[deckSize];
			for (int i = 0; !deck.IsEmpty; i++) 
				cards[i] = deck.Draw(Random.Shared);

			float[] w = new float[playersCount];
			uint* r = stackalloc uint[playersCount];

			// Zëvendëso Parallel.For me një loop të thjeshtë 'for' sekuencial
			for (int i = 0; i < deckSize - 4; i++)
			{
				var flop1 = *(cards + i);
				for (int j = i + 1; j < deckSize - 3; j++)
				{
					var flop2 = *(cards + j);
					for (int k = j + 1; k < deckSize - 2; k++)
					{
						var flop3 = *(cards + k);
						for (int l = k + 1; l < deckSize - 1; l++)
						{
							var turn = *(cards + l);
							for (int m = l + 1; m < deckSize; m++)
							{
								var river = *(cards + m);

								uint maxScore = 0;
								for (int n = 0; n < playersCount; n++)
								{
									var pn = players[n];


									var hand = pn.Cards | Cards.Select(flop1, flop2, flop3, turn, river);

									uint rn = HAND.Evaluate(hand);
									r[n] = rn;
									if (rn > maxScore) maxScore = rn;
								}

								int K_winners = 0;
								for (int n = 0; n < playersCount; n++)
								{
									if (r[n] == maxScore) K_winners++;
								}

								float bonus = (float)playersCount / K_winners;
								for (int n = 0; n < playersCount; n++)
								{
									if (r[n] == maxScore) w[n] += bonus;
								}
							}
						}
					}
				}
			}
			return w;
		}

	}
}