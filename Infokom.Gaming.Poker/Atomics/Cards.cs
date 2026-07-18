using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Infokom.Gaming.Poker.Atomics
{

	public enum Cards : ulong
	{
		NONE				= 0,

		// === CLUBS	===
		TWO_OF_CLUBS		= 1ul << 00,
		THREE_OF_CLUBS		= 1ul << 01,
		FOUR_OF_CLUBS		= 1ul << 02,
		FIVE_OF_CLUBS		= 1ul << 03,
		SIX_OF_CLUBS		= 1ul << 04,
		SEVEN_OF_CLUBS		= 1ul << 05,
		EIGHT_OF_CLUBS		= 1ul << 06,
		NINE_OF_CLUBS		= 1ul << 07,
		TEN_OF_CLUBS		= 1ul << 08,
		JACK_OF_CLUBS		= 1ul << 09,
		QUEEN_OF_CLUBS		= 1ul << 10,
		KING_OF_CLUBS		= 1ul << 11,
		ACE_OF_CLUBS		= 1ul << 12,

		// === DIAMONDS	===
		TWO_OF_DIAMS		= 1ul << 13,
		THREE_OF_DIAMS		= 1ul << 14,
		FOUR_OF_DIAMS		= 1ul << 15,
		FIVE_OF_DIAMS		= 1ul << 16,
		SIX_OF_DIAMS		= 1ul << 17,
		SEVEN_OF_DIAMS		= 1ul << 18,
		EIGHT_OF_DIAMS		= 1ul << 19,
		NINE_OF_DIAMS		= 1ul << 20,
		TEN_OF_DIAMS		= 1ul << 21,
		JACK_OF_DIAMS		= 1ul << 22,
		QUEEN_OF_DIAMS		= 1ul << 23,
		KING_OF_DIAMS		= 1ul << 24,
		ACE_OF_DIAMS		= 1ul << 25,

		// === HEARTS	===
		TWO_OF_HEARTS		= 1ul << 26,
		THREE_OF_HEARTS	= 1ul << 27,
		FOUR_OF_HEARTS		= 1ul << 28,
		FIVE_OF_HEARTS		= 1ul << 29,
		SIX_OF_HEARTS		= 1ul << 30,
		SEVEN_OF_HEARTS	= 1ul << 31,
		EIGHT_OF_HEARTS	= 1ul << 32,
		NINE_OF_HEARTS		= 1ul << 33,
		TEN_OF_HEARTS		= 1ul << 34,
		JACK_OF_HEARTS		= 1ul << 35,
		QUEEN_OF_HEARTS	= 1ul << 36,
		KING_OF_HEARTS		= 1ul << 37,
		ACE_OF_HEARTS		= 1ul << 38,

		// === SPADES ===
		TWO_OF_SPADES		= 1ul << 39,
		THREE_OF_SPADES	= 1ul << 40,
		FOUR_OF_SPADES		= 1ul << 41,
		FIVE_OF_SPADES		= 1ul << 42,
		SIX_OF_SPADES		= 1ul << 43,
		SEVEN_OF_SPADES	= 1ul << 44,
		EIGHT_OF_SPADES	= 1ul << 45,
		NINE_OF_SPADES		= 1ul << 46,
		TEN_OF_SPADES		= 1ul << 47,
		JACK_OF_SPADES		= 1ul << 48,
		QUEEN_OF_SPADES	= 1ul << 49,
		KING_OF_SPADES		= 1ul << 50,
		ACE_OF_SPADES		= 1ul << 51,
	}

	public static class CardsExtensions
	{
		extension(Cards)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Select(Card c1) => (Cards)c1.ID;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Select(Card c1, Card c2) => (Cards)(c1.ID | c2.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Select(Card c1, Card c2, Card c3) => (Cards)(c1.ID | c2.ID | c3.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Select(Card c1, Card c2, Card c3, Card c4) => (Cards)(c1.ID | c2.ID | c3.ID | c4.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Select(Card c1, Card c2, Card c3, Card c4, Card c5) => (Cards)(c1.ID | c2.ID | c3.ID | c4.ID | c5.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Select(Card c1, Card c2, Card c3, Card c4, Card c5, Card c6) => (Cards)(c1.ID | c2.ID | c3.ID | c4.ID | c5.ID | c6.ID);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Select(Card c1, Card c2, Card c3, Card c4, Card c5, Card c6, Card c7) => (Cards)(c1.ID | c2.ID | c3.ID | c4.ID | c5.ID | c6.ID | c7.ID);


			public static Cards CLUBS => (Cards)0b1111111111111UL;
			public static Cards DIAMONDS => (Cards)(0b1111111111111UL << 13);
			public static Cards HEARTS => (Cards)(0b1111111111111UL << 26);
			public static Cards SPADES => (Cards)(0b1111111111111UL << 39);
			public static Cards ALL => Cards.CLUBS | Cards.DIAMONDS | Cards.HEARTS | Cards.SPADES;

			public static Cards Select(params ReadOnlySpan<Card> elements)
			{
				var result = Cards.ACE_OF_CLUBS;

				int n = elements.Length;
				int i = 0;

				while (i < n)
				{
					result |= (Cards)elements[i++].ID;
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

					result |= root.Clubs.IsEmpty ? Suits.None : Suits.Club;
					result |= root.Diams.IsEmpty ? Suits.None : Suits.Diamond;
					result |= root.Hearts.IsEmpty ? Suits.None : Suits.Heart;
					result |= root.Spades.IsEmpty ? Suits.None : Suits.Spade;

					return result;
				}
			}


			public int Count => BitOperations.PopCount((ulong)root);
			public bool IsEmpty => root == Cards.NONE;

			internal int LowerFlagIndex => ((ulong)root).BSF;

			internal int UpperFlagIndex => ((ulong)root).BSR;

			internal Cards LowerFlag => (Cards)(1ul << ((ulong)root).BSF);

			internal Cards UpperFlag => (Cards)(1ul << ((ulong)root).BSR);

			public Card First => Card.Values[root.LowerFlagIndex];

			public Card Last => Card.Values[root.UpperFlagIndex];




			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Deconstruct(out Ranks clubs, out Ranks diamonds, out Ranks hearts, out Ranks spades)
			{
				clubs = (Ranks)((ulong)(root & Cards.CLUBS) >> 00);
				diamonds = (Ranks)((ulong)(root & Cards.DIAMONDS) >> 13);
				hearts = (Ranks)((ulong)(root & Cards.HEARTS) >> 26);
				spades = (Ranks)((ulong)(root & Cards.SPADES) >> 39);
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsIncluded(Card element) => (root.ID & element.ID).CNT != 0;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Cards Include(Card c1) => (Cards)(root.ID | c1.ID);


			public Cards Include(Card c1, Card c2) => (Cards)(root.ID | c1.ID | c2.ID);

			
			
			
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Cards Exclude(Card element) => (Cards)(root.ID & ~element.ID);


			public Cards Exclude(Card c1, Card c2) => (Cards)(root.ID & ~c1.ID & ~c2.ID);




			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

				var cards = root;

				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, cards.Count, "target.Length");

				var count = 0;
				while (!cards.IsEmpty)
				{
					var card = cards.First;
					
					target[count++] = card;

					cards = cards.Exclude(card);
				}

				return count;
			}

			public int CopyTo(Span<Card> target, int count)
			{
				var mask = (ulong)root;

				var cards = root;

				ArgumentOutOfRangeException.ThrowIfLessThan(root.Count, count, "root.Count");
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, count, "target.Length");

				count = 0;
				while(!cards.IsEmpty)
				{
					var card = cards.First;
					
					target[count++] = card;

					cards = cards.Exclude(card);
				}

				return count;
			}
		}



	}



}
