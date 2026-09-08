using BinarySerializer;

using Poker.Calc.Common;

namespace Poker.Calc;

[Games(PokerGames.Omaha)]
[BinarySerializable]
public record struct OmahaPreflopRange : IPreflopRange
{
	[Tag(1)]
	public OmahaPreflopRangePairs Pairs { get; init; }

	[Tag(2)]
	public OmahaPreflopRangeCards Cards { get; init; }

	[Tag(3)]
	public OmahaPreflopRangeSuiteness Suiteness { get; init; }

	[Tag(4)]
	public OmahaPreflopRangeStraightness Straightness { get; init; }

	[Tag(5)]
	public OmahaSixPreflopRangeCards SixCards { get; init; }

	[Tag(6)]
	public OmahaPreflopRangeOther Other { get; init; }

	public bool IsEmpty
	{
		get
		{
			if (Pairs == OmahaPreflopRangePairs.None && Cards == OmahaPreflopRangeCards.None && Suiteness == OmahaPreflopRangeSuiteness.None && Straightness == OmahaPreflopRangeStraightness.None && SixCards == OmahaSixPreflopRangeCards.None)
			{
				return Other == OmahaPreflopRangeOther.None;
			}
			return false;
		}
	}

	public bool IsNotEmpty => !IsEmpty;

	public OmahaPreflopRangeCards TopCardRank => Cards.Slice(OmahaPreflopRangeCards.TopCardRankAce, OmahaPreflopRangeCards.TopCardRankTwo);

	public OmahaPreflopRangeCards SecondCardRank => Cards.Slice(OmahaPreflopRangeCards.SecondCardRankAce, OmahaPreflopRangeCards.SecondCardRankTwo);

	public OmahaPreflopRangeCards ThirdCardRank => Cards.Slice(OmahaPreflopRangeCards.ThirdCardRankAce, OmahaPreflopRangeCards.ThirdCardRankTwo);

	public OmahaPreflopRangeCards FourthCardRank => Cards.Slice(OmahaPreflopRangeCards.FourthCardRankAce, OmahaPreflopRangeCards.FourthCardRankTwo);

	public OmahaSixPreflopRangeCards FifthCardRank => SixCards.Slice(OmahaSixPreflopRangeCards.FifthCardRankAce, OmahaSixPreflopRangeCards.FifthCardRankDeuce);

	public OmahaSixPreflopRangeCards SixthCardRank => SixCards.Slice(OmahaSixPreflopRangeCards.SixthCardRankAce, OmahaSixPreflopRangeCards.SixthCardRankDeuce);

	public const int OmahaUniquePreflopHands = 16432;

	public const int OmahaFivePreflopHands = 134459;

	public const int OmahaSixPreflopHands = 962988;

	public static OmahaPreflopRange Empty = new OmahaPreflopRange(OmahaPreflopRangePairs.None, OmahaPreflopRangeCards.None, OmahaPreflopRangeSuiteness.None, OmahaPreflopRangeStraightness.None, OmahaSixPreflopRangeCards.None, OmahaPreflopRangeOther.None);

	public OmahaPreflopRange(OmahaPreflopRangePairs pairs, OmahaPreflopRangeCards cards, OmahaPreflopRangeSuiteness suiteness, OmahaPreflopRangeStraightness straightness, OmahaSixPreflopRangeCards sixCards, OmahaPreflopRangeOther other)
	{
		Pairs = pairs;
		Cards = cards;
		Suiteness = suiteness;
		Straightness = straightness;
		SixCards = sixCards;
		Other = other;
	}

	public OmahaPreflopRange Merge(OmahaPreflopRange other)
	{
		return new OmahaPreflopRange(Pairs | other.Pairs, Cards | other.Cards, Suiteness | other.Suiteness, Straightness | other.Straightness, SixCards | other.SixCards, Other | other.Other);
	}
}
