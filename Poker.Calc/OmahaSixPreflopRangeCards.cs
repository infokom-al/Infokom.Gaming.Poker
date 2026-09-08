namespace Poker.Calc;

[Flags]
public enum OmahaSixPreflopRangeCards : long
{
	None = 0L,
	[Name("5th card A")]
	FifthCardRankAce = 1L,
	[Name("5th card K")]
	FifthCardRankKing = 2L,
	[Name("5th card Q")]
	FifthCardRankQueen = 4L,
	[Name("5th card J")]
	FifthCardRankJack = 8L,
	[Name("5th card T")]
	FifthCardRankTen = 0x10L,
	[Name("5th card 9")]
	FifthCardRankNine = 0x20L,
	[Name("5th card 8")]
	FifthCardRankEight = 0x40L,
	[Name("5th card 7")]
	FifthCardRankSeven = 0x80L,
	[Name("5th card 6")]
	FifthCardRankSix = 0x100L,
	[Name("5th card 5")]
	FifthCardRankFive = 0x200L,
	[Name("5th card 4")]
	FifthCardRankFour = 0x400L,
	[Name("5th card 3")]
	FifthCardRankThree = 0x800L,
	[Name("5th card 2")]
	FifthCardRankDeuce = 0x1000L,
	[Name("6th card A")]
	SixthCardRankAce = 0x2000L,
	[Name("6th card K")]
	SixthCardRankKing = 0x4000L,
	[Name("6th card Q")]
	SixthCardRankQueen = 0x8000L,
	[Name("6th card J")]
	SixthCardRankJack = 0x10000L,
	[Name("6th card T")]
	SixthCardRankTen = 0x20000L,
	[Name("6th card 9")]
	SixthCardRankNine = 0x40000L,
	[Name("6th card 8")]
	SixthCardRankEight = 0x80000L,
	[Name("6th card 7")]
	SixthCardRankSeven = 0x100000L,
	[Name("6th card 6")]
	SixthCardRankSix = 0x200000L,
	[Name("6th card 5")]
	SixthCardRankFive = 0x400000L,
	[Name("6th card 4")]
	SixthCardRankFour = 0x800000L,
	[Name("6th card 3")]
	SixthCardRankThree = 0x1000000L,
	[Name("6th card 2")]
	SixthCardRankDeuce = 0x2000000L
}
