using Infokom.Numerics.Atomics;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Extensions
{
	public static class BinaryInteger
	{
		extension(uint source)
		{
			/// <inheritdoc cref="IBinaryInteger{TSelf}.PopCount(TSelf)"/>
			public int CNT
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount(source);
			}

			/// <inheritdoc cref="IBinaryInteger{TSelf}.LeadingZeroCount(TSelf)"/>
			public int LZC
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.LeadingZeroCount(source);
			}

			/// <inheritdoc cref="IBinaryInteger{TSelf}.TrailingZeroCount(TSelf)"/>
			public int TZC
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.TrailingZeroCount(source);
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryWriteTo(Span<byte> target) => MemoryMarshal.TryWrite(target, in source);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void WriteTo(Span<byte> target) => MemoryMarshal.Write(target, in source);
		}

		extension(ulong source)
		{
			/// <inheritdoc cref = "IBinaryInteger{TSelf}.PopCount(TSelf)" />
			public int CNT
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount(source);
			}

			public int LZC
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.LeadingZeroCount(source);
			}

			public int TZC
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.TrailingZeroCount(source);
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryWriteTo(Span<byte> target) => MemoryMarshal.TryWrite(target, in source);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void WriteTo(Span<byte> target) => MemoryMarshal.Write(target, in source);
		}




		extension(ulong source)
		{
			/// <summary>
			/// Bit Test.
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool BitTest(int index) => (source & (1ul << index)) != 0;



			/// <summary>
			/// Bit scan forward. Returns bit index of lowest set bit in input.
			/// </summary>
			public int BSF
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source == default ? -1 : source.TZC;
			}

			/// <summary>
			/// Bit scan reverse. Returns bit index of highest set bit in input
			/// </summary>
			public int BSR
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source == default ? -1 : 63 - source.LZC;
			}



			public ulong HI
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => 1UL << source.BSR;
			}

			public ulong LO
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => 1UL << source.BSF;
			}




			public (ulong HI, ulong LO) HILO(Index index)
			{
				// Merr vlerën numerike të indeksit (p.sh. ^3 bëhet 3, dhe 3 mbetet 3)
				int n = index.Value;
				int totalBits = source.CNT;

				var (hi, lo) = (0ul, source);

				if (n <= 0)
				{
					if (index.IsFromEnd) (hi, lo) = (lo, hi);
					return (hi, lo);
				}

				if (n >= totalBits)
				{
					if (!index.IsFromEnd) (hi, lo) = (lo, hi);
					return (hi, lo);
				}


				if (index.IsFromEnd)//LOHI
				{
					(hi, lo) = (lo, hi);
					for (int i = 0; i < n; i++)
					{
						ulong currentLo = hi.LO;
						lo |= currentLo;
						hi ^= currentLo;
					}
					hi = source ^ lo;

					return (hi, lo);
				}



				//HILO
				hi = 0ul;
				lo = source;
				for (int i = 0; i < n; i++)
				{
					ulong currentHi = lo.HI;
					hi |= currentHi;
					lo ^= currentHi;
				}
				lo = source ^ hi;

				return (hi, lo);
			}
		}
	}

}
