using Infokom.Gaming.Poker;
using Infokom.Numerics.Extensions;

using System.Collections;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Infokom.Gaming.Poker
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public readonly partial struct CardSet : IEquatable<CardSet>, IEqualityOperators<CardSet, CardSet, bool>, IEquatable<ulong>, IEqualityOperators<CardSet, ulong, bool>
	{
		private const ulong Φ_BITS = 0b0000000000000000_0000000000000000_0000000000000000_0000000000000000ul;
		private const ulong Ω_BITS = 0b0111111111111100_0111111111111100_0111111111111100_0111111111111100ul;
		private const ulong S_BITS = 0b0000000000000000_0000000000000000_0000000000000000_0111111111111100ul;
		private const ulong D_BITS = 0b0000000000000000_0000000000000000_0111111111111100_0000000000000000ul;
		private const ulong C_BITS = 0b0000000000000000_0111111111111100_0000000000000000_0000000000000000ul;
		private const ulong H_BITS = 0b0111111111111100_0000000000000000_0000000000000000_0000000000000000ul;

		[FieldOffset(0)] private readonly ulong _bits;

		[FieldOffset(0)] private readonly ushort _s;
		[FieldOffset(2)] private readonly ushort _d;
		[FieldOffset(4)] private readonly ushort _c;
		[FieldOffset(6)] private readonly ushort _h;

		/// <summary>
		/// Ranks of spades.
		/// </summary>
		[FieldOffset(0)] public readonly RankSet S;

		/// <summary>
		/// Ranks of diamonds.
		/// </summary>
		[FieldOffset(2)] public readonly RankSet D;

		/// <summary>
		/// Ranks of clubs.
		/// </summary>
		[FieldOffset(4)] public readonly RankSet C;

		/// <summary>
		/// Ranks of hearts.
		/// </summary>
		[FieldOffset(6)] public readonly RankSet H;


		private CardSet(ulong bits) => _bits = bits;


		/// <summary>
		/// $X \equiv \Phi$
		/// </summary>
		public bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (_bits & Ω_BITS) == 0;
		}

		/// <summary>
		/// $|X|$
		/// </summary>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => BitOperations.PopCount(_bits & Ω_BITS);
		}

		/// <summary>
		/// Ranks of all cards in the set, regardless of suit.
		/// </summary>
		public RankSet Ranks
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this.S | this.D | this.C | this.H;
		}

		/// <summary>
		/// Deconstructs the card set into its constituent rank sets for each suit.
		/// </summary>
		/// <param name="s">Ranks of spades.</param>
		/// <param name="d">Ranks of diamonds.</param>
		/// <param name="c">Ranks of clubs.</param>
		/// <param name="h">Ranks of hearts.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Deconstruct(out RankSet s, out RankSet d, out RankSet c, out RankSet h) => (s, d, c, h) = (this.S, this.D, this.C, this.H);



		public CardSet Spades
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this & S_;
		}

		public CardSet Diamonds
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this & D_;
		}

		public CardSet Clubs
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this & C_;
		}

		public CardSet Hearts
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this & H_;
		}




		








		public bool Equals(CardSet other) => _bits == other._bits;
		public bool Equals(ulong other) => _bits == other;
		public override bool Equals(object obj) => obj is CardSet other && Equals(other);
		public override int GetHashCode() => _bits.GetHashCode();


		#region Operators
		public static bool operator ==(CardSet a, CardSet b) => a._bits == b._bits;
		public static bool operator !=(CardSet a, CardSet b) => a._bits != b._bits;
		public static bool operator ==(CardSet a, ulong b) => a._bits == b;
		public static bool operator !=(CardSet a, ulong b) => a._bits != b;
		public static bool operator ==(ulong a, CardSet b) => a == b._bits;
		public static bool operator !=(ulong a, CardSet b) => a != b._bits;

		public static CardSet operator ~(CardSet a) => new((~a._bits) & (ulong)Ω);
		public static CardSet operator &(CardSet a, CardSet b) => new(a._bits & b._bits);
		public static CardSet operator |(CardSet a, CardSet b) => new((a._bits | b._bits) & Ω_BITS);
		public static CardSet operator ^(CardSet a, CardSet b) => new((a._bits ^ b._bits) & Ω_BITS);
		public static CardSet operator &(CardSet x, ulong y) => new(x._bits & y);
		public static CardSet operator |(CardSet x, ulong y) => new CardSet(x._bits | y) & Ω;
		public static CardSet operator ^(CardSet x, ulong y) => new CardSet(x._bits ^ y) & Ω;

		public static ulong operator >>(CardSet x, int y) => x._bits >> y;
		public static ulong operator <<(CardSet x, int y) => x._bits << y;

		public static implicit operator ulong(CardSet x) => x._bits;
		public static explicit operator CardSet(ulong x) => new(x & Ω._bits);
		public static explicit operator checked CardSet(ulong x) => (x & ~Ω._bits) == 0 ? new(x) : throw new OverflowException();
		#endregion

		#region Predefined Sets
		public static readonly CardSet Φ = new(0b0000000000000000_0000000000000000_0000000000000000_0000000000000000ul);
		public static readonly CardSet Ω = new(0b0111111111111100_0111111111111100_0111111111111100_0111111111111100ul);
		private static readonly ulong __Ω = 0x7FFC7FFC7FFC7FFCul;

		public static readonly CardSet _2 = new(0b0000000000000100_0000000000000100_0000000000000100_0000000000000100ul);
		public static readonly CardSet _3 = new(0b0000000000001000_0000000000001000_0000000000001000_0000000000001000ul);
		public static readonly CardSet _4 = new(0b0000000000010000_0000000000010000_0000000000010000_0000000000010000ul);
		public static readonly CardSet _5 = new(0b0000000000100000_0000000000100000_0000000000100000_0000000000100000ul);
		public static readonly CardSet _6 = new(0b0000000001000000_0000000001000000_0000000001000000_0000000001000000ul);
		public static readonly CardSet _7 = new(0b0000000010000000_0000000010000000_0000000010000000_0000000010000000ul);
		public static readonly CardSet _8 = new(0b0000000100000000_0000000100000000_0000000100000000_0000000100000000ul);
		public static readonly CardSet _9 = new(0b0000001000000000_0000001000000000_0000001000000000_0000001000000000ul);
		public static readonly CardSet _T = new(0b0000010000000000_0000010000000000_0000010000000000_0000010000000000ul);
		public static readonly CardSet _J = new(0b0000100000000000_0000100000000000_0000100000000000_0000100000000000ul);
		public static readonly CardSet _Q = new(0b0001000000000000_0001000000000000_0001000000000000_0001000000000000ul);
		public static readonly CardSet _K = new(0b0010000000000000_0010000000000000_0010000000000000_0010000000000000ul);
		public static readonly CardSet _A = new(0b0100000000000000_0100000000000000_0100000000000000_0100000000000000ul);

		public static readonly CardSet S_ = new(0b0000000000000000_0000000000000000_0000000000000000_0111111111111100ul);
		public static readonly CardSet D_ = new(0b0000000000000000_0000000000000000_0111111111111100_0000000000000000ul);
		public static readonly CardSet C_ = new(0b0000000000000000_0111111111111100_0000000000000000_0000000000000000ul);
		public static readonly CardSet H_ = new(0b0111111111111100_0000000000000000_0000000000000000_0000000000000000ul);

		public static readonly CardSet S2 = new((ulong)S_ & (ulong)_2);
		public static readonly CardSet S3 = new((ulong)S_ & (ulong)_3);
		public static readonly CardSet SA = new((ulong)S_ & (ulong)_A);

		public static readonly CardSet D2 = new((ulong)D_ & (ulong)_2);
		public static readonly CardSet DA = new((ulong)D_ & (ulong)_A);

		public static readonly CardSet C2 = new((ulong)C_ & (ulong)_2);
		public static readonly CardSet CA = new((ulong)C_ & (ulong)_A);

		public static readonly CardSet H2 = new((ulong)H_ & (ulong)_2);
		public static readonly CardSet HA = new((ulong)H_ & (ulong)_A);
		#endregion

		#region Set Operations

		/// <summary>
		/// Extension of the <paramref name="source"/> including another <paramref name="member"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CardSet Include(Card member) => this | CardSet.Select(member);

		/// <summary>
		/// Restriction of the <paramref name="source"/> excluding a possible <paramref name="member"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CardSet Exclude(Card member) => this & (~CardSet.Select(member));

		/// <summary>
		/// Ownership predicate of the <paramref name="source"/> for a <paramref name="member"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasMember(Card member) => (this._bits & ((CardSet)(1ul << member))) != 0;

		/// <summary><paramref name="source"/> ⊆ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="source"/></summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsSubsetOf(CardSet target) => (this._bits & target._bits) == this._bits;

		/// <summary><paramref name="source"/> ⊂ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="source"/> and <paramref name="source"/> ≠ <paramref name="target"/></summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsProperSubsetOf(CardSet target) => (this._bits & target._bits) == this._bits && this._bits != target._bits;

		/// <summary><paramref name="source"/> ⊇ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="target"/></summary>			
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsSupersetOf(CardSet target) => (this._bits & target._bits) == target._bits;

		/// <summary><paramref name="source"/> ⊃ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="target"/> and <paramref name="source"/> ≠ <paramref name="target"/></summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsProperSupersetOf(CardSet target) => (this._bits & target._bits) == target._bits && this._bits != target._bits;

		/// <summary>
		/// this ∩ other ≠ ∅
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Overlaps(CardSet other) => (this._bits & other._bits) != 0;

		/// <summary>
		/// Computes the complement of a set of cards with respect to the universal set of cards; i.e. the set of cards that are not present in the set.
		/// </summary>
		/// <param name="A"></param>
		/// <returns><see cref="CardSet.Ω"/> \ <paramref name="A"/></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSet Complement(CardSet A) => Ω & ~A;

		/// <summary>
		/// Computes the union of two sets of cards; i.e. the set of cards that are present in either of the sets.
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns><paramref name="A"/> ∪ <paramref name="B"/></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSet Union(CardSet A, CardSet B) => A | B;

		/// <summary>
		/// Computes the intersection of two sets of cards; i.e. the set of cards that are present in both.
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns><paramref name="A"/> ∩ <paramref name="B"/></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSet Intersection(CardSet A, CardSet B) => A & B;

		/// <summary>
		/// Computes the difference of a set with respect to another set; i.e. the set of elements in the first set that are not in the other set.
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns><paramref name="A"/> \ <paramref name="B"/></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSet Difference(CardSet A, CardSet B) => A & ~B;

		/// <summary>
		/// Computes the symmetric difference of two sets of cards; i.e. the set of elements that are in either of the sets but not in their intersection.
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns><paramref name="A"/> △ <paramref name="B"/></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSet SymmetricDifference(CardSet A, CardSet B) => A ^ B;

		#endregion

		#region Set Creators

		/// <summary>
		/// All cards of a given rank, regardless of suit.
		/// </summary>
		/// <param name="rank"></param>
		/// <returns>{ <paramref name="rank"/> } × { <see cref="Suit.Spade"/>, <see cref="Suit.Diamond"/>, <see cref="Suit.Club"/>, <see cref="Suit.Heart"/> }</returns>
		public static CardSet Select(Rank rank) => (CardSet)(
			(1ul << (rank)) |
			(1ul << (rank + 16)) |
			(1ul << (rank + 32)) |
			(1ul << (rank + 48)));

		/// <summary>
		/// All cards of a given set of ranks, regardless of suit.
		/// </summary>
		/// <param name="ranks"></param>
		/// <returns><paramref name="ranks"/> × { <see cref="Suit.Spade"/>, <see cref="Suit.Diamond"/>, <see cref="Suit.Club"/>, <see cref="Suit.Heart"/> }</returns>
		public static CardSet Select(RankSet ranks) => (CardSet)(
			((ulong)ranks) |
			((ulong)ranks << 16) |
			((ulong)ranks << 32) |
			((ulong)ranks << 48));


		/// <summary>
		/// Creates a set containing a single card.
		/// </summary>
		/// <param name="x1">The card.</param>
		/// <returns>{ <paramref name="x1"/> }</returns>
		public static CardSet Select(Card x1) => (CardSet)(1ul << x1);

		/// <summary>
		/// Creates a set containing two cards.
		/// </summary>
		/// <param name="x1">The first card.</param>
		/// <param name="x2">The second card.</param>
		/// <returns>{ <paramref name="x1"/>, <paramref name="x2"/> }</returns>
		public static CardSet Select(Card x1, Card x2) => (CardSet)((1ul << x1) | (1ul << x2));

		/// <summary>
		/// Creates a set containing three cards.
		/// </summary>
		/// <param name="x1">The first card.</param>
		/// <param name="x2">The second card.</param>
		/// <param name="x3">The third card.</param>
		/// <returns>{ <paramref name="x1"/>, <paramref name="x2"/>, <paramref name="x3"/> }</returns>
		public static CardSet Select(Card x1, Card x2, Card x3) => (CardSet)((1ul << x1) | (1ul << x2) | (1ul << x3));

		/// <summary>
		/// Creates a set containing four cards.
		/// </summary>
		/// <param name="x1">The first card.</param>
		/// <param name="x2">The second card.</param>
		/// <param name="x3">The third card.</param>
		/// <param name="x4">The fourth card.</param>
		/// <returns>{ <paramref name="x1"/>, <paramref name="x2"/>, <paramref name="x3"/>, <paramref name="x4"/> }</returns>
		public static CardSet Select(Card x1, Card x2, Card x3, Card x4) => (CardSet)((1ul << x1) | (1ul << x2) | (1ul << x3) | (1ul << x4));

		/// <summary>
		/// Creates a set containing five cards.
		/// </summary>
		/// <param name="x1">The first card.</param>
		/// <param name="x2">The second card.</param>
		/// <param name="x3">The third card.</param>
		/// <param name="x4">The fourth card.</param>
		/// <param name="x5">The fifth card.</param>
		/// <returns>{ <paramref name="x1"/>, <paramref name="x2"/>, <paramref name="x3"/>, <paramref name="x4"/>, <paramref name="x5"/> }</returns>
		public static CardSet Select(Card x1, Card x2, Card x3, Card x4, Card x5) => (CardSet)((1ul << x1) | (1ul << x2) | (1ul << x3) | (1ul << x4) | (1ul << x5));

		/// <summary>
		/// Creates a set containing an arbitrary number of cards.
		/// </summary>
		/// <param name="elements">The cards.</param>
		/// <returns>{ x | x ∈ <paramref name="elements"/> }</returns>
		public static CardSet Select(params ReadOnlySpan<Card> elements)
		{
			var collection = Φ;

			foreach (var x in elements)
			{
				collection |= (CardSet)(1ul << x);
			}

			return collection;
		}


		public static CardSet Create(Func<Rank, Suit, bool> source)
		{
			var collection = Φ;
			foreach (var r in RankSet.Ω.Order())
			{
				foreach (var s in SuitSet.Ω.Order())
				{
					var c = Card.Of(r, s);
					if (source(r, s))
						collection |= (CardSet)(1ul << c);
				}
			}
			return collection;
		}

		public static CardSet Create(Func<Card, bool> source)
		{
			var collection = Φ;
			foreach (var r in RankSet.Ω.Order())
			{
				foreach (var s in SuitSet.Ω.Order())
				{
					var c = Card.Of(r, s);
					if (source(c))
						collection |= (CardSet)(1ul << c);
				}
			}
			return collection;
		}
		#endregion


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RankSet operator /(CardSet cards, Suit s) => (RankSet)((ulong)cards >> (BitOperations.TrailingZeroCount((byte)s) * sizeof(RankSet)));








		
		
	}






}
