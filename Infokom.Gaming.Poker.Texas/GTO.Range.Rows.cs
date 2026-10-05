using Infokom.Numerics.Atomics;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		



		public readonly partial struct Range
		{

			/// <summary>
			/// Creates a <see cref="Range"/> that represents a single row in the preflop matrix corresponding to the specified rank.
			/// </summary>
			/// <param name="rank"></param>
			/// <returns></returns>
			private static Range Row(Rank rank) => new(BitMatrix16x16.OfRow((int)rank, BitVector16.Ω));

			private Range IsolateRow(Rank rank) => this & Row(rank);



			/// <summary>
			/// Iterator for rows in the matrix
			/// </summary>
			public readonly struct RowCollection : IReadOnlyCollection<Range>
			{
				private readonly Range _owner;

				public RowCollection(Range owner)
				{
					_owner = owner;
				}

				public struct Enumerator : IEnumerator<Range>
				{
					private readonly RowCollection _owner;
					private Rank _y;

					public Enumerator(RowCollection owner)
					{
						_owner = owner;
						_y = unchecked((Rank)(-1));
					}


					/// <summary>
					/// Determines whether the specified cell is contained within the current row of the preflop matrix.
					/// </summary>
					/// <param name="cell">The cell to test for containment.</param>
					/// <returns><c>true</c> if the cell is contained within the current row; otherwise, <c>false</c>.</returns>
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					private readonly bool IsMemeberOfCurrent(Cell cell) => cell.Y == _y;

					public readonly Range Current
					{
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						get => Range.Create(IsMemeberOfCurrent);
					}

					readonly object IEnumerator.Current => this.Current;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public bool MoveNext() => ++_y < 13;
					public void Reset() => _y = unchecked((Rank)(-1));
					readonly void IDisposable.Dispose() { }
				}

				public int Count => 13;
				public Enumerator GetEnumerator() => new(this);
				IEnumerator<Range> IEnumerable<Range>.GetEnumerator() => this.GetEnumerator();
				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
			}

			/// <summary>
			/// Gets the collection of rows in the preflop matrix where cell
			/// </summary>
			public RowCollection Rows => new(this);
		}
	}
}