namespace Poker.Calc;

[Flags]
public enum OmahaPreflopRangeOther : long
{
	None = 0L,
	NoDanglers = 1L,
	OneDangler = 2L,
	TwoDanglers = 4L,
	ThreeDanglersOrMore = 8L,
	Quads = 0x10L,
	Trips = 0x20L,
	NoTripsOrQuads = 0x40L,
	[Name("Top dangler A")]
	TopDanglerRankAce = 0x80L,
	[Name("Top dangler K")]
	TopDanglerRankKing = 0x100L,
	[Name("Top dangler Q")]
	TopDanglerRankQueen = 0x200L,
	[Name("Top dangler J")]
	TopDanglerRankJack = 0x400L,
	[Name("Top dangler T")]
	TopDanglerRankTen = 0x800L,
	[Name("Top dangler 9")]
	TopDanglerRankNine = 0x1000L,
	[Name("Top dangler 8")]
	TopDanglerRankEight = 0x2000L,
	[Name("Top dangler 7")]
	TopDanglerRankSeven = 0x4000L,
	[Name("Top dangler 6")]
	TopDanglerRankSix = 0x8000L,
	[Name("Top dangler 5")]
	TopDanglerRankFive = 0x10000L,
	[Name("Top dangler 4")]
	TopDanglerRankFour = 0x20000L,
	[Name("Top dangler 3")]
	TopDanglerRankThree = 0x40000L,
	[Name("Top dangler 2")]
	TopDanglerRankTwo = 0x80000L
}
