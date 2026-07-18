using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using static System.Collections.Specialized.BitVector32;

namespace Infokom.Gaming.Poker.Atomics
{

	[Flags]
	public enum Suits : byte
	{
		None		= 0b_0000,
		Club		= 0b_0001,
		Diamond	= 0b_0010,
		Heart	= 0b_0100,
		Spade	= 0b_1000,
		All		= 0b_1111
	}

	public static class SuitOptionExtensions
	{
		#region static extensions
		extension(Suits)
		{

			public static Suits Select(Suit s1)												=> (Suits)s1.ID;
			public static Suits Select(Suit s1, Suit s2)											=> (Suits)(s1.ID | s2.ID);
			public static Suits Select(Suit s1, Suit s2, Suit s3)									=> (Suits)(s1.ID | s2.ID | s3.ID);
			public static Suits Select(Suit s1, Suit s2, Suit s3, Suit s4)							=> (Suits)(s1.ID | s2.ID | s3.ID | s4.ID);
			public static Suits Select(Suit s1, Suit s2, Suit s3, Suit s4, Suit s5)					=> (Suits)(s1.ID | s2.ID | s3.ID | s4.ID | s5.ID);
			public static Suits Select(Suit s1, Suit s2, Suit s3, Suit s4, Suit s5, Suit s6)			=> (Suits)(s1.ID | s2.ID | s3.ID | s4.ID | s5.ID | s6.ID);
			public static Suits Select(Suit s1, Suit s2, Suit s3, Suit s4, Suit s5, Suit s6, Suit s7)		=> (Suits)(s1.ID | s2.ID | s3.ID | s4.ID | s5.ID | s6.ID | s7.ID);

			public static Suits Select(params ReadOnlySpan<Suit> elements)
			{
				var selection = Suits.None;

				int i = 0;
				int n = elements.Length;

				while (i < n)
				{
					selection = (Suits)(selection.ID | elements[i++].ID);
				}

				return selection;
			}
		}
		#endregion

		#region instance extensions
		extension(Suits selection)
		{
			public ulong ID => (ulong)selection;

			public bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (selection & Suits.All) == 0;
			}

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount((uint)(selection & Suits.All));
			}

			public Suits Include(Suit element) => (Suits)(selection.ID | element.ID);

			public Suits Exclude(Suit element) => (Suits)(selection.ID & ~element.ID);

			public bool Contains(Suit element) => (selection.ID & element.ID) != 0;



			public int CopyTo(Span<Suit> target)
			{
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, selection.Count, "target.Length");

				int n = 0;
				
				if (selection.Contains(Suit.Club))	target[n++] = Suit.Club;				
				if (selection.Contains(Suit.Diamond))	target[n++] = Suit.Diamond;
				if (selection.Contains(Suit.Heart))	target[n++] = Suit.Heart;
				if (selection.Contains(Suit.Spade))	target[n++] = Suit.Spade;

				return n;
			}
		}
		#endregion
	}
}
