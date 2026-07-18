using Holdem.Core.Extensions;

using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics;

using System.Text.RegularExpressions;

namespace Holdem.Core
{
	public interface IGTO<TGTO, T> : IMatrixial<TGTO, T> where TGTO : IGTO<TGTO, T>
	{
		static (int M, int N) IMatrixial<TGTO, T>.Size { get; } = (13, 13);
	}


	public static partial class GTO
	{
		public partial class Range
		{

			internal readonly struct Mask : IMatrixial<Mask, IOption<GTO.Cell>>
			{



				private const int ROW_COUNT = 13;
				private const int COL_COUNT = 13;
				private const int COUNT = ROW_COUNT * COL_COUNT;


				private const ulong LO_MAX = ulong.MaxValue;
				private const ulong MI_MAX = ulong.MaxValue;
				private const ulong HI_MAX = (1UL << 41) - 1;

				private const ulong LO_MIN = ulong.MinValue;
				private const ulong MI_MIN = ulong.MinValue;
				private const ulong HI_MIN = ulong.MinValue;



				private readonly ulong _lo;
				private readonly ulong _mi;
				private readonly ulong _hi;

				private Mask(ulong lo, ulong mi, ulong hi)
				{
					_lo = lo;
					_mi = mi;
					_hi = hi;

				}


				public bool IsEmpty => _lo == 0 && _mi == 0 && _hi == 0;



				internal bool this[int index]
				{
					get
					{
						if (index < 64)
							return (_lo & (1UL << index)) != 0;
						if (index < 128)
							return (_mi & (1UL << (index - 64))) != 0;
						return (_hi & (1UL << (index - 128))) != 0;
					}
				}


				/// <summary>
				/// Check if some cell is included in this mask.
				/// </summary>
				/// <param name="row">Row index of the cell to be checked</param>
				/// <param name="col">Col index of the cell to be checked</param>
				/// <returns>True if the cell with <paramref name="row"/> index and <paramref name="col"/> index is included in the mask, false otherwise</returns>
				internal bool this[int row, int col]
				{
					get
					{
						int index = row * 13 + col;

						if (index < 64)
							return (_lo & (1UL << index)) != 0;
						if (index < 128)
							return (_mi & (1UL << (index - 64))) != 0;
						return (_hi & (1UL << (index - 128))) != 0;
					}
				}

				/// <summary>
				/// Check if some cell is included in this mask
				/// </summary>
				/// <param name="cell">The cell to check</param>
				/// <returns>True if the <paramref name="cell"/> is included in this msk, false otherwise</returns>
				internal bool this[GTO.Cell cell]
				{
					get
					{
						var (row, col) = cell.Index;

						return this[row, col];
					}
				}



				IOption<Cell> IMatrix<IOption<Cell>>.this[int i, int j] => new Option<GTO.Cell>(GTO.CELLS[i, j], this[i,j]);


				public IMatrix<IOption<Cell>> AsMatrix() => this;

				public static (int M, int N) Size => (ROW_COUNT, COL_COUNT);

				




				public Mask Include(int row, int col)
				{
					int index = Arrays.LinearIndex(row, col, ROW_COUNT, COL_COUNT);

					if ((uint)index >= COUNT)
						return this;

					ulong lo = this._lo;
					ulong mi = this._mi;
					ulong hi = this._hi;

					if (index < 64)
						lo |= (1UL << index);
					else if (index < 128)
						mi |= (1UL << (index - 64));
					else
						hi |= (1UL << (index - 128));

					return new Mask(lo, mi, hi);
				}

				public Mask Exclude(int row, int col)
				{
					int index = Arrays.LinearIndex(row, col, ROW_COUNT, COL_COUNT);

					if ((uint)index >= COUNT)
						return this;

					ulong lo = this._lo;
					ulong mi = this._mi;
					ulong hi = this._hi;

					if (index < 64)
						lo &= ~(1UL << index);
					else if (index < 128)
						mi &= ~(1UL << (index - 64));
					else
						hi &= ~(1UL << (index - 128));

					return new Mask(lo, mi, hi);
				}




				public Mask Include(GTO.Cell cell)
				{
					var (row, col) = cell.Index;

					return this.Include(row, col);
				}

				public Mask Exclude(GTO.Cell cell)
				{
					var (row, col) = cell.Index;

					return this.Exclude(row, col);
				}


				public IEnumerable<GTO.Cell> Cells
				{
					get
					{
						for (int index = 0; index < 169; index++)
						{
							bool isBitSet = false;
							if (index < 64)
								isBitSet = (this._lo & (1UL << index)) != 0;
							else if (index < 128)
								isBitSet = (this._mi & (1UL << (index - 64))) != 0;
							else
								isBitSet = (this._hi & (1UL << (index - 128))) != 0;

							if (isBitSet)
							{
								yield return GTO.Cell.FromIndex(index);
							}
						}
					}
				}


				public static Mask Of(GTO.Cell cell)
				{
					var (row, col) = cell.Index;

					int index = Arrays.LinearIndex(row, col, ROW_COUNT, COL_COUNT);

					var (l, m, h) = (0UL, 0UL, 0UL);

					if (index < 64)
						l |= (1UL << index);
					else if (index < 128)
						m |= (1UL << (index - 64));
					else
						h |= (1UL << (index - 128));

					return new Mask(l, m, h);
				}


				public static Mask operator |(Mask a, Mask b) => new(a._lo | b._lo, a._mi | b._mi, a._hi | b._hi);
				public static Mask operator &(Mask a, Mask b) => new(a._lo & b._lo, a._mi & b._mi, a._hi & b._hi);
				public static Mask operator ^(Mask a, Mask b) => new(a._lo ^ b._lo, a._mi ^ b._mi, a._hi ^ b._hi);


				public static Mask operator ~(Mask a)
				{
					ulong invertedHi = ~a._hi;
					ulong validHiMask = (1UL << (COUNT - 128)) - 1; // 169 - 128 = 41 bite të vlefshme
					invertedHi &= validHiMask;// bits 169...191 are not affected

					return new Mask(~a._lo, ~a._mi, invertedHi);
				}


				public static readonly Mask Min = new(LO_MIN, MI_MIN, HI_MIN);

				public static readonly Mask Max = new(LO_MAX, MI_MAX, HI_MAX);


























				#region PAIRS
				private const string PAIR_RANGE_PATTERN = @"^([2-9TJQKA])\1(\+?)$";

				private static bool TryParsePairs(string token, out Mask mask)
				{
					mask = Mask.Min;

					if (string.IsNullOrEmpty(token))
						return false;

					Match match = Regex.Match(token, PAIR_RANGE_PATTERN);
					if (!match.Success)
						return false;

					try
					{
						var rank = Rank.ConvertFrom(match.Groups[1].Value[0]);


						bool hasPlus = match.Groups[2].Success && match.Groups[2].Value == "+";

						if (!hasPlus)
						{
							mask |= mask.Include(Cell.Pair(rank));

							return true;
						}
						else
						{
							for (int i = rank.Index; i <= 12; i++)
							{
								rank = Rank.Values[i];

								mask |= mask.Include(Cell.Pair(rank));
							}
						}
					}
					catch
					{
						return false;
					}

					return true;
				}
				#endregion


				#region SUITED

				private const string SUITED_RANGE_PATTERN = @"^([2-9TJQKA])([2-9TJQKA])s(\+?)$";

				private static bool TryParseSuited(string token, out Mask mask)
				{
					mask = Mask.Min;

					if (string.IsNullOrEmpty(token))
						return false;

					Match match = Regex.Match(token, SUITED_RANGE_PATTERN);
					if (!match.Success)
						return false;

					var hi = Rank.ConvertFrom(match.Groups[1].Value[0]);
					var lo = Rank.ConvertFrom(match.Groups[2].Value[0]);
					bool hasPlus = match.Groups[3].Success && match.Groups[3].Value == "+";

					try
					{
						if (!hasPlus)
						{
							mask = mask.Include(Cell.Suited(hi, lo));
						}
						else
						{
							for (int i = lo.Index; i < hi.Index; i++)
							{
								lo = Rank.Values[i];
								mask |= mask.Include(Cell.Suited(hi, lo));
							}
						}
					}
					catch
					{
						return false;
					}

					return true;
				}
				#endregion


				#region OFFSUITED


				private const string OFFSUITED_RANGE_PATTERN = @"^([2-9TJQKA])([2-9TJQKA])o(\+?)$";

				private static bool TryParseOffsuit(string token, out Mask mask)
				{
					mask = Mask.Min;

					if (string.IsNullOrEmpty(token))
						return false;

					Match match = Regex.Match(token, OFFSUITED_RANGE_PATTERN);
					if (!match.Success)
						return false;

					var hi = Rank.ConvertFrom(match.Groups[1].Value[0]);
					var lo = Rank.ConvertFrom(match.Groups[2].Value[0]);
					bool hasPlus = match.Groups[3].Success && match.Groups[3].Value == "+";

					try
					{
						if (!hasPlus)
						{
							mask = mask.Include(Cell.Offsuited(hi, lo));
						}
						else
						{
							for (int i = lo.Index; i < hi.Index; i++)
							{
								lo = Rank.Values[i];
								mask |= mask.Include(Cell.Offsuited(hi, lo));
							}
						}
					}
					catch
					{
						return false;
					}

					return true;
				}
				#endregion



				public static Mask Parse(string expression)
				{
					var result = Mask.Min;

					if (!TryParse(expression, out result))
					{
						throw new FormatException();
					}

					return result;
				}

				public static bool TryParse(string expression, out Mask mask)
				{
					mask = Mask.Min;

					if (string.IsNullOrEmpty(expression))
						return false;

					// Përdorim ReadOnlySpan për të shmangur alokimin e stringjeve të reja gjatë ndarjes
					ReadOnlySpan<char> span = expression.AsSpan();
					int start = 0;

					while (start < span.Length)
					{
						// Gjejmë pozicionin e presjes së radhës
						int commaIndex = span[start..].IndexOf(',');
						int end = commaIndex == -1 ? span.Length : start + commaIndex;

						// Izolojmë token-in aktual si një Slice (pa krijuar string të ri)
						ReadOnlySpan<char> tokenSpan = span[start..end].Trim();

						if (tokenSpan.Length > 0)
						{
							// Regex dhe metodat tona presin string, ndaj e konvertojmë vetëm këtë pjesë të vogël
							var token = tokenSpan.ToString();
							var tokenMask = Mask.Min;
							var parsed = false;

							// Tentojmë të parsojmë sipas të treja kategorive simetrike që ndërtuam
							if (TryParsePairs(token, out tokenMask)) parsed = true;
							else if (TryParseSuited(token, out tokenMask)) parsed = true;
							else if (TryParseOffsuit(token, out tokenMask)) parsed = true;

							// Nëse boditë qoftë edhe një token i vetëm i pavlefshëm (gabim shkrimi), e gjithë metoda dështon
							if (!parsed)
							{
								return false;
							}

							// Bashkojmë maskën e qelizave të këtij token-i me maskën globale me operatorin OR (|)
							mask |= tokenMask;
						}

						// Kalojmë te token-i i radhës pas presjes
						start = commaIndex == -1 ? span.Length : end + 1;
					}

					return !mask.IsEmpty;
				}
			}

		}

	}



}
