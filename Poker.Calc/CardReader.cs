using MemoryPools;

namespace Poker.Calc;

public class CardReader
{
	private static ThreadLocal<CardReader> ThreadShared = new ThreadLocal<CardReader>(() => new CardReader(), trackAllValues: true);

	public InlineList<Card> Cards { get; private set; }

	public int Cursor { get; private set; }

	public bool HasNext => Cursor < Cards.Count;

	public bool HasNextNext => Cursor + 1 < Cards.Count;

	public bool HasPrevious => Cursor > 0;

	public Card Next
	{
		get
		{
			if (!HasNext)
			{
				throw new InvalidOperationException("No more cards to read");
			}
			return Cards[Cursor];
		}
	}

	public Card NextNext
	{
		get
		{
			if (!HasNextNext)
			{
				throw new InvalidOperationException("No next next card");
			}
			return Cards[Cursor + 1];
		}
	}

	public Card Previous
	{
		get
		{
			if (!HasPrevious)
			{
				throw new InvalidOperationException("No previous card");
			}
			return Cards[Cursor - 1];
		}
	}

	public int CardsLeft => Cards.Count - Cursor;

	public static CardReader Create(InlineList<Card> cards)
	{
		CardReader cardReader = ObjectPool<CardReader>.ThreadShared.RentObject();
		cardReader.Cards = cards;
		cardReader.Cursor = 0;
		return cardReader;
	}

	public static CardReader GetThreadShared(InlineList<Card> cards)
	{
		if (!Pooling.EnableThreadObjectPooling)
		{
			return new CardReader
			{
				Cards = cards
			};
		}
		CardReader value = ThreadShared.Value;
		value.Cards = cards;
		value.Cursor = 0;
		return value;
	}

	public bool TryReadRepeatedRank(out CardRanks rank, out int count)
	{
		if (!TryReadRepeatedCards(out var result))
		{
			count = 0;
			rank = CardRanks.Deuce;
			return false;
		}
		rank = result[0].Rank;
		count = result.Count;
		return true;
	}

	public bool TryReadRepeatedCards(out InlineList<Card> result)
	{
		result = default(InlineList<Card>);
		if (!HasNext)
		{
			return false;
		}
		int cursor = Cursor;
		CardRanks rank = Next.Rank;
		while (HasNext && Next.Rank == rank)
		{
			result.Add(Next);
			Cursor++;
		}
		if (result.Count >= 2)
		{
			return true;
		}
		Cursor = cursor;
		result = default(InlineList<Card>);
		return false;
	}

	public bool TryReadOmahaRundown(out InlineList<CardRanks> result)
	{
		int cursor = Cursor;
		if (!TryReadRundown(out result))
		{
			result = default(InlineList<CardRanks>);
			return false;
		}
		if (result.Count < 3)
		{
			Cursor = cursor;
			return false;
		}
		return true;
	}

	public bool TryReadRundown(out InlineList<CardRanks> result)
	{
		if (CardsLeft < 2)
		{
			result = default(InlineList<CardRanks>);
			return false;
		}
		int cursor = Cursor;
		result = default(InlineList<CardRanks>);
		_ = Next.Rank;
		result.Add(Next.Rank);
		SkipOne();
		while (HasNext)
		{
			var (card, num) = PeekNextWithGap();
			if (num == -1)
			{
				Cursor++;
				continue;
			}
			if (num > 2)
			{
				if (result.Count < 2)
				{
					break;
				}
				return true;
			}
			InlineList<CardRanks> inlineList = result.With(card.Rank);
			if (inlineList.Count == 2 || inlineList.IsRundown())
			{
				result = inlineList;
				Cursor++;
				continue;
			}
			if (result.Count < 2)
			{
				break;
			}
			return true;
		}
		if (result.IsRundown())
		{
			return true;
		}
		Cursor = cursor;
		return false;
	}

	public (Card card, int gap) PeekNextWithGap()
	{
		VerifyHasNext();
		VerifyHasPrevious();
		Card previous = Previous;
		Card next = Next;
		return (card: next, gap: previous.Rank - next.Rank - 1);
	}

	public Card ReadNext()
	{
		if (!HasNext)
		{
			throw new InvalidOperationException("No more cards to read");
		}
		Card next = Next;
		Cursor++;
		return next;
	}

	public InlineList<Card> ReadNextSuitedCards()
	{
		InlineList<Card> result = default(InlineList<Card>);
		if (!HasNext)
		{
			return result;
		}
		_ = Cursor;
		Suits suit = Next.Suit;
		while (HasNext && Next.Suit == suit)
		{
			result.Add(Next);
			Cursor++;
		}
		return result;
	}

	public CardReader SkipOne()
	{
		VerifyHasNext();
		Cursor++;
		return this;
	}

	public CardReader VerifyHasNext()
	{
		if (!HasNext)
		{
			throw new InvalidOperationException("No more cards to read");
		}
		return this;
	}

	public CardReader VerifyHasPrevious()
	{
		if (!HasPrevious)
		{
			throw new InvalidOperationException("No previous card to read");
		}
		return this;
	}
}
