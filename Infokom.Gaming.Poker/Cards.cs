using Infokom.Numerics.Extensions;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Infokom.Gaming.Poker
{
	public enum Cards : ulong
	{
		Φ = 0,

		_2 = 0x_0004_0004_0004_0004ul,
		_3 = 0x_0008_0008_0008_0008ul,
		_4 = 0x_0010_0010_0010_0010ul,
		_5 = 0x_0020_0020_0020_0020ul,
		_6 = 0x_0040_0040_0040_0040ul,
		_7 = 0x_0080_0080_0080_0080ul,
		_8 = 0x_0100_0100_0100_0100ul,
		_9 = 0x_0200_0200_0200_0200ul,
		_T = 0x_0400_0400_0400_0400ul,
		_J = 0x_0800_0800_0800_0800ul,
		_Q = 0x_1000_1000_1000_1000ul,
		_K = 0x_2000_2000_2000_2000ul,
		_A = 0x_4000_4000_4000_4000ul,

		S_ = 0x7FFCul << (16 * 0),
		D_ = 0x7FFCul << (16 * 1),
		C_ = 0x7FFCul << (16 * 2),
		H_ = 0x7FFCul << (16 * 3),

		/*-----------|-------------|-------------|-------------|-------------|-------------|-------------|-------------|-------------|-------------|-------------|-------------|-------------|*/
		S2 = S_ & _2, S3 = S_ & _3, S4 = S_ & _4, S5 = S_ & _5, S6 = S_ & _6, S7 = S_ & _7, S8 = S_ & _8, S9 = S_ & _9, ST = S_ & _T, SJ = S_ & _J, SQ = S_ & _Q, SK = S_ & _K, SA = S_ & _A,
		D2 = D_ & _2, D3 = D_ & _3, D4 = D_ & _4, D5 = D_ & _5, D6 = D_ & _6, D7 = D_ & _7, D8 = D_ & _8, D9 = D_ & _9, DT = D_ & _T, DJ = D_ & _J, DQ = D_ & _Q, DK = D_ & _K, DA = D_ & _A,
		C2 = C_ & _2, C3 = C_ & _3, C4 = C_ & _4, C5 = C_ & _5, C6 = C_ & _6, C7 = C_ & _7, C8 = C_ & _8, C9 = C_ & _9, CT = C_ & _T, CJ = C_ & _J, CQ = C_ & _Q, CK = C_ & _K, CA = C_ & _A,
		H2 = H_ & _2, H3 = H_ & _3, H4 = H_ & _4, H5 = H_ & _5, H6 = H_ & _6, H7 = H_ & _7, H8 = H_ & _8, H9 = H_ & _9, HT = H_ & _T, HJ = H_ & _J, HQ = H_ & _Q, HK = H_ & _K, HA = H_ & _A,

		__ = S_ | D_ | C_ | H_,

		Ω = __
	}

	public struct CardsEnumerator : IEnumerator<Card>
	{
		private Cards _cards;
		private Card _current;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CardsEnumerator(Cards owner)
		{
			_cards = owner;
			_current = default;
		}

		public readonly Card Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _current;
		}

		readonly object IEnumerator.Current => this.Current;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_cards == 0)
			{
				return false;
			}

			_current = _cards.Top();
			_cards = _cards.Exclude(_current);

			return true;
		}
		void IEnumerator.Reset() => throw new NotSupportedException();

		readonly void IDisposable.Dispose() { }
	}

	[StructLayout(LayoutKind.Sequential)]
	public readonly struct CardsIterator : IEnumerable<Card>
	{
		private readonly ulong _source;

		public CardsIterator(Cards source) => _source = (ulong)source;

		public struct Enumerator : IEnumerator<Card>
		{
			private ulong _source;
			private sbyte _current;

			public Enumerator(CardsIterator owner)
			{
				_source = owner._source;
				_current = -1;
			}

			public readonly Card Current => (Card)_current;

			readonly object IEnumerator.Current => this.Current;

			public bool MoveNext()
			{
				if (_source == default)
					return false;

				_current = (sbyte)BitOperations.Log2(_source);
				_source &= ~(1ul << _current);
				return true;
			}

			void IEnumerator.Reset() => throw new NotSupportedException();

			readonly void IDisposable.Dispose() { }

		}

		public Enumerator GetEnumerator() => new(this);
		IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => this.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}

	[StructLayout(LayoutKind.Sequential)]
	public readonly struct CardsReverseIterator : IEnumerable<Card>
	{
		public readonly ulong _source;

		private CardsReverseIterator(Cards source) => _source = (ulong)source;

		public struct Enumerator : IEnumerator<Card>
		{
			private ulong _source;
			private sbyte _current;

			public Enumerator(CardsReverseIterator owner)
			{
				_source = owner._source;
				_current = -1;
			}

			public readonly Card Current => (Card)_current;

			readonly object IEnumerator.Current => this.Current;

			public bool MoveNext()
			{
				if (_source == default)
					return false;

				_current = (sbyte)BitOperations.Log2(_source);
				_source &= ~(1ul << _current);
				return true;
			}

			void IEnumerator.Reset() => throw new NotSupportedException();

			readonly void IDisposable.Dispose() { }

		}

		public Enumerator GetEnumerator() => new(this);
		IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => this.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}

	[StructLayout(LayoutKind.Explicit)]
	public readonly struct RankOrderedCardsIterator : IEnumerable<Card>
	{
		[FieldOffset(0)] private readonly Cards _owner;
		[FieldOffset(0)] private readonly Ranks _s;
		[FieldOffset(2)] private readonly Ranks _d;
		[FieldOffset(4)] private readonly Ranks _c;
		[FieldOffset(6)] private readonly Ranks _h;

		public RankOrderedCardsIterator(Cards owner) => _owner = owner;


		public struct Enumerator : IEnumerator<Card>
		{
			private RanksEnumerator _s;
			private RanksEnumerator _d;
			private RanksEnumerator _c;
			private RanksEnumerator _h;

			private Rank _rs;
			private Rank _rd;
			private Rank _rc;
			private Rank _rh;

			private bool _initialized;
			private Card _current;

			public Enumerator(RankOrderedCardsIterator owner)
			{
				_s = new(owner._s);
				_d = new(owner._d);
				_c = new(owner._c);
				_h = new(owner._h);

				_rs = _rd = _rc = _rh = (Rank)(-1);

				_initialized = false;
				_current = default;
			}

			public readonly Card Current => _current;

			readonly object IEnumerator.Current => Current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				// First call: get the first rank from every suit.
				if (!_initialized)
				{
					_initialized = true;

					_rs = _s.MoveNext() ? _s.Current : (Rank)(-1);
					_rd = _d.MoveNext() ? _d.Current : (Rank)(-1);
					_rc = _c.MoveNext() ? _c.Current : (Rank)(-1);
					_rh = _h.MoveNext() ? _h.Current : (Rank)(-1);
				}

				// Find the smallest non-exhausted rank.
				var rank = _rs;

				if (_rd >= 0 && (rank < 0 || _rd < rank))
					rank = _rd;

				if (_rc >= 0 && (rank < 0 || _rc < rank))
					rank = _rc;

				if (_rh >= 0 && (rank < 0 || _rh < rank))
					rank = _rh;

				if (rank < 0)
					return false;

				// Fixed suit order: S, D, C, H.
				if (_rs == rank)
				{
					_current = (Rank)rank * Suit.Spade;
					_rs = _s.MoveNext() ? _s.Current : (Rank)(-1);
					return true;
				}

				if (_rd == rank)
				{
					_current = (Rank)rank * Suit.Diamond;
					_rd = _d.MoveNext() ? _d.Current : (Rank)(-1);
					return true;
				}

				if (_rc == rank)
				{
					_current = (Rank)rank * Suit.Club;
					_rc = _c.MoveNext() ? _c.Current : (Rank)(-1);
					return true;
				}

				_current = (Rank)rank * Suit.Heart;
				_rh = _h.MoveNext() ? _h.Current : (Rank)(-1);
				return true;
			}

			void IEnumerator.Reset() => throw new NotSupportedException();

			readonly void IDisposable.Dispose() { }
		}

		public Enumerator GetEnumerator() => new(this);

		IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public DescendingRankOrderedCardsIterator Reverse() => _owner.OrderByRankDescending();
	}

	[StructLayout(LayoutKind.Explicit)]
	public readonly struct DescendingRankOrderedCardsIterator : IEnumerable<Card>
	{
		[FieldOffset(0)] private readonly Cards _source;
		[FieldOffset(0)] private readonly Ranks _s;
		[FieldOffset(2)] private readonly Ranks _d;
		[FieldOffset(4)] private readonly Ranks _c;
		[FieldOffset(6)] private readonly Ranks _h;

		public DescendingRankOrderedCardsIterator(Cards source)
		{
			_source = source;
		}

		public struct Enumerator : IEnumerator<Card>
		{
			private DescendingRanksEnumerator _s, _d, _c, _h;
			private Rank _rs, _rd, _rc, _rh;
			private bool _initialized;
			private Card _current;

			public Enumerator(DescendingRankOrderedCardsIterator owner)
			{
				_h = new(owner._h);
				_c = new(owner._c);
				_d = new(owner._d);
				_s = new(owner._s);

				_rs = _rd = _rc = _rh = (Rank)(-1);

				_initialized = false;
				_current = default;
			}

			public readonly Card Current => _current;

			readonly object IEnumerator.Current => Current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				// First call: get the first rank from every suit.
				if (!_initialized)
				{
					_initialized = true;

					_rh = _h.MoveNext() ? _h.Current : (Rank)(-1);
					_rc = _c.MoveNext() ? _c.Current : (Rank)(-1);
					_rd = _d.MoveNext() ? _d.Current : (Rank)(-1);
					_rs = _s.MoveNext() ? _s.Current : (Rank)(-1);
				}

				// Find the smallest non-exhausted rank.
				var rank = _rs;

				if (_rd >= 0 && (rank < 0 || _rd > rank))
					rank = _rd;

				if (_rc >= 0 && (rank < 0 || _rc > rank))
					rank = _rc;

				if (_rh >= 0 && (rank < 0 || _rh > rank))
					rank = _rh;

				if (rank < 0)
					return false;

				// Fixed suit order: S, D, C, H.
				if (_rs == rank)
				{
					_current = (Rank)rank * Suit.Spade;
					_rs = _s.MoveNext() ? _s.Current : (Rank)(-1);
					return true;
				}

				if (_rd == rank)
				{
					_current = (Rank)rank * Suit.Diamond;
					_rd = _d.MoveNext() ? _d.Current : (Rank)(-1);
					return true;
				}

				if (_rc == rank)
				{
					_current = (Rank)rank * Suit.Club;
					_rc = _c.MoveNext() ? _c.Current : (Rank)(-1);
					return true;
				}

				_current = (Rank)rank * Suit.Heart;
				_rh = _h.MoveNext() ? _h.Current : (Rank)(-1);
				return true;
			}

			void IEnumerator.Reset() => throw new NotSupportedException();

			readonly void IDisposable.Dispose() { }
		}

		public Enumerator GetEnumerator() => new(this);
		IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		public RankOrderedCardsIterator Reverse() => _source.OrderByRank();
	}



	public static class CardsExtensions
	{
		extension<T>(T) where T : unmanaged, IShiftOperators<T, int, T>
		{
			public static T operator <<(T x, Card y) => x << (int)y;
			public static T operator >>(T x, Card y) => x >> (int)y;
		}

		extension(Ranks)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards operator *(Ranks r, Suit s) => (Cards)((ushort)r << (BitOperations.TrailingZeroCount((byte)s) * sizeof(Ranks)));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards operator *(Suit suit, Ranks ranks) => ranks * suit;
		}









		extension(Cards source)
		{

			/// <summary>
			/// Enumerates the cards contained in <paramref name="source"/> by suit in the order of Spades, Diamonds, Clubs, Hearts and by rank in descending order of their card values (from Two to Ace).
			/// </summary>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public CardsIterator.Enumerator GetEnumerator() => Unsafe.As<Cards, CardsIterator>(ref source).GetEnumerator();


			/// <summary>
			/// Enumerates the cards contained in <paramref name="source"/> in rank ascending order of their card values (from Two to Ace) then by suit in the order of Spades, Diamonds, Clubs, Hearts.
			/// </summary>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RankOrderedCardsIterator OrderByRank() => new(source);


			/// <summary>
			/// Enumerates the cards contained in <paramref name="source"/> in rank descending order of their card values (from Ace to Two) then by suit in the order of Spades, Diamonds, Clubs, Hearts.
			/// </summary>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DescendingRankOrderedCardsIterator OrderByRankDescending() => new(source);

			/// <summary>
			/// Returns the top card of the latest suit present in  <paramref name="source"/> or -1 if <paramref name="source"/> is empty.
			/// </summary>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Card Top() => (Card)(source.IsEmpty ? -1 : BitOperations.Log2((ulong)source));



		}





		extension(Cards)
		{
			/// <summary>
			/// Computes the complement of a set of cards with respect to the universal set of cards; i.e. the set of cards that are not present in the set.
			/// </summary>
			/// <param name="A"></param>
			/// <returns><see cref="Cards.Ω"/> \ <paramref name="A"/></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Complement(Cards A) => Cards.Ω & ~A;

			/// <summary>
			/// Computes the union of two sets of cards; i.e. the set of cards that are present in either of the sets.
			/// </summary>
			/// <param name="A"></param>
			/// <param name="B"></param>
			/// <returns><paramref name="A"/> ∪ <paramref name="B"/></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Union(Cards A, Cards B) => A | B;

			/// <summary>
			/// Computes the intersection of two sets of cards; i.e. the set of cards that are present in both.
			/// </summary>
			/// <param name="A"></param>
			/// <param name="B"></param>
			/// <returns><paramref name="A"/> ∩ <paramref name="B"/></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Intersection(Cards A, Cards B) => A & B;

			/// <summary>
			/// Computes the difference of a set with respect to another set; i.e. the set of elements in the first set that are not in the other set.
			/// </summary>
			/// <param name="A"></param>
			/// <param name="B"></param>
			/// <returns><paramref name="A"/> \ <paramref name="B"/></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards Difference(Cards A, Cards B) => A & ~B;

			/// <summary>
			/// Computes the symmetric difference of two sets of cards; i.e. the set of elements that are in either of the sets but not in their intersection.
			/// </summary>
			/// <param name="A"></param>
			/// <param name="B"></param>
			/// <returns><paramref name="A"/> △ <paramref name="B"/></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cards SymmetricDifference(Cards A, Cards B) => A ^ B;

			public static Cards operator &(Cards x, ulong y) => (Cards)((ulong)x & y);
			public static Cards operator |(Cards x, ulong y) => (Cards)((ulong)x | y) & Cards.__;
			public static Cards operator ^(Cards x, ulong y) => (Cards)((ulong)x ^ y) & Cards.__;
			public static ulong operator >>(Cards x, int y) => (ulong)x >> y;
			public static ulong operator <<(Cards x, int y) => (ulong)x >> y;



			public static Cards Select(Rank rank) => (Cards)(
				(1ul << (rank)) |
				(1ul << (rank + 16)) |
				(1ul << (rank + 32)) |
				(1ul << (rank + 48)));


			public static Cards Select(Ranks ranks) => (Cards)(
				((ulong)ranks) |
				((ulong)ranks << 16) |
				((ulong)ranks << 32) |
				((ulong)ranks << 48));



			public static Cards Select(Card x1) => (Cards)(1ul << x1);
			public static Cards Select(Card x1, Card x2) => (Cards)((1ul << x1) | (1ul << x2));
			public static Cards Select(Card x1, Card x2, Card x3) => (Cards)((1ul << x1) | (1ul << x2) | (1ul << x3));
			public static Cards Select(Card x1, Card x2, Card x3, Card x4) => (Cards)((1ul << x1) | (1ul << x2) | (1ul << x3) | (1ul << x4));
			public static Cards Select(Card x1, Card x2, Card x3, Card x4, Card x5) => (Cards)((1ul << x1) | (1ul << x2) | (1ul << x3) | (1ul << x4) | (1ul << x5));
			public static Cards Select(params ReadOnlySpan<Card> elements)
			{
				var collection = Cards.Φ;

				foreach (var x in elements)
				{
					collection |= 1ul << x;
				}

				return collection;
			}



			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Ranks operator /(Cards cards, Suit s) => (Ranks)((ulong)cards >> (BitOperations.TrailingZeroCount((byte)s) * sizeof(Ranks)));

		}

		extension(Cards source)
		{
			/// <summary>
			/// $X \equiv \Phi$
			/// </summary>
			public bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (source & Cards.__) == Cards.Φ;
			}

			/// <summary>
			/// $|X|$
			/// </summary>
			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount((ulong)source);
			}


			public Cards Spades
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source & Cards.S_;
			}

			public Cards Diamonds
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source & Cards.D_;
			}

			public Cards Clubs
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source & Cards.C_;
			}

			public Cards Hearts
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source & Cards.H_;
			}

			/// <summary>
			/// Extension of the <paramref name="source"/> including another <paramref name="member"/>.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Cards Include(Card member) => source | (1ul << member);

			/// <summary>
			/// Restriction of the <paramref name="source"/> excluding a possible <paramref name="member"/>.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Cards Exclude(Card member) => source & (~(1ul << member));

			/// <summary>
			/// Ownership predicate of the <paramref name="source"/> for a <paramref name="member"/>.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool HasMember(Card member) => (source & (1ul << member)) != 0;

			/// <summary><paramref name="source"/> ⊆ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="source"/></summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsSubsetOf(Cards target) => (source & target) == source;

			/// <summary><paramref name="source"/> ⊂ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="source"/> and <paramref name="source"/> ≠ <paramref name="target"/></summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsProperSubsetOf(Cards target) => (source & target) == source && source != target;

			/// <summary><paramref name="source"/> ⊇ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="target"/></summary>			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsSupersetOf(Cards target) => (source & target) == target;

			/// <summary><paramref name="source"/> ⊃ <paramref name="target"/> : <paramref name="source"/> ∩ <paramref name="target"/> = <paramref name="target"/> and <paramref name="source"/> ≠ <paramref name="target"/></summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsProperSupersetOf(Cards target) => (source & target) == target && source != target;

			/// <summary>
			/// this ∩ other ≠ ∅
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Overlaps(Cards other) => (source & other) != 0;

			/// <summary>
			/// $X \to \langle \{c|\sigma(c)=spade\},\{c|\sigma(c)=diam\},\{c|\sigma(c)=club\},\{c|\sigma(c)=heart\} \rangle$
			/// </summary>
			/// <param name="s"></param>
			/// <param name="d"></param>
			/// <param name="c"></param>
			/// <param name="h"></param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Deconstruct(out Ranks s, out Ranks d, out Ranks c, out Ranks h)
			{
				s = (Ranks)((source & Cards.S_) >> 00);
				d = (Ranks)((source & Cards.D_) >> 16);
				c = (Ranks)((source & Cards.C_) >> 32);
				h = (Ranks)((source & Cards.H_) >> 48);
			}

			public Ranks Ranks
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Ranks)((source >> 00) | (source >> 16) | (source >> 32) | (source >> 48)) & Ranks.Ω;
			}


			public int CopyTo(Span<Card> target)
			{
				var n = source.Count;
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, n, nameof(target));

				int i = 0;
				foreach (var card in source)
				{
					if (i >= n) break;
					target[i++] = card;
				}
				return i;
			}

			public int CopyTo(Span<Card> target, int n)
			{
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, n, nameof(target));

				int i = 0; n = Math.Min(n, source.Count);
				foreach (var card in source)
				{
					if (i >= n) break;
					target[i++] = card;
				}
				return i;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public CardsCombinations.Iterator Combinations(int k) => new(source, k);
		}
	}


	public static class CardsCombinations
	{
		public readonly struct Iterator : IReadOnlyCollection<Cards>
		{

			private readonly ulong _source;
			private readonly int _k;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Iterator(Cards source, int k) => (_source, _k) = ((ulong)source, k);

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Math.Choose(BitOperations.PopCount(_source), _k);
			}

			public struct Enumerator : IEnumerator<Cards>
			{
				private readonly ulong _source;
				private readonly int _k;
				private ulong _current, _bmiIdx, _bmiMax, _submask;
				private bool _zeroCase;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(Iterator owner)
				{
					(_source, _k) = (owner._source, owner._k);
					int pop = BitOperations.PopCount(_source);
					if (_k < 0 || _k > pop) return;
					_zeroCase = (_k == 0);

					if (Bmi2.X64.IsSupported)
					{
						_bmiIdx = (1UL << _k) - 1UL;
						_bmiMax = _bmiIdx << (pop - _k);
					}
					else _submask = _source;
				}

				public readonly Cards Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (Cards)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext()
				{
					if (Bmi2.X64.IsSupported)
					{
						if (_bmiIdx == 0) return false;
						_current = Bmi2.X64.ParallelBitDeposit(_bmiIdx, _source);
						if (_bmiIdx == _bmiMax) _bmiIdx = 0;
						else
						{
							ulong c = _bmiIdx & (ulong)-(long)_bmiIdx;
							ulong r = _bmiIdx + c;
							_bmiIdx = (((r ^ _bmiIdx) >> 2) / c) | r;
						}
						return true;
					}

					while (_submask > 0)
					{
						ulong pot = _submask;
						_submask = (_submask - 1) & _source;
						if (BitOperations.PopCount(pot) == _k) { _current = pot; return true; }
					}

					if (_zeroCase) { _zeroCase = false; _current = 0; return true; }
					return false;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Reset()
				{
					int pop = BitOperations.PopCount(_source);
					if (_k < 0 || _k > pop) return;
					_zeroCase = (_k == 0);

					if (Bmi2.X64.IsSupported)
					{
						_bmiIdx = (1UL << _k) - 1UL;
						_bmiMax = _bmiIdx << (pop - _k);
					}
					else _submask = _source;
				}
				readonly void IDisposable.Dispose() { }
			}
			public Enumerator GetEnumerator() => new(this);
			IEnumerator<Cards> IEnumerable<Cards>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
		}
	}
}
