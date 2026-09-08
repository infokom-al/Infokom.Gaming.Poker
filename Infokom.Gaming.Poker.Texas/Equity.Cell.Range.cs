using Infokom.Numerics.Atomics;

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Infokom.Gaming.Poker.Texas.Equity;

namespace Infokom.Gaming.Poker.Texas
{
	public partial class Equity
	{
		public struct Matrix
		{
			
		}


		public readonly partial struct Cell
		{
			// ╔══════════════════════════════════════════════════════════════════════════╗
			// ║                         RANGE BIT MATRIX                                 ║
			// ╠══════════════════════════════════════════════════════════════════════════╣
			// ║                                                                          ║
			// ║   Logical matrix: 16 × 16 = 256 bits                                     ║
			// ║                                                                          ║
			// ║          x →   0    1    2    3    4   ...   13   14   15                ║
			// ║              ┌────┬────┬────┬────┬────┬───────┬────┬────┬────┐           ║
			// ║       y = 0  │ ·  │ ·  │ ·  │ ·  │ ·  │  ...  │ ·  │ ·  │ ·  │           ║
			// ║       y = 1  │ ·  │ ·  │ ·  │ ·  │ ·  │  ...  │ ·  │ ·  │ ·  │           ║
			// ║       y = 2  │ ·  │ ·  │ ■  │ ■  │ ■  │  ...  │ ■  │ ■  │ ·  │           ║
			// ║         ...  │    │    │    │    │    │       │    │    │    │           ║
			// ║      y = 14  │ ·  │ ·  │ ■  │ ■  │ ■  │  ...  │ ■  │ ■  │ ·  │           ║
			// ║      y = 15  │ ·  │ ·  │ ·  │ ·  │ ·  │  ...  │ ·  │ ·  │ ·  │           ║
			// ║              └────┴────┴────┴────┴────┴───────┴────┴────┴────┘           ║
			// ║                                                                          ║
			// ║   Only coordinates [2..14] × [2..14] are valid → 13 × 13 = 169 cells.    ║
			// ║   The remaining positions are structural padding.                        ║
			// ║                                                                          ║
			// ║   Physical storage: 4 × ulong = 256 bits                                 ║
			// ║                                                                          ║
			// ║   _bits0 ──┬─ row  0 ── 16 bits ────────────────────────────────┐        ║
			// ║            ├─ row  1 ── 16 bits ────────────────────────────────┤        ║
			// ║            ├─ row  2 ── 16 bits ────────────────────────────────┤ 64b    ║
			// ║            └─ row  3 ── 16 bits ────────────────────────────────┘        ║
			// ║                                                                          ║
			// ║   _bits1 ──┬─ row  4 ── 16 bits                                          ║
			// ║            ├─ row  5 ── 16 bits                                          ║
			// ║            ├─ row  6 ── 16 bits                                  64b     ║
			// ║            └─ row  7 ── 16 bits                                          ║
			// ║                                                                          ║
			// ║   _bits2 ──┬─ row  8 ── 16 bits                                          ║
			// ║            ├─ row  9 ── 16 bits                                          ║
			// ║            ├─ row 10 ── 16 bits                                  64b     ║
			// ║            └─ row 11 ── 16 bits                                          ║
			// ║                                                                          ║
			// ║   _bits3 ──┬─ row 12 ── 16 bits                                          ║
			// ║            ├─ row 13 ── 16 bits                                          ║
			// ║            ├─ row 14 ── 16 bits                                  64b     ║
			// ║            └─ row 15 ── 16 bits                                          ║
			// ║                                                                          ║
			// ║   Bit position:                                                          ║
			// ║                                                                          ║
			// ║       word = y >> 2                                                      ║
			// ║       bit  = ((y & 3) << 4) | x                                          ║
			// ║                                                                          ║
			// ║   Thus Cell(x,y) maps directly to one bit — no 169-cell reindexing.      ║
			// ╚══════════════════════════════════════════════════════════════════════════╝

			[StructLayout(LayoutKind.Explicit)]
			public readonly partial struct Range : IEquatable<Range>
			{
				/// <summary>
				/// binary data for rows 0..3
				/// </summary>
				[FieldOffset(0)] private readonly ulong _00_08;
				[FieldOffset(0)] private readonly uint _00_04;
				[FieldOffset(4)] private readonly uint _04_08;
				[field: FieldOffset(0)] private readonly ushort _00;
				[field: FieldOffset(2)] private readonly ushort _01;
				[field: FieldOffset(4)] private readonly ushort _02;
				[field: FieldOffset(6)] private readonly ushort _03;

				/// <summary>
				/// binary data for rows 4..7
				/// </summary>
				[FieldOffset(8)] private readonly ulong _08_16;
				[FieldOffset(8)] private readonly uint _08_12;
				[FieldOffset(8)] private readonly uint _12_16;
				[field: FieldOffset(8)] private readonly ushort _04;
				[field: FieldOffset(10)] private readonly ushort _05;
				[field: FieldOffset(12)] private readonly ushort _06;
				[field: FieldOffset(14)] private readonly ushort _07;

				/// <summary>
				/// binary data for rows 8..11
				/// </summary>
				[FieldOffset(16)] private readonly ulong _16_24;
				[FieldOffset(16)] private readonly uint _16_20;
				[FieldOffset(20)] private readonly uint _20_24;
				[field: FieldOffset(16)] private readonly ushort _08;
				[field: FieldOffset(18)] private readonly ushort _09;
				[field: FieldOffset(20)] private readonly ushort _10;
				[field: FieldOffset(22)] private readonly ushort _11;

				/// <summary>
				/// binary data for rows 12..15
				/// </summary>
				[FieldOffset(24)] private readonly ulong _24_32;
				[FieldOffset(24)] private readonly uint _24_28;
				[FieldOffset(28)] private readonly uint _28_32;
				[field: FieldOffset(24)] private readonly ushort _12;
				[field: FieldOffset(26)] private readonly ushort _13;
				[field: FieldOffset(28)] private readonly ushort _14;
				[field: FieldOffset(30)] private readonly ushort _15;





				[field: FieldOffset(4)] public readonly Ranks Row2;
				[field: FieldOffset(6)] public readonly Ranks Row3;
				[field: FieldOffset(8)] public readonly Ranks Row4;
				[field: FieldOffset(10)] public readonly Ranks Row5;
				[field: FieldOffset(12)] public readonly Ranks Row6;
				[field: FieldOffset(14)] public readonly Ranks Row7;
				[field: FieldOffset(16)] public readonly Ranks Row8;
				[field: FieldOffset(18)] public readonly Ranks Row9;
				[field: FieldOffset(20)] public readonly Ranks RowT;
				[field: FieldOffset(22)] public readonly Ranks RowJ;
				[field: FieldOffset(24)] public readonly Ranks RowQ;
				[field: FieldOffset(26)] public readonly Ranks RowK;
				[field: FieldOffset(28)] public readonly Ranks RowA;


				[FieldOffset(0)] private readonly BitMatrix16x16 _mask;
				[FieldOffset(0)] private readonly InlineArray16<Ranks> _rows;

				public ReadOnlySpan<Ranks> Rows => Unsafe.AsRef(in _rows);
				//public ReadOnlySpan<Ranks> Rows => MemoryMarshal.CreateReadOnlySpan(in _rows[0], 16);



				private Range(BitMatrix16x16 mask)
				{
					
				}
				private Range(ulong bits0, ulong bits1, ulong bits2, ulong bits3)
				{
					_00_08 = bits0;
					_08_16 = bits1;
					_16_24 = bits2;
					_24_32 = bits3;
				}

				private Range(ReadOnlySpan<ushort> rowData) : this()
				{
					//if (rowData.Length != 16)
					//	throw new ArgumentException("Row data must have exactly 16 elements.", nameof(rowData));
					_00 = rowData[0];
					_01 = rowData[1];
					_02 = rowData[2];
					_03 = rowData[3];
					_04 = rowData[4];
					_05 = rowData[5];
					_06 = rowData[6];
					_07 = rowData[7];
					_08 = rowData[8];
					_09 = rowData[9];
					_10 = rowData[10];
					_11 = rowData[11];
					_12 = rowData[12];
					_13 = rowData[13];
					_14 = rowData[14];
					_15 = rowData[15];
				}

				private Range(ReadOnlySpan<Ranks> data) => _rows = Unsafe.BitCast<ReadOnlySpan<Ranks>, InlineArray16<Ranks>>(data);

				public bool IsEmpty => (_00_08 | _08_16 | _16_24 | _24_32) == 0;


				public void Write(Span<byte> target)
				{
					ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, 32, nameof(target));

					MemoryMarshal.Write(target[00..08], in _00_08);
					MemoryMarshal.Write(target[08..16], in _08_16);
					MemoryMarshal.Write(target[16..24], in _16_24);
					MemoryMarshal.Write(target[24..32], in _24_32);
				}

				public unsafe void Write(Span<ushort> target)
				{
					ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, 16, nameof(target));

					Unsafe.Write(Unsafe.AsPointer(ref target[00]), _00);
					Unsafe.Write(Unsafe.AsPointer(ref target[01]), _01);
					Unsafe.Write(Unsafe.AsPointer(ref target[02]), _02);
					Unsafe.Write(Unsafe.AsPointer(ref target[03]), _03);
					Unsafe.Write(Unsafe.AsPointer(ref target[04]), _04);
					Unsafe.Write(Unsafe.AsPointer(ref target[05]), _05);
					Unsafe.Write(Unsafe.AsPointer(ref target[06]), _06);
					Unsafe.Write(Unsafe.AsPointer(ref target[07]), _07);
					Unsafe.Write(Unsafe.AsPointer(ref target[08]), _08);
					Unsafe.Write(Unsafe.AsPointer(ref target[09]), _09);
					Unsafe.Write(Unsafe.AsPointer(ref target[10]), _10);
					Unsafe.Write(Unsafe.AsPointer(ref target[11]), _11);
					Unsafe.Write(Unsafe.AsPointer(ref target[12]), _12);
					Unsafe.Write(Unsafe.AsPointer(ref target[13]), _13);
					Unsafe.Write(Unsafe.AsPointer(ref target[14]), _14);
					Unsafe.Write(Unsafe.AsPointer(ref target[15]), _15);
				}

				public unsafe void Write(Span<uint> target)
				{
					ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, 8, nameof(target));

					Unsafe.Write(Unsafe.AsPointer(ref target[0]), _00_04);
					Unsafe.Write(Unsafe.AsPointer(ref target[1]), _04_08);
					Unsafe.Write(Unsafe.AsPointer(ref target[2]), _08_12);
					Unsafe.Write(Unsafe.AsPointer(ref target[3]), _12_16);
					Unsafe.Write(Unsafe.AsPointer(ref target[4]), _16_20);
					Unsafe.Write(Unsafe.AsPointer(ref target[5]), _20_24);
					Unsafe.Write(Unsafe.AsPointer(ref target[6]), _24_28);
					Unsafe.Write(Unsafe.AsPointer(ref target[7]), _28_32);
				}

				public unsafe void Write(Span<ulong> target)
				{
					ArgumentOutOfRangeException.ThrowIfLessThan(target.Length, 4, nameof(target));

					Unsafe.Write(Unsafe.AsPointer(ref target[0]), _00_08);
					Unsafe.Write(Unsafe.AsPointer(ref target[1]), _08_16);
					Unsafe.Write(Unsafe.AsPointer(ref target[2]), _16_24);
					Unsafe.Write(Unsafe.AsPointer(ref target[3]), _24_32);
				}

				#region EQUALITY
				public bool Equals(Range other) =>
				    _00_08.Equals(other._00_08) &&
				    _08_16.Equals(other._08_16) &&
				    _16_24.Equals(other._16_24) &&
				    _24_32.Equals(other._24_32);

				public override bool Equals(object obj) => obj is Range other && Equals(other);

				public override int GetHashCode() => HashCode.Combine(_00_08, _08_16, _16_24, _24_32);

				public static bool operator ==(Range r1, Range r2) =>
					r1._00_08 == r2._00_08 &&
					r1._08_16 == r2._08_16 &&
					r1._16_24 == r2._16_24 &&
					r1._24_32 == r2._24_32;

				public static bool operator !=(Range r1, Range r2) =>
					r1._00_08 != r2._00_08 ||
					r1._08_16 != r2._08_16 ||
					r1._16_24 != r2._16_24 ||
					r1._24_32 != r2._24_32;
				#endregion


				public bool Contains(Cell cell)
				{
					var (x, y) = (cell._y, cell._y);

					int word = y >> 2;
					int bit = ((y & 3) << 4) + x;

					ulong mask = 1UL << bit;

					return word switch
					{
						0 => (_00_08 & mask) != 0,
						1 => (_08_16 & mask) != 0,
						2 => (_16_24 & mask) != 0,
						3 => (_24_32 & mask) != 0,
						_ => false
					};
				}

				public Range Include(Cell cell)
				{
					var (x, y) = (cell._x, cell._y);

					int word = y >> 2;
					int bit = ((y & 3) << 4) + x;

					ulong mask = 1UL << bit;

					return word switch
					{
						0 => new(_00_08 | mask, _08_16, _16_24, _24_32),
						1 => new(_00_08, _08_16 | mask, _16_24, _24_32),
						2 => new(_00_08, _08_16, _16_24 | mask, _24_32),
						3 => new(_00_08, _08_16, _16_24, _24_32 | mask),
						_ => this
					};
				}

				public Range Exclude(Cell cell)
				{
					var (x, y) = (cell._y, cell._y);

					int word = y >> 2;
					int bit = ((y & 3) << 4) + x;

					ulong mask = 1UL << bit;

					return word switch
					{
						0 => new(_00_08 & mask, _08_16, _16_24, _24_32),
						1 => new(_00_08, _08_16 & mask, _16_24, _24_32),
						2 => new(_00_08, _08_16, _16_24 & mask, _24_32),
						3 => new(_00_08, _08_16, _16_24, _24_32 & mask),
						_ => this
					};
				}

				public Range Union(Range other) => new(
					_00_08 | other._00_08,
					_08_16 | other._08_16,
					_16_24 | other._16_24,
					_24_32 | other._24_32);

				public Range Intersect(Range other) => new(
					_00_08 & other._00_08,
					_08_16 & other._08_16,
					_16_24 & other._16_24,
					_24_32 & other._24_32);

				public Range Except(Range other) => new(
					_00_08 & ~other._00_08,
					_08_16 & ~other._08_16,
					_16_24 & ~other._16_24,
					_24_32 & ~other._24_32);





				// 
				//			Range
				//               │
				//      ┌────────┴────────┐
				//      │                 │
				//   Cells()          Matches()
				//      │                 │
				//   Cell set          Pocket?
				//      │                 │
				//      └────────┬────────┘
				//               ↓
				//          RangeSampler
				/// <summary>
				/// 
				/// </summary>
				/// <param name="pocket"></param>
				/// <returns></returns>
				public bool Matches(Pocket pocket)
				{
					var ((r1, s1), (r2, s2)) = pocket;
					var (x, y) = (0, 0);


					if (r1 == r2)
					{
						(x, y) = (+r1, +r2);
					}
					else if (s1 == s2)
					{
						(x, y) = (+Rank.Min(r1, r2), +Rank.Max(r1, r2));
					}
					else
					{
						(x, y) = (+Rank.Max(r1, r2), +Rank.Min(r1, r2));
					}

					int word = y >> 2;
					int bit = ((y & 0b11) << 4) | x;

					ulong mask = 1ul << bit;

					return word switch
					{
						0 => (_00_08 & mask) != 0,
						1 => (_08_16 & mask) != 0,
						2 => (_16_24 & mask) != 0,
						3 => (_24_32 & mask) != 0,
						_ => false
					};
				}




				public int Size
				{
					get
					{
						// TODO: Optimize this by using a bit count algorithm instead of iterating through cells.

						int count = 0;

						foreach (var cell in this.Cells)
							count += cell.Count;

						return count;
					}
				}

				/// <summary>
				/// Represents the empty set of cells.
				/// </summary>
				public static readonly Range Φ;

				/// <summary>
				/// Represents the universal set of all possible cells.
				/// </summary>
				public static readonly Range Ω = new([
					0b0000000000000000, // padding
					0b0000000000000000, // padding
					0b0111111111111100, // 2
					0b0111111111111100, // 3
					0b0111111111111100, // 4
					0b0111111111111100, // 5	
					0b0111111111111100, // 6
					0b0111111111111100, // 7
					0b0111111111111100, // 8
					0b0111111111111100, // 9
					0b0111111111111100, // T
					0b0111111111111100, // J
					0b0111111111111100, // Q
					0b0111111111111100, // K
					0b0111111111111100, // A
					0b0000000000000000, // padding
				]);


				/// <summary>
				/// All paired cells: 22, 33, 44, 55, 66, 77, 88, 99, TT, JJ, QQ, KK, AA
				/// </summary>
				public static readonly Cell.Range XX = new([
					0b0000000000000000, // padding
					0b0000000000000000, // padding
					0b0000000000000100, // 2
					0b0000000000001000, // 3
					0b0000000000010000, // 4
					0b0000000000100000, // 5	
					0b0000000001000000, // 6
					0b0000000010000000, // 7
					0b0000000100000000, // 8
					0b0000001000000000, // 9
					0b0000010000000000, // T
					0b0000100000000000, // J
					0b0001000000000000, // Q
					0b0010000000000000, // K
					0b0100000000000000, // A
					0b0000000000000000, // padding
				]);

				public static readonly Cell.Range XYs = new([
					0b0000000000000000, // padding
					0b0000000000000000, // padding
					0b0000000000000000, // 2
					0b0000000000000100, // 3
					0b0000000000001100, // 4
					0b0000000000011100, // 5	
					0b0000000000111100, // 6
					0b0000000001111100, // 7
					0b0000000011111100, // 8
					0b0000000111111100, // 9
					0b0000001111111100, // T
					0b0000011111111100, // J
					0b0000111111111100, // Q
					0b0001111111111100, // K
					0b0011111111111100, // A
					0b0000000000000000, // padding
				]);

				public static readonly Cell.Range XYo = new([
					0b0000000000000000, // padding
					0b0000000000000000, // padding
					0b0111111111111000, // 2
					0b0111111111110000, // 3
					0b0111111111100000, // 4
					0b0111111111000000, // 5	
					0b0111111110000000, // 6
					0b0111111100000000, // 7
					0b0111111000000000, // 8
					0b0111110000000000, // 9
					0b0111100000000000, // T
					0b0111000000000000, // J
					0b0110000000000000, // Q
					0b0100000000000000, // K
					0b0000000000000000, // A
					0b0000000000000000, // padding
				]);






				/// <summary>
				/// Intersection operator: returns a new range containing only the cells that are present in both the left and right ranges.
				/// </summary>
				/// <param name="a"></param>
				/// <param name="b"></param>
				/// <returns></returns>
				public static Range operator &(Range a, Range b) => new(
				   a._00_08 & b._00_08,
				   a._08_16 & b._08_16,
				   a._16_24 & b._16_24,
				   a._24_32 & b._24_32);


				/// <summary>
				/// Union operator: returns a new range containing all the cells that are present in either the <paramref name="a"/> or <paramref name="b"/> range.
				/// </summary>
				/// <param name="a"></param>
				/// <param name="b"></param>
				/// <returns></returns>
				public static Range operator |(Range a, Range b) =>
				    new(
					   a._00_08 | b._00_08,
					   a._08_16 | b._08_16,
					   a._16_24 | b._16_24,
					   a._24_32 | b._24_32);

				/// <summary>
				/// Symmetric difference operator: returns a new range containing only the cells that are present in either the <paramref name="a"/> or <paramref name="b"/> range, but not both.
				/// </summary>
				/// <param name="a"></param>
				/// <param name="b"></param>
				/// <returns></returns>
				public static Range operator ^(Range a, Range b) =>
				    new(
					   a._00_08 ^ b._00_08,
					   a._08_16 ^ b._08_16,
					   a._16_24 ^ b._16_24,
					   a._24_32 ^ b._24_32);

				/// <summary>
				/// Complement operator: returns a new range containing only the cells that are not present in <paramref name="range"/>.
				/// </summary>
				/// <param name="range"></param>
				/// <returns></returns>
				public static Range operator ~(Range range) => new Range(~range._00_08, ~range._08_16, ~range._16_24, ~range._24_32) & Range.Ω;




				/// <summary>
				/// Returns a new range containing only the cells above the specified minimum rank.
				/// </summary>
				/// <param name="min">The minimum rank.</param>
				/// <returns>A new range containing only the cells above the specified minimum rank.</returns>
				public Cell.Range Above(Rank min)
				{
					Span<ushort> rows = stackalloc ushort[16];

					for (var r = +min; r <= Rank.MAX; r++)
					{
						rows[r] = ushort.MaxValue << min;
					}

					return this & new Range(rows);
				}

				// 
				/// <summary>
				/// Returns a new range containing only the cells below the specified maximum rank.
				/// </summary>
				/// <param name="max">The maximum rank.</param>
				/// <returns>A new range containing only the cells below the specified maximum rank.</returns>
				/// <remarks>
				/// NOTE: Exist as a symmetrical counterpart to <see cref="Above(Rank)"/> to be formally complete,
				/// but in practice the bottom-up approach of <see cref="Above(Rank)"/> is more useful for hand
				/// range construction and filtering.
				/// </remarks>
				public Cell.Range Below(Rank max)
				{
					Span<ushort> rows = stackalloc ushort[16];

					for (var r = +max; r >= Rank.MIN; r--)
					{
						rows[r] = (ushort)~(ushort.MaxValue << max);
					}

					return this & new Range(rows);
				}

				/// <summary>
				/// Returns a new range containing only the suited cells from the current range.
				/// </summary>
				/// <returns></returns>
				public Cell.Range Suited() => this & XYs;

				/// <summary>
				/// Returns a new range containing only the paired cells from the current range.
				/// </summary>
				/// <returns></returns>
				public Cell.Range Paired() => this & XX;

				public unsafe Cell.Range Paired(Rank x)
				{
					//NOTE: R2 = R1.Paired(x) => R2[x,x] = R1[x,x] 
					Span<Ranks> targetRows = stackalloc Ranks[16];
					if (x is >= Rank.Two and <= Rank.Ace)
					{
						targetRows[(int)x] = Ranks.Select(x) & _rows[(int)x];
					}

					return new Range(targetRows);
				}

				/// <summary>
				/// Returns a new range containing only the off-suit cells from the current range.
				/// </summary>
				/// <returns></returns>
				public Cell.Range Offsuit() => this & XYo;



				// NOTE: Suited cells have a pattern XYs where X is the highest rank, Y the other rank that, ignoring connectivity and parity,
				// may be seen as a dead weight (strict flush pragmatism) that is lightened with higher Y (for higher pairing with board)
				// closer to X (connectivity for straightness). 
				// By definition XYs implies X > Y so another way to restrict freedom on arbitrary y, may be used the form (Rank X, int dX) in the sense
				// Suited Hands with high X and offset down up to dX (non negative)
				public Cell.Range Suited(Rank x, Rank y)
				{
					Span<ushort> rows = stackalloc ushort[16];

					for (int i = +x; i <= Rank.MAX; i++)
					{
						rows[i] &= (ushort)(1 << (int)y);
					}

					return this & XYs & new Range(rows);
				}




				public static Cell.Range Pair(Rank x)
				{
					BitMatrix16x16 data = new BitMatrix16x16();
					data[x,x] = true;
					return new Range(data);
				}
			}

			#region Range : ILookup<Cell, Pocket>

			public partial struct Range : ILookup<Cell, Pocket>
			{
				#region Range.CellIterator : IReadOnlyCollection<Cell>
				public readonly struct CellIterator : IReadOnlyCollection<Cell>
				{
					private readonly Range _owner;

					public CellIterator(Range owner) => _owner = owner;

					public int Count =>
							BitOperations.PopCount(_owner._00_08) +
							BitOperations.PopCount(_owner._08_16) +
							BitOperations.PopCount(_owner._16_24) +
							BitOperations.PopCount(_owner._24_32);

					// TODO: replace with custom enumerator
					public IEnumerator<Cell> GetEnumerator()
					{
						for (int y = 2; y <= 14; y++)
						{
							ulong bits = y switch
							{
								<= 3 => _owner._00_08 >> ((y & 3) << 4),
								<= 7 => _owner._08_16 >> ((y & 3) << 4),
								<= 11 => _owner._16_24 >> ((y & 3) << 4),
								_ => _owner._24_32 >> ((y & 3) << 4)
							};

							bits &= 0x7FFC; // columns 2..14

							while (bits != 0)
							{
								int x = BitOperations.TrailingZeroCount(bits) + 0;
								yield return new Cell((sbyte)x, (sbyte)y);
								bits &= bits - 1;
							}
						}
					}
					IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
				}
				#endregion

				public CellIterator Cells => new(this);



				int ILookup<Cell, Pocket>.Count => Cells.Count;

				IEnumerable<Pocket> ILookup<Cell, Pocket>.this[Cell key] => this.Contains(key) ? key.Pockets : Array.Empty<Pocket>();

				IEnumerator<IGrouping<Cell, Pocket>> IEnumerable<IGrouping<Cell, Pocket>>.GetEnumerator()
				{
					foreach (var cell in this.Cells)
					{
						yield return new PocketIterator(cell);
					}
				}

				IEnumerator IEnumerable.GetEnumerator() => ((ILookup<Cell, Pocket>)this).GetEnumerator();


				public ILookup<Cell, Pocket> AsLookup() => this;
			}
			#endregion

		}




		public static Cell.Range Range() => Cell.Range.Ω;

		public static Cell.Range Range(ReadOnlySpan<Cell> cells)
		{
			Cell.Range range = Cell.Range.Φ;
			foreach (var cell in cells)
				range = range.Include(cell);
			return range;
		}

		/// <summary>
		/// All paired cells: 22, 33, 44, 55, 66, 77, 88, 99, TT, JJ, QQ, KK, AA
		/// </summary>
		/// <returns></returns>
		public static Cell.Range Pairs()
		{
			Cell.Range range = Cell.Range.Φ;

			for (Rank r = Rank.MIN; r <= Rank.MAX; r++)
			{
				range = range.Include(Cell.Paired(r));
			}

			return range;
		}

		/// <summary>
		/// All paired cells with rank greater than or equal to the specified minimum rank.
		/// </summary>
		/// <param name="min"></param>
		/// <returns></returns>
		public static Cell.Range Pairs(Rank min)
		{
			Cell.Range range = Cell.Range.Φ;

			if (min is < Rank.MAX and > Rank.MIN)
			{
				min = Rank.Max(Rank.MIN, min);

				for (Rank r = min; r <= Rank.MAX; r++)
				{
					range = range.Include(Cell.Paired(r));
				}
			}

			return range;
		}

		/// <summary>
		/// All paired cells with rank between the specified minimum and maximum ranks (inclusive).
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static Cell.Range Pairs(Rank min, Rank max)
		{
			Cell.Range range = Cell.Range.Φ;

			if (min is < Rank.MAX && max is > Rank.MIN && min <= max)
			{
				(min, max) = (Rank.Max(Rank.MIN, min), Rank.Min(Rank.MAX, max));

				for (Rank r = min; r <= max; r++)
				{
					range = range.Include(Cell.Paired(r));
				}
			}

			return range;
		}


	}
}
