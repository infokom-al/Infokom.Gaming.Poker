using Infokom.Numerics;
using Infokom.Numerics.Atomics;

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		/// <summary>
		/// Represents a single cell in the preflop matrix, corresponding to the smallest non empty pocket range.
		/// </summary>
		[DebuggerDisplay("({Row},{Col}) - {Symbol}")]
		[StructLayout(LayoutKind.Explicit)]
		public readonly partial struct Cell
		{
			private const Suit σ1 = Suit.Spade, σ2 = Suit.Diamond, σ3 = Suit.Club, σ4 = Suit.Heart;
			[FieldOffset(0)] public readonly Rank X;
			[FieldOffset(1)] public readonly Rank Y;

			[FieldOffset(0)] public readonly Point<Rank, Rank> Location;


			private Cell(Rank x, Rank y)
			{
				this.X = x;
				this.Y = y;
			}

			public int Row
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.Ace - Y;
			}
			public int Col
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.Ace - X;
			}



			public bool IsValid
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => X is >= Rank.Two and <= Rank.Ace && Y is >= Rank.Two and <= Rank.Ace;
			}

			public Rank Hi
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.Max(Y, X);
			}
			public Rank Lo
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.Min(Y, X);
			}
			public bool IsCoranked
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => X == Y;
			}
			public bool IsCosuited
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => X < Y;
			}
			public bool IsUnsuited
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => X > Y;
			}


			public string Symbol => string.Create(3, this, (span, state) =>
			{
				var (x, y) = (state.X, state.Y);

				if (state.IsCosuited)
				{
					span[0] = y.Symbol;
					span[1] = x.Symbol;
					span[2] = 'ₛ';
					return;
				}

				if (state.IsUnsuited)
				{
					span[0] = x.Symbol;
					span[1] = y.Symbol;
					span[2] = 'ₒ';
					return;
				}

				if (state.IsCoranked)
				{
					span[0] = span[1] = x.Symbol;
					span[2] = ' ';
					return;
				}

				span[0] = span[1] = span[2] = ' ';
			});

			public override string ToString() => this.Symbol;





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

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator GetEnumerator() => new(_owner);
				IEnumerator<CardSet> IEnumerable<CardSet>.GetEnumerator() => this.GetEnumerator();
				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();



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