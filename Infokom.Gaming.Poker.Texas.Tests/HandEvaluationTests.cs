using Xunit;

namespace Infokom.Gaming.Poker.Texas.Tests;

public sealed class HandEvaluationTests
{
	public static TheoryData<Hand.Family, string[]> HandsByFamily => new()
	{
		{ Hand.Family.HighCard, ["As", "Kd", "Qc", "Jh", "9s", "7d", "3c"] },
		{ Hand.Family.OnePair, ["As", "Ad", "Kc", "Qh", "9s", "7d", "3c"] },
		{ Hand.Family.TwoPair, ["As", "Ad", "Kc", "Kh", "9s", "7d", "3c"] },
		{ Hand.Family.ThreeOfAKind, ["As", "Ad", "Ac", "Kh", "9s", "7d", "3c"] },
		{ Hand.Family.Straight, ["9s", "8d", "7c", "6h", "5s", "Kd", "Ac"] },
		{ Hand.Family.Flush, ["As", "Js", "9s", "6s", "3s", "Kd", "2c"] },
		{ Hand.Family.FullHouse, ["As", "Ad", "Ac", "Ks", "Kd", "7c", "2h"] },
		{ Hand.Family.FourOfAKind, ["As", "Ad", "Ac", "Ah", "Kd", "7c", "2h"] },
		{ Hand.Family.StraightFlush, ["9s", "8s", "7s", "6s", "5s", "Kd", "Ac"] },
		{ Hand.Family.RoyalFlush, ["As", "Ks", "Qs", "Js", "Ts", "Kd", "2c"] },
	};

	[Theory]
	[MemberData(nameof(HandsByFamily))]
	public void Evaluate_RecognizesEachHandFamily(Hand.Family expected, string[] source)
	{
		var ranking = Hand.Evaluate(Cards(source));

		Assert.Equal(expected, ranking.Family);
	}


	[Theory]
	[InlineData("As", "Ad", "Ac", "Ks", "Kd")]
	[InlineData("As", "Ad", "Ac", "Ks", "Kd", "Kh")]
	public void Evaluate_RecognizesFullHouseWithoutAnUnrelatedKicker(params string[] source)
	{
		var ranking = Hand.Evaluate(Cards(source));

		Assert.Equal(Hand.Family.FullHouse, ranking.Family);
	}

	[Fact]
	public void CardParse_UsesDistinctSixteenBitSuitLanes()
	{
		Assert.Equal(Card.SA, Card.Parse("As"));
		Assert.Equal(Card.DA, Card.Parse("Ad"));
		Assert.Equal(Card.CA, Card.Parse("Ac"));
		Assert.Equal(Card.HA, Card.Parse("Ah"));
	}

	[Fact]
	public void Table_Evaluate_ReturnsRankingsIndexedBySeat()
	{
		var table = new Table();
		table.OccupySeat(0);
		table.OccupySeat(3);

		foreach (var card in new[] { "As", "Kd", "Ks", "Kc", "5s", "6s", "7s", "Ah", "Kh" })
			table.OnNext(Card.Parse(card));

		var rankings = table.Evaluate();

		Assert.Equal(Table.MaxSeats, rankings.Length);
		Assert.Equal(Hand.Family.Flush, rankings[0].Family);
		Assert.Equal(Hand.Family.ThreeOfAKind, rankings[3].Family);
		Assert.Equal(default, rankings[1]);
		Assert.Equal(default, rankings[9]);
	}

	private static Cards Cards(IEnumerable<string> source)
	{
		var cards = Poker.Cards.Φ;
		foreach (var text in source)
			cards = cards.Include(Card.Parse(text));

		return cards;
	}
}
