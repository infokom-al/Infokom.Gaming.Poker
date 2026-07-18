using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics;
using Infokom.Numerics.Attributes;

using System.Collections.Immutable;
using System.Numerics;
using System.Xml.Linq;

using static Infokom.Gaming.Poker.Deck;

namespace Infokom.Gaming.Poker
{

	public partial struct Deck
	{
		private Cards _cards;


		public Deck()
		{
			_cards = Cards.ALL;
		}

		public readonly bool IsEmpty => _cards.IsEmpty;

		public readonly int Count => _cards.Count;


		public readonly bool Contains(Card card) => _cards.IsIncluded(card);


		public bool TryDraw(Random random, out Card card)
		{
			card = this.TryDraw(random, out int index) ? Card.Values[index] : default;

			return card != default;
		}

		public Card Draw(Random random) => this.TryDraw(random, out int index) ? Card.Values[index] : throw new InvalidOperationException("Empty deck");


		/// <summary>
		/// 
		/// </summary>
		/// <param name="rand"></param>
		/// <param name="target"></param>
		/// <param name="count"></param>
		/// <param name="offset"></param>
		/// <returns>Number of drwn elements</returns>
		public void DrawTo(Random rand, Card[] target, int count, int offset = 0)
		{
			ArgumentNullException.ThrowIfNull(target, nameof(target));
			ArgumentOutOfRangeException.ThrowIfGreaterThan(count, _cards.Count);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(offset + count, target.Length);


			for (int i = 0; i < count; i++)
			{
				target[i + offset] = this.Draw(rand);
			}
		}

		public void DrawTo(Random rand, Span<Card> target, int count, int offset = 0)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(count, _cards.Count);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(offset + count, target.Length);


			for (int i = 0; i < count; i++)
			{
				target[i + offset] = this.Draw(rand);
			}
		}



		public void Exclude(Card element)
		{
			_cards = _cards.Exclude(element);
		}

		public void Exclude(params ReadOnlySpan<Card> elements)
		{
			_cards = _cards.Exclude(elements);
		}

		public void Exclude(Cards elements)
		{
			_cards |= ~elements;
		}





		private bool TryDraw(Random random, out int index)
		{
			index = -1;

			if (!_cards.IsEmpty)
			{

				// 1. Zgjedhim indeksin e bitit të ndezur që duam të tërheqim
				index = random.Next(_cards.Count);

				// 2. Truku Branchless duke përdorur BMI2 (Hardware Accelerated)
				// Pdep (Parallel Deposit) vendos bitin e parë të ndezur të '1UL << targetIndex' 
				// saktësisht te biti i N-të i ndezur i maskës sonë '_deck'.
				Cards bitToClear;
				if (System.Runtime.Intrinsics.X86.Bmi2.X64.IsSupported)
				{
					bitToClear = (Cards)System.Runtime.Intrinsics.X86.Bmi2.X64.ParallelBitDeposit(1UL << index, (ulong)_cards);
				}
				else
				{
					// Versioni fallback pa cikle të gjata nëse BMI2 nuk mbështetet (Software alternative e shpejtë)
					Cards temp = _cards;
					for (int i = 0; i < index; i++) temp &= (Cards)(temp - 1);
					bitToClear = (Cards)((ulong)temp & (ulong)(-(long)(ulong)temp)); // Izolon bitin më të ulët të mbetur
				}

				index = BitOperations.TrailingZeroCount((ulong)bitToClear);


				_cards &= (~bitToClear);//deleting card at index
			}

			return index != -1;
		}

	}
}
