using Infokom.Numerics;
using Infokom.Numerics.Attributes;
using Infokom.Numerics.Extensions;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Infokom.Gaming.Poker.Atomics
{
	public enum Rank : byte
	{
		[ID<ulong>(1UL << 0x0), Index<int>(0x0), Label("2")] Two	= Symbol.Two,
		[ID<ulong>(1UL << 0x1), Index<int>(0x1), Label("3")] Three	= Symbol.Three,
		[ID<ulong>(1UL << 0x2), Index<int>(0x2), Label("4")] Four	= Symbol.Four,
		[ID<ulong>(1UL << 0x3), Index<int>(0x3), Label("5")] Five	= Symbol.Five,
		[ID<ulong>(1UL << 0x4), Index<int>(0x4), Label("6")] Six	= Symbol.Six,
		[ID<ulong>(1UL << 0x5), Index<int>(0x5), Label("7")] Seven	= Symbol.Seven,
		[ID<ulong>(1UL << 0x6), Index<int>(0x6), Label("8")] Eight	= Symbol.Eight,
		[ID<ulong>(1UL << 0x7), Index<int>(0x7), Label("9")] Nine	= Symbol.Nine,
		[ID<ulong>(1UL << 0x8), Index<int>(0x8), Label("T")] Ten	= Symbol.Ten,
		[ID<ulong>(1UL << 0x9), Index<int>(0x9), Label("J")] Jack	= Symbol.Jack,
		[ID<ulong>(1UL << 0xA), Index<int>(0xA), Label("Q")] Queen	= Symbol.Queen,
		[ID<ulong>(1UL << 0xB), Index<int>(0xB), Label("K")] King	= Symbol.King,
		[ID<ulong>(1UL << 0xC), Index<int>(0xC), Label("A")] Ace	= Symbol.Ace,
	}




	public static class RankExtensions
	{


		private static readonly ImmutableArray<Rank> VALUES = [Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace];

		extension(Rank)
		{
			public static ImmutableArray<Rank> Values => VALUES;

			public static int Count => 13;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ulong GetID(Rank rank) => rank switch
			{
				Rank.Two		=> 0b0000000000001,
				Rank.Three	=> 0b0000000000010,
				Rank.Four		=> 0b0000000000100,
				Rank.Five		=> 0b0000000001000,
				Rank.Six		=> 0b0000000010000,
				Rank.Seven	=> 0b0000000100000,
				Rank.Eight	=> 0b0000001000000,
				Rank.Nine		=> 0b0000010000000,
				Rank.Ten		=> 0b0000100000000,
				Rank.Jack		=> 0b0001000000000,
				Rank.Queen	=> 0b0010000000000,
				Rank.King		=> 0b0100000000000,
				Rank.Ace		=> 0b1000000000000,
				_			=> default
			};


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int IndexOf(Rank source)
			{
				var id = Rank.GetID(source);

				return id == default ? -1 : BitOperations.TrailingZeroCount(id); 
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static char SymbolOf(Rank source) => (char)source;


			/// <summary>
			/// Try to convert a <see cref="char"/> value to <see cref="Rank"/> value;
			/// </summary>
			/// <param name="symbol">Rank symbol</param>
			/// <param name="target">Rank value</param>
			/// <returns>True if <paramref name="target"/> is declared in <see cref="Rank"/> enumeration</returns>
			public static bool TryConvertFrom(char symbol, out Rank target)
			{
				target = ((Rank)symbol).IsKnown ? (Rank)symbol : default;


				return target.IsKnown;
			}
		}

		extension(Rank source)
		{
			public ulong ID
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.GetID(source);
			}

			public int Index 
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.IndexOf(source);
			}

			public char Symbol
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Rank.SymbolOf(source);
			}

			public bool IsKnown
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source.ID != default;
			}
		}
	}


}
