using Infokom.Numerics;
using Infokom.Numerics.Extensions;
using Infokom.Numerics.Operators;

using System.Collections.Concurrent;
using System.Net.Security;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Infokom.Gaming.Poker.Atomics
{
	[Flags]
	public enum Ranks : ulong
	{
		None = 0b0000000000000,
		Two = 0b0000000000001,
		Three = 0b0000000000010,
		Four = 0b0000000000100,
		Five = 0b0000000001000,
		Six = 0b0000000010000,
		Seven = 0b0000000100000,
		Eight = 0b0000001000000,
		Nine = 0b0000010000000,
		Ten = 0b0000100000000,
		Jack = 0b0001000000000,
		Queen = 0b0010000000000,
		King = 0b0100000000000,
		Ace = 0b1000000000000,
		All = 0b1111111111111
	}




	public static class RanksEnumExtension
	{
		extension(Ranks)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1) => (Ranks)(r1.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2) => (Ranks)(r1.ID | r2.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3) => (Ranks)(r1.ID | r2.ID | r3.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4) => (Ranks)(r1.ID | r2.ID | r3.ID | r4.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4, Rank r5) => (Ranks)(r1.ID | r2.ID | r3.ID | r4.ID | r5.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4, Rank r5, Rank r6) => (Ranks)(r1.ID | r2.ID | r3.ID | r4.ID | r5.ID | r6.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4, Rank r5, Rank r6, Rank r7) => (Ranks)(r1.ID | r2.ID | r3.ID | r4.ID | r5.ID | r6.ID | r7.ID);

			public static Ranks Select(params ReadOnlySpan<Rank> elements)
			{
				var selection = Ranks.None;

				int i = 0;
				int n = elements.Length;

				while (i < n)
				{
					selection |= selection.Include(elements[i++]);
				}

				return selection;
			}

		}


		extension(Ranks selection)
		{
			public ulong ID
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (ulong)selection;
			}

			public bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (selection & Ranks.All) == 0;
			}

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount((ulong)(selection & Ranks.All));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Ranks Include(Rank element) => (Ranks)(selection.ID | element.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Ranks Exclude(Rank element) => (Ranks)(selection.ID & ~element.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Contains(Rank element) => (selection.ID & element.ID) != 0;

			internal (Ranks HI, Ranks LO) HILO(int i)
			{
				return ((Ranks, Ranks))((ulong)selection).HILO(i);
			}

			internal (Ranks LO, Ranks HI) LOHI(int i)
			{
				return ((Ranks, Ranks))((ulong)selection).HILO(^i);
			}

			public Ranks HI
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Ranks)(1UL << ((ulong)selection).BSR);
			}

			public Ranks LO
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Ranks)(1UL << ((ulong)selection).BSF);
			}


			public Rank Hi
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.Values[((ulong)selection).BSR];
			}

			public Rank Lo
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.Values[((ulong)selection).BSF];
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CopyTo(Span<Rank> target)
			{
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, selection.Count, "target.Length");

				var mask =((ulong)selection);

				int n = 0;

				if (mask.BitTest(n))	target[n++] = Rank.Two;
				if (mask.BitTest(n))	target[n++] = Rank.Three;
				if (mask.BitTest(n))	target[n++] = Rank.Four;
				if (mask.BitTest(n))	target[n++] = Rank.Five;
				if (mask.BitTest(n))	target[n++] = Rank.Six;
				if (mask.BitTest(n))	target[n++] = Rank.Seven;
				if (mask.BitTest(n))	target[n++] = Rank.Eight;
				if (mask.BitTest(n))	target[n++] = Rank.Nine;
				if (mask.BitTest(n))	target[n++] = Rank.Ten;
				if (mask.BitTest(n))	target[n++] = Rank.Jack;
				if (mask.BitTest(n))	target[n++] = Rank.Queen;
				if (mask.BitTest(n))	target[n++] = Rank.King;
				if (mask.BitTest(n))	target[n++] = Rank.Ace;
				
				return n;
			}
		}

	}
}
