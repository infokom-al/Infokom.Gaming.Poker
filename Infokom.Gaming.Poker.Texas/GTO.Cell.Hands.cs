using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		public readonly partial struct Cell
		{
			[StructLayout(LayoutKind.Sequential)]
			public readonly struct HandCollection : IReadOnlyCollection<CardSet>
			{
				private readonly Cell _owner;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public HandCollection(Cell owner) => _owner = owner;

				public int Count
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => _owner.IsCoranked ? 6 : _owner.IsCosuited ? 4 : 12;
				}

				



				public struct Enumerator : IEnumerator<CardSet>
				{
					private readonly Cell _owner;
					private readonly RankSet _ρ1;
					private readonly RankSet _ρ2;

					private int _index;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public Enumerator(Cell owner)
					{
						_owner = owner;

						_ρ1 = RankSet.Select(owner.Hi);
						_ρ2 = RankSet.Select(owner.Lo);

						_index = -1;
					}

					public readonly CardSet Current
					{
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						get
						{
							var ρ1 = _ρ1;
							var ρ2 = _ρ2;

							if (_owner.IsCoranked)
							{
								var ρ = ρ1;

								return _index switch
								{
									0 => (ρ * σ1) | (ρ * σ2),
									1 => (ρ * σ1) | (ρ * σ3),
									2 => (ρ * σ1) | (ρ * σ4),
									3 => (ρ * σ2) | (ρ * σ3),
									4 => (ρ * σ2) | (ρ * σ4),
									5 => (ρ * σ3) | (ρ * σ4),
									_ => default
								};
							}

							if (_owner.IsCosuited)
							{
								return _index switch
								{
									0 => (ρ1 * σ1) | (ρ2 * σ1),
									1 => (ρ1 * σ2) | (ρ2 * σ2),
									2 => (ρ1 * σ3) | (ρ2 * σ3),
									3 => (ρ1 * σ4) | (ρ2 * σ4),
									_ => default
								};
							}

							return _index switch
							{
								0 => (ρ1 * σ1) | (ρ2 * σ2),
								1 => (ρ1 * σ1) | (ρ2 * σ3),
								2 => (ρ1 * σ1) | (ρ2 * σ4),

								3 => (ρ1 * σ2) | (ρ2 * σ1),
								4 => (ρ1 * σ2) | (ρ2 * σ3),
								5 => (ρ1 * σ2) | (ρ2 * σ4),

								6 => (ρ1 * σ3) | (ρ2 * σ1),
								7 => (ρ1 * σ3) | (ρ2 * σ2),
								8 => (ρ1 * σ3) | (ρ2 * σ4),

								9 => (ρ1 * σ4) | (ρ2 * σ1),
								10 => (ρ1 * σ4) | (ρ2 * σ2),
								11 => (ρ1 * σ4) | (ρ2 * σ3),

								_ => default
							};
						}
					}

					readonly object IEnumerator.Current => Current;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public bool MoveNext()
					{
						int next = _index + 1;

						int count =
							_owner.IsCoranked ? 6 :
							_owner.IsCosuited ? 4 :
							12;

						if (next >= count)
							return false;

						_index = next;
						return true;
					}

					public void Reset() => _index = -1;

					readonly void IDisposable.Dispose() { }
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator GetEnumerator() => new(_owner);
				IEnumerator<CardSet> IEnumerable<CardSet>.GetEnumerator() => this.GetEnumerator();
				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


				/// <summary>
				/// Copies the hands in this collection to a span.
				/// </summary>
				/// <param name="target">The span to copy the hands to.</param>
				/// <returns>The number of hands copied to the target span.</returns>
				public int CopyTo(Span<CardSet> target)
				{
					int n = 0;
					
					using (var x = this.GetEnumerator())
					{
						while(x.MoveNext() && n < target.Length)
						{
							target[n++] = x.Current;
						}
					}

					return n;
				}

				public int CopyTo(Span<CardSet> target, int startIndex)
				{
					int n = startIndex;

					using (var x = this.GetEnumerator())
					{
						while (x.MoveNext() && n < target.Length)
						{
							target[n++] = x.Current;
						}
					}

					return n;
				}

				public CardSet[] ToArray()
				{
					int n = this.Count;
					Span<CardSet> data = stackalloc CardSet[n];

					using (var x = this.GetEnumerator())
					{
						while (x.MoveNext())
						{
							data[--n] = x.Current;
						}
					}

					return data.ToArray();
				}

				/// <summary>
				/// Copies the hands in this collection to an array. The array is resized exactly to fit all the hands.
				/// </summary>
				/// <param name="target">The array to copy the hands to. If it is null, a new array will be created. If it is undersized/oversized, 
				/// it will be resized exactly to fit all the hands, neither more nor less.</param>
				/// <returns>The array containing the copied hands.</returns>
				public CardSet[] ToArray(CardSet[] target = null)
				{
					int n = this.Count;

					Array.Resize(ref target, n);

					using (var x = this.GetEnumerator())
					{
						while (x.MoveNext())
						{
							target[n++] = x.Current;
						}
					}

					return target;
				}
			}


			/// <summary>
			/// Collection of hands for the current cell.
			/// </summary>
			public HandCollection Hands
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new(this);
			}
		}
	}
}