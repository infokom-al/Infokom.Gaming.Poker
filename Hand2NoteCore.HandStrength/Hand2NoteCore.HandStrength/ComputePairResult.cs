using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

public struct ComputePairResult
{
	public bool IsPair { get; }

	public PairTypes PairTypes { get; }

	public int KickerIndex { get; }

	public CardRanks PairRank { get; }

	public CardRanks KickerRank { get; }

	public ComputePairResult(bool isPair, PairTypes pairTypes, int kickerIndex, CardRanks pairRank, CardRanks kickerRank)
	{
		IsPair = isPair;
		PairTypes = pairTypes;
		KickerIndex = kickerIndex;
		PairRank = pairRank;
		KickerRank = kickerRank;
	}
}
