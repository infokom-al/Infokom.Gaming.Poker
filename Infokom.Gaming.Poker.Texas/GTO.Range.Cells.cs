using Infokom.Numerics.Atomics;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		public readonly partial struct Range
		{
			[StructLayout(LayoutKind.Sequential)]
			public readonly struct CellCollection : IReadOnlyCollection<Cell>
			{
				private readonly BitMatrix16x16 _data;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public CellCollection(Range owner) => _data = owner._data;

				public int Count
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => _data.NNZ;
				}





				public struct Enumerator : IEnumerator<Cell>
				{
					private readonly BitMatrix16x16 _data;
					private int _x;
					private int _y;
					private Cell _current;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public Enumerator(BitMatrix16x16 data)
					{
						_data = data;
						_x = 1;
						_y = 1;
						_current = default;
					}

					public readonly Cell Current
					{
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						get => _current;
					}

					readonly object IEnumerator.Current => Current;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public bool MoveNext()
					{
						while (++_x <= 14)
						{
							if (_data[_y, _x])
							{
								var point = Point.X((Rank)_x).Y((Rank)_y);
								_current = Unsafe.As<Point<sbyte, sbyte>, Cell>(ref point);
								return true;
							}
						}

						while (++_y <= 14)
						{
							_x = 1;

							while (++_x <= 14)
							{
								if (_data[_y, _x])
								{
									var point = Point.X((Rank)_x).Y((Rank)_y);
									_current = Unsafe.As<Point<sbyte, sbyte>, Cell>(ref point);
									return true;
								}
							}
						}

						return false;
					}

					public void Reset()
					{
						_x = 1;
						_y = 1;
						_current = default;
					}

					readonly void IDisposable.Dispose() { }
				}

				public Enumerator GetEnumerator() => new(_data);
				IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() => this.GetEnumerator();
				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();




				public Cell[] ToArray(Cell[] target = null)
				{
					int n = this.Count;

					Array.Resize(ref target, n);
					using (var x = this.GetEnumerator())
					{
						while(x.MoveNext())
						{
							target[--n] = x.Current;
						}
					}
					return target;
				}

			}

			/// <summary>
			/// Gets the collection of cells in the hand range.
			/// </summary>
			public CellCollection Cells
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new(this);
			}


			public readonly Cell this[int r, int c]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					r = (int)Rank.Ace - r;
					c = (int)Rank.Ace - c;
					if ((uint)r < 16 && (uint)c < 16 && _data[r, c])
					{
						
						return unchecked(Unsafe.BitCast<Point<Rank, Rank>, Cell>((X: (Rank)c, Y: (Rank)r)));
					}
					return default;
				}
			}
		}
	}
}
