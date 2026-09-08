namespace Poker.Calc;

public static class HandRank
{
	public const int HandTypeShift = 24;

	public const int KickersWidth = 4;

	public const int TopKickerShirt = 16;

	public const int SecondKickerShift = 12;

	public const int ThirdKickerShift = 8;

	public const int FourthKickerShift = 4;

	public const int FifthKickerShift = 0;

	public const int PairRankShift = 16;

	public const int PairTopKickerShift = 12;

	public const int PairSecondKickerShift = 8;

	public const int PairThirdKickerShift = 4;

	public const int PairFourthKickerShift = 0;

	public const int TopPairRankShift = 16;

	public const int SecondPairRankShift = 12;

	public const int TwoPairsKickerShift = 8;

	public const int TripsRankShift = 16;

	public const int TripsTopKickerShift = 12;

	public const int TripsSecondKickerShift = 8;

	public const int StraightRankShift = 16;

	public const int FullHouseTripsRanksShift = 16;

	public const int FullHousePairRankShift = 12;

	public const int QuadsRankShift = 16;

	public const int QuadsKickerShift = 12;
}
