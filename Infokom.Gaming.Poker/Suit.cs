namespace Infokom.Gaming.Poker
{
	//9824	♠	&spades;	&#9824;	Spade
	//9827	♣	&clubs;	&#9827;	Club
	//9829	♥	&hearts;	&#9829;	Heart
	//9830	♦	&diams;	&#9830;	Diamond

	// $S = \{s,d,c,h\}$
	public enum Suit : sbyte { Spade = 1, Diamond = 2, Club = 3, Heart = 4 }

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
