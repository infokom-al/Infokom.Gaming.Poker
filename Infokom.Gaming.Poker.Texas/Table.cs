using Infokom.Numerics.Atomics;

using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas
{
	/// <summary>
	/// Maintains the dealing and evaluation state for one Texas Hold'em table.
	/// Cards are dealt to occupied seats in round-robin order until every seat has
	/// two pocket cards; subsequent cards are dealt to the shared board.
	/// </summary>
	public sealed class TexasTable : IObserver<Card>
	{
		public const int MaxSeats = 10;
		private readonly CardSet[] _pockets = new CardSet[MaxSeats];
		private Bit.Vector<ushort> _occupied;
		private CardSet _board;
		private bool _completed;
		private int _nextSeat;
		private Exception _error;

		public int OccupiedSeatCount { get; private set; }
		public bool IsCompleted => _completed;
		public Exception Error => _error;
		public Board Board => Board.Create(_board);
		public int BoardCardCount => _board.Count;

		public void OccupySeat(int seat)
		{
			EnsureNotDealing();
			ValidateSeat(seat);

			if (_occupied[seat])
				throw new InvalidOperationException($"Seat {seat} is already occupied.");

			_occupied[seat] = true;
			OccupiedSeatCount++;
		}

		public void VacateSeat(int seat)
		{
			EnsureNotDealing();
			ValidateSeat(seat);

			if (!_occupied[seat])
				return;

			_occupied[seat] = false;
			_pockets[seat] = CardSet.Φ;
			OccupiedSeatCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsOccupied(int seat)
		{
			ValidateSeat(seat);
			return _occupied[seat];
		}

		public Pocket GetPocket(int seat)
		{
			ValidateOccupiedSeat(seat);
			return Pocket.Create(_pockets[seat]);
		}

		public void OnNext(Card card)
		{
			if (_completed)
				throw new InvalidOperationException("The table has already completed dealing.");
			if (_error is not null)
				throw new InvalidOperationException("The table is in an error state.", _error);
			if (OccupiedSeatCount == 0)
				throw new InvalidOperationException("Cannot deal cards without occupied seats.");
			if (!IsValidCard(card))
				throw new ArgumentOutOfRangeException(nameof(card));
			if (Contains(card))
				throw new InvalidOperationException($"Card {card} has already been dealt.");

			if (TryGetNextPocketSeat(out var seat))
			{
				_pockets[seat] = _pockets[seat].Include(card);
				_nextSeat = (seat + 1) % MaxSeats;
				return;
			}

			if (_board.Count == 5)
				throw new InvalidOperationException("All pocket and board cards have already been dealt.");

			_board = _board.Include(card);

			if(!(_board.Count < 5))
				this.OnCompleted();
		}

		public void OnCompleted() => _completed = true;

		public void OnError(Exception error)
		{
			ArgumentNullException.ThrowIfNull(error);
			_error = error;
		}

		/// <summary>
		/// Evaluates every occupied seat. The returned array is indexed by seat;
		/// free seats remain <see langword="default"/>. Evaluation is available from the flop onward.
		/// </summary>
		public Hand.Ranking[] Evaluate()
		{
			if (_board.Count < 3)
				throw new InvalidOperationException("At least three board cards are required for evaluation.");
			
			Hand.Ranking[] results = new Hand.Ranking[TexasTable.MaxSeats];
			
			for (int seat = 0; seat < MaxSeats; seat++)
			{
				if (!_occupied[seat])
					continue;
				if (_pockets[seat].Count != 2)
					throw new InvalidOperationException($"Seat {seat} does not have two pocket cards.");

				results[seat] = Hand.Evaluate(_pockets[seat] | _board);
			}

			return results;
		}

		public Hand.Ranking Evaluate(Span<Hand.Ranking> results)
		{
			if (_board.Count < 3)
				throw new InvalidOperationException("At least three board cards are required for evaluation.");

			if (results.Length < MaxSeats)
				throw new ArgumentException($"Destination must contain at least {MaxSeats} elements.", nameof(results));

			var winner = default(Hand.Ranking);
			for (int seat = 0; seat < MaxSeats; seat++)
			{
				if (!_occupied[seat] || _pockets[seat].Count != 2)
					continue;

				ref Hand.Ranking ranking = ref results[seat];
				if(Hand.TryEvaluate(_pockets[seat] | _board, out ranking))
				{
					if(ranking > winner)
						winner = ranking;
				}
			}

			return winner;
		}

		private bool TryGetNextPocketSeat(out int seat)
		{
			for (int offset = 0; offset < MaxSeats; offset++)
			{
				seat = (_nextSeat + offset) % MaxSeats;
				if (_occupied[seat] && _pockets[seat].Count < 2)
					return true;
			}

			seat = -1;
			return false;
		}

		private bool Contains(Card card)
		{
			if (_board.HasMember(card))
				return true;

			for (int seat = 0; seat < MaxSeats; seat++)
			{
				if (_pockets[seat].HasMember(card))
					return true;
			}

			return false;
		}

		private void EnsureNotDealing()
		{
			if (_board.Count != 0 || _pockets.Any(static pocket => pocket != CardSet.Φ))
				throw new InvalidOperationException("Seats cannot change after dealing has started.");
		}

		private static bool IsValidCard(Card card)
		{
			var value = (int)card;
			var rank = value & 0xF;
			return value is >= 2 and <= 62 && rank is >= 2 and <= 14;
		}

		private static void ValidateSeat(int seat)
		{
			if ((uint)seat >= MaxSeats)
				throw new ArgumentOutOfRangeException(nameof(seat));
		}

		private void ValidateOccupiedSeat(int seat)
		{
			ValidateSeat(seat);
			if (!_occupied[seat])
				throw new InvalidOperationException($"Seat {seat} is free.");
		}
	}
}
