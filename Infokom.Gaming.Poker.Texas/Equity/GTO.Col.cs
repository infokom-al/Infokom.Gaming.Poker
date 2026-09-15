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
		/// Represents a single column in the preflop matrix, corresponding to a specific rank.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public readonly struct Col : IGrouping<Rank, Cell>
		{
			private readonly sbyte _x;

			private Col(sbyte x) => _x = x;

			public Rank Key => Rank.Ace - _x;
			public IEnumerator<Cell> GetEnumerator()
			{
				foreach (var rank in RankSet.Ω.Order().Reverse())
				{
					var y = Rank.Ace - rank;
					var x = _x;
					yield return Unsafe.BitCast<Point<sbyte, sbyte>, Cell>((x, y));
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


			public bool IsFirst => _x == 0;

			public bool HasNext => (byte)_x is < 12;

			public bool TryGetNext(out Col next)
			{
				if (this.HasNext)
				{
					next = Unsafe.BitCast<sbyte, Col>((sbyte)(_x + 1));
					return true;
				}
				next = default;
				return false;
			}

			public override string ToString() => $"{this.Key.Symbol} [13,1]";
			public override int GetHashCode() => _x.GetHashCode();
			public bool Equals(Col other) => _x.Equals(other._x);
			public override bool Equals([NotNullWhen(true)] object obj) => base.Equals(obj);
			public static bool operator ==(Col col1, Col col2) => col1._x == col2._x;
			public static bool operator !=(Col col1, Col col2) => col1._x != col2._x;

			public readonly struct Iterator : IReadOnlyCollection<Col>
			{
				private readonly Range _owner;
				public Iterator(Range owner)
				{
					_owner = owner;
				}

				public struct Enumerator : IEnumerator<Col>
				{
					private readonly Iterator _owner;
					private sbyte _x;

					public Enumerator(Iterator owner)
					{
						_owner = owner;
						_x = -1;
					}

					public Col Current => Unsafe.As<sbyte, Col>(ref _x);


					object IEnumerator.Current => this.Current;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public bool MoveNext() => ++_x < 13;
					public void Reset() => _x = -1;
					readonly void IDisposable.Dispose() { }
				}

				public int Count => 13;
				public Enumerator GetEnumerator() => new(this);
				IEnumerator<Col> IEnumerable<Col>.GetEnumerator() => this.GetEnumerator();
				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
			}
		}
	}
}