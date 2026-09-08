namespace Poker.Calc;

internal static class CardMaskCactus
{
	public static int ToCardMaskCactus(this Card card)
	{
		return Tables.CardsMasksCactus[card.Index];
	}

	public static int CardIndexToCardMaskCactus(this int cardIndex)
	{
		return Tables.CardsMasksCactus[cardIndex];
	}
}
