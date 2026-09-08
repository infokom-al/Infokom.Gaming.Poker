using System.Collections.Immutable;
using System.Text;

namespace Poker.Calc;

public static class Cards
{
	private static readonly string[] Ranks = new string[13]
	{
		"2", "3", "4", "5", "6", "7", "8", "9", "T", "J",
		"Q", "K", "A"
	};

	private static readonly string[] SuitsChars = new string[4] { "h", "d", "c", "s" };

	public static ImmutableList<Card> AllCards = Enumerable.Range(0, 52).MapToImmutableList(CardIndexToCard);

	public static ImmutableList<Card> AllCardsShortDeck = AllCards.Where((Card x) => x.Rank >= CardRanks.Six).ToImmutableList();

	public static ImmutableList<CardRanks> RanksDecending = ImmutableList.Create((ReadOnlySpan<CardRanks>)new CardRanks[13]
	{
		CardRanks.Ace,
		CardRanks.King,
		CardRanks.Queen,
		CardRanks.Jack,
		CardRanks.Ten,
		CardRanks.Nine,
		CardRanks.Eight,
		CardRanks.Seven,
		CardRanks.Six,
		CardRanks.Five,
		CardRanks.Four,
		CardRanks.Three,
		CardRanks.Deuce
	});

	public static Suits[] AllSuits = new Suits[4]
	{
		Suits.Hearts,
		Suits.Spades,
		Suits.Clubs,
		Suits.Diamonds
	};

	public static ImmutableList<Card> GetAllCards(this PokerGames game)
	{
		if (!game.IsShortDeckFamily())
		{
			return AllCards;
		}
		return AllCardsShortDeck;
	}

	public static Card CardIndexToCard(this int cardIndex)
	{
		return Card.FromCardIndex(cardIndex);
	}

	public static long ToCardMask(this Card card)
	{
		return card.Index.CardIndexToCardMaskPeval();
	}

	public static long ToCardsMask(this InlineList<Card> cards)
	{
		long num = 0L;
		InlineList<Card>.Enumerator enumerator = cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Card current = enumerator.Current;
			num |= current.ToCardMask();
		}
		return num;
	}

	public static long ToCardsMask(this Span<Card> cards)
	{
		long num = 0L;
		Span<Card> span = cards;
		for (int i = 0; i < span.Length; i++)
		{
			Card card = span[i];
			num |= card.ToCardMask();
		}
		return num;
	}

	public static long ToCardsMask(this InlineList<Card> cards, int cardsToTake)
	{
		long num = 0L;
		for (int i = 0; i < cardsToTake; i++)
		{
			num |= cards[i].ToCardMask();
		}
		return num;
	}

	internal static InlineList<int> ToCardsMasksCactus(this Span<Card> cards)
	{
		return cards.MapToInlineList((Card card) => card.ToCardMaskCactus());
	}

	internal static InlineList<int> ToCardsMasksCactus(this InlineList<Card> cards)
	{
		return cards.Map((Card card) => card.ToCardMaskCactus());
	}

	public static long ToCardsMask(this InlineList<Card> cards, out long[] separateCardsMasks)
	{
		separateCardsMasks = new long[cards.Length];
		long num = 0L;
		for (int i = 0; i < cards.Length; i++)
		{
			long num2 = cards[i].ToCardMask();
			num |= num2;
			separateCardsMasks[i] = num2;
		}
		return num;
	}

	public static CardRanks Min(CardRanks rank1, CardRanks rank2)
	{
		return (CardRanks)Math.Min((int)rank1, (int)rank2);
	}

	public static CardRanks Max(CardRanks rank1, CardRanks rank2)
	{
		return (CardRanks)Math.Max((int)rank1, (int)rank2);
	}

	public static bool IsDead(this Card card, Span<Card> deadCards)
	{
		return deadCards.Contains(card);
	}

	public static bool IsAce(this CardRanks rank)
	{
		return rank == CardRanks.Ace;
	}

	public static string ToAbbreviation(this CardRanks rank)
	{
		return Ranks[(int)rank];
	}

	public static string ToAbbreviation(this Suits suit)
	{
		return SuitsChars[(int)suit];
	}

	public static string ToAbbreviation(this IEnumerable<Card> cards)
	{
		return cards.Aggregate(string.Empty, (string x, Card y) => x + y.Abbreviation);
	}

	public static string ToAbbreviation(this InlineList<Card> cards)
	{
		return cards.Aggregate(string.Empty, (string x, Card y) => x + y.Abbreviation);
	}

	public static string ToAbbreviationSpaced(this InlineList<Card> cards)
	{
		return InlineListHelper.AggregateToString(in cards, " ");
	}

	public static InlineList<Card> ParseCards(this string str)
	{
		if (!str.TryParseCards(out var result))
		{
			throw new InvalidOperationException("Failed to parse cards from \"" + str + "\"");
		}
		return result;
	}

	public static InlineList<Card> ParseCardsEnumerable(this string @string)
	{
		InlineList<Card> result = default(InlineList<Card>);
		for (int i = 0; i < @string.Length; i += 2)
		{
			if (!TryParseCard(@string[i], @string[i + 1], out var card))
			{
				throw new InvalidOperationException("Failed to parse cards from \"" + @string + "\"");
			}
			result.Add(card);
		}
		return result;
	}

	public static bool TryParseCards(this string @string, out InlineList<Card> result)
	{
		result = default(InlineList<Card>);
		List<CardRanks> list = new List<CardRanks>();
		for (int i = 0; i < @string.Length - 1; i++)
		{
			if (TryParseCard(@string[i], @string[i + 1], out var card))
			{
				if (list.Count > 0)
				{
					foreach (CardRanks item in list)
					{
						result.Add(new Card(item, card.Suit));
					}
					list.Clear();
				}
				result.Add(card);
				i++;
			}
			else
			{
				if (!@string[i].TryParseCardRank(out var res))
				{
					return false;
				}
				list.Add(res);
			}
		}
		return true;
	}

	public static Card ParseCard(this string abbreviation)
	{
		return new Card(abbreviation[0].ParseCardRank(), abbreviation[1].ParseSuit());
	}

	private static CardRanks ParseCardRank(this char c)
	{
		if (c.TryParseCardRank(out var res))
		{
			return res;
		}
		throw new InvalidOperationException($"Failed to parse card rank from '{c}'");
	}

	public static bool TryParseCard(this string abbreviation, out Card card)
	{
		if (abbreviation != null && abbreviation.Length == 2)
		{
			return TryParseCard(abbreviation[0], abbreviation[1], out card);
		}
		card = default(Card);
		return false;
	}

	public static bool TryParseCard(char rankChar, char suitChar, out Card card)
	{
		if (suitChar.TryParseSuits(out var suit) && rankChar.TryParseCardRank(out var res))
		{
			card = new Card(res, suit);
			return true;
		}
		card = default(Card);
		return false;
	}

	public static Card Parse(char rankChar, char suitChar)
	{
		if (!TryParseCard(rankChar, suitChar, out var card))
		{
			throw new InvalidOperationException($"Failed to parse card from '{rankChar}' '{suitChar}'");
		}
		return card;
	}

	public static bool TryParseCardRank(this char c, out CardRanks res)
	{
		switch (c)
		{
			case 'A':
			case 'a':
				res = CardRanks.Ace;
				return true;
			case 'K':
			case 'k':
				res = CardRanks.King;
				return true;
			case 'Q':
			case 'q':
				res = CardRanks.Queen;
				return true;
			case 'J':
			case 'j':
				res = CardRanks.Jack;
				return true;
			case 'T':
			case 't':
				res = CardRanks.Ten;
				return true;
			case '9':
				res = CardRanks.Nine;
				return true;
			case '8':
				res = CardRanks.Eight;
				return true;
			case '7':
				res = CardRanks.Seven;
				return true;
			case '6':
				res = CardRanks.Six;
				return true;
			case '5':
				res = CardRanks.Five;
				return true;
			case '4':
				res = CardRanks.Four;
				return true;
			case '3':
				res = CardRanks.Three;
				return true;
			case '2':
				res = CardRanks.Deuce;
				return true;
			default:
				res = CardRanks.Deuce;
				return false;
		}
	}

	public static Suits ParseSuit(this char c)
	{
		switch (c)
		{
			case 'H':
			case 'h':
				return Suits.Hearts;
			case 'S':
			case 's':
				return Suits.Spades;
			case 'C':
			case 'c':
				return Suits.Clubs;
			case 'D':
			case 'd':
				return Suits.Diamonds;
			default:
				throw new ArgumentException($"Can't convert '{c}' to Suits");
		}
	}

	public static bool TryParseSuits(this char suitChar, out Suits suit)
	{
		switch (suitChar)
		{
			case 'H':
			case 'h':
				suit = Suits.Hearts;
				return true;
			case 'S':
			case 's':
				suit = Suits.Spades;
				return true;
			case 'C':
			case 'c':
				suit = Suits.Clubs;
				return true;
			case 'D':
			case 'd':
				suit = Suits.Diamonds;
				return true;
			default:
				suit = Suits.Hearts;
				return false;
		}
	}

	public static Card GetRandomCard(this IList<Card> cards, ICollection<Card> deadCards, FastRandom random)
	{
		if (cards.Count == 1)
		{
			return cards[0];
		}
		Card card;
		while (true)
		{
			card = cards[random.Next(cards.Count - 1)];
			if (!deadCards.Any((Card x) => object.Equals(x, card)))
			{
				break;
			}
		}
		return card;
	}

	public static InlineList<Card> GetAllPossibleCards(this CardRanks rank)
	{
		return rank.GetAllPossibleCards(AllSuits.ToInlineList());
	}

	public static IEnumerable<Card> GetAllPossibleCards(this IEnumerable<CardRanks> ranks)
	{
		return ranks.Distinct().SelectMany((CardRanks rank) => rank.GetAllPossibleCards().AsEnumerable());
	}

	public static InlineList<Card> GetAllPossibleCards(this CardRanks rank, InlineList<Suits> suits)
	{
		return suits.Map((Suits suit) => new Card(rank, suit));
	}

	public static IEnumerable<Card> GetAllPossibleCards(this IEnumerable<CardRanks> ranks, params Suits[] suits)
	{
		return ranks.SelectMany((CardRanks rank) => rank.GetAllPossibleCards(suits.ToInlineList()).AsEnumerable());
	}

	public static IEnumerable<Card> GetAliveCards(this IEnumerable<Card> deadCards)
	{
		return AllCards.Except(deadCards);
	}

	public static Card GetRandomCard(this InlineList<Card> cards, InlineList<Card> deadCards, FastRandom random)
	{
		if (cards.Count == 1)
		{
			return cards[0];
		}
		Card card;
		while (true)
		{
			card = cards.GetRandomItem(random);
			if (!deadCards.Any((Card x) => object.Equals(x, card)))
			{
				break;
			}
		}
		return card;
	}

	public static long GetRandomCardMask(this IList<long> cards, long deadCards, FastRandom random)
	{
		if (cards.Count == 1)
		{
			return cards[0];
		}
		long randomItem;
		do
		{
			randomItem = cards.GetRandomItem(random);
		}
		while ((deadCards & randomItem) != 0L);
		return randomItem;
	}

	public static bool IsDead(this Card card, InlineList<Card> deadCards)
	{
		return deadCards.Contains(card);
	}

	public static string GetAbbreviation(this InlineList<Card> cards)
	{
		StringBuilder stringBuilder = new StringBuilder();
		InlineList<Card>.Enumerator enumerator = cards.GetEnumerator();
		while (enumerator.MoveNext())
		{
			stringBuilder.Append(enumerator.Current.Abbreviation);
		}
		return stringBuilder.ToString();
	}

	public static string GetAbbreviation(this IEnumerable<Card> cards)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Card card in cards)
		{
			stringBuilder.Append(card.Abbreviation);
		}
		return stringBuilder.ToString();
	}

	public static string GetAbbreviation(this InlineList<Card> cards, string? separator = null)
	{
		return cards.AggregateToString((Card x) => x.Abbreviation, separator);
	}

	public static int Compare(this Card card, Card other)
	{
		int num = card.Rank.CompareRanks(other.Rank);
		if (num != 0)
		{
			return num;
		}
		return card.Suit.CompareSuits(other.Suit);
	}

	public static int CompareRanks(this CardRanks rank, CardRanks other)
	{
		if (rank <= other)
		{
			if (rank >= other)
			{
				return 0;
			}
			return -1;
		}
		return 1;
	}

	public static int CompareSuits(this Suits suit, Suits other)
	{
		if (suit <= other)
		{
			if (suit >= other)
			{
				return 0;
			}
			return -1;
		}
		return 1;
	}

	public static bool IsPair(this (CardRanks rank1, CardRanks rank2) ranks)
	{
		return ranks.rank1 == ranks.rank2;
	}

	public static Card ToCard(this CardRanks rank, Suits suit)
	{
		return new Card(rank, suit);
	}

	public static InlineList<Card> OrderByRank(this InlineList<Card> cards)
	{
		return cards.OrderByDescending((Card card) => (int)((int)card.Rank * 100 + card.Suit));
	}

	public static InlineList<Card> OrderByRankThenBySuit(this InlineList<Card> cards)
	{
		return cards.OrderByDescending((Card card) => (int)((int)card.Rank * 100 - card.Suit));
	}

	public static InlineList<Card> VerifyOrderedByRankDescending(this InlineList<Card> cards)
	{
		for (int i = 1; i < cards.Count; i++)
		{
			if (cards[i - 1].Rank.CompareTo(cards[i].Rank) < 0)
			{
				throw new InvalidOperationException("Cards are not ordered by rank descending");
			}
		}
		return cards;
	}

	public static InlineList<Card> VerifyOrderedBySuitThenByRank(this InlineList<Card> cards)
	{
		for (int i = 1; i < cards.Count; i++)
		{
			int num = cards[i - 1].Suit.CompareTo(cards[i].Suit);
			if (num > 0 || (num == 0 && cards[i - 1].Rank < cards[i].Rank))
			{
				throw new InvalidOperationException("Cards are not ordered by suit then by rank");
			}
		}
		return cards;
	}

	public static InlineList<Card> VerifyOrderedBySuit(this InlineList<Card> cards)
	{
		for (int i = 1; i < cards.Count; i++)
		{
			if (cards[i - 1].Suit > cards[i].Suit)
			{
				throw new InvalidOperationException("Cards are not ordered by suit");
			}
		}
		return cards;
	}

	public static InlineList<(int? gap, CardRanks rank)> WithGaps(this InlineList<CardRanks> ranks)
	{
		InlineList<(int?, CardRanks)> result = default(InlineList<(int?, CardRanks)>);
		for (int i = 0; i < ranks.Count; i++)
		{
			CardRanks cardRanks = ranks[i];
			if (i == 0)
			{
				result.Add((null, cardRanks));
				continue;
			}
			CardRanks cardRanks2 = ranks[i - 1];
			result.Add((cardRanks2 - cardRanks - 1, cardRanks));
		}
		return result;
	}

	public static bool IsPaired(this Card card, InlineList<Card> cards)
	{
		return cards.Any((Card other, Card card2) => other.Rank == card2.Rank && other.Suit != card2.Suit, card);
	}
}
