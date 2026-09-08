namespace Poker.Calc.Tools
{
	public class EquityTable
	{
		private readonly PokerGames _game;
		private readonly List<double> _result = new();
		private readonly List<(string, double)> _resultToCards = new();
		private readonly PreflopRangeHoldem _preflopRange;

		public EquityTable(PokerGames game)
		{
			this._game = game;
			this._preflopRange = (game == PokerGames.TexasHoldem)
			    ? PreflopRangeHoldem.FullRangeTexasHoldem
			    : PreflopRangeHoldem.FullRangeShortDeck;
		}

		public (List<double>, List<(string, double)>) Generate()
		{
			foreach (PreflopRangeHoldemCell pairCell in this._preflopRange.PairCells)
			{
				this.PairVsSuited(pairCell);
				this.PairVsOfSuited(pairCell);
				this.PairVsPair(pairCell);
			}
			this._preflopRange.SuitedCells.AsParallel().AsOrdered().ForEach(this.SuitedVsOfSuited);
			this._preflopRange.SuitedCells.AsParallel().AsOrdered().ForEach(this.SuitedVsSuited);
			this._preflopRange.OffsuitedCells
			    .Where(x => !x.IsPair)
			    .AsParallel().AsOrdered()
			    .ForEach(this.OfSuitedVsOfSuited);

			return (this._result, this._resultToCards);
		}

		private void AddNewResult(PocketCardsHoldem cards, PocketCardsHoldem other)
		{
			double first = cards.ParallelComputeExactEquityHoldem(other, this._game).First;
			this._result.Add(first);
			this._resultToCards.Add(($"{cards} vs {other}", first));
		}

		private PocketCardsHoldem GetPocketCardsWithAnySuitBlocker(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasSuitBlockerAny(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithNoSuitBlockers(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasNoSuitBlockers(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithOneSuitBlocker(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCards in cell.GetPocketCards(cards))
				if (cards.Cards.Count(x => pocketCards.Cards.Any(y => y.Suit == x.Suit)) == 1)
					return pocketCards;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithSuitBlockerAnyVsHigh(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasSuitBlockerAnyVsHigh(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithSuitBlockerAnyVsLow(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasSuitBlockerAnyVsLow(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithSuitBlockerHighRankVsHighRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasSuitBlockerHighRankVsHighRank(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithSuitBlockerHighRankVsLowRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasSuitBlockerHighRankVsLowRank(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithSuitBlockerLowRankVsHighRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasSuitBlockerLowRankVsHighRank(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithSuitBlockerLowRankVsLowRank(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasSuitBlockerLowRankVsLowRank(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithTwoSuitBlockers(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasTwoSuitBlockers(pocketCard))
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithTwoSuitBlockersHighVsHigh(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasTwoSuitBlockers(pocketCard) && cards.HighCard.Suit == pocketCard.HighCard.Suit)
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private PocketCardsHoldem GetPocketCardsWithTwoSuitBlockersHighVsLow(PreflopRangeHoldemCell cell, PocketCardsHoldem cards)
		{
			foreach (var pocketCard in cell.GetPocketCards(cards))
				if (cards.HasTwoSuitBlockers(pocketCard) && cards.HighCard.Suit == pocketCard.LowCard.Suit)
					return pocketCard;
			throw new InvalidOperationException("Should not get here");
		}

		private void OfSuitedVsOfSuited(PreflopRangeHoldemCell cell)
		{
			var cards = cell.GetPocketCards().First();
			foreach (var item in this._preflopRange.OffsuitedCells.Where(x => !x.IsPair && x.Row >= cell.Row))
			{
				if (item.Row != cell.Row || item.Column >= cell.Column)
				{
					this.AddNewResult(cards, this.GetPocketCardsWithNoSuitBlockers(item, cards));
					if (cell.Column != item.Column)
					{
						if (cell.Row != item.Row)
							this.AddNewResult(cards, this.GetPocketCardsWithTwoSuitBlockersHighVsHigh(item, cards));
						this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerHighRankVsHighRank(item, cards));
					}
					if (cell.Row != item.Column)
					{
						this.AddNewResult(cards, this.GetPocketCardsWithTwoSuitBlockersHighVsLow(item, cards));
						this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerLowRankVsHighRank(item, cards));
					}
					this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerHighRankVsLowRank(item, cards));
					if (cell.Row != item.Row)
						this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerLowRankVsLowRank(item, cards));
				}
			}
		}

		private void PairVsOfSuited(PreflopRangeHoldemCell cell)
		{
			var cards = cell.GetPocketCards().First();
			foreach (var item in this._preflopRange.OffsuitedCells.Where(x => !x.IsPair))
			{
				this.AddNewResult(cards, this.GetPocketCardsWithNoSuitBlockers(item, cards));
				if (cell.Row != item.Row && cell.Column != item.Column)
					this.AddNewResult(cards, this.GetPocketCardsWithTwoSuitBlockers(item, cards));
				if (cell.Column != item.Column)
					this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerAnyVsHigh(item, cards));
				if (cell.Row != item.Row)
					this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerAnyVsLow(item, cards));
			}
		}

		private void PairVsPair(PreflopRangeHoldemCell cell)
		{
			var cards = cell.GetPocketCards().First();
			foreach (var item in this._preflopRange.PairCells.Where(x => x.Row >= cell.Row))
			{
				this.AddNewResult(cards, this.GetPocketCardsWithNoSuitBlockers(item, cards));
				if (cell.Row != item.Row)
				{
					this.AddNewResult(cards, this.GetPocketCardsWithOneSuitBlocker(item, cards));
					this.AddNewResult(cards, this.GetPocketCardsWithTwoSuitBlockers(item, cards));
				}
			}
		}

		private void PairVsSuited(PreflopRangeHoldemCell cell)
		{
			var cards = cell.GetPocketCards().First();
			foreach (var suitedCell in this._preflopRange.SuitedCells)
			{
				this.AddNewResult(cards, this.GetPocketCardsWithNoSuitBlockers(suitedCell, cards));
				if (cell.Row != suitedCell.Row && cell.Column != suitedCell.Column)
					this.AddNewResult(cards, this.GetPocketCardsWithAnySuitBlocker(suitedCell, cards));
			}
		}

		private void SuitedVsOfSuited(PreflopRangeHoldemCell cell)
		{
			var cards = cell.GetPocketCards().First();
			foreach (var item in this._preflopRange.OffsuitedCells.Where(x => !x.IsPair))
			{
				this.AddNewResult(cards, this.GetPocketCardsWithNoSuitBlockers(item, cards));
				if (cell.Row != item.Column && cell.Column != item.Column)
					this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerAnyVsHigh(item, cards));
				if (cell.Column != item.Row && cell.Row != item.Row)
					this.AddNewResult(cards, this.GetPocketCardsWithSuitBlockerAnyVsLow(item, cards));
			}
		}

		private void SuitedVsSuited(PreflopRangeHoldemCell cell)
		{
			var cards = cell.GetPocketCards().First();
			foreach (var item in this._preflopRange.SuitedCells.Where(x => x.Row >= cell.Row))
			{
				if (item.Row != cell.Row || item.Column >= cell.Column)
				{
					this.AddNewResult(cards, this.GetPocketCardsWithNoSuitBlockers(item, cards));
					if (cell.Row != item.Row && cell.Column != item.Row && cell.Column != item.Column)
						this.AddNewResult(cards, this.GetPocketCardsWithAnySuitBlocker(item, cards));
				}
			}
		}
	}
}