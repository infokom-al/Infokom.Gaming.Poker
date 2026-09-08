using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

public struct ComputeDrawResult
{
	public bool IsNutsHighFlushDraw { get; }

	public bool IsSecondNutsHighFlushDraw { get; }

	public bool IsThirdNutsHighFlushDraw { get; }

	public bool IsFourthOrLowerNutsHighFlushDraw { get; }

	public bool IsFlushDraw { get; }

	public bool IsStraightDraw { get; }

	public bool IsBottomStraightDrawWithOnePocketCard { get; }

	public bool IsTwoPocketsBackdoor { get; }

	public bool IsTopFlushBackdoor { get; }

	public bool IsTurnedFlushDrawBackdoor { get; }

	public bool IsGutshot { get; }

	public CardRanks GutshotOutRank { get; }

	public bool IsDoubleFlushDraw { get; }

	public int FlushOutsCount { get; }

	public int StraightOutsCount { get; }

	public int BackdoorCount { get; }

	public ComputeDrawResult(bool isBottomStraightDrawWithOnePocketCard, bool isNutsHighFlushDraw, bool isSecondNutsHighFlushDraw, bool isThirdNutsHighFlushDraw, bool isFourthOrLowerNutsHighFlushDraw, bool isFlushDraw, bool isStraightDraw, bool isTwoPocketsBackdoor, bool isTopFlushBackdoor, bool isTurnedFlushDrawBackdoor, bool isGutshot, CardRanks gutshotOutRank, bool isDoubleFlushDraw, int flushOutsCount, int straightOutsCount, int backdoorCount)
	{
		IsBottomStraightDrawWithOnePocketCard = isBottomStraightDrawWithOnePocketCard;
		IsNutsHighFlushDraw = isNutsHighFlushDraw;
		IsFlushDraw = isFlushDraw;
		IsStraightDraw = isStraightDraw;
		IsTwoPocketsBackdoor = isTwoPocketsBackdoor;
		IsTopFlushBackdoor = isTopFlushBackdoor;
		IsTurnedFlushDrawBackdoor = isTurnedFlushDrawBackdoor;
		IsGutshot = isGutshot;
		GutshotOutRank = gutshotOutRank;
		IsDoubleFlushDraw = isDoubleFlushDraw;
		FlushOutsCount = flushOutsCount;
		StraightOutsCount = straightOutsCount;
		BackdoorCount = backdoorCount;
		IsSecondNutsHighFlushDraw = isSecondNutsHighFlushDraw;
		IsThirdNutsHighFlushDraw = isThirdNutsHighFlushDraw;
		IsFourthOrLowerNutsHighFlushDraw = isFourthOrLowerNutsHighFlushDraw;
	}
}
