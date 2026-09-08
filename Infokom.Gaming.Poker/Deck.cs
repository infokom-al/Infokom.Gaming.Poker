using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;


namespace Infokom.Gaming.Poker
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct Deck
	{
		private const ushort RANKING_MASK = 0x7FFC;

		private const ulong FACTORY_MASK = 0x7FFC7FFC7FFC7FFCUL;

		private const ulong SHUFFLED_FLAG = 1UL;

		[FieldOffset(0)] private ulong _binary;
		[FieldOffset(0)] private readonly ushort _s;
		[FieldOffset(2)] private readonly ushort _d;
		[FieldOffset(4)] private readonly ushort _c;
		[FieldOffset(6)] private readonly ushort _h;



		[FieldOffset(0)] public readonly Cards State;


		private Deck(ulong binary) : this() => _binary = binary;

		

		public readonly int Count => BitOperations.PopCount(_binary & FACTORY_MASK);

		public readonly bool IsEmpty => (_binary & FACTORY_MASK) == 0;

		public readonly bool IsComplete => (_binary & FACTORY_MASK) == FACTORY_MASK;

		public readonly bool IsShuffled => (_binary & SHUFFLED_FLAG) != 0;

		public void Shuffle()
		{
			if (!IsComplete)
				throw new InvalidOperationException(
				    "The deck can only be shuffled when all cards are present.");

			_binary = FACTORY_MASK | SHUFFLED_FLAG;
		}

		/// <summary>
		/// Restores the deck to factory order, which is the order in which the cards were originally arranged.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		/// <remarks>
		/// We are following ordering according to <see href="https://en.wikipedia.org/wiki/Standard_52-card_deck#New-deck_order_(NDO)">New-deck order (NDO)</see>
		/// with a single difference about aces being the upmost instead of lowest as in NDO.
		/// </remarks>
		public void Reorder()
		{
			if (!IsComplete)
				throw new InvalidOperationException(
				    "The deck can only be restored to factory order when all cards are present.");

			_binary = FACTORY_MASK;
		}

		internal readonly bool Contains(Card card) => _binary.BitTest((int)card);








		internal readonly Card Peek()
		{
			ulong binary = _binary & FACTORY_MASK;

			if (binary == 0)
				throw new InvalidOperationException("The deck is empty.");

			int offset = IsShuffled ? binary.BitScanShuffle() : binary.BitScanForward();
			
			return (Card)offset;
		}

		public readonly bool TryPeek(out Card card)
		{
			ulong binary = _binary & FACTORY_MASK;
			if (binary == 0)
			{
				card = default;
				return false;
			}
			int offset = IsShuffled ? binary.BitScanShuffle() : binary.BitScanForward();

			card = (Card)offset;
			return true;
		}
		
		public Card Pop()
		{
			if (TryPeek(out Card card))
			{
				_binary = _binary.BitClear((int)card);
				return card;
			}
			throw new InvalidOperationException("The deck is empty.");
		}

		public bool TryPop(out Card card)
		{
			if (TryPeek(out card))
			{
				_binary = _binary.BitClear((int)card);
				return true;
			}

			return false;
		}

		internal void Remove(Card card)
		{
			_binary = _binary.BitClear((int)card);
		}






		public static readonly Deck Empty = default;
		public static readonly Deck Factory = new(FACTORY_MASK);

		internal void Push(Card card) => throw new NotImplementedException();
	}

	public struct Hand
	{
		private ulong _bits;

		public readonly int Count =>
		    BitOperations.PopCount(_bits);

		public readonly bool IsEmpty =>
		    _bits == 0;

		public readonly bool IsFull =>
		    Count == 5;

		internal readonly bool Contains(Card card)
		{
			ulong flag = 1UL << (int)card;
			return (_bits & flag) != 0;
		}

		internal readonly bool CanAccept(Card card)
		{
			return !IsFull && !Contains(card);
		}

		internal void Accept(Card card)
		{
			_bits |= 1UL << (int)card;
		}
	}

	public readonly struct Dealer
	{
		public static readonly Dealer Default = new();

		public void Draw(ref Deck source, ref Hand target)
		{
			if (!TryDraw(ref source, ref target))
				throw new InvalidOperationException(
				    "The draw operation cannot be completed.");
		}

		public bool TryDraw(ref Deck source, ref Hand target)
		{
			if (source.IsEmpty)
				return false;

			Card card = source.Pop();

			if (target.CanAccept(card))
			{
				target.Accept(card);
				return true;
			}
			else
			{
				source.Push(card);
				return false;
			}
		}

	}
}
