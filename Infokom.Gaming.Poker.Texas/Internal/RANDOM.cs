using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Holdem.Core.Internal
{
	internal static class RANDOM
	{
		private const ulong SPADES_MASK = 0B0000000000000000000000000000000000000001111111111111UL;
		private const ulong HEARTS_MASK = 0B0000000000000000000000000011111111111110000000000000UL;
		private const ulong DIAMONDS_MASK = 0B0000000000000111111111111100000000000000000000000000UL;
		private const ulong CLUBS_MASK = 0B1111111111111000000000000000000000000000000000000000UL;

		private static readonly ulong[] SuitMasks = { SPADES_MASK, HEARTS_MASK, DIAMONDS_MASK, CLUBS_MASK };

		/// <summary>
		/// Merr maskën aktuale të kuvertës, zgjedh një letër random sipas skemës dy-fazore,
		/// dhe kthen maskën e izoluar të letrës së zgjedhur.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong GenerateNextShuffleMask(ulong currentDeckMask)
		{
			if (currentDeckMask == 0) return 0; // Kuverta është boshtë

			// ----------------------------------------------------
			// FAZA 1: Përzgjedhja e Suit-it direkt nga maska aktuale
			// ----------------------------------------------------
			
			ulong activeSuitsMask = 0;
			int activeSuitsCount = 0;

			// Ndërtojmë një maskë bitësh për suflat që kanë të paktën 1 letër të mbetur (maksimumi 4 bitë)
			for (int i = 0; i < 4; i++)
			{
				if ((currentDeckMask & SuitMasks[i]) != 0)
				{
					activeSuitsMask |= (1UL << i);
					activeSuitsCount++;
				}
			}

			// Zgjedhim një nga suflat aktive (p.sh. nëse kanë mbetur 3, zgjedhim indeksin e mbetur 0, 1 ose 2)
			int targetSuitRemainingIndex = Random.Shared.Next(activeSuitsCount);

			// Izolojmë bitin e suit-it të zgjedhur
			ulong isolatedSuitBit = GetNthSetBitSoftware(activeSuitsMask, targetSuitRemainingIndex);
			int chosenSuitId = BitOperations.TrailingZeroCount(isolatedSuitBit);

			// ----------------------------------------------------
			// FAZA 2: Përzgjedhja e Rank-ut brenda atij suiti
			// ----------------------------------------------------
			// Izolojmë vetëm bitët e mbetur të atij suiti specifik
			ulong targetSuitCurrentMask = currentDeckMask & SuitMasks[chosenSuitId];

			// Numërojmë sa letra kanë mbetur në këtë suit akti
			int cardsRemainingInSuit = BitOperations.PopCount(targetSuitCurrentMask);

			// Zgjedhim një indeks të rastësishëm nga ato që kanë mbetur
			int targetRankRemainingIndex = Random.Shared.Next(cardsRemainingInSuit);

			// Izolojmë bitin fiks të letrës finale
			ulong chosenCardMask = GetNthSetBit(targetSuitCurrentMask, targetRankRemainingIndex);

			return chosenCardMask; // Kthejmë maskën unike të letrës së shpërndarë (vetëm 1 bit i ndezur)
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong GetNthSetBit(ulong mask, int n)
		{
			// Nëse CPU mbështet hardware acceleration BMI2, kjo ekzekutohet instant
			if (System.Runtime.Intrinsics.X86.Bmi2.X64.IsSupported)
			{
				return System.Runtime.Intrinsics.X86.Bmi2.X64.ParallelBitDeposit(1UL << n, mask);
			}
			return GetNthSetBitSoftware(mask, n);
		}

		private static ulong GetNthSetBitSoftware(ulong mask, int n)
		{
			ulong temp = mask;
			for (int i = 0; i < n; i++) temp &= temp - 1;
			return temp & (ulong)(-(long)temp);
		}
	}
}
