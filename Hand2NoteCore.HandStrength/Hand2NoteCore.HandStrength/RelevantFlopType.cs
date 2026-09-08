using System;
using System.ComponentModel;

namespace Hand2NoteCore.HandStrength;

[Flags]
public enum RelevantFlopType : long
{
	[Description("High card A")]
	HighCardA = 1L,
	[Description("High card K")]
	HighCardK = 2L,
	[Description("High card Q")]
	HighCardQ = 4L,
	[Description("High card J")]
	HighCardJ = 8L,
	[Description("High card T")]
	HighCardT = 0x10L,
	[Description("High card 9")]
	HighCard9 = 0x20L,
	[Description("High card 8")]
	HighCard8 = 0x40L,
	[Description("High card 7")]
	HighCard7 = 0x80L,
	[Description("High card 6")]
	HighCard6 = 0x100L,
	[Description("High card 5")]
	HighCard5 = 0x200L,
	[Description("High card 4")]
	HighCard4 = 0x400L,
	[Description("High card 3")]
	HighCard3 = 0x800L,
	[Description("High card 2")]
	HighCard2 = 0x1000L,
	[Description("2nd card K")]
	SecondCardK = 0x2000L,
	[Description("2nd card Q")]
	SecondCardQ = 0x4000L,
	[Description("2nd card J")]
	SecondCardJ = 0x8000L,
	[Description("2nd card T")]
	SecondCardT = 0x10000L,
	[Description("2nd card 9")]
	SecondCard9 = 0x20000L,
	[Description("2nd card 8")]
	SecondCard8 = 0x40000L,
	[Description("2nd card 7")]
	SecondCard7 = 0x80000L,
	[Description("2nd card 6")]
	SecondCard6 = 0x100000L,
	[Description("2nd card 5")]
	SecondCard5 = 0x200000L,
	[Description("2nd card 4")]
	SecondCard4 = 0x400000L,
	[Description("2nd card 3")]
	SecondCard3 = 0x800000L,
	[Description("2nd card 2")]
	SecondCard2 = 0x1000000L,
	[Description("3rd card Q")]
	ThirdCardQ = 0x2000000L,
	[Description("3rd card J")]
	ThirdCardJ = 0x4000000L,
	[Description("3rd card T")]
	ThirdCardT = 0x8000000L,
	[Description("3rd card 9")]
	ThirdCard9 = 0x10000000L,
	[Description("3rd card 8")]
	ThirdCard8 = 0x20000000L,
	[Description("3rd card 7")]
	ThirdCard7 = 0x40000000L,
	[Description("3rd card 6")]
	ThirdCard6 = 0x80000000L,
	[Description("3rd card 5")]
	ThirdCard5 = 0x100000000L,
	[Description("3rd card 4")]
	ThirdCard4 = 0x200000000L,
	[Description("3rd card 3")]
	ThirdCard3 = 0x400000000L,
	[Description("3rd card 2")]
	ThirdCard2 = 0x800000000L,
	[Description("Three on board")]
	ThreeOnBoard = 0x1000000000L,
	[Description("Board paired above second card")]
	BoardPairedAboveSecondCard = 0x2000000000L,
	[Description("Board paired below second card")]
	BoardPairedBelowSecondCard = 0x4000000000L,
	[Description("All three card of one suit")]
	AllThreeCardsOfOneSuit = 0x8000000000L,
	[Description("Exactly two cards of one suit")]
	ExactlyTwoCardsOfOneSuit = 0x10000000000L,
	[Description("All three cards of different suit")]
	AllThreeCardsOfDifferentSuit = 0x20000000000L,
	[Description("All three cards in a row")]
	AllThreeCardsInARow = 0x40000000000L,
	[Description("Cards in a row with 1 gap")]
	CardsAreInARow1Gap = 0x80000000000L,
	[Description("Cards in a row with 2 gaps")]
	CardsAreInARow2Gap = 0x100000000000L,
	[Description("At least two cards are connected")]
	AnyTwoCardsAreConnected = 0x200000000000L,
	[Description("At least two cards are gapped")]
	TwoCardsAreGaped = 0x400000000000L,
	[Description("All cards of different rank")]
	AllCardsOfDifferentRank = 0x800000000000L,
	[Description("Straight possible")]
	StraightPossible = 0x1000000000000L,
	[Description("High card 5-2")]
	HighCardFiveOrLess = 0x1E00L,
	[Description("2nd card 5-2")]
	SecondCardFiveOrLess = 0x1E00000L,
	[Description("3rd card 5-2")]
	ThirdCardFiveOrLess = 0xF00000000L
}
