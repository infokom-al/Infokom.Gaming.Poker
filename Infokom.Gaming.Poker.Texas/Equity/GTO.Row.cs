using Infokom.Numerics.Atomics;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{

	public static partial class GTO
	{
		/// <summary>
		/// Represents a single row in the preflop matrix, corresponding to a specific rank.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public readonly struct Row : IGrouping<Rank, Cell>, IEquatable<Row>
		{
			private readonly sbyte _y;

			public Rank Key => Rank.Ace - _y;

			public IEnumerator<Cell> GetEnumerator()
			{
				foreach (var rank in RankSet.Ω.OrderDescending())
				{
					var y = _y;
					var x = Rank.Ace - rank;
					yield return Unsafe.BitCast<Point<sbyte, sbyte>, Cell>((x, y));
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


			public bool IsFirst => _y == 0;

			public bool HasNext => (byte)_y < 12;

			public bool TryGetNext(out Row y)
			{
				if (this.HasNext)
				{
					y = Unsafe.BitCast<sbyte, Row>((sbyte)(_y + 1));
					return true;
				}
				y = default;
				return false;
			}

			public override string ToString() => $"{this.Key.Symbol}: [1,13]";
			public override int GetHashCode() => _y.GetHashCode();
			public bool Equals(Row row) => _y.Equals(row._y);
			public override bool Equals([NotNullWhen(true)] object obj) => base.Equals(obj);
			public static bool operator ==(Row row1, Row row2) => row1._y == row2._y;
			public static bool operator !=(Row row1, Row row2) => row1._y != row2._y;
		}
	}
}
