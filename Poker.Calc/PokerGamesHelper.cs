using Poker.Calc.Common;

using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Poker.Calc;

public static class PokerGamesHelper
{
	public static ImmutableArray<PokerGames> AllGames = ImmutableCollectionsMarshal.AsImmutableArray(EnumHelper.GetSingleFlags<PokerGames>().ToArray());

	public static int GetTotalComboCount(this PokerGames game)
	{
		return game switch
		{
			PokerGames.TexasHoldem => 1326,
			PokerGames.ShortDeck => 630,
			_ => throw new NotImplementedException(),
		};
	}

	public static PokerGames GetOmahaGameFromCardNumber(this int cardNumber)
	{
		return cardNumber switch
		{
			4 => PokerGames.Omaha,
			5 => PokerGames.OmahaFive,
			6 => PokerGames.OmahaSix,
			_ => throw new InvalidOperationException($"Invliad card number in omaha {cardNumber}"),
		};
	}

	public static bool IsOmahaFamily(this PokerGames pokerGame)
	{
		if (pokerGame == PokerGames.Omaha || pokerGame == PokerGames.OmahaFive || pokerGame == PokerGames.OmahaSix)
		{
			return true;
		}
		return false;
	}

	public static bool IsShortDeckFamily(this PokerGames pokerGame)
	{
		if (pokerGame == PokerGames.ShortDeck || pokerGame == PokerGames.ShortDeckTbs || pokerGame == PokerGames.ShortDeckFamily)
		{
			return true;
		}
		return false;
	}

	public static bool RequiresPocketsInHandCombo(this PokerGames pokerGame)
	{
		if (pokerGame == PokerGames.Omaha || pokerGame == PokerGames.OmahaFive || pokerGame == PokerGames.OmahaSix)
		{
			return true;
		}
		return false;
	}

	public static bool IsFourPocketsGame(this PokerGames pokerGame)
	{
		return pokerGame.PocketCardsCount() == 4;
	}

	public static bool IsTwoPocketsGame(this PokerGames pokerGame)
	{
		return pokerGame.PocketCardsCount() == 2;
	}

	public static bool IsSameFamily(this PokerGames game, PokerGames other)
	{
		if (game != other && (!game.IsOmahaFamily() || !other.IsOmahaFamily()))
		{
			if (game.IsShortDeckFamily())
			{
				return other.IsShortDeckFamily();
			}
			return false;
		}
		return true;
	}

	public static int PocketCardsCount(this PokerGames pokerGame)
	{
		if ((pokerGame == PokerGames.TexasHoldem || pokerGame == PokerGames.ShortDeck || pokerGame == PokerGames.ShortDeckTbs) ? true : false)
		{
			return 2;
		}
		return pokerGame switch
		{
			PokerGames.Omaha => 4,
			PokerGames.OmahaFive => 5,
			PokerGames.OmahaSix => 6,
			_ => throw new ArgumentNullException(pokerGame.ToString()),
		};
	}

	public static CardRanks MinCardRank(this PokerGames pokerGame)
	{
		if (!pokerGame.IsShortDeckFamily())
		{
			return CardRanks.Deuce;
		}
		return CardRanks.Six;
	}

	public static int CardsInDeck(this PokerGames game)
	{
		if (!game.IsShortDeckFamily())
		{
			return 52;
		}
		return 36;
	}

	public static int RanksInDeck(this PokerGames game)
	{
		if (!game.IsShortDeckFamily())
		{
			return 13;
		}
		return 9;
	}

	public static bool IsCardIndexHitsPokerGame(this int cardIndex, PokerGames game)
	{
		if (game.IsShortDeckFamily())
		{
			return cardIndex % 13 >= 4;
		}
		return true;
	}

	public static string GetName(this PokerGames game)
	{
		return game.GetEnumAttribute<NameAttribute>().Value;
	}

	public static PokerGames AllIfZero(this PokerGames games)
	{
		if (games != 0)
		{
			return games;
		}
		return PokerGames.AllGames;
	}

	public static PokerGames VerifyIsHoldem(this PokerGames game)
	{
		if (!game.IsSingleFlag())
		{
			throw new ArgumentException("Expecting a single game", "game");
		}
		if (game.IsShortDeckFamily() || game == PokerGames.TexasHoldem)
		{
			return game;
		}
		throw new InvalidOperationException($"Expecting a holdem but was {game}");
	}
}
