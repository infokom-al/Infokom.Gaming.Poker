namespace Poker.Calc;

[Flags]
public enum OmahaPreflopRangeStraightness : long
{
	None = 0L,
	[Name("3-card rundown")]
	ThreeCardRundown = 1L,
	[Name("4-card rundown")]
	FourCardRundown = 2L,
	[Name("5-card rundown")]
	FiveCardRundown = 4L,
	[Name("6-card rundown")]
	SixCardRundown = 8L,
	DoubleRundown = 0x10L,
	RundownWithNoGap = 0x20L,
	RundownWithOneGap = 0x40L,
	RundownWithTwoGaps = 0x80L,
	RundownWithDoubleGap = 0x100L,
	[Name("Top rundown rank A")]
	TopRundownRankAce = 0x200L,
	[Name("Top rundown rank K")]
	TopRundownRankKing = 0x400L,
	[Name("Top rundown rank Q")]
	TopRundownRankQueen = 0x800L,
	[Name("Top rundown rank J")]
	TopRundownRankJack = 0x1000L,
	[Name("Top rundown rank T")]
	TopRundownRankTen = 0x2000L,
	[Name("Top rundown rank 9")]
	TopRundownRankNine = 0x4000L,
	[Name("Top rundown rank 8")]
	TopRundownRankEight = 0x8000L,
	[Name("Top rundown rank 7")]
	TopRundownRankSeven = 0x10000L,
	[Name("Top rundown rank 6")]
	TopRundownRankSix = 0x20000L,
	[Name("Top rundown rank 5")]
	TopRundownRankFive = 0x40000L,
	[Name("Top rundown rank 4")]
	TopRundownRankFour = 0x80000L,
	[Name("2nd rundown rank K")]
	SecondRundownRankKing = 0x100000L,
	[Name("2nd rundown rank Q")]
	SecondRundownRankQueen = 0x200000L,
	[Name("2nd rundown rank J")]
	SecondRundownRankJack = 0x400000L,
	[Name("2nd rundown rank T")]
	SecondRundownRankTen = 0x800000L,
	[Name("2nd rundown rank 9")]
	SecondRundownRankNine = 0x1000000L,
	[Name("2nd rundown rank 8")]
	SecondRundownRankEight = 0x2000000L,
	[Name("2nd rundown rank 7")]
	SecondRundownRankSeven = 0x4000000L,
	[Name("2nd rundown rank 6")]
	SecondRundownRankSix = 0x8000000L,
	[Name("2nd rundown rank 5")]
	SecondRundownRankFive = 0x10000000L,
	[Name("2nd rundown rank 4")]
	SecondRundownRankFour = 0x20000000L
}
