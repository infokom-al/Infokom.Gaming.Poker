using Infokom.Numerics.Extensions;
using Infokom.Numerics.Operators;

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
			private static Ranks Select() => Ranks.None;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r) => Ranks.Select()
				.Include(r);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2) => Ranks.Select()
				.Include(r1)
				.Include(r2);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3) => Ranks.Select()
				.Include(r1)
				.Include(r2)
				.Include(r3);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4) => Ranks.Select()
				.Include(r1)
				.Include(r2)
				.Include(r3)
				.Include(r4);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4, Rank r5) => Ranks.Select()
				.Include(r1)
				.Include(r2)
				.Include(r3)
				.Include(r4)
				.Include(r5);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4, Rank r5, Rank r6) => Ranks.Select()
				.Include(r1)
				.Include(r2)
				.Include(r3)
				.Include(r4)
				.Include(r5)
				.Include(r6);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks Select(Rank r1, Rank r2, Rank r3, Rank r4, Rank r5, Rank r6, Rank r7) => Ranks.Select()
				.Include(r1)
				.Include(r2)
				.Include(r3)
				.Include(r4)
				.Include(r5)
				.Include(r6)
				.Include(r7);

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

			


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static int GetUppIndex(Ranks source) => ((ulong)source).UppBit;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static int GetLowIndex(Ranks source) => ((ulong)source).LowBit;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank GetUpperBound(Ranks source) => Rank.Values[Ranks.GetUppIndex(source)];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank GetLowerBound(Ranks source) => Rank.Values[Ranks.GetLowIndex(source)];
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
			public Ranks Include(Rank rank) => (Ranks)(selection.ID | Rank.GetID(rank));

			public Ranks Exclude(Rank rank) => (Ranks)(selection.ID & ~Rank.GetID(rank));

			public bool IsIncluded(Rank rank) => (selection.ID & Rank.GetID(rank)) != 0;



			public Rank UpperBound
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Ranks.GetUpperBound(selection);
			}

			public Rank LowerBound
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Ranks.GetLowerBound(selection);
			}



			public int CopyTo(Span<Rank> target)
			{
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, selection.Count, "target.Length");

				int n = 0;

				if (selection.IsIncluded(Rank.Two))	target[n++] = Rank.Two;
				if (selection.IsIncluded(Rank.Three))	target[n++] = Rank.Three;
				if (selection.IsIncluded(Rank.Four))	target[n++] = Rank.Four;
				if (selection.IsIncluded(Rank.Five))	target[n++] = Rank.Five;
				if (selection.IsIncluded(Rank.Six))	target[n++] = Rank.Six;
				if (selection.IsIncluded(Rank.Seven))	target[n++] = Rank.Seven;
				if (selection.IsIncluded(Rank.Eight))	target[n++] = Rank.Eight;
				if (selection.IsIncluded(Rank.Nine))	target[n++] = Rank.Nine;
				if (selection.IsIncluded(Rank.Ten))	target[n++] = Rank.Ten;
				if (selection.IsIncluded(Rank.Jack))	target[n++] = Rank.Jack;
				if (selection.IsIncluded(Rank.Queen))	target[n++] = Rank.Queen;
				if (selection.IsIncluded(Rank.King))	target[n++] = Rank.King;
				if (selection.IsIncluded(Rank.Ace))	target[n++] = Rank.Ace;

				return n;
			}
		}

	}
}
