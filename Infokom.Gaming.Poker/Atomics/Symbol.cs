using Infokom.Numerics;
using Infokom.Numerics.Atomics;

using System.Collections;
using System.Collections.Immutable;
using System.Numerics;

namespace Infokom.Gaming.Poker.Atomics
{
	public enum Symbol : byte
	{
		Two		= ASCII.DIGIT_2,
		Three	= ASCII.DIGIT_3,
		Four		= ASCII.DIGIT_4,
		Five		= ASCII.DIGIT_5,
		Six		= ASCII.DIGIT_6,
		Seven	= ASCII.DIGIT_7,
		Eight	= ASCII.DIGIT_8,
		Nine		= ASCII.DIGIT_9,
		Ten		= ASCII.UPPERCASE_T,
		Jack		= ASCII.UPPERCASE_J,
		Queen	= ASCII.UPPERCASE_Q,
		King		= ASCII.UPPERCASE_K,
		Ace		= ASCII.UPPERCASE_A,
		Club		= ASCII.LOWERCASE_C,
		Diamond	= ASCII.LOWERCASE_D,
		Heart	= ASCII.LOWERCASE_H,
		Spade	= ASCII.LOWERCASE_S,
	}


	public static class SymbolExtensions
	{
		private static readonly ImmutableArray<Symbol> RANKS = [Symbol.Two, Symbol.Three, Symbol.Four, Symbol.Five, Symbol.Six, Symbol.Seven, Symbol.Eight, Symbol.Nine, Symbol.Ten, Symbol.Jack, Symbol.Queen, Symbol.King, Symbol.Ace];
		private static readonly ImmutableArray<Symbol> SUITS = [Symbol.Club, Symbol.Diamond, Symbol.Heart, Symbol.Spade];
		extension(Symbol source)
		{

			public bool IsRank => source is Symbol.Two or Symbol.Three or Symbol.Four or Symbol.Five or Symbol.Six or Symbol.Seven or Symbol.Eight or Symbol.Nine or Symbol.Ten or Symbol.Jack or Symbol.Queen or Symbol.King or Symbol.Ace;

			public bool IsSuit => source is Symbol.Club or Symbol.Diamond or Symbol.Heart or Symbol.Spade;



			public static ImmutableArray<Symbol> Ranks => RANKS;

			public static ImmutableArray<Symbol> Suits => SUITS;
		}
	}

	[Flags]
	public enum Symbols : uint
	{
		NONE		= 0b_0000000000000_0000,

		CLUB		= 0b_0000000000000_0001,
		DIAMOND	= 0b_0000000000000_0010,
		HEART	= 0b_0000000000000_0100,
		SPADE	= 0b_0000000000000_1000,
		SUITS	= 0b_0000000000000_1111,

		TWO		= 0b_0000000000001_0000,
		THREE	= 0b_0000000000010_0000,
		FOUR		= 0b_0000000000100_0000,
		FIVE		= 0b_0000000001000_0000,
		SIX		= 0b_0000000010000_0000,
		SEVEN	= 0b_0000000100000_0000,
		EIGHT	= 0b_0000001000000_0000,
		NINE		= 0b_0000010000000_0000,
		TEN		= 0b_0000100000000_0000,
		JACK		= 0b_0001000000000_0000,
		QUEEN	= 0b_0010000000000_0000,
		KING		= 0b_0100000000000_0000,
		ACE		= 0b_1000000000000_0000,
		RANKS	= 0b_1111111111111_0000,		

		ALL		= 0b_1111111111111_1111
	}

	
}
