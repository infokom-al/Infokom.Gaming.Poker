using Xunit;

namespace Infokom.Gaming.Poker.Texas.Tests;

public sealed class DeckTests
{
	[Fact]
	public void NewDeck_ContainsAllFiftyTwoCards()
	{
		var deck = Deck.NewDeck();

		Assert.Equal(52, deck.Count);
	}

	[Fact]
	public void Draw_DrawsEveryCardExactlyOnce()
	{
		var deck = Deck.NewDeck();
		var drawn = Cards.Φ;

		for (int i = 0; i < 52; i++)
		{
			var card = deck.Draw();
			Assert.False(drawn.IsIncluded(card));
			drawn = drawn.Include(card);
			Assert.Equal(51 - i, deck.Count);
		}

		Assert.Equal(Cards.Ω, drawn);
		Assert.True(deck.IsEmpty);
		Assert.Throws<InvalidOperationException>(() => deck.Draw());
	}
}
