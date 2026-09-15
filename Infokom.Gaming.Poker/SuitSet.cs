using Infokom.Numerics.Extensions;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker
{
	public readonly struct SuitSet
	{
		private const byte Φ_BITS = 0b__0000_0000;
		private const byte S_BITS = 0b__0000_0001;
		private const byte D_BITS = 0b__0000_0010;
		private const byte C_BITS = 0b__0000_0100;
		private const byte H_BITS = 0b__0000_1000;
		private const byte Ω_BITS = S_BITS | D_BITS | C_BITS | H_BITS;


		private readonly byte _bits;

		private SuitSet(byte bits) => _bits = bits;

		public static SuitSet Φ => new(Φ_BITS);
		public static SuitSet Ω => new(Ω_BITS);
		public static SuitSet Spade => new(S_BITS);
		public static SuitSet Diamond => new(D_BITS);
		public static SuitSet Club => new(C_BITS);
		public static SuitSet Heart => new(H_BITS);


		public readonly struct Iterator : IReadOnlyCollection<Suit>
		{
			public readonly SuitSet Source;

			public int Count => throw new NotImplementedException();

			public Iterator(SuitSet source) => Source = source;

			[StructLayout(LayoutKind.Sequential)]
			public struct Enumerator : IEnumerator<Suit>
			{
				private byte _upcoming;
				private sbyte _current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(Iterator owner)
				{
					_upcoming = owner.Source._bits;
					_current = 0;
				}

				public readonly Suit Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (Suit)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext() => _upcoming.BitScanForward(out _current, out _upcoming);

				void IEnumerator.Reset() => throw new NotSupportedException();

				readonly void IDisposable.Dispose() { }
			}

			public Enumerator GetEnumerator() => new(this);
			IEnumerator<Suit> IEnumerable<Suit>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


			/// <summary>
			/// Revertes the order of the elements.
			/// </summary>
			/// <returns></returns>
			public ReverseIterator Reverse() => new(Source);

			/// <summary>
			/// Shuffles the elements in a random order.
			/// </summary>
			public ShuffleIterator Shuffle() => new(Source);
		}


		/// <summary>
		/// Sorts the elements in conventional order: i.e. <c> <see cref="Suit.Spade">♠</see> ≺ <see cref="Suit.Diamond">♦</see> ≺ <see cref="Suit.Club">♣</see> ≺ <see cref="Suit.Heart">♥</see>.</c>
		/// </summary>
		/// <returns></returns>
		public Iterator Order() => new(this);



		public readonly struct ReverseIterator
		{
			public readonly SuitSet Source;

			public ReverseIterator(SuitSet source) => Source = source;

			[StructLayout(LayoutKind.Sequential)]
			public struct Enumerator : IEnumerator<Suit>
			{
				private byte _upcoming;
				private sbyte _current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(ReverseIterator owner)
				{
					_upcoming = owner.Source._bits;
					_current = -1;
				}

				public readonly Suit Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (Suit)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext() => _upcoming.BitScanReverse(out _current, out _upcoming);

				void IEnumerator.Reset() => throw new NotSupportedException();

				readonly void IDisposable.Dispose() { }
			}


			/// <summary>
			/// Revertes the order of the elements.
			/// </summary>
			/// <returns></returns>
			public Iterator Reverse() => new(Source);

			/// <summary>
			/// Shuffles the elements in a random order.
			/// </summary>
			public ShuffleIterator Shuffle() => new(Source);
		}

		public readonly struct ShuffleIterator
		{
			public readonly SuitSet Source;

			public ShuffleIterator(SuitSet source) => Source = source;

			[StructLayout(LayoutKind.Sequential)]
			public struct Enumerator : IEnumerator<Suit>
			{
				private byte _upcoming;
				private sbyte _current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(SuitSet source)
				{
					_upcoming = source._bits;
					_current = -1;
				}

				public readonly Suit Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (Suit)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext() => _upcoming.BitScanRandom(out _current, out _upcoming);

				void IEnumerator.Reset() => throw new NotSupportedException();

				readonly void IDisposable.Dispose() { }
			}
		}
	}
}
