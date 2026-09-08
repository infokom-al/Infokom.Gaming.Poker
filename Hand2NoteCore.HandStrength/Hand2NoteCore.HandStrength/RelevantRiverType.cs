using System;
using System.ComponentModel;

namespace Hand2NoteCore.HandStrength;

[Flags]
public enum RelevantRiverType : ulong
{
	[Description("Card A")]
	RiverCardA = 1uL,
	[Description("Card K")]
	RiverCardK = 2uL,
	[Description("Card Q")]
	RiverCardQ = 4uL,
	[Description("Card J")]
	RiverCardJ = 8uL,
	[Description("Card T")]
	RiverCardT = 0x10uL,
	[Description("Card 9")]
	RiverCard9 = 0x20uL,
	[Description("Card 8")]
	RiverCard8 = 0x40uL,
	[Description("Card 7")]
	RiverCard7 = 0x80uL,
	[Description("Card 6")]
	RiverCard6 = 0x100uL,
	[Description("Card 5")]
	RiverCard5 = 0x200uL,
	[Description("Card 4")]
	RiverCard4 = 0x400uL,
	[Description("Card 3")]
	RiverCard3 = 0x800uL,
	[Description("Card 2")]
	RiverCard2 = 0x1000uL,
	[Description("High card A")]
	HighCardA = 0x2000uL,
	[Description("High card K")]
	HighCardK = 0x4000uL,
	[Description("High card Q")]
	HighCardQ = 0x10000uL,
	[Description("High card J")]
	HighCardJ = 0x20000uL,
	[Description("High card T")]
	HighCardT = 0x40000uL,
	[Description("High card 9")]
	HighCard9 = 0x80000uL,
	[Description("High card 8")]
	HighCard8 = 0x100000uL,
	[Description("High card 7")]
	HighCard7 = 0x200000uL,
	[Description("High card 6")]
	HighCard6 = 0x400000uL,
	[Description("High card 5")]
	HighCard5 = 0x800000uL,
	[Description("High card 4")]
	HighCard4 = 0x1000000uL,
	[Description("High card 3")]
	HighCard3 = 0x2000000uL,
	[Description("2nd card K")]
	SecondCardK = 0x4000000uL,
	[Description("2nd card Q")]
	SecondCardQ = 0x8000000uL,
	[Description("2nd card J")]
	SecondCardJ = 0x10000000uL,
	[Description("2nd card T")]
	SecondCardT = 0x20000000uL,
	[Description("2nd card 9")]
	SecondCard9 = 0x40000000uL,
	[Description("2nd card 8")]
	SecondCard8 = 0x80000000uL,
	[Description("2nd card 7")]
	SecondCard7 = 0x100000000uL,
	[Description("2nd card 6")]
	SecondCard6 = 0x200000000uL,
	[Description("2nd card 5")]
	SecondCard5 = 0x400000000uL,
	[Description("2nd card 4")]
	SecondCard4 = 0x800000000uL,
	[Description("2nd card 3")]
	SecondCard3 = 0x1000000000uL,
	[Description("2nd card 2")]
	SecondCard2 = 0x2000000000uL,
	[Description("5-flush")]
	FiveFlushCardOnBoard = 0x4000000000uL,
	[Description("4-flush")]
	FourFlushCardsOnBoard = 0x8000000000uL,
	[Description("Third card to flush")]
	ThirdCardToFlush = 0x10000000000uL,
	[Description("5-straight")]
	StraightOnBoard = 0x20000000000uL,
	[Description("4-straight with 1 gap")]
	FourCardInRowWith1Gap = 0x40000000000uL,
	[Description("River is an overcard")]
	RiverIsOvercard = 0x80000000000uL,
	[Description("Four card in a row")]
	FourCardsInARow = 0x100000000000uL,
	AllCardsOfDifferentRank = 0x200000000000uL,
	[Description("xxyzw")]
	xxyzw = 0x400000000000uL,
	[Description("xxyyz")]
	xxyyz = 0x800000000000uL,
	[Description("xxxyz")]
	xxxyz = 0x1000000000000uL,
	[Description("xxxyy")]
	xxxyy = 0x2000000000000uL,
	[Description("xxxxy")]
	xxxxy = 0x4000000000000uL,
	[Description("Third card to backdoor flush")]
	ThirdCardToBackdoorFlush = 0x8000000000000uL,
	[Description("Straight possible")]
	StraightPossible = 0x10000000000000uL,
	[Description("Paired top card")]
	PairedTopCard = 0x20000000000000uL,
	[Description("Paired second card")]
	PairedSecondCard = 0x40000000000000uL,
	[Description("Paired third card")]
	PairedThirdCard = 0x80000000000000uL,
	[Description("Paired bottom card")]
	PairedBottomCard = 0x100000000000000uL,
	[Description("Top two paired")]
	TopTwoPaired = 0x200000000000000uL,
	[Description("Top and bottom paired")]
	TopAndBottomPaired = 0x400000000000000uL,
	[Description("Bottom two paired")]
	BottomTwoPaired = 0x800000000000000uL,
	[Description("River paired board")]
	RiverPairedBoard = 0x1000000000000000uL,
	RiverCardFiveOrLess = 0x1E00uL,
	[Description("Five or less")]
	HighCardFiveOrLess = 0x3800000uL,
	[Description("Five or less")]
	SecondCardFiveOrLess = 0x3C00000000uL
}
