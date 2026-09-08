using System;
using System.ComponentModel;

namespace Hand2NoteCore.HandStrength;

[Flags]
public enum RelevantTurnType : ulong
{
	[Description("Card A")]
	TurnCardA = 1uL,
	[Description("Card K")]
	TurnCardK = 2uL,
	[Description("Card Q")]
	TurnCardQ = 4uL,
	[Description("Card J")]
	TurnCardJ = 8uL,
	[Description("Card T")]
	TurnCardT = 0x10uL,
	[Description("Card 9")]
	TurnCard9 = 0x20uL,
	[Description("Card 8")]
	TurnCard8 = 0x40uL,
	[Description("Card 7")]
	TurnCard7 = 0x80uL,
	[Description("Card 6")]
	TurnCard6 = 0x100uL,
	[Description("Card 5")]
	TurnCard5 = 0x200uL,
	[Description("Card 4")]
	TurnCard4 = 0x400uL,
	[Description("Card 3")]
	TurnCard3 = 0x800uL,
	[Description("Card 2")]
	TurnCard2 = 0x1000uL,
	[Description("High card A")]
	HighCardA = 0x2000uL,
	[Description("High card K")]
	HighCardK = 0x4000uL,
	[Description("High card Q")]
	HighCardQ = 0x8000uL,
	[Description("High card J")]
	HighCardJ = 0x10000uL,
	[Description("High card T")]
	HighCardT = 0x20000uL,
	[Description("High card 9")]
	HighCard9 = 0x40000uL,
	[Description("High card 8")]
	HighCard8 = 0x80000uL,
	[Description("High card 7")]
	HighCard7 = 0x100000uL,
	[Description("High card 6")]
	HighCard6 = 0x200000uL,
	[Description("High card 5")]
	HighCard5 = 0x400000uL,
	[Description("High card 4")]
	HighCard4 = 0x800000uL,
	[Description("High card 3")]
	HighCard3 = 0x1000000uL,
	[Description("High card 2")]
	HighCard2 = 0x2000000uL,
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
	[Description("4-flush")]
	AllFourCardsOfOneSuit = 0x4000000000uL,
	[Description("Third card to flush")]
	TurnIsTheThirdCardOfSuit = 0x8000000000uL,
	[Description("Rainbow")]
	FourDifferentSuitOnBoard = 0x10000000000uL,
	[Description("Straight")]
	AllFourCardsInRow = 0x20000000000uL,
	[Description("Straight with 1 gap")]
	AllFourCardsInRowWithGap = 0x40000000000uL,
	[Description("Turn is an overcard")]
	TurnIsAnOverCard = 0x80000000000uL,
	[Description("At least 2 cards with 1 gap")]
	AtLeast2CardsWith1Gap = 0x100000000000uL,
	AllCardsOfDifferentRank = 0x200000000000uL,
	[Description("xxyz")]
	xxyz = 0x400000000000uL,
	[Description("xxyy")]
	xxyy = 0x800000000000uL,
	[Description("xxxy")]
	xxxy = 0x1000000000000uL,
	[Description("xxxx")]
	xxxx = 0x2000000000000uL,
	[Description("Two flush draw")]
	TwoFlushDraw = 0x4000000000000uL,
	[Description("Straight possible")]
	StraightPossible = 0x8000000000000uL,
	[Description("Five or less")]
	TurnCardFiveOrLess = 0x1E00uL,
	[Description("Five or less")]
	HighCardFiveOrLess = 0x3C00000uL,
	[Description("Five or less")]
	SecondCardFiveOrLess = 0x3C00000000uL
}
