using Infokom.Numerics.Atomics;

using System.Collections.Specialized;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Extensions
{
	
	



	public static partial class BitwiseOperations
	{
		extension(ulong)
		{
			public static int BitSize => 64;
			public static int Size => 8;
			public static ulong Zero	=> 0ul;
			public static ulong One	=> 1ul;


			/// <summary>
			/// Get the index of lowest bit <c>1</c> in an <see cref="ulong"/> value
			/// </summary>
			/// <param name="source"></param>
			/// <returns>Index of the lowest <c>1</c> bit in <paramref name="source"/></returns>
			public static int GetLowBit(ulong source) => source == default ? -1 : BitOperations.TrailingZeroCount(source);

			/// <summary>
			/// Get the index of highest bit <c>1</c> in an <see cref="ulong"/> value
			/// </summary>
			/// <param name="source"></param>
			/// <returns>Index of the lowest <c>1</c> bit in <paramref name="source"/></returns>
			public static int GetUppBit(ulong source) => source == default ? -1 : 63 - BitOperations.LeadingZeroCount(source);


			public static UInt64BitEnumerator GetBits(ulong source) => new(source);



			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ulong Flag(int index) => 1ul << index;


			public static bool GetBit(ulong mask, int index)
			{
				mask &= 1ul << index;
				
				return mask is not default(ulong);
			}

			public static void SetBit(ref ulong mask, int index, bool value)
			{
				var flag = 1ul << index;

				if (value)
					mask |=  flag;
				else
					mask &= ~flag;
			}



		}

		

		extension(ulong source)
		{

			public int BitCount
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount(source);
			}

			public int LowBit
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => ulong.GetLowBit(source);
			}

			public int UppBit
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => ulong.GetUppBit(source);
			}

			public UInt64BitEnumerator Bits => ulong.GetBits(source);
		}


	}

}
