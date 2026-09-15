using Infokom.Numerics.Extensions;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Infokom.Gaming.Poker
{




	public readonly partial struct CardSet
	{
		[StructLayout(LayoutKind.Explicit, Size = 8)]
		public readonly struct Iterator : IReadOnlyCollection<Card>
		{
			[FieldOffset(0)] public readonly CardSet Source;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Iterator(CardSet source) => Source = source;

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => this.Source.Count;
			}


			public struct Enumerator : IEnumerator<Card>
			{
				private ulong _upcoming;
				private sbyte _current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(Iterator owner)
				{
					_upcoming = owner.Source;
					_current = -1;
				}

				public readonly Card Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (Card)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext()
				{
					if (_upcoming == default)
						return false;

					_current = (sbyte)BitOperations.TrailingZeroCount(_upcoming);
					_upcoming &= ~(1ul << _current);
					return true;
				}

				void IEnumerator.Reset() => throw new NotSupportedException();

				readonly void IDisposable.Dispose() { }
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator GetEnumerator() => new(this);
			IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			/// <summary>
			/// Get a collection by reversing the order of this.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ReverseIterator Reverse() => new(this.Source);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RankForwardIterator ByRank() => new(this.Source);



			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CopyTo(Span<Card> target)
			{
				var n = this.Count;
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, n, nameof(target));

				int i = 0;

				foreach (var card in this)
				{
					target[i++] = card;
				}

				return i;
			}
		}


		/// <summary>
		/// Get a collection ordered suitwise (Spades, Diamonds, Clubs, Hearts) and for each suit by rankwise ascending.
		/// </summary>
		public Iterator Order() => new(this);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ShuffleIterator Shuffle() => new(this);



		[StructLayout(LayoutKind.Explicit, Size = 8)]
		public readonly struct ReverseIterator : IReadOnlyCollection<Card>
		{
			[FieldOffset(0)] public readonly CardSet Source;

			public ReverseIterator(CardSet source) => Source = source;

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => this.Source.Count;
			}

			public struct Enumerator : IEnumerator<Card>
			{
				private ulong _source;
				private sbyte _current;

				public Enumerator(ReverseIterator owner)
				{
					_source = owner.Source;
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





			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CopyTo(Span<Card> target)
			{
				var n = this.Count;
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, n, nameof(target));

				int i = 0;

				foreach (var card in this)
				{
					target[i++] = card;
				}

				return i;
			}
		}



		public readonly struct ShuffleIterator : IReadOnlyCollection<Card>
		{
			public readonly CardSet Source;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ShuffleIterator(CardSet source) => Source = source;



			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => this.Source.Count;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct Enumerator : IEnumerator<Card>
			{
				private ulong _upcoming;
				private sbyte _current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(CardSet source)
				{
					_upcoming = source._bits;
					_current = -1;
				}

				public readonly Card Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (Card)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext() => _upcoming.BitScanRandom(out _current, out _upcoming);

				void IEnumerator.Reset() => throw new NotSupportedException();

				readonly void IDisposable.Dispose() { }
			}

			public Enumerator GetEnumerator() => new(this.Source);

			IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CopyTo(Span<Card> target)
			{
				var n = this.Count;
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, n, nameof(target));

				int i = 0;

				foreach (var card in this)
				{
					target[i++] = card;
				}

				return i;
			}

			
		}





		[StructLayout(LayoutKind.Explicit, Size = 8)]
		public readonly struct RankForwardIterator : IReadOnlyCollection<Card>
		{
			[FieldOffset(0)] public readonly CardSet Source;

			[FieldOffset(0)] public readonly RankSet.ForwardIterator S;
			[FieldOffset(2)] public readonly RankSet.ForwardIterator D;
			[FieldOffset(4)] public readonly RankSet.ForwardIterator C;
			[FieldOffset(6)] public readonly RankSet.ForwardIterator H;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RankForwardIterator(CardSet source) => Source = source;


			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => this.Source.Count;
			}



			public Card First()
			{
				if (this.Source != default)
				{
					var rank = Source.Ranks.Lowest();

					var (s, d, c, h) = this.Source;

					if (s.Lowest () == rank)
						return rank * Suit.Spade;
					if (d.Lowest() == rank)
						return rank * Suit.Diamond;
					if (c.Lowest() == rank)
						return rank * Suit.Club;
					if (h.Lowest() == rank)
						return rank * Suit.Heart;
				}

				throw new InvalidOperationException("The collection is empty.");
			}

			public Card FirstOrDefault()
			{
				if (this.Source != default)
				{
					var rank = Source.Ranks.Lowest();

					var (s, d, c, h) = this.Source;

					if (s.Lowest() == rank)
						return rank * Suit.Spade;
					if (d.Lowest() == rank)
						return rank * Suit.Diamond;
					if (c.Lowest() == rank)
						return rank * Suit.Club;
					if (h.Lowest() == rank)
						return rank * Suit.Heart;
				}

				return default;
			}

			public struct Enumerator : IEnumerator<Card>
			{
				private RankSet.ForwardIterator.Enumerator _s;
				private RankSet.ForwardIterator.Enumerator _d;
				private RankSet.ForwardIterator.Enumerator _c;
				private RankSet.ForwardIterator.Enumerator _h;

				private Rank _rs;
				private Rank _rd;
				private Rank _rc;
				private Rank _rh;

				private bool _initialized;
				private Card _current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(RankForwardIterator owner)
				{
					_s = owner.S.GetEnumerator();
					_d = owner.D.GetEnumerator();
					_c = owner.C.GetEnumerator();
					_h = owner.H.GetEnumerator();

					_rs = _rd = _rc = _rh = (Rank)(-1);

					_initialized = false;
					_current = default;
				}

				public readonly Card Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => _current;
				}

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

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator GetEnumerator() => new(this);

			IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();




			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RankReverseIterator Descending() => new(this.Source);



			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CopyTo(Span<Card> target)
			{
				var n = this.Count;
				ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, n, nameof(target));

				int i = 0;

				foreach (var card in this)
				{
					target[i++] = card;
				}
				

				return i;
			}
		}

		



		[StructLayout(LayoutKind.Explicit)]
		public readonly struct RankReverseIterator : IReadOnlyCollection<Card>
		{
			[FieldOffset(0)] public readonly CardSet Source;

			[FieldOffset(0)] public readonly RankSet.ReverseIterator S;
			[FieldOffset(2)] public readonly RankSet.ReverseIterator D;
			[FieldOffset(4)] public readonly RankSet.ReverseIterator C;
			[FieldOffset(6)] public readonly RankSet.ReverseIterator H;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RankReverseIterator(CardSet source) => Source = source;

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => this.Source.Count;
			}

			public Card First()
			{
				if (this.Source != default)
				{
					var rank = Source.Ranks.Upmost();

					var (s, d, c, h) = this.Source;

					if (s.Upmost() == rank)
						return rank * Suit.Spade;
					if (d.Upmost() == rank)
						return rank * Suit.Diamond;
					if (c.Upmost() == rank)
						return rank * Suit.Club;
					if (h.Upmost() == rank)
						return rank * Suit.Heart;
				}

				throw new InvalidOperationException("The collection is empty.");
			}

			public Card FirstOrDefault()
			{
				if (this.Source != default)
				{
					var rank = Source.Ranks.Upmost();

					var (s, d, c, h) = this.Source;

					if (s.Upmost() == rank)
						return rank * Suit.Spade;
					if (d.Upmost() == rank)
						return rank * Suit.Diamond;
					if (c.Upmost() == rank)
						return rank * Suit.Club;
					if (h.Upmost() == rank)
						return rank * Suit.Heart;
				}

				return default;
			}

			public struct Enumerator : IEnumerator<Card>
			{
				private RankSet.ReverseIterator.Enumerator _s, _d, _c, _h;
				private Rank _rs, _rd, _rc, _rh;
				private bool _initialized;
				private Card _current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(RankReverseIterator owner)
				{
					_s = owner.S.GetEnumerator();
					_d = owner.D.GetEnumerator();
					_c = owner.C.GetEnumerator();
					_h = owner.H.GetEnumerator();

					_rs = _rd = _rc = _rh = (Rank)(-1);

					_initialized = false;
					_current = default;
				}

				public readonly Card Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => _current;
				}

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

					if (_rs >= 0 && (rank < 0 || _rs > rank))
						rank = _rs;

					if (_rd >= 0 && (rank < 0 || _rd > rank))
						rank = _rd;

					if (_rc >= 0 && (rank < 0 || _rc > rank))
						rank = _rc;

					if (_rh >= 0 && (rank < 0 || _rh > rank))
						rank = _rh;

					if (rank < 0)
						return false;


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
		}

		public override string ToString()
		{
			return $"[ {string.Join(", ", this.Order().ByRank().Descending().Select(c => c.Symbol))} ]";
		}
	}
}
