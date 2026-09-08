using System;
using System.ComponentModel;

namespace Hand2NoteCore.HandStrength;

[Flags]
public enum RelevantHandValue : long
{
	[Description("A high")]
	AceHigh = 1L,
	[Description("K high")]
	KingHigh = 2L,
	[Description("Q high")]
	QueenHigh = 4L,
	[Description("J high")]
	JackHigh = 8L,
	[Description("T high")]
	TenHigh = 0x10L,
	[Description("9 high")]
	NineHigh = 0x20L,
	[Description("8 high")]
	EightHigh = 0x40L,
	[Description("7 high")]
	SevenHigh = 0x80L,
	[Description("6 high")]
	SixHigh = 0x100L,
	[Description("5 high")]
	FiveHigh = 0x200L,
	[Description("4 high")]
	FourHigh = 0x400L,
	[Description("3 high")]
	ThreeHigh = 0x800L,
	[Description("2 high")]
	DeuceHigh = 0x1000L,
	[Description("Air")]
	Air = 0x2000L,
	[Description("Air with two overcards")]
	AirWithTwoOverCards = 0x4000L,
	[Description("Air with one overcard")]
	AirWithOneOverCard = 0x8000L,
	[Description("Air without overcards")]
	AirWithoutOverCards = 0x10000L,
	[Description("Top pair")]
	TopPair = 0x20000L,
	[Description("Middle pair")]
	MiddlePair = 0x40000L,
	[Description("Low pair")]
	LowPair = 0x80000L,
	[Description("Top pair top kicker")]
	TopPairTopKicker = 0x100000L,
	[Description("Top pair second kicker")]
	TopPairSecondKicker = 0x200000L,
	[Description("Top pair third kicker")]
	TopPairThirdOrLessKicker = 0x400000L,
	[Description("Middle pair top kicker")]
	MiddlePairTopKicker = 0x800000L,
	[Description("Overpair")]
	OverPair = 0x1000000L,
	[Description("Pocket middle pair")]
	PocketMiddlePair = 0x2000000L,
	[Description("Pocket low pair")]
	PocketLowPair = 0x4000000L,
	[Description("Two pairs")]
	TwoPairs = 0x8000000L,
	[Description("Trips")]
	Trips = 0x10000000L,
	[Description("Set")]
	Set = 0x20000000L,
	[Description("Straight")]
	Straight = 0x40000000L,
	[Description("Flush")]
	Flush = 0x80000000L,
	[Description("Full house")]
	FullHouse = 0x100000000L,
	[Description("Quads")]
	Quads = 0x200000000L,
	[Description("Straight flush")]
	StraightFlush = 0x400000000L,
	BestPossibleHandOnBoard = 0x800000000L,
	[Description("Flush draw")]
	FlushDraw = 0x1000000000L,
	[Description("Ace high flush draw")]
	NutsFlushDraw = 0x2000000000L,
	[Description("Backdoor flush draw")]
	BackdoorFlushDraw = 0x4000000000L,
	[Description("Double flush draw")]
	DoubleFlushDraw = 0x8000000000L,
	[Description("Straight draw")]
	StraightDraw = 0x10000000000L,
	[Description("Gutshot")]
	Gutshot = 0x20000000000L,
	[Description("Combo draw with pair")]
	ComboDrawWithPair = 0x40000000000L,
	[Description("Combo draw without pair")]
	ComboDrawWithoutPair = 0x80000000000L,
	[Description("Pair with Q high kicker")]
	PairQhigh = 0x100000000000L,
	[Description("Second nuts flush draw")]
	SecondNutsFlushDraw = 0x200000000000L,
	[Description("Third nuts flush draw")]
	ThirdNutsFlushDraw = 0x400000000000L,
	[Description("Fourth or lower nuts flush draw")]
	FourthOrLowerNutsFlushDraw = 0x800000000000L,
	[Description("Bottom Straight draw with one hero card")]
	BottomStraightDrawOneHeroCard = 0x4000000000000L,
	AllHands = 0x7FFFFL,
	[Description("Combo draw")]
	ComboDraw = 0xC0000000000L,
	TripsOrHigherHoldem = 0x7F0000000L,
	TwoPairOrHigherHoldem = 0x7F8000000L,
	[Description("J-2 high")]
	JackHighOrLower = 0x1FF8L,
	[Description("Full house or higher")]
	FullHouseOrHigher = 0x700000000L,
	[Description("Flush or higher")]
	FlushOrHigher = 0x680000000L,
	AnyPair = 0x70E0000L,
	AnyDraw = 0x31000000000L,
	DeuceToTenHigh = 0x1FF0L
}
