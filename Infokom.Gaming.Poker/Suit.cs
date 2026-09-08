using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker
{
	// $S = \{s,d,c,h\}$
	public enum Suit : byte { Spade = 0b0001, Diamond = 0b0010, Club = 0b0100, Heart = 0b1000 }

	public static class SuitExtensions
	{
		extension(Suit)
		{
			public static bool TryCast(in char source, out Suit target)
			{
				switch (source)
				{	
					case 's': target = Suit.Spade; return true;
					case 'd': target = Suit.Diamond; return true;
					case 'c': target = Suit.Club; return true;
					case 'h': target = Suit.Heart; return true;
					default: target = default; return false;
				}
			}

			public static Suit Cast(in char source) => source switch
			{
				's' => Suit.Spade,
				'd' => Suit.Diamond,
				'c' => Suit.Club,
				'h' => Suit.Heart,
				_ => throw new InvalidCastException($"Cannot cast from {source}.")
			};

			/// <summary>
			/// 
			/// </summary>
			/// <param name="source"></param>
			/// <param name="defaultResult"></param>
			/// <returns></returns>
			/// <remarks>
			/// In some scenarios, throwing an exception is not desirable. This method provides a way to safely cast
			/// by delegating the failure handling to the caller, allowing them to specify a default result.
			/// </remarks>	
			public static Suit Cast(in char source, Suit defaultResult) => source switch
			{
				's' => Suit.Spade,
				'd' => Suit.Diamond,
				'c' => Suit.Club,
				'h' => Suit.Heart,
				_ => defaultResult
			};

			public static bool TryParse(ReadOnlySpan<char> source, out Suit target)
			{
				if (source.Length != 1)
				{
					target = default;
					return false;
				}

				return Suit.TryCast(source[0], out target);
			}

			public static Suit Parse(ReadOnlySpan<char> source)
			{
				return source.Length == 1 ? Suit.Cast(source[0]) : throw new FormatException($"Cannot parse from {source}");
			}
		}
	}
}
