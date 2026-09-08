using Infokom.Numerics.Atomics;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker
{
	[Flags]
	public enum Ranks : ushort
	{
		Φ = 0b0000000000000000,

		Two = 1 << Rank.Two,
		Three = 1 << Rank.Three,
		Four = 1 << Rank.Four,
		Five = 1 << Rank.Five,
		Six = 1 << Rank.Six,
		Seven = 1 << Rank.Seven,
		Eight = 1 << Rank.Eight,
		Nine = 1 << Rank.Nine,
		Ten = 1 << Rank.Ten,
		Jack = 1 << Rank.Jack,
		Queen = 1 << Rank.Queen,
		King = 1 << Rank.King,
		Ace = 1 << Rank.Ace,

		Ω = 0b0111111111111100,
	}

	public readonly struct RanksIterator
	{
		public struct Enumerator : IEnumerator<Rank>
		{
			private ushort _source;
			private sbyte _current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(Ranks owner)
			{
				_source = (ushort)owner;
				_current = default;
			}

			public readonly Rank Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (Rank)_current;
			}

			readonly object IEnumerator.Current => this.Current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (_source == 0)
				{
					return false;
				}

				_current = (sbyte)BitOperations.TrailingZeroCount((uint)_source);
				_source &= (ushort)~(1 << _current);

				return true;
			}
			void IEnumerator.Reset() => throw new NotSupportedException();
			readonly void IDisposable.Dispose() { }
		}
	}

	public struct RanksEnumerator : IEnumerator<Rank>
	{
		private ushort _source;
		private sbyte _current;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RanksEnumerator(Ranks owner)
		{
			_source = (ushort)owner;
			_current = default;
		}

		public readonly Rank Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Rank)_current;
		}

		readonly object IEnumerator.Current => this.Current;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_source == 0)
			{
				return false;
			}

			_current = (sbyte)BitOperations.TrailingZeroCount((uint)_source);
			_source &= (ushort)~(1 << _current);

			return true;
		}
		void IEnumerator.Reset() => throw new NotSupportedException();
		readonly void IDisposable.Dispose() { }
	}


	public readonly struct RankDescendingIterator : IReadOnlyCollection<Rank>
	{
		private readonly Ranks _source;

		public int Count => BitOperations.PopCount((uint)_source);

		public RankDescendingIterator(Ranks data) => _source = data;

		public struct Enumerator : IEnumerator<Rank>
		{
			private Ranks _upcoming;
			private Rank _current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(RankDescendingIterator owner)
			{
				_upcoming = owner._source;
				_current = default;
			}

			public readonly Rank Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _current;
			}

			readonly object IEnumerator.Current => this.Current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (_upcoming == 0)
				{
					return false;
				}

				_current = _upcoming.Upmost();
				_upcoming = _upcoming.Exclude(_current);

				return true;
			}
			void IEnumerator.Reset() => throw new NotSupportedException();
			readonly void IDisposable.Dispose() { }
		}

		public Enumerator GetEnumerator() => new(this);
		IEnumerator<Rank> IEnumerable<Rank>.GetEnumerator() => this.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();



		public override string ToString() => "[" + string.Join(", ", this.Select(x => x.Symbol)) + "]";
	}


	public struct DescendingRanksEnumerator : IEnumerator<Rank>
	{
		private ushort _source;
		private sbyte _current;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DescendingRanksEnumerator(Ranks owner)
		{
			_source = (ushort)owner;
			_current = default;
		}

		public readonly Rank Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Rank)_current;
		}

		readonly object IEnumerator.Current => this.Current;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_source == 0)
			{
				return false;
			}

			_current = (sbyte)BitOperations.Log2((uint)_source);
			_source &= (ushort)~(1 << _current);

			return true;
		}
		void IEnumerator.Reset() => throw new NotSupportedException();
		readonly void IDisposable.Dispose() { }
	}

	public static class RanksExtensions
	{
		extension(Ranks)
		{
			public static Ranks Select(Rank element) => (Ranks)(1 << element);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool Get(int offset, out Rank result)
			{
				if (offset is < 5 or > 14)
				{
					result = default;
					return false;
				}

				result = (Rank)offset;
				return true;
			}
		}

		extension(Ranks source)
		{
			public Binary<ushort> Bits
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Unsafe.As<Ranks, Binary<ushort>>(ref source);
			}

			/// <summary>
			/// <paramref name="source"/> is <see cref="Ranks.Φ"/> ?
			/// </summary>
			public bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (source & Ranks.Ω) == Ranks.Φ;
			}

			/// <summary>
			/// |<paramref name="source"/>|
			/// </summary>
			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount((ulong)source);
			}

			/// <summary>
			/// Extend the <paramref name="source"/> including an element
			/// </summary>
			/// <param name="element">the element to add</param>
			/// <returns>
			/// <paramref name="source"/> ∪ {<paramref name="element"/>}
			/// </returns>
			public Ranks Include(Rank element) => (Ranks)((ushort)source | (1 << (int)element));

			/// <summary>
			/// Restrict the <paramref name="source"/> set excluding an element
			/// </summary>
			/// <param name="element">the element to exclude</param>
			/// <returns>
			/// <paramref name="source"/> \ {<paramref name="element"/>}
			/// </returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Ranks Exclude(Rank target) => (Ranks)((uint)source & ~(1u << target));


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Rank Upmost() => (Rank)(source.IsEmpty ? -1 : BitOperations.Log2((uint)source));


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Rank Lowest() => (Rank)(source.IsEmpty ? 0 : BitOperations.Log2((uint)source));

			

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RanksEnumerator GetEnumerator() => new(source);

			public RankDescendingIterator Descending() => new(source);
		}
	}

	public enum ScanDirection
	{
		Reverse = -1,

		Forward = +1,
	}
}
