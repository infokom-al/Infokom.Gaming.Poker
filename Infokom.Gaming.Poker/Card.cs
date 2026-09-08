using Infokom.Numerics.Atomics;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker
{
	// $C = R \times S$
	public enum Card : sbyte
	{
		_2 = -14,
		S2 = _2 + 16, S3, S4, S5, S6, S7, S8, S9, ST, SJ, SQ, SK, SA,
		D2 = S2 + 16, D3, D4, D5, D6, D7, D8, D9, DT, DJ, DQ, DK, DA,
		C2 = D2 + 16, C3, C4, C5, C6, C7, C8, C9, CT, CJ, CQ, CK, CA,
		H2 = C2 + 16, H3, H4, H5, H6, H7, H8, H9, HT, HJ, HQ, HK, HA,
	}

	public static class CardExtensions
	{
		extension(Rank)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Card operator *(Rank r, Suit s) => (Card)((sbyte)r + (BitOperations.TrailingZeroCount((byte)s) * 16));

			public static ValueTuple<Card, Card> operator *(Rank r, in ValueTuple<Suit, Suit> s) => (r * s.Item1, r * s.Item2);
			public static ValueTuple<Card, Card, Card> operator *(Rank r, in ValueTuple<Suit, Suit, Suit> s) => (r * s.Item1, r * s.Item2, r * s.Item3);
			public static ValueTuple<Card, Card, Card, Card> operator *(Rank r, in ValueTuple<Suit, Suit, Suit, Suit> s) => (r * s.Item1, r * s.Item2, r * s.Item3, r * s.Item4);
		}

		extension(Suit)
		{

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Card operator *(Suit suit, Rank ranks) => ranks * suit;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Card operator *(int r, Suit s) => (Card)((sbyte)r + (BitOperations.TrailingZeroCount((byte)s) * 16));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Card operator *(Suit suit, int ranks) => ranks * suit;
		}

		extension(Card)
		{
			/**
			 * <summary>
			 * Get a card from a given rank and suit
			 * </summary>
			 * <param name="ρ">
			 * <see cref="Rank">Rank</see> of the <see cref="Card">card</see>
			 * </param>
			 * <param name="σ">
			 * <see cref="Suit">Suit</see> of the required <see cref="Card">card</see>
			 * </param>
			 * <returns>
			 * A <see cref="Card">card</see> with rank <paramref name="ρ"/> and suit <paramref name="σ"/>
			 * </returns>
			 */
			public static Card Of(Rank ρ, Suit σ) => (Card)((sbyte)ρ + 16 * σ switch
			{
				Suit.Spade => +0,
				Suit.Diamond => +1,
				Suit.Club => +2,
				Suit.Heart => +3,
				_ => -1
			});


			public static Card Hi(Card a, Card b) => a.R > b.R ? a : a.R == b.R && a.S > b.S ? a : b;

			public static Card Lo(Card a, Card b) => a.R < b.R ? a : a.R == b.R & a.S < b.S ? a : b;

			public static bool operator <(Card a, Rank b) => a.Rank < b;
			public static bool operator <(Card a, int b) => a.R < b;

			public static bool operator >(Card a, Rank b) => a.Rank > b;
			public static bool operator >(Card a, int b) => a.R > b;

			/**
			 * <summary>
			 * Get a <see cref="Card">card</see> from text.
			 * </summary>
			 * <param name="source">
			 * text to parse
			 * </param>
			 * <remarks>
			 * <c>var x = <see cref="Card">Card</see>.<see cref="Parse(ReadOnlySpan{char})">Parse</see>("As")</c>
			 * </remarks>
			 */
			public static Card Parse(ReadOnlySpan<char> source)
			{
				ArgumentOutOfRangeException.ThrowIfNotEqual(source.Length, 2);

				var (r, s) = (Rank.Cast(source[0]), Suit.Cast(source[1]));

				return r * s;
			}


			/// <summary>
			/// Try to get a <see cref="Card">card</see> from a text buffer presumed to contain a <see cref="get_Symbol(Card)">card symbol</see>.
			/// </summary>
			/// <param name="source">
			/// The symbol of the desired card
			/// </param>
			/// <param name="card">
			/// Value found
			/// </param>
			/// <returns>
			/// True if any <math></math> \exist x card with <paramref name="source"/> as symbol was found, false otherwise
			/// </returns>
			public static bool TryParse(ReadOnlySpan<char> source, out Card card)
			{
				if (source.Length != 2 || !Rank.TryCast(source[0], out var r) || !Suit.TryCast(source[1], out var s))
				{
					card = default;
					return false;
				}

				card = r * s;
				return true;
			}
		}

		extension(Card source)
		{
			public string Symbol => string.Create(2, source.ToString(), (span, card) =>
			{
				if (!string.IsNullOrEmpty(card) && card.Length == 2)
				{
					span[0] = card[1];
					span[1] = char.ToLowerInvariant(card[0]);
				}
				else
				{
					span[0] = '?';
					span[1] = '?';
				}
			});


			public int R => (int)source % 16;
			public int S => (int)source / 16;

			public Rank Rank => (Rank)(sbyte)((sbyte)source % 16);
			public Suit Suit => (Suit)(sbyte)(1u << ((sbyte)source / 16));

			public void Deconstruct(out Rank ρ, out Suit σ)
			{
				(ρ, σ) = ((Rank)(sbyte)((sbyte)source % 16), (Suit)(sbyte)(1u << ((sbyte)source / 16)));
			}
		}




		extension(BitMap<ulong, Card> source)
		{
			public void Deconstruct(out BitMap<ushort, Rank> s, out BitMap<ushort, Rank> d, out BitMap<ushort, Rank> c, out BitMap<ushort, Rank> h)
			{
				s = (BitMap<ushort, Rank>)((ushort)(((ulong)source) & 0x7FFCul));
				d = (BitMap<ushort, Rank>)((ushort)(((ulong)source >> 16) & 0x7FFCul));
				c = (BitMap<ushort, Rank>)((ushort)(((ulong)source >> 32) & 0x7FFCul));
				h = (BitMap<ushort, Rank>)((ushort)(((ulong)source >> 48) & 0x7FFCul));
			}
		}

	}
}
