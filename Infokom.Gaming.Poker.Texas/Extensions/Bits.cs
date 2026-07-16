using System.Numerics;
using System.Runtime.CompilerServices;

namespace Holdem.Core.Internal
{
	internal static class Bits
	{
		extension(uint source)
		{
			/// <inheritdoc cref="BitOperations.TrailingZeroCount(uint)"/>
			public int CTZ
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.TrailingZeroCount(source);
			}

			/// <inheritdoc cref="BitOperations.LeadingZeroCount(uint)"/>
			public int CLZ
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.LeadingZeroCount(source);
			}

			

			/// <inheritdoc cref="BitOperations.PopCount(uint))"/>
			public int CNT
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitOperations.PopCount(source);
			}
		}

	}
}
