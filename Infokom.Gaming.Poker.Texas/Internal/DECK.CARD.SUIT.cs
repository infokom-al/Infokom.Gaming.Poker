namespace Holdem.Core.Internal
{

	internal static partial class DECK
	{
		internal static partial class CARD
		{
			public static class SUIT
			{
				public const int COUNT = 4;

				public enum ID : int
				{
					CLUBS	= 0,
					DIAMONDS	= 1,
					HEARTS	= 2,
					SPADES	= 3,
				}

				public enum FLAG : uint
				{
					CLUBS	= 1U << ID.CLUBS,
					DIAMONDS	= 1U << ID.DIAMONDS,
					HEARTS	= 1U << ID.HEARTS,
					SPADES	= 1U << ID.SPADES,
				}

				public enum SYMBOL : ushort
				{
					CLUBS	= 0X63,//c
					DIAMONDS	= 0X64,//d
					HEARTS	= 0X68,//h
					SPADES	= 0X73,//s
				}

				public const string SYMBOLS = "cdhs";






			}
		}
	}
}
