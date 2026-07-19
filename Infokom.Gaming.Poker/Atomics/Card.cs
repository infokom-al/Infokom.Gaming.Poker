using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System.Collections.Immutable;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Atomics
{
	public enum Card : ushort
	{
		/// <summary>
		/// Two of Clubs
		/// </summary>
		[Description("2c")]
		TwoOfClubs = Rank.Two | (Suit.Club << 8),

		/// <summary>
		/// Three of Clubs
		/// </summary>
		[Description("3c")]
		ThreeOfClubs = Rank.Three | (Suit.Club << 8),

		/// <summary>
		/// Four of Clubs
		/// </summary>
		[Description("4c")]
		FourOfClubs = Rank.Four | (Suit.Club << 8),

		/// <summary>
		/// Five of Clubs
		/// </summary>
		[Description("5c")]
		FiveOfClubs = Rank.Five | (Suit.Club << 8),

		/// <summary>
		/// Six of Clubs
		/// </summary>
		[Description("6c")]
		SixOfClubs = Rank.Six | (Suit.Club << 8),

		/// <summary>
		/// Seven of Clubs
		/// </summary>
		[Description("7c")]
		SevenOfClubs = Rank.Seven | (Suit.Club << 8),

		/// <summary>
		/// Eight of Clubs
		/// </summary>
		[Description("8c")]
		EightOfClubs = Rank.Eight | (Suit.Club << 8),

		/// <summary>
		/// Nine of Clubs
		/// </summary>
		[Description("9c")]
		NineOfClubs = Rank.Nine | (Suit.Club << 8),

		/// <summary>
		/// Ten of Clubs
		/// </summary>
		[Description("10c")]
		TenOfClubs = Rank.Ten | (Suit.Club << 8),

		/// <summary>
		/// Jack of Clubs
		/// </summary>
		[Description("Jc")]
		JackOfClubs = Rank.Jack | (Suit.Club << 8),

		/// <summary>
		/// Queen of Clubs
		/// </summary>
		[Description("Qc")]
		QueenOfClubs = Rank.Queen | (Suit.Club << 8),

		/// <summary>
		/// King of Clubs
		/// </summary>
		[Description("Kc")]
		KingOfClubs = Rank.King | (Suit.Club << 8),

		/// <summary>
		/// Ace of Clubs
		/// </summary>
		[Description("Ac")]
		AceOfClubs = Rank.Ace | (Suit.Club << 8),


		// === DIAMONDS (d) ===

		/// <summary>
		/// Two of Diamonds
		/// </summary>
		[Description("2d")]
		TwoOfDiamonds = Rank.Two | (Suit.Diamond << 8),

		/// <summary>
		/// Three of Diamonds
		/// </summary>
		[Description("3d")]
		ThreeOfDiamonds = Rank.Three | (Suit.Diamond << 8),

		/// <summary>
		/// Four of Diamonds
		/// </summary>
		[Description("4d")]
		FourOfDiamonds = Rank.Four | (Suit.Diamond << 8),

		/// <summary>
		/// Five of Diamonds
		/// </summary>
		[Description("5d")]
		FiveOfDiamonds = Rank.Five | (Suit.Diamond << 8),

		/// <summary>
		/// Six of Diamonds
		/// </summary>
		[Description("6d")]
		SixOfDiamonds = Rank.Six | (Suit.Diamond << 8),

		/// <summary>
		/// Seven of Diamonds
		/// </summary>
		[Description("7d")]
		SevenOfDiamonds = Rank.Seven | (Suit.Diamond << 8),

		/// <summary>
		/// Eight of Diamonds
		/// </summary>
		[Description("8d")]
		EightOfDiamonds = Rank.Eight | (Suit.Diamond << 8),

		/// <summary>
		/// Nine of Diamonds
		/// </summary>
		[Description("9d")]
		NineOfDiamonds = Rank.Nine | (Suit.Diamond << 8),

		/// <summary>
		/// Ten of Diamonds
		/// </summary>
		[Description("10d")]
		TenOfDiamonds = Rank.Ten | (Suit.Diamond << 8),

		/// <summary>
		/// Jack of Diamonds
		/// </summary>
		[Description("Jd")]
		JackOfDiamonds = Rank.Jack | (Suit.Diamond << 8),

		/// <summary>
		/// Queen of Diamonds
		/// </summary>
		[Description("Qd")]
		QueenOfDiamonds = Rank.Queen | (Suit.Diamond << 8),

		/// <summary>
		/// King of Diamonds
		/// </summary>
		[Description("Kd")]
		KingOfDiamonds = Rank.King | (Suit.Diamond << 8),

		/// <summary>
		/// Ace of Diamonds
		/// </summary>
		[Description("Ad")]
		AceOfDiamonds = Rank.Ace | (Suit.Diamond << 8),


		// === HEARTS (h) ===

		/// <summary>
		/// Two of Hearts
		/// </summary>
		[Description("2h")]
		TwoOfHearts = Rank.Two | (Suit.Heart << 8),

		/// <summary>
		/// Three of Hearts
		/// </summary>
		[Description("3h")]
		ThreeOfHearts = Rank.Three | (Suit.Heart << 8),

		/// <summary>
		/// Four of Hearts
		/// </summary>
		[Description("4h")]
		FourOfHearts = Rank.Four | (Suit.Heart << 8),

		/// <summary>
		/// Five of Hearts
		/// </summary>
		[Description("5h")]
		FiveOfHearts = Rank.Five | (Suit.Heart << 8),

		/// <summary>
		/// Six of Hearts
		/// </summary>
		[Description("6h")]
		SixOfHearts = Rank.Six | (Suit.Heart << 8),

		/// <summary>
		/// Seven of Hearts
		/// </summary>
		[Description("7h")]
		SevenOfHearts = Rank.Seven | (Suit.Heart << 8),

		/// <summary>
		/// Eight of Hearts
		/// </summary>
		[Description("8h")]
		EightOfHearts = Rank.Eight | (Suit.Heart << 8),

		/// <summary>
		/// Nine of Hearts
		/// </summary>
		[Description("9h")]
		NineOfHearts = Rank.Nine | (Suit.Heart << 8),

		/// <summary>
		/// Ten of Hearts
		/// </summary>
		[Description("10h")]
		TenOfHearts = Rank.Ten | (Suit.Heart << 8),

		/// <summary>
		/// Jack of Hearts
		/// </summary>
		[Description("Jh")]
		JackOfHearts = Rank.Jack | (Suit.Heart << 8),

		/// <summary>
		/// Queen of Hearts
		/// </summary>
		[Description("Qh")]
		QueenOfHearts = Rank.Queen | (Suit.Heart << 8),

		/// <summary>
		/// King of Hearts
		/// </summary>
		[Description("Kh")]
		KingOfHearts = Rank.King | (Suit.Heart << 8),

		/// <summary>
		/// Ace of Hearts
		/// </summary>
		[Description("Ah")]
		AceOfHearts = Rank.Ace | (Suit.Heart << 8),


		// === SPADES (s) ===

		/// <summary>
		/// Two of Spades
		/// </summary>
		[Description("2s")]
		TwoOfSpades = Rank.Two | (Suit.Spade << 8),

		/// <summary>
		/// Three of Spades
		/// </summary>
		[Description("3s")]
		ThreeOfSpades = Rank.Three | (Suit.Spade << 8),

		/// <summary>
		/// Four of Spades
		/// </summary>
		[Description("4s")]
		FourOfSpades = Rank.Four | (Suit.Spade << 8),

		/// <summary>
		/// Five of Spades
		/// </summary>
		[Description("5s")]
		FiveOfSpades = Rank.Five | (Suit.Spade << 8),

		/// <summary>
		/// Six of Spades
		/// </summary>
		[Description("6s")]
		SixOfSpades = Rank.Six | (Suit.Spade << 8),

		/// <summary>
		/// Seven of Spades
		/// </summary>
		[Description("7s")]
		SevenOfSpades = Rank.Seven | (Suit.Spade << 8),

		/// <summary>
		/// Eight of Spades
		/// </summary>
		[Description("8s")]
		EightOfSpades = Rank.Eight | (Suit.Spade << 8),

		/// <summary>
		/// Nine of Spades
		/// </summary>
		[Description("9s")]
		NineOfSpades = Rank.Nine | (Suit.Spade << 8),

		/// <summary>
		/// Ten of Spades
		/// </summary>
		[Description("10s")]
		TenOfSpades = Rank.Ten | (Suit.Spade << 8),

		/// <summary>
		/// Jack of Spades
		/// </summary>
		[Description("Js")]
		JackOfSpades = Rank.Jack | (Suit.Spade << 8),

		/// <summary>
		/// Queen of Spades
		/// </summary>
		[Description("Qs")]
		QueenOfSpades = Rank.Queen | (Suit.Spade << 8),

		/// <summary>
		/// King of Spades
		/// </summary>
		[Description("Ks")]
		KingOfSpades = Rank.King | (Suit.Spade << 8),

		/// <summary>
		/// Ace of Spades
		/// </summary>
		[Description("As")]
		AceOfSpades = Rank.Ace | (Suit.Spade << 8)
	}



	public static class CardExtensions
	{
		private static readonly ImmutableArray<Card> VALUES = [.. Suit.Values.SelectMany(s => Rank.Values.Select(r => Card.Of(r, s)))];



		extension(Card source)
		{
			public Rank Rank
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Rank)(((ushort)source >> 0) & 0xFF);
			}

			public Suit Suit
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Suit)(((ushort)source >> 8) & 0xFF);
			}


			public ulong ID
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					var idx = source.Index;
					if (idx < 0) return 0;
					return 1UL << idx;
				}
			}

			public int Index
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					var r = source.Rank.Index;
					if (r < 0) return -1;

					var s = source.Suit.Index;
					if (s < 0) return -1;
					
					return r + s * 13;
				}
			}

			public string Symbol => $"{source.Rank.Symbol}{source.Suit.Symbol}";




			/// <summary>
			/// Check if at least the <see cref="Rank"/> component of a card is known
			/// </summary>
			public bool IsRankable
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source.Rank.IsKnown;
			}

			/// <summary>
			///  Check if at least the <see cref="Suit"/> component of a card is known
			/// </summary>
			public bool IsSuitable
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source.Suit.IsKnown;
			}


			/// <summary>
			/// Check if a <see cref="Card"/> value has a known rank and suit
			/// </summary>
			public bool IsDrawable
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source.IsKnown;//expected equivalent to IsRankable && IsSuitable
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Deconstruct(out Rank rank, out Suit suit)
			{
				rank = (Rank)(((ushort)source >> 0) & 0xFF);
				suit = (Suit)(((ushort)source >> 8) & 0xFF);
			}
		}

		extension(Card)
		{
			
			public static ImmutableArray<Card> Values => VALUES;



			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int IndexOf(Rank r, Suit s) => Rank.IndexOf(r) + Suit.IndexOf(s) * Rank.Count;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int IndexOf(Card c)
			{
				var (r, s) = c;

				return Card.IndexOf(r, s);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool IndexOf(Card c, out int index) => (index = IndexOf(c)) >= 0;



			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Card Of(Rank r, Suit s) => (Card)((ushort)r | ((ushort)s << 8));


			public static Card ValueOf(int index)
			{
				var r = index % Rank.Count;
				var s = index / Rank.Count;

				var rank = Rank.ValueOf(r);
				var suit = Suit.ValueOf(s);

				return Card.Of(rank, suit);
			}

			public static bool ValueOf(int index, out Card card) => (card = 
				Rank.ValueOf(index % Rank.Count, out var r) && 
				Suit.ValueOf(index / Rank.Count, out var s)
				? Card.Of(r, s) : default) != default;




			public static bool TryParse(ReadOnlySpan<char> source, out Card target)
			{
				ArgumentOutOfRangeException.ThrowIfNotEqual(source.Length, 2);

				target = default;

				if (Rank.TryConvertFrom(source[0], out var rank))
				{
					if (Suit.TryConvertFrom(source[1], out var suit))
					{
						target = Card.Of(rank, suit);
						return true;
					}
				}

				return false;
			}

			public static Card Parse(ReadOnlySpan<char> source)
			{
				if (TryParse(source, out var result))
				{
					return result;
				}

				throw new FormatException();
			}
		}
	}
}
