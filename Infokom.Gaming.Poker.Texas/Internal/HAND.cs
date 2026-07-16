using Holdem.Core.Extensions;

using Infokom.Gaming.Poker.Atomics;
using Infokom.Gaming.Poker.Texas;
using Infokom.Numerics.Extensions;

using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

using static System.Math;

namespace Holdem.Core.Internal
{
	internal static class HAND
	{
		



		public static Vector512<uint> Evaluate(ReadOnlySpan<Pocket> players, Card flop1, Card flop2, Card flop3, Card turn, Card river)
		{
			int n = players.Length;

			// 1. Krijojmë vektorin bosh direkt në stack (0 Alokime në Heap)
			Vector512<uint> result = Vector512<uint>.Zero;

			unsafe
			{
				// 2. Marrim pointer-in e vektorit për të shkruar direkt në slote (No Copy)
				uint* w = (uint*)&result;

				// 3. Cikli yt i parë kompakt - kjo e mban metodën të vogël në ASM
				// duke lejuar CPU-në ta mbajë të gjithë kodin në L1 Instruction Cache
				for (int i = 0; i < n; i++)
				{
					var (hi, lo) = players[i];

					w[i] = HAND.Evaluate(hi, lo, flop1, flop2, flop3, turn, river);
				}
			}

			return result;
		}



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Evaluate(Cards hand)
		{
			Span<Card> cards = stackalloc Card[hand.Count];
			Span<Rank> ranks = stackalloc Rank[hand.Count];
			Span<Suit> suits = stackalloc Suit[hand.Count];


			_ = hand.CopyTo(cards);
			_ = hand.Ranks.CopyTo(ranks);
			_ = hand.Suits.CopyTo(suits);

			// 1. Grumbullimi i të gjitha rangjeve në një maskë të vetme
			var rankSpectrum = (uint)(hand.Ranks);

			// 2. Kontrolli i shpejtë dhe i saktë për FLUSH (cdhs - 4 bit)
			var (c, d, h, s) = (hand.Clubs, hand.Diams, hand.Hearts, hand.Spades);


			var sCount = hand.Spades.Count;
			var hCount = hand.Hearts.Count;
			var dCount = hand.Diams.Count;
			var cCount = hand.Clubs.Count;

			

			if (sCount >= 5 || hCount >= 5 || dCount >= 5 || cCount >= 5)
			{
				uint flushSuitMask = sCount >= 5 ? 1U : (hCount >= 5 ? 2U : (dCount >= 5 ? 4U : 8U));

				uint flushRanks = 0;
				if ((suits[0].Mask & flushSuitMask) != 0) flushRanks |= (uint)ranks[0];
				if ((suits[1] & flushSuitMask) != 0) flushRanks |= (uint)ranks[1];
				if ((suits[2] & flushSuitMask) != 0) flushRanks |= (uint)ranks[2];
				if ((suits[3] & flushSuitMask) != 0) flushRanks |= (uint)ranks[3];
				if ((suits[4] & flushSuitMask) != 0) flushRanks |= (uint)ranks[4];
				if ((suits[5] & flushSuitMask) != 0) flushRanks |= (uint)ranks[5];
				if ((suits[6] & flushSuitMask) != 0) flushRanks |= (uint)ranks[6];

				uint sfRanks = GetBest5CardStraight(flushRanks);
				if (sfRanks != 0)
					return 0x80000000 + sfRanks; // Straight Flush (Prefiksi më i lartë fiks!)

				return 0x50000000 + GetTop5Bits(flushRanks); // Flush (0x50000000)
			}


			var pairs = (c & d) | (c & h) | (c & s) | (d & h) | (d & s) | (h & s);
			var trips = (c & d & h) | (c & d & s) | (c & h & s) | (d & h & s);
			var quads = (c & d & h & s);

			// 4. VLERËSIMI FINAL HIERARKIK
			if (!quads.IsEmpty)
			{
				var rank = quads.UpperBound;
				var kicker = (hand.Ranks & ~quads).UpperBound;
				return 0x70000000 + (Rank.GetID(rank) << 13) + Rank.GetID(kicker); // Poker
			}

			if (!trips.IsEmpty)
			{
				var r3 = Ranks.Select(trips.UpperBound);
				var r2 = pairs & ~trips;

				// Full House: Ndodh nëse ka një çift tjetër, ose nëse ka më shumë se 1 rang me trips
				if (r2.Count > 0 || trips.Count > 1)
				{
					r2 = !r2.IsEmpty ? r2 : (trips & ~r3);
					r2 = Ranks.Select(r2.UpperBound);
					return 0x60000000u + ((ulong)r3 << 13) + (ulong)r2; // Full House (0x60000000)
				}
			}

			// Kontrolli i Straight të thjeshtë
			uint straightRanks = GetBest5CardStraight(rankSpectrum);
			if (straightRanks != 0)
				return 0x40000000 + straightRanks; // Straight (0x40000000)

			if (trips != 0)
			{
				uint tripRank = 1U << (31 - BitOperations.LeadingZeroCount(trips));
				uint kickers = GetTop2Bits(rankSpectrum & ~tripRank);
				return 0x30000000 + (tripRank << 13) + kickers; // Tris
			}

			int pairCount = pairs.Count;
			if (pairCount >= 2)
			{
				var topPair = pairs.UpperBound;
				var nextPair = pairs.Exclude(topPair);
				uint kicker = 1U << (31 - BitOperations.LeadingZeroCount(rankSpectrum & ~(topPair | nextPair)));
				return 0x20000000 + ((topPair | nextPair) << 13) + kicker; // Two Pair
			}
			if (pairCount == 1)
			{
				uint kickers = GetTop3Bits(rankSpectrum & ~pairs);
				return 0x10000000 + (pairs << 13) + kickers; // One Pair
			}

			return 0x00000000 + GetTop5Bits(rankSpectrum); // High Card
		}


		// --- Metodat Ndihmëse Branchless / Pa Cikle ---

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint GetBest5CardStraight(uint ranks)
		{
			if ((ranks & 0x1F00) == 0x1F00) return 0x1F00; // AKQJT
			if ((ranks & 0x0F80) == 0x0F80) return 0x0F80; // KQJT9
			if ((ranks & 0x07C0) == 0x07C0) return 0x07C0; // QJT98
			if ((ranks & 0x03E0) == 0x03E0) return 0x03E0; // JT987
			if ((ranks & 0x01F0) == 0x01F0) return 0x01F0; // T9876
			if ((ranks & 0x00F8) == 0x00F8) return 0x00F8; // 98765
			if ((ranks & 0x007C) == 0x007C) return 0x007C; // 87654
			if ((ranks & 0x003E) == 0x003E) return 0x003E; // 76543
			if ((ranks & 0x001F) == 0x001F) return 0x001F; // 65432
			if ((ranks & 0x100F) == 0x100F) return 0x000F; // 5432A (Ace-low straight)
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint GetTop5Bits(uint mask)
		{
			int pop = BitOperations.PopCount(mask);
			if (pop == 7) mask &= mask - 1;
			if (pop >= 6) mask &= mask - 1;
			return mask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint GetTop3Bits(uint mask)
		{
			int pop = BitOperations.PopCount(mask);
			if (pop == 7) mask &= mask - 1;
			if (pop >= 6) mask &= mask - 1;
			if (pop >= 5) mask &= mask - 1;
			if (pop >= 4) mask &= mask - 1;
			return mask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint GetTop2Bits(uint mask)
		{
			int pop = BitOperations.PopCount(mask);
			if (pop == 7) mask &= mask - 1;
			if (pop >= 6) mask &= mask - 1;
			if (pop >= 5) mask &= mask - 1;
			if (pop >= 4) mask &= mask - 1;
			if (pop >= 3) mask &= mask - 1;
			return mask;
		}
	}
}
