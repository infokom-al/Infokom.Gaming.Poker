using Infokom.Numerics;
using Infokom.Numerics.Atomics;

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

			[FieldOffset(0)] public readonly Point<sbyte, sbyte> Position;


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
					span[2] = 's';
					return;
				}

				if (state.IsUnsuited)
				{
					span[0] = x.Symbol;
					span[1] = y.Symbol;
					span[2] = 'o';
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

		}





	}
}