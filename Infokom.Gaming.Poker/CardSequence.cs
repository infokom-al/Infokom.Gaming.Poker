using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Infokom.Gaming.Poker
{

	public readonly struct CardSequence : IEquatable<CardSequence>, IEnumerable<Card>, IReadOnlyCollection<Card>, IReadOnlyList<Card>
	{
		#region constants
		private const ulong LEN0_MAX_ID = 0;

		private const ulong LEN1_MIN_ID = 1;
		private const ulong LEN1_MAX_ID = 52;

		private const ulong LEN2_MIN_ID = 53;
		private const ulong LEN2_MAX_ID = 2808;

		private const ulong LEN3_MIN_ID = 2809;
		private const ulong LEN3_MAX_ID = 148876;

		private const ulong LEN4_MIN_ID = 148877;
		private const ulong LEN4_MAX_ID = 7890480;

		private const ulong LEN5_MIN_ID = 7890481;
		private const ulong LEN5_MAX_ID = 418195492;

		private const ulong LEN6_MIN_ID = 418195493;
		private const ulong LEN6_MAX_ID = 22164361128;

		private const ulong LEN7_MIN_ID = 22164361129;
		private const ulong LEN7_MAX_ID = 1174711139836;

		private const ulong LEN8_MIN_ID = 1174711139837;
		private const ulong LEN8_MAX_ID = 62259690411360;

		private const ulong LEN9_MIN_ID = 62259690411361;
		private const ulong LEN9_MAX_ID = 3299763591802132;

		private const ulong LEN10_MIN_ID = 3299763591802133;
		private const ulong LEN10_MAX_ID = 174887470365513048;

		private const ulong LEN11_MIN_ID = 174887470365513049;
		private const ulong LEN11_MAX_ID = 9269035929372191596;

		private const ulong LEN12_MIN_ID = 9269035929372191597;
		private const ulong LEN12_MAX_ID = ulong.MaxValue; // Limiti teorik i sekuencës 12 kalon ulong.MaxValue
		#endregion

		#region static encode/decode

		
		/// <summary>
		/// Enkodon deri në 12 karta në një numër unik ulong duke përdorur Card.Index.
		/// Hedh OverflowException nëse sekuenca prej 12 kartash kalon ulong.MaxValue.
		/// </summary>
		private static ulong Encode(ReadOnlySpan<Card> cards)
		{
			if (cards.Length > 11)
				throw new ArgumentException("Sequence length limited to 11.");

			ulong id = 0;

			checked
			{
				if (cards.Length > 0x0) id = id * 53 + (ulong)(cards[0x0].Index + 1);
				if (cards.Length > 0x1) id = id * 53 + (ulong)(cards[0x1].Index + 1);
				if (cards.Length > 0x2) id = id * 53 + (ulong)(cards[0x2].Index + 1);
				if (cards.Length > 0x3) id = id * 53 + (ulong)(cards[0x3].Index + 1);
				if (cards.Length > 0x4) id = id * 53 + (ulong)(cards[0x4].Index + 1);
				if (cards.Length > 0x5) id = id * 53 + (ulong)(cards[0x5].Index + 1);
				if (cards.Length > 0x6) id = id * 53 + (ulong)(cards[0x6].Index + 1);
				if (cards.Length > 0x7) id = id * 53 + (ulong)(cards[0x7].Index + 1);
				if (cards.Length > 0x8) id = id * 53 + (ulong)(cards[0x8].Index + 1);
				if (cards.Length > 0x9) id = id * 53 + (ulong)(cards[0x9].Index + 1);
				if (cards.Length > 0xA) id = id * 53 + (ulong)(cards[0xA].Index + 1);
				if (cards.Length > 0xB) id = id * 53 + (ulong)(cards[0xB].Index + 1);
			}

			return id;
		}

		private static Card[] Decode(ulong id)
		{
			var n = id switch
			{
				LEN0_MAX_ID => 0,
				<= LEN1_MAX_ID => 1,
				<= LEN2_MAX_ID => 2,
				<= LEN3_MAX_ID => 3,
				<= LEN4_MAX_ID => 4,
				<= LEN5_MAX_ID => 5,
				<= LEN6_MAX_ID => 6,
				<= LEN7_MAX_ID => 7,
				<= LEN8_MAX_ID => 8,
				<= LEN9_MAX_ID => 9,
				<= LEN10_MAX_ID => 10,
				<= LEN11_MAX_ID => 11,
				_ => 12,
			};

			Card[] array = new Card[n];
			Span<Card> result = new(array);

			for (int i = n - 1; i >= 0; i--)
			{
				var rem = id % 53;
				if (rem == 0)
				{
					result[i] = Card.Values[51];
					id = (id / 53) - 1;
				}
				else
				{
					result[i] = Card.Values[(int)rem - 1];
					id /= 53;
				}
			}

			return array;
		}
		#endregion





		private readonly ulong _id;//sequence indexer

		public CardSequence(ulong id)
		{
			_id = id;
		}


		/// <summary>
		/// Kthen numrin e kartave të enkoduara në këtë sekuencë pa e dekoduar atë.
		/// </summary>
		public int Count
		{
			get
			{
				if (_id == LEN0_MAX_ID) return 0;
				if (_id <= LEN1_MAX_ID) return 1;
				if (_id <= LEN2_MAX_ID) return 2;
				if (_id <= LEN3_MAX_ID) return 3;
				if (_id <= LEN4_MAX_ID) return 4;
				if (_id <= LEN5_MAX_ID) return 5;
				if (_id <= LEN6_MAX_ID) return 6;
				if (_id <= LEN7_MAX_ID) return 7;
				if (_id <= LEN8_MAX_ID) return 8;
				if (_id <= LEN9_MAX_ID) return 9;
				if (_id <= LEN10_MAX_ID) return 10;
				if (_id <= LEN11_MAX_ID) return 11;
				return 12;
			}
		}


		public Card this[int i]
		{
			get
			{
				int n = Count;

				if ((uint)i >= n)
					throw new IndexOutOfRangeException($"{i} out of [0..{n}]");

				var id = _id;

				// Gjejmë sa hapa larg fundit ndodhet ky pozicion
				int stepsFromEnd = n - 1 - i;

				// Llogarisim fuqinë e pozicionit të 53-shit pa cikle
				ulong divisor = 1;
				if (stepsFromEnd > 0) divisor *= 53;
				if (stepsFromEnd > 1) divisor *= 53;
				if (stepsFromEnd > 2) divisor *= 53;
				if (stepsFromEnd > 3) divisor *= 53;
				if (stepsFromEnd > 4) divisor *= 53;
				if (stepsFromEnd > 5) divisor *= 53;
				if (stepsFromEnd > 6) divisor *= 53;
				if (stepsFromEnd > 7) divisor *= 53;
				if (stepsFromEnd > 8) divisor *= 53;
				if (stepsFromEnd > 9) divisor *= 53;
				if (stepsFromEnd > 10) divisor *= 53;

				// Izolojmë shifrën korresponduese bijective në atë pozicion
				id /= divisor;
				ulong rem = id % 53;

				if (rem == 0)
				{
					return Card.Values[51]; // Ace of Spades (karta e fundit)
				}

				return Card.Values[(int)rem - 1];
			}
		}

		public CardSequence Append(Card card)
		{
			if (this.Count >= 11)
				throw new InvalidOperationException("The sequence has already reached the limit length of 12");

			// Në bijective base-53, shtimi në fund bëhet duke e zhvendosur të gjithë numrin majtas (shumëzim me 53)
			// dhe duke shtuar vlerën e kartës së re në pozicionin e mbetur bosh (karta.Index + 1)
			checked
			{
				ulong newId = _id * 53 + (ulong)(card.Index + 1);
				return new CardSequence(newId);
			}
		}


		#region IEquatable<CardSequence>
		public override int GetHashCode() => _id.GetHashCode();
		public bool Equals(CardSequence other) => this._id == other._id;
		public override bool Equals([NotNullWhen(true)] object obj) => obj is CardSequence other && this.Equals(other);
		public static bool operator ==(CardSequence left, CardSequence right) => left.Equals(right);
		public static bool operator !=(CardSequence left, CardSequence right) => !(left == right);
		#endregion

		#region IEnumerable<Card>
		public struct Enumerator : IEnumerator<Card>
		{
			private ulong _currentId;
			private readonly int _totalCount;
			private int _currentIndex;
			private Card _currentCard;

			public Enumerator(ulong id, int count)
			{
				_currentId = id;
				_totalCount = count;
				_currentIndex = 0;
				_currentCard = default;
			}

			public readonly Card Current => _currentCard;


			public bool MoveNext()
			{
				if (_currentIndex >= _totalCount)
					return false;

				// Gjejmë pozicionin mbrapsht (sa hapa larg fundit jemi)
				int stepsFromEnd = _totalCount - 1 - _currentIndex;

				// Llogarisim fuqinë e duhur të 53-shit pa cikle
				ulong divisor = 1;
				if (stepsFromEnd > 0) divisor *= 53;
				if (stepsFromEnd > 1) divisor *= 53;
				if (stepsFromEnd > 2) divisor *= 53;
				if (stepsFromEnd > 3) divisor *= 53;
				if (stepsFromEnd > 4) divisor *= 53;
				if (stepsFromEnd > 5) divisor *= 53;
				if (stepsFromEnd > 6) divisor *= 53;
				if (stepsFromEnd > 7) divisor *= 53;
				if (stepsFromEnd > 8) divisor *= 53;
				if (stepsFromEnd > 9) divisor *= 53;
				if (stepsFromEnd > 10) divisor *= 53;

				// Nxjerrim vlerën e kartës në atë pozicion specifik
				ulong tempId = _currentId / divisor;
				ulong rem = tempId % 53;

				if (rem == 0)
				{
					_currentCard = Card.Values[51]; // Ace of Spades
											  // Kur rem == 0, në logjikën bijective zbresim 1 nga id e mbetur për atë pozicion
					_currentId -= divisor * 53;
				}
				else
				{
					_currentCard = Card.Values[(int)rem - 1];
					_currentId -= divisor * rem;
				}

				_currentIndex++;
				return true;
			}

			void IEnumerator.Reset() => throw new NotSupportedException();
			readonly object IEnumerator.Current => Current;
			readonly void IDisposable.Dispose() { }
		}


		public Enumerator GetEnumerator() => new(_id, Count);
		IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => this.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
		#endregion



		public CardSpectrum ToSpectrum()
		{
			var id = _id;
			int n = id switch
			{
				LEN0_MAX_ID => 0,
				<= LEN1_MAX_ID => 1,
				<= LEN2_MAX_ID => 2,
				<= LEN3_MAX_ID => 3,
				<= LEN4_MAX_ID => 4,
				<= LEN5_MAX_ID => 5,
				<= LEN6_MAX_ID => 6,
				<= LEN7_MAX_ID => 7,
				<= LEN8_MAX_ID => 8,
				<= LEN9_MAX_ID => 9,
				<= LEN10_MAX_ID => 10,
				<= LEN11_MAX_ID => 11,
				_ => 12,
			};

			var spectrum = CardSpectrum.Empty;

			for (int i = n - 1; i >= 0; i--)
			{
				var rem = id % 53;
				if (rem == 0)
				{
					spectrum |= (CardSpectrum)(1ul << 51);
					id = (id / 53) - 1;
				}
				else
				{
					spectrum |= (CardSpectrum)(1ul << ((int)rem - 1));
					id /= 53;
				}
			}

			return spectrum;
		}





		public override string ToString()
		{
			var result = string.Join(", ", this.Select(c => c.Symbol));

			return $"[{result}]";
		}








		public static readonly CardSequence Empty = new(0);

		public static CardSequence Create(params ReadOnlySpan<Card> cards)
		{
			var id = Encode(cards);

			return new(id);
		}


		public static CardSequence Create(Random rand, int length)
		{
			var n = Math.Clamp(length, 0, 11);

			ulong id = 0;
			if(n > 0) id = id * 53;
			if(n > 1) id = id * 53;
			if(n > 2) id = id * 53;
			if(n > 3) id = id * 53;
			if(n > 4) id = id * 53;
			if(n > 5) id = id * 53;
			if(n > 6) id = id * 53;
			if(n > 7) id = id * 53;
			if(n > 8) id = id * 53;
			if(n > 9) id = id * 53;
			if(n > 10) id = id * 53;

			id = id + (ulong)rand.Next(0, (int)(id - id / 53));


			return new(id);
		}




		public static implicit operator CardSequence(ValueTuple<Card> source) => Create(source.Item1);

		public static implicit operator CardSequence(ValueTuple<Card, Card> source)
		{
			var (c1, c2) = source;

			return Create(c1, c2);
		}

		public static implicit operator CardSequence(ValueTuple<Card, Card, Card> source)
		{
			var (c1, c2, c3) = source;

			return Create(c1, c2, c3);
		}

		public static implicit operator CardSequence(ValueTuple<Card, Card, Card, Card> source)
		{
			var (c1, c2, c3, c4) = source;

			return Create(c1, c2, c3, c4);
		}

		public static implicit operator CardSequence(ValueTuple<Card, Card, Card, Card, Card> source)
		{
			var (c1, c2, c3, c4, c5) = source;

			return Create(c1, c2, c3, c4, c5);
		}

		public static implicit operator CardSequence(ValueTuple<Card, Card, Card, Card, Card, Card> source)
		{
			var (c1, c2, c3, c4, c5, c6) = source;

			return Create(c1, c2, c3, c4, c5, c6);
		}

		public static implicit operator CardSequence(ValueTuple<Card, Card, Card, Card, Card, Card, Card> source)
		{
			var (c1, c2, c3, c4, c5, c6, c7) = source;

			return Create(c1, c2, c3, c4, c5, c6, c7);
		}
	}


}
