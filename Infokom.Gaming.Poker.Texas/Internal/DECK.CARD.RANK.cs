namespace Holdem.Core.Internal
{

	internal static partial class DECK
	{
		internal static partial class CARD
		{
			public static class RANK
			{
				public const int COUNT = 13;

				public enum ID : int
				{
					TWO		= 0,
					THREE,
					FOUR,
					FIVE,
					SIX,
					SEVEN,
					EIGHT,
					NINE,
					TEN,
					JACK,
					QUEEN,
					KING,
					ACE,
				}

				public enum FLAG : uint
				{
					TWO		= 1U << ID.TWO,
					THREE	= 1U << ID.THREE,
					FOUR		= 1U << ID.FOUR,
					FIVE		= 1U << ID.FIVE,
					SIX		= 1U << ID.SIX,
					SEVEN	= 1U << ID.SEVEN,
					EIGHT	= 1U << ID.EIGHT,
					NINE		= 1U << ID.NINE,
					TEN		= 1U << ID.TEN,
					JACK		= 1U << ID.JACK,
					QUEEN	= 1U << ID.QUEEN,
					KING		= 1U << ID.KING,
					ACE		= 1U << ID.ACE,
				}

				public enum SYMBOL : ushort
				{
					TWO		= 0X32,
					THREE	= 0X33,
					FOUR		= 0X34,
					FIVE		= 0X35,
					SIX		= 0X36,
					SEVEN	= 0X37,
					EIGHT	= 0X38,
					NINE		= 0X39,
					TEN		= 0X54,
					JACK		= 0X4A,
					QUEEN	= 0X51,
					KING		= 0X4B,
					ACE		= 0X41
				}
			}
		}
	}
}
