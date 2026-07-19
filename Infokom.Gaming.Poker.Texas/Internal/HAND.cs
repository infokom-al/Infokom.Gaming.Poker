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

using static Holdem.Core.Internal.DECK;
using static System.Math;

namespace Holdem.Core.Internal
{
	internal static class HAND
	{
		public static Vector512<uint> Evaluate(ReadOnlySpan<Pocket> players, Card flop1, Card flop2, Card flop3, Card turn, Card river)
		{
			int n = players.Length;

			var board = Cards.Select(flop1, flop2, flop3, turn, river);

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
					var hand = board.Include(hi, lo);

					w[i] = (uint)HAND.Evaluate(hand);
				}
			}

			return result;
		}



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Evaluate(Cards hand)
		{
			var handCards = hand;
			var cdhs = hand.Ranks;
			var suits = hand.Suits;

			var ccc = handCards & ~handCards;

			Span<Card> cards = stackalloc Card[hand.Count];
			_ = handCards.CopyTo(cards);

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
				if ((cards[0].Suit.ID & flushSuitMask) != 0) flushRanks |= (uint)cards[0].Rank;
				if ((cards[1].Suit.ID & flushSuitMask) != 0) flushRanks |= (uint)cards[1].Rank;
				if ((cards[2].Suit.ID & flushSuitMask) != 0) flushRanks |= (uint)cards[2].Rank;
				if ((cards[3].Suit.ID & flushSuitMask) != 0) flushRanks |= (uint)cards[3].Rank;
				if ((cards[4].Suit.ID & flushSuitMask) != 0) flushRanks |= (uint)cards[4].Rank;
				//if ((cards[5].Suit.ID & flushSuitMask) != 0) flushRanks |= (uint)cards[5].Rank;
				//if ((cards[6].Suit.ID & flushSuitMask) != 0) flushRanks |= (uint)cards[6].Rank;

				uint sfRanks = GetBest5CardStraight(flushRanks);
				if (sfRanks != 0)
					return 0x80000000 + sfRanks; // Straight Flush (Prefiksi më i lartë fiks!)

				return 0x50000000 + GetTop5Bits(flushRanks); // Flush (0x50000000)
			}

			var r1 = (c | d | h | s);
			var r2 = (c & d) | (c & h) | (c & s) | (d & h) | (d & s) | (h & s);
			var r3 = (c & d & h) | (c & d & s) | (c & h & s) | (d & h & s);
			var r4 = (c & d & h & s);

			// 4. VLERËSIMI FINAL HIERARKIK
			if (!r4.IsEmpty)
			{
				(r4, _) = r4.HILO(1);//top quad rank
				(r1, _) = (r1 ^ r4).HILO(1);//top rank not r4

				return (uint)(0x70000000 + (r4.ID << 13) + r1.ID); // Poker
			}

			if (!r3.IsEmpty)
			{
				(r3, _) = r3.HILO(1);
				(r2, _) = (r2 ^ r3).HILO(1);

				if(!r2.IsEmpty)
				{
					return (uint)(0x60000000u + (r3.ID << 13) + r2.ID); // Full House (0x60000000)
				}

				r1 = (r1 ^ r3).HILO(2).HI;

				return (uint)(0x30000000 + (r3.ID << 13) + r1.ID); // Tris
			}

			// Kontrolli i Straight të thjeshtë
			uint straightRanks = GetBest5CardStraight(rankSpectrum);
			if (straightRanks != 0)
				return 0x40000000 + straightRanks; // Straight (0x40000000)


			if (r2.Count == 2)
			{
				(r2, _) = r2.HILO(2);
				(r1, _) = (r1 ^ r2).HILO(1);

				return (uint)(0x20000000 + (r2.ID << 13) + r1.ID); // Two Pair
			}

			if (r2.Count == 1)
			{
				(r2, _) = r2.HILO(1);
				(r1, _) = (r1 ^ r2).HILO(3); 

				return (uint)(0x10000000 + (r2.ID << 13) + r1.ID); // One Pair
			}

			return (uint)(0x00000000 + hand.Ranks.HILO(5).HI.ID); // High Card
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
	}
}
