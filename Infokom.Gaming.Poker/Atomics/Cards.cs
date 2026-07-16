using Infokom.Numerics.Extensions;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Atomics
{
	public enum Cards : ulong
	{
		NONE		= 0b_0000000000000_0000000000000_0000000000000_0000000000000,
		CLUBS	= 0b_0000000000000_0000000000000_0000000000000_1111111111111,
		DIAMONDS	= 0b_0000000000000_0000000000000_1111111111111_0000000000000,
		HEARTS	= 0b_0000000000000_1111111111111_0000000000000_0000000000000,
		SPADES	= 0b_1111111111111_0000000000000_0000000000000_0000000000000,

		TWOS		= 0b_0000000000001_0000000000001_0000000000001_0000000000001,
		THREES	= 0b_0000000000010_0000000000010_0000000000010_0000000000010,
		FOURS	= 0B_0000000000100_0000000000100_0000000000100_0000000000100,
		FIVES	= 0B_0000000001000_0000000001000_0000000001000_0000000001000,
		SIXES	= 0B_0000000010000_0000000010000_0000000010000_0000000010000,
		SEVENS	= 0B_0000000100000_0000000100000_0000000100000_0000000100000,
		EIGHTS	= 0B_0000001000000_0000001000000_0000001000000_0000001000000,
		NINES	= 0B_0000010000000_0000010000000_0000010000000_0000010000000,
		TENS		= 0B_0000100000000_0000100000000_0000100000000_0000100000000,
		JACKS	= 0B_0001000000000_0001000000000_0001000000000_0001000000000,
		QUEENS	= 0B_0010000000000_0010000000000_0010000000000_0010000000000,
		KINGS	= 0B_0100000000000_0100000000000_0100000000000_0100000000000,
		ACES		= 0b_1000000000000_1000000000000_1000000000000_1000000000000,

		// === CLUBS	===
		TWO_OF_CLUBS = TWOS & CLUBS,
		THREE_OF_CLUBS = THREES & CLUBS,
		FOUR_OF_CLUBS = FOURS & CLUBS,
		FIVE_OF_CLUBS = FIVES & CLUBS,
		SIX_OF_CLUBS = SIXES & CLUBS,
		SEVEN_OF_CLUBS = SEVENS & CLUBS,
		EIGHT_OF_CLUBS = EIGHTS & CLUBS,
		NINE_OF_CLUBS = NINES & CLUBS,
		TEN_OF_CLUBS = TENS & CLUBS,
		JACK_OF_CLUBS = JACKS & CLUBS,
		QUEEN_OF_CLUBS = QUEENS & CLUBS,
		KING_OF_CLUBS = KINGS & CLUBS,
		ACE_OF_CLUBS = ACES & CLUBS,

		// === DIAMONDS	===
		TWO_OF_DIAMS = TWOS & DIAMONDS,
		THREE_OF_DIAMS = THREES & DIAMONDS,
		FOUR_OF_DIAMS = FOURS & DIAMONDS,
		FIVE_OF_DIAMS = FIVES & DIAMONDS,
		SIX_OF_DIAMS = SIXES & DIAMONDS,
		SEVEN_OF_DIAMS = SEVENS & DIAMONDS,
		EIGHT_OF_DIAMS = EIGHTS & DIAMONDS,
		NINE_OF_DIAMS = NINES & DIAMONDS,
		TEN_OF_DIAMS = TENS & DIAMONDS,
		JACK_OF_DIAMS = JACKS & DIAMONDS,
		QUEEN_OF_DIAMS = QUEENS & DIAMONDS,
		KING_OF_DIAMS = KINGS & DIAMONDS,
		ACE_OF_DIAMS = ACES & DIAMONDS,

		// === HEARTS	===
		TWO_OF_HEARTS = TWOS & HEARTS,
		THREE_OF_HEARTS = THREES & HEARTS,
		FOUR_OF_HEARTS = FOURS & HEARTS,
		FIVE_OF_HEARTS = FIVES & HEARTS,
		SIX_OF_HEARTS = SIXES & HEARTS,
		SEVEN_OF_HEARTS = SEVENS & HEARTS,
		EIGHT_OF_HEARTS = EIGHTS & HEARTS,
		NINE_OF_HEARTS = NINES & HEARTS,
		TEN_OF_HEARTS = TENS & HEARTS,
		JACK_OF_HEARTS = JACKS & HEARTS,
		QUEEN_OF_HEARTS = QUEENS & HEARTS,
		KING_OF_HEARTS = KINGS & HEARTS,
		ACE_OF_HEARTS = ACES & HEARTS,

		// === SPADES ===
		TWO_OF_SPADES = TWOS & SPADES,
		THREE_OF_SPADES = THREES & SPADES,
		FOUR_OF_SPADES = FOURS & SPADES,
		FIVE_OF_SPADES = FIVES & SPADES,
		SIX_OF_SPADES = SIXES & SPADES,
		SEVEN_OF_SPADES = SEVENS & SPADES,
		EIGHT_OF_SPADES = EIGHTS & SPADES,
		NINE_OF_SPADES = NINES & SPADES,
		TEN_OF_SPADES = TENS & SPADES,
		JACK_OF_SPADES = JACKS & SPADES,
		QUEEN_OF_SPADES = QUEENS & SPADES,
		KING_OF_SPADES = KINGS & SPADES,
		ACE_OF_SPADES = ACES & SPADES,

		ALL = 0b_1111111111111_1111111111111_1111111111111_1111111111111,
	}

	public static class CardsExtensions
	{
		extension(Cards)
		{
			public static Cards Select(Card element) => (Cards)Card.KeyOf(element);

			public static Cards Select(params ReadOnlySpan<Card> elements)
			{
				var result = Cards.NONE;

				int n = elements.Length;
				int i = 0;

				while (i < n)
				{
					result |= Cards.Select(elements[i++]);
				}
				

				return result;
			}
		}

		extension(Cards root)
		{
			public ulong ID => (ulong)root;

			public Ranks Clubs
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Ranks)((ulong)(root & Cards.CLUBS) >> 00);
			}

			public Ranks Diams
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Ranks)((ulong)(root & Cards.DIAMONDS) >> 13);
			}

			public Ranks Hearts
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Ranks)((ulong)(root & Cards.HEARTS) >> 26);
			}

			public Ranks Spades
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Ranks)((ulong)(root & Cards.SPADES) >> 39);
			}

			public Ranks Ranks
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => root.Clubs | root.Diams | root.Hearts | root.Spades;
			}

			public Suits Suits
			{
				get
				{
					var result = Suits.None;

					result |= root.Clubs.IsEmpty	? Suits.None : Suits.Club;
					result |= root.Diams.IsEmpty	? Suits.None : Suits.Diamond;
					result |= root.Hearts.IsEmpty	? Suits.None : Suits.Heart;
					result |= root.Spades.IsEmpty	? Suits.None : Suits.Spade;

					return result;
				}
			}


			public int Count => BitOperations.PopCount((ulong)root);
			public bool IsEmpty => root == Cards.NONE;


			internal int UppIndex => ((ulong)root).UppBit;
			internal int LowIndex => ((ulong)root).LowBit;

			public Card LowerBound => Card.Values[root.LowIndex];
			public Card UpperBound => Card.Values[root.UppIndex];


			public void Deconstruct(out Ranks clubs, out Ranks diamonds, out Ranks hearts, out Ranks spades)
			{
				clubs = (Ranks)((ulong)(root & Cards.CLUBS) >> 00);
				diamonds = (Ranks)((ulong)(root & Cards.DIAMONDS) >> 13);
				hearts = (Ranks)((ulong)(root & Cards.HEARTS) >> 26);
				spades = (Ranks)((ulong)(root & Cards.SPADES) >> 39);
			}


			public bool IsIncluded(Card element) => (root.ID & element.ID) != 0;


			public Cards Include(Card element) => (Cards)(root.ID |  element.ID);

			public Cards Exclude(Card element) => (Cards)(root.ID & ~element.ID);

			public Cards Exclude(params ReadOnlySpan<Card> elements)
			{
				var result = root;
				
				for (int i = 0; i < elements.Length; i++)
				{
					result = result.Exclude(elements[i]);
				}

				return result;
			}

			public int CopyTo(Span<Card> target)
			{
				var mask = (ulong)root;

				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, root.Count, "target.Length");

				int count = 0;
				foreach (var index in mask.Bits)
				{
					target[count] = Card.Values[index];
					count++;
				}

				return count;
			}
		}
	}

}
