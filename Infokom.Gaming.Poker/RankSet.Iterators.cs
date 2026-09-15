using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker
{
	public readonly partial struct RankSet
	{
		[StructLayout(LayoutKind.Sequential, Size = 2)]
		public readonly partial struct ForwardIterator : IReadOnlyCollection<Rank>
		{
			public readonly RankSet Source;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ForwardIterator(RankSet source) => Source = source;

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount(Source);
			}

			public struct Enumerator : IEnumerator<Rank>
			{
				/// <summary>
				/// Bits yet to be enumerated
				/// </summary>
				private ushort _upcoming;

				/// <summary>
				/// Offset of the current enumerated bit, or -1 if enumeration has not started or has finished.
				/// </summary>
				private sbyte _current;


				/// <summary>
				/// Initializes a new instance of the <see cref="Enumerator"/> struct.
				/// </summary>
				/// <param name="source">The <see cref="ForwardIterator"/> instance to enumerate.</param>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(ForwardIterator source)
				{
					_upcoming = source.Source;
					_current = default;
				}

				/// <summary>
				/// Current <see cref="Rank">rank</see>.
				/// </summary>
				public readonly Rank Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (Rank)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				/// <summary>
				/// Move to the next <see cref="Rank">rank</see> greater than the <see cref="Current">current one</see> in this sequence.
				/// </summary>
				/// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext()
				{
					if (_upcoming == 0)
					{
						return false;
					}

					_current = (sbyte)BitOperations.TrailingZeroCount((uint)_upcoming);
					_upcoming &= (ushort)~(1 << _current);

					return true;
				}

				void IEnumerator.Reset() => throw new NotSupportedException();
				readonly void IDisposable.Dispose() { }
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator GetEnumerator() => new(this);
			IEnumerator<Rank> IEnumerable<Rank>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public override string ToString() => "[" + string.Join(", ", this.Select(x => x.Symbol)) + "]";			
		}

		public ForwardIterator Order() => new(this);


		public readonly struct ReverseIterator : IReadOnlyCollection<Rank>
		{
			public readonly RankSet Source;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ReverseIterator(RankSet source) => Source = source;

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount(Source);
			}


			public struct Enumerator : IEnumerator<Rank>
			{
				/// <summary>
				/// Bits yet to be enumerated
				/// </summary>
				private ushort _upcoming;

				/// <summary>
				/// Offset of the current enumerated bit, or -1 if enumeration has not started or has finished.
				/// </summary>
				private sbyte _current;


				/// <summary>
				/// Initializes a new instance of the <see cref="Enumerator"/> struct.
				/// </summary>
				/// <param name="owner">The <see cref="ReverseIterator"/> instance to enumerate.</param>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(ReverseIterator owner)
				{
					_upcoming = owner.Source;
					_current = default;
				}

				/// <summary>
				/// Current <see cref="Rank">rank</see>.
				/// </summary>
				public readonly Rank Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => unchecked((Rank)_current);
				}

				readonly object IEnumerator.Current => this.Current;

				/// <summary>
				/// Move to the next <see cref="Rank">rank</see> less than the <see cref="Current">current one</see> in this sequence.
				/// </summary>
				/// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext()
				{
					if (_upcoming == 0)
					{
						_current = default;
						return false;
					}

					_current = (sbyte)BitOperations.Log2(_upcoming);
					_upcoming &= (ushort)~(1u << _current);

					return true;
				}

				readonly void IEnumerator.Reset() => throw new NotSupportedException();
				readonly void IDisposable.Dispose() { }
			}

			/// <summary>
			/// Get an enumerator to visit ranks in descending order.
			/// </summary>
			/// <returns></returns>
			public Enumerator GetEnumerator() => new(this);
			IEnumerator<Rank> IEnumerable<Rank>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public override string ToString() => "[" + string.Join(", ", this.Select(x => x.Symbol)) + "]";

			public ForwardIterator Reverse() => new(this.Source);
		}


		public readonly partial struct ForwardIterator
		{
			public ReverseIterator Reverse() => new(this.Source);
		}


		/// <summary>
		/// Get an enumerator to visit ranks in descending order.
		/// </summary>
		/// <remarks>
		/// Equivalent to <c><see langword="this"></see>.<see cref="Order">Order()</see>.<see cref="ForwardIterator.Reverse">Reverse()</see></c>
		/// </remarks>
		public ReverseIterator OrderDescending() => new(this);
	}
}
