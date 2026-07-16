using Infokom.Gaming.Poker.Atomics;
using Infokom.Gaming.Poker.Texas;

using System.Collections;
using System.Numerics;

namespace Holdem.Core
{

	public partial class GTO
	{



		public readonly struct Cell : IReadOnlyCollection<Pocket>, IEquatable<Cell>, IEqualityOperators<Cell, Cell, bool>
		{
			private Cell(Rank hi, Rank lo, bool isSuited)
			{
				this.Hi = hi;
				this.Lo = lo;
				this.IsSuited = isSuited;
			}

			public Rank Hi { get; }
			public Rank Lo { get; }

			public bool IsSuited { get; }

			public bool IsPaired => this.Hi == this.Lo;

			public bool IsEmpty => !this.Hi.IsKnown && !this.Lo.IsKnown;



			public (int Row, int Col) Index
			{
				get
				{
					if(!this.IsPaired)
					{
						if(!this.IsSuited)
							return (Rank.IndexOf(this.Lo), Rank.IndexOf(this.Hi));
						return (Rank.IndexOf(this.Hi), Rank.IndexOf(this.Lo));
					}
					return (Rank.IndexOf(this.Hi), Rank.IndexOf(this.Hi));
				}
			}


			public string Symbol
			{
				get
				{
					var (c1, c2, c3) = this.IsEmpty ? (' ',' ',' ') : (Rank.SymbolOf(this.Hi), Rank.SymbolOf(this.Lo), this.IsPaired ? ' ' : this.IsSuited ? 's' : 'o');
					
					return $"{c1}{c2}{c3}";
				}
			}

			public override string ToString() => this.Symbol;


			public override int GetHashCode() => HashCode.Combine(this.Hi, this.Lo, this.IsSuited);


			









			public bool Equals(Cell other) => this.Hi.Equals(other.Hi) && this.Lo.Equals(other.Lo);
			public override bool Equals(object obj) => obj is Cell other && this.Equals(other);

			public static bool operator ==(Cell left, Cell right) => left.Hi == right.Hi && left.Lo == right.Lo;
			public static bool operator !=(Cell left, Cell right) => left.Hi != right.Hi || left.Lo != right.Lo;


			#region IReadOnlyCollection<Pocket> impl
			int IReadOnlyCollection<Pocket>.Count => this.IsEmpty ? 0 : this.IsSuited ? 4 : this.IsPaired ? 6 : 12;
			IEnumerator<Pocket> IEnumerable<Pocket>.GetEnumerator()
			{
				if (this.IsSuited)
				{
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Spade), Card.Of(this.Lo, Suit.Spade));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Heart), Card.Of(this.Lo, Suit.Heart));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Diamond), Card.Of(this.Lo, Suit.Diamond));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Club), Card.Of(this.Lo, Suit.Club));
				}
				else if (this.IsPaired)
				{
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Spade), Card.Of(this.Hi, Suit.Heart));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Spade), Card.Of(this.Hi, Suit.Diamond));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Spade), Card.Of(this.Hi, Suit.Club));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Heart), Card.Of(this.Hi, Suit.Diamond));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Heart), Card.Of(this.Hi, Suit.Club));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Diamond), Card.Of(this.Hi, Suit.Club));
				}
				else
				{
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Spade), Card.Of(this.Lo, Suit.Heart));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Spade), Card.Of(this.Lo, Suit.Diamond));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Spade), Card.Of(this.Lo, Suit.Club));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Heart), Card.Of(this.Lo, Suit.Spade));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Heart), Card.Of(this.Lo, Suit.Diamond));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Heart), Card.Of(this.Lo, Suit.Club));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Diamond), Card.Of(this.Lo, Suit.Spade));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Diamond), Card.Of(this.Lo, Suit.Heart));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Diamond), Card.Of(this.Lo, Suit.Club));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Club), Card.Of(this.Lo, Suit.Spade));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Club), Card.Of(this.Lo, Suit.Heart));
					yield return Pocket.Create(Card.Of(this.Hi, Suit.Club), Card.Of(this.Lo, Suit.Diamond));
				}
			}
			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Pocket>)this).GetEnumerator();
			#endregion

			public IReadOnlyCollection<Pocket> Combos => this;






			internal static Cell FromIndex(int rowIndex, int colIndex)
			{
				Rank rowRank = Rank.Values[rowIndex];
				Rank colRank = Rank.Values[colIndex];

				return rowIndex == colIndex 
					? new GTO.Cell(rowRank, rowRank, isSuited: false) 
					: rowIndex > colIndex 
						? new GTO.Cell(rowRank, colRank, isSuited: true) 
						: new GTO.Cell(colRank, rowRank, isSuited: false);
			}

			internal static Cell FromIndex(int index) => (uint)index < 169 
				? FromIndex(index / 13, index % 13) 
				: throw new ArgumentOutOfRangeException(nameof(index), "Indeksi duhet të jetë midis 0 dhe 168.");

			public static Cell Pair(Rank rank) => new(rank, rank, false);

			public static Cell Suited(Rank rank1, Rank rank2)
			{
				ArgumentOutOfRangeException.ThrowIfEqual(rank1, rank2);

				return rank1 > rank2 ? new(rank1, rank2,true) : new(rank2, rank1, true);
			}

			public static Cell Offsuited(Rank rank1, Rank rank2)
			{
				ArgumentOutOfRangeException.ThrowIfEqual(rank1, rank2);

				return rank1 > rank2 ? new(rank1, rank2, false) : new(rank2, rank1, false);
			}




			public static IEnumerable<Cell> AllPairs
			{
				get
				{
					yield return GTO.Cell.Pair(Rank.Two);
					yield return GTO.Cell.Pair(Rank.Three);
					yield return GTO.Cell.Pair(Rank.Four);
					yield return GTO.Cell.Pair(Rank.Five);
					yield return GTO.Cell.Pair(Rank.Six);
					yield return GTO.Cell.Pair(Rank.Seven);
					yield return GTO.Cell.Pair(Rank.Eight);
					yield return GTO.Cell.Pair(Rank.Nine);
					yield return GTO.Cell.Pair(Rank.Ten);
					yield return GTO.Cell.Pair(Rank.Jack);
					yield return GTO.Cell.Pair(Rank.Queen);
					yield return GTO.Cell.Pair(Rank.King);
					yield return GTO.Cell.Pair(Rank.Ace);
				}
			}


			public static IEnumerable<Cell> AllSuited
			{
				get
				{
					for (int i = 0; i < Rank.Values.Length; i++)
					{
						for (int j = i + 1; j < Rank.Values.Length; j++)
						{
							yield return GTO.Cell.Suited(Rank.Values[i], Rank.Values[j]);
						}
					}
				}
			}


			public static IEnumerable<Cell> AllOffsuited
			{
				get
				{
					for (int i = 0; i < Rank.Values.Length; i++)
					{
						for (int j = i + 1; j < Rank.Values.Length; j++)
						{
							yield return GTO.Cell.Offsuited(Rank.Values[i], Rank.Values[j]);
						}
					}
				}
			}

		}



	}



}
