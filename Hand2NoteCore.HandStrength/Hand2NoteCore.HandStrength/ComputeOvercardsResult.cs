using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

public struct ComputeOvercardsResult
{
	public bool IsHighCard { get; }

	public OvercardsTypes Type { get; }

	public CardRanks HighCardRank { get; }

	public bool IsTwoOvercards => Type == OvercardsTypes.TwoOvercards;

	public bool IsOneOvercard => Type == OvercardsTypes.OneOvercard;

	public bool IsNoOvercards => Type == OvercardsTypes.NoOvercards;

	public ComputeOvercardsResult(bool isHighCard, OvercardsTypes type, CardRanks highCardRank)
	{
		IsHighCard = isHighCard;
		HighCardRank = highCardRank;
		Type = type;
	}
}
