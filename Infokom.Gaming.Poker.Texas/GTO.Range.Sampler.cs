using System.Collections;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
public readonly partial struct Range
		{
			/// <summary>
			/// Represents a sampler for a pocket range, allowing iteration over feasible pockets based on the available cards.
			/// </summary>
			public readonly struct Sampler : IReadOnlyCollection<CardSet>
			{
				private readonly GTO.Range _owner;
				private readonly CardSet _source;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Sampler(GTO.Range owner, CardSet source)
				{
					_owner = owner;
					_source = source;
				}

				/// <summary>
				/// Determines whether an hand event is reachable considering the available cards and observable given predefined range.
				/// </summary>
				/// <param name="target">The target pocket to check for feasibility.</param>
				/// <returns>True if the target pocket is feasible; otherwise, false.</returns>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public readonly bool IsFeasible(CardSet target)
				{
					return (_source & target) == target;
				}


				public bool IsEmpty
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => Size == 0;
				}


				//TODO inspect improvement
				public int Size
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get
					{
						int count = 0;

						foreach (var cell in _owner.Cells)
						{
							foreach (var hand in cell.Hands)
							{
								if (IsFeasible(hand))
									count++;
							}
						}

						return count;
					}
				}


				public Enumerator GetEnumerator() => new(_owner, _source);
				int IReadOnlyCollection<CardSet>.Count => this.Size;

				IEnumerator<CardSet> IEnumerable<CardSet>.GetEnumerator() => this.GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

				public int CopyTo(Span<CardSet> target, int arrayIndex)
				{
					int n = 0;
					foreach (var pocket in this)
					{
						target[n++] = pocket;
					}
					return arrayIndex;
				}

				public struct Enumerator : IEnumerator<CardSet>
				{
					private readonly GTO.Range.CellCollection.Enumerator _cells;
					private GTO.Cell.HandCollection.Enumerator _hands;
					private readonly CardSet _source;

					private bool _hasCell;
					private CardSet _current;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public Enumerator(GTO.Range owner, CardSet source)
					{
						_cells = owner.Cells.GetEnumerator();
						_hands = default;
						_source = source;
						_hasCell = false;
						_current = default;
					}

					public readonly CardSet Current
					{
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						get => _current;
					}

					readonly object IEnumerator.Current => _current;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public bool MoveNext()
					{
						while (true)
						{
							// We have a cell: continue consuming its hands.
							if (_hasCell && _hands.MoveNext())
							{
								var hand = _hands.Current;

								if (_source.IsSupersetOf(hand))
								{
									_current = hand;
									return true;
								}

								continue;
							}

							// Move to the next cell.
							if (!_cells.MoveNext())
								return false;

							_hands = _cells.Current.Hands.GetEnumerator();
							_hasCell = true;
						}
					}

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public void Reset()
					{
						_cells.Reset();
						_hands = default;
						_hasCell = false;
						_current = default;
					}

					readonly void IDisposable.Dispose() { }
				}
			}
		}
	}
}