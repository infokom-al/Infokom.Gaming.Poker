using Infokom.Gaming.Poker.Texas.Internal;
using Infokom.Numerics.Atomics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas
{
	public partial struct Hand
	{
		[Flags]
		public enum Features
		{
			//$$ H \subset C, |H| \geq 5, |\rho(H)| \geq 5, |\sigma(H)| \geq 2, \langle to-be-completed \rangle $$
			HasHighCard,
			HasOnePair,
			HasTwoPair,
			HasThreeOfAKind,
			HasStraight,


			HasFlush,
			HasFullHouse,
			HasFourOfAKind,
			HasStraightFlush,
			HasRoyalFlush,
		}

		/// <summary>
		/// 
		/// </summary>
		public enum Property : byte
		{
			IsHighCard = 0x1,
			IsOnePair = 0x2,
			IsTwoPair = 0x3,
			IsThreeOfAKind = 0x4,
			IsStraight = 0x5,
			IsFlush = 0x6,
			IsFullHouse = 0x7,
			IsFourOfAKind = 0x8,
			IsStraightFlush = 0x9,
			IsRoyalFlush = 0xA
		}

		/// <summary>
		/// 
		/// </summary>
		public enum Family : byte
		{
			HighCard = (byte)(HAND.HIGH >> 28),
			OnePair = (byte)(HAND.PAIR >> 28),
			TwoPair = (byte)(HAND.TWOP >> 28),
			ThreeOfAKind = (byte)(HAND.TRIP >> 28),
			Straight = (byte)(HAND.STRA >> 28),//0b0101
			Flush = (byte)(HAND.FLUS >> 28),
			FullHouse = (byte)(HAND.FULL >> 28),
			FourOfAKind = (byte)(HAND.QUAD >> 28),
			StraightFlush = (byte)(HAND.STRF >> 28),
			RoyalFlush = (byte)(HAND.ROYF >> 28)
		}



		public readonly struct Ranking : IEquatable<Ranking>, IComparable<Ranking>
		{
			private readonly uint _value;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Ranking(uint value) => _value = value;

			public Family Family => (Family)(_value >> 28);

			public int Y2 => (int)((_value >> 24) & 0xFF);
			public int Y1 => (int)((_value >> 20) & 0xFF);
			public Rank X4 => (Rank)((_value >> 16) & 0xF);
			public Rank X3 => (Rank)((_value >> 12) & 0xF);
			public Rank X2 => (Rank)((_value >>  8) & 0xF);
			public Rank X1 => (Rank)((_value >>  4) & 0xF);
			public Rank X0 => (Rank)((_value      ) & 0xF);

			public override string ToString() => $"{Family} ({X4.Symbol}{X3.Symbol}{X2.Symbol}{X1.Symbol}{X0.Symbol})";
			public override int GetHashCode() => base.GetHashCode();

			public bool Equals(Ranking other) => _value.Equals(other._value);
			public override bool Equals([NotNullWhen(true)] object obj) => obj is Ranking other && _value.Equals(other._value);
			public static bool operator ==(Ranking a, Ranking b) => a._value == b._value;
			public static bool operator !=(Ranking a, Ranking b) => a._value != b._value;

			public int CompareTo(Ranking other) => _value.CompareTo(other._value);
			public static bool operator <(Ranking a, Ranking b) => a._value < b._value;
			public static bool operator >(Ranking a, Ranking b) => a._value > b._value;
			public static bool operator <=(Ranking a, Ranking b) => a._value <= b._value;
			public static bool operator >=(Ranking a, Ranking b) => a._value >= b._value;


			public static readonly Ranking Zero;

			public static Ranking RoyalFlush
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new((uint)Hand.Family.RoyalFlush << 28);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="quadRank"></param>
			/// <param name="kickRank"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking FourOfAkind(Rank quadRank, Rank kickRank)
			{
				return new Ranking(((uint)Hand.Family.FourOfAKind << 28) | ((uint)quadRank << 4) | ((uint)kickRank));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking FullHouse(Rank α, Rank β)
			{
				return new Ranking(((uint)Hand.Family.FullHouse << 28) | ((uint)α << 4) | ((uint)β));
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="rank"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking Straight(Rank rank)
			{
				return new Ranking(((uint)Hand.Family.Straight << 28) | (uint)rank);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="ρ1"></param>
			/// <param name="ρ2"></param>
			/// <param name="ρ3"></param>
			/// <param name="ρ4"></param>
			/// <param name="ρ5"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking Flush(Rank ρ1, Rank ρ2, Rank ρ3, Rank ρ4, Rank ρ5)
			{
				var ρ = (uint)Hand.Family.Flush << 28;
				ρ |= (uint)ρ1 << 16;
				ρ |= (uint)ρ2 << 12;
				ρ |= (uint)ρ3 <<  8;
				ρ |= (uint)ρ4 <<  4;
				ρ |= (uint)ρ5      ;
				return new(ρ);
			}

			

			/// <summary>
			/// 
			/// </summary>
			/// <param name="suit"></param>
			/// <param name="rank"></param>
			/// <returns></returns>
			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking StraightFlush(Suit suit, Rank rank)
			{
				return new Ranking(((uint)Hand.Family.StraightFlush << 28) | ((uint)suit << 24) | (uint)rank);
			}




			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking ThreeOfAKind(Rank ρ1, Rank ρ2, Rank ρ3)
			{
				var ρ = (uint)Hand.Family.ThreeOfAKind << 28;
				ρ |= (uint)ρ1 << 8;
				ρ |= (uint)ρ2 << 4;
				ρ |= (uint)ρ3     ;
				return new(ρ);
			}

			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking TwoPair(Rank ρ1, Rank ρ2, Rank ρ3)
			{
				var ρ = (uint)Hand.Family.TwoPair << 28;
				ρ |= (uint)ρ1 << 8;
				ρ |= (uint)ρ2 << 4;
				ρ |= (uint)ρ3;
				return new(ρ);
			}

			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking OnePair(Rank ρ1, Rank ρ2, Rank ρ3, Rank ρ4)
			{
				var ρ = (uint)Hand.Family.OnePair << 28;
				ρ |= (uint)ρ1 << 12;
				ρ |= (uint)ρ2 <<  8;
				ρ |= (uint)ρ3 <<  4;
				ρ |= (uint)ρ4      ;
				return new(ρ);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="ρ1"></param>
			/// <param name="ρ2"></param>
			/// <param name="ρ3"></param>
			/// <param name="ρ4"></param>
			/// <param name="ρ5"></param>
			/// <returns></returns>
			[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranking HighCard(Rank ρ1, Rank ρ2, Rank ρ3, Rank ρ4, Rank ρ5)
			{
				var ρ = (uint)Hand.Family.HighCard << 28;
				ρ |= (uint)ρ1 << 16;
				ρ |= (uint)ρ2 << 12;
				ρ |= (uint)ρ3 <<  8;
				ρ |= (uint)ρ4 <<  4;
				ρ |= (uint)ρ5      ;
				return new(ρ);
			}			
		}

		public static bool TryEvaluate(CardSet source, out Hand.Ranking target)
		{
			if (source.Count is >= 5 and <= 7)
			{
				var (S, D, H, C) = source;


				var R = S | D | H | C;

				Rank α, β, γ, δ, ε;

				if (HAND.ROYAL_FLUSH.TryDetect(S, D, C, H))
					target = Hand.Ranking.RoyalFlush;

				if (HAND.STRAIGHT_FLUSH.TryDetect(S, D, C, H, out var straightFlushSuit, out α))
				{
					target = Hand.Ranking.StraightFlush(straightFlushSuit, α);
					return true;
				}
				if (HAND.FOUR_OF_A_KIND.TryDetect(S, D, C, H, out α, out β))
				{
					target = Hand.Ranking.FourOfAkind(α, β);
					return true;
				}

				if (HAND.FULL_HOUSE.TryDetect(S, D, C, H, out α, out β))
				{
					target = Hand.Ranking.FullHouse(α, β);
					return true;
				}
				if (HAND.FLUSH.TryDetect(S, D, C, H, out α, out β, out γ, out δ, out ε))
				{
					target = Hand.Ranking.Flush(α, β, γ, δ, ε);
					return true;
				}

				if (HAND.STRAIGHT.TryDetect(R, out α))
				{
					target = Hand.Ranking.Straight(α);
					return true;
				}	

				if (HAND.THREE_OF_A_KIND.TryDetect(S, D, C, H, out α, out β, out γ))
				{
					target = Hand.Ranking.ThreeOfAKind(α, β, γ);
					return true;
				}

				if (HAND.TWO_PAIR.TryDetect(S, D, C, H, out α, out β, out γ))
				{
					target = Hand.Ranking.TwoPair(α, β, γ);
					return true;
				}

				if (HAND.ONE_PAIR.TryDetect(S, D, C, H, out α, out β, out γ, out δ))
				{
					target = Hand.Ranking.OnePair(α, β, γ, δ);
					return true;
				}

				if (HAND.HIGH_CARD.TryDetect(R, out α, out β, out γ, out δ, out ε))
				{
					target = Hand.Ranking.HighCard(α, β, γ, δ, ε);
					return true;
				}
			}

			target = default;
			return false;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Hand.Ranking Evaluate(CardSet source)
		{
			if (source.Count is >= 5 and <= 7)
			{
				var (S, D, H, C) = source;


				var R = S | D | H | C;

				Rank α, β, γ, δ, ε;

				if (HAND.ROYAL_FLUSH.TryDetect(S, D, C, H))
					return Hand.Ranking.RoyalFlush;
				
				if (HAND.STRAIGHT_FLUSH.TryDetect(S, D, C, H, out var straightFlushSuit, out α))
					return Hand.Ranking.StraightFlush(straightFlushSuit, α);

				if (HAND.FOUR_OF_A_KIND.TryDetect(S, D, C, H, out α, out β))
					return Hand.Ranking.FourOfAkind(α, β);

				if (HAND.FULL_HOUSE.TryDetect(S, D, C, H, out α, out β))
					return Hand.Ranking.FullHouse(α, β);

				if (HAND.FLUSH.TryDetect(S, D, C, H, out α, out β, out γ, out δ, out ε))
					return Hand.Ranking.Flush(α, β, γ, δ, ε);

				if (HAND.STRAIGHT.TryDetect(R, out α))
					return Hand.Ranking.Straight(α);

				if (HAND.THREE_OF_A_KIND.TryDetect(S, D, C, H, out α, out β, out γ))
					return Hand.Ranking.ThreeOfAKind(α, β, γ);

				if (HAND.TWO_PAIR.TryDetect(S, D, C, H, out α, out β, out γ))
					return Hand.Ranking.TwoPair(α, β, γ);
				
				if (HAND.ONE_PAIR.TryDetect(S, D, C, H, out α, out β, out γ, out δ))
					return Hand.Ranking.OnePair(α, β, γ, δ);

				if (HAND.HIGH_CARD.TryDetect(R, out α, out β, out γ, out δ, out ε))
					return Hand.Ranking.HighCard(α, β, γ, δ, ε);
			}

			return default;
		}

		public static Hand.Ranking[] Evaluate(ReadOnlySpan<Hand> hands)
		{
			var ranks = new Hand.Ranking[hands.Length];

			int i = 0;
			foreach(Hand hand in hands)
			{
				ranks[i] = Evaluate(hand.Cards);
				i++;
			}
			return ranks;
		}
	}
}
