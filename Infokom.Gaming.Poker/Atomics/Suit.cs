using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System.Collections;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Atomics
{
	public enum Suit : byte
	{
		Club		= Symbol.Club,
		Diamond	= Symbol.Diamond,
		Heart	= Symbol.Heart,
		Spade	= Symbol.Spade,
	}


	public static class SuitExtensions
	{
		private static readonly ImmutableArray<Suit> VALUES = [Suit.Club, Suit.Diamond, Suit.Heart, Suit.Spade];

		extension(Suit)
		{

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int IndexOf(Suit source)
			{
				var id = source.ID;

				return id == default ? -1 : BitOperations.TrailingZeroCount(id);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static char SymbolOf(Suit suit) => (char)suit;




			/// <summary>
			/// Try to convert a <see cref="char"/> value to <see cref="Rank"/> value;
			/// </summary>
			/// <param name="symbol">Rank symbol</param>
			/// <param name="target">Rank value</param>
			/// <returns>True if <paramref name="target"/> is declared in <see cref="Rank"/> enumeration</returns>
			public static bool TryConvertFrom(char symbol, out Suit target)
			{
				target = ((Suit)symbol).IsKnown ? (Suit)symbol : default;


				return target.IsKnown;
			}




			public static int Count => 4;


			public static Suit Of(Index index) => index.Value switch
			{
				0 => Suit.Club,
				1 => Suit.Diamond,
				2 => Suit.Heart,
				3 => Suit.Spade,
				_ => throw new IndexOutOfRangeException(),
			};

			public static ImmutableArray<Suit> Values => VALUES;
		}


		extension(Suit source)
		{
			public ulong ID
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source switch
				{
					Suit.Club      => 0b0001,
					Suit.Diamond   => 0b0010,
					Suit.Heart     => 0b0100,
					Suit.Spade     => 0b1000,
					_			=> 0b0000,
				};
			}

			public int Index
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source switch
				{
					Suit.Club		=>  0,
					Suit.Diamond	=>  1,
					Suit.Heart	=>  2,
					Suit.Spade	=>  3,
					_			=> -1,
				};
			}

			public char Symbol
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (char)source;
			}

			public bool IsKnown
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source.ID != default;
			}
		}
	}

	
}
