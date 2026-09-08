using System.Collections;

namespace Infokom.Gaming.Poker.Texas
{
	public partial class Equity
	{
		//	
		//	┌─────┬─────┼─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┼─────┐     ▲  
		//	│     |     |     |     |     |     |     |     |     |     |     |     |     |     |     |     | 0xF |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼-----┤
		//	│     |     | AA  | AKs | AQs | AJs | ATs | A9s | A8s | A7s | A6s | A5s | A4s | A3s | A2s |     | 0xE |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | AKo | KK  | KQs | KJs | KTs | K9s | K8s | K7s | K6s | K5s | K4s | K3s | K2s |     | 0xD |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | AQo | KQo | QQ  | QJs | QTs | Q9s | Q8s | Q7s | Q6s | Q5s | Q4s | Q3s | Q2s |     | 0xC |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | AJo | KJo | QJo | JJ  | JTs | J9s | J8s | J7s | J6s | J5s | J4s | J3s | J2s |     | 0xB |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | ATo | KTo | QTo | JTo | TT  | T9s | T8s | T7s | T6s | T5s | T4s | T3s | T2s |     | 0xA |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A9o | K9o | Q9o | J9o | T9o | 99  | 98s | 97s | 96s | 95s | 94s | 93s | 92s |     | 0x9 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A8o | K8o | Q8o | J8o | T8o | 98o | 88  | 87s | 86s | 85s | 84s | 83s | 82s |     | 0x8 |     Y
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A7o | K7o | Q7o | J7o | T7o | 97o | 87o | 77  | 76s | 75s | 74s | 73s | 72s |     | 0x7 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A6o | K6o | Q6o | J6o | T6o | 96o | 86o | 76o | 66  | 65s | 64s | 63s | 62s |     | 0x6 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A5o | K5o | Q5o | J5o | T5o | 95o | 85o | 75o | 65o | 55  | 54s | 53s | 52s |     | 0x5 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A4o | K4o | Q4o | J4o | T4o | 94o | 84o | 74o | 64o | 54o | 44  | 43s | 42s |     | 0x4 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A3o | K3o | Q3o | J3o | T3o | 93o | 83o | 73o | 63o | 53o | 43o | 33  | 32s |     | 0x3 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     | A2o | K2o | Q2o | J2o | T2o | 92o | 82o | 72o | 62o | 52o | 42o | 32o | 22  |     | 0x2 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼-----┤
		//	│     |     |     |     |     |     |     |     |     |     |     |     |     |     |     |     | 0x1 |
		//	├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤     |
		//	│     |     |     |     |     |     |     |     |     |     |     |     |     |     |     |     | 0x0 |
		//	└─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┘     ┴
		//	  0xF   0xE   0xD   0xC   0xB   0xA   0x9   0x8   0x7   0x6   0x5   0x4   0x3   0x2   0x1   0x0            
		//	            ├─────────────────────────────────────────────────────────────────────────────┤
		//	◂─────────────────────────────────────────── x ─────────────────────────────────────────────────┤



		public readonly partial struct Cell : IEquatable<Cell>
		{
			private readonly sbyte _x, _y;

			public Cell(sbyte x, sbyte y)
			{
				if ((uint)x > 15)
					throw new ArgumentOutOfRangeException(nameof(x));

				if ((uint)y > 15)
					throw new ArgumentOutOfRangeException(nameof(y));

				_x = x;
				_y = y;
			}

			public Rank X => (Rank)_x;

			public Rank Y => (Rank)_y;

			/// <summary>
			/// The higher of the two ranks in this cell.
			/// </summary>
			public Rank Hi => _x > _y ? (Rank)_x : (Rank)_y;

			/// <summary>
			/// The lower of the two ranks in this cell.
			/// </summary>
			public Rank Lo => _x < _y ? (Rank)_x : (Rank)_y;


			//$$hi(X_{i,j}) = lo(X_{i,j})$$
			/// <summary>
			/// True when both cards have the same suit.
			/// </summary>
			public bool IsSuited => _x is >= 2 and <= 14 && _x < _y;

			/// <summary>
			/// True when both cards have the same rank.
			/// </summary>
			public bool IsPaired => _x is >= 2 and <= 14 && _x == _y;


			public int Index
			{
				get
				{
					int hi = Math.Max(_x, _y);
					int lo = Math.Min(_x, _y);

					// 169-cell index
					if (hi == lo)
						return hi - 2;

					int nonPairIndex = hi * (hi - 1) / 2 + lo;

					return IsSuited
					    ? 13 + nonPairIndex
					    : 91 + nonPairIndex;
				}
			}



			/// <summary>
			/// 
			/// </summary>
			/// <param name="pocket"></param>
			/// <returns></returns>
			/// <remarks>
			/// "Does this concrete pocket belong to me?"
			/// </remarks>
			public bool Matches(Pocket pocket)
			{
				var ((r1, s1), (r2, s2)) = pocket;


				var (a, b) = pocket;

				if (_x == _y)
					return a.R == _x && b.R == _y;

				if (_x < _y)
				{
					// Suited cell: low rank on x, high rank on y.
					return a.S == b.S
					    && Math.Min((sbyte)r1, (sbyte)r2) == _x
					    && Math.Max((sbyte)r1, (sbyte)r2) == _y;
				}

				// Offsuit cell: high rank on x, low rank on y.
				return a.Suit != b.Suit
				    && Math.Max((sbyte)r1, (sbyte)r2) == _x
				    && Math.Min((sbyte)r1, (sbyte)r2) == _y;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <remarks>
			/// TODO: integrate inside Hands iterator
			/// </remarks>
			public int Count => IsPaired ? 6 : IsSuited ? 4 : 12;


			

			public override string ToString() => string.Create(3, this, static (buffer, source) =>
			{
				buffer[0] = source.Hi.Symbol;
				buffer[1] = source.Lo.Symbol;
				buffer[2] = source.IsPaired ? ' ' : source.IsSuited ? 's' : 'o';
			});

			public override int GetHashCode() => HashCode.Combine(_x, _y);
			public bool Equals(Cell other) => _x == other._x && _y == other._y;
			public override bool Equals(object obj) => obj is Cell other && Equals(other);
			public static bool operator ==(Cell left, Cell right) => left.Equals(right);
			public static bool operator !=(Cell left, Cell right) => !left.Equals(right);

			public static Cell Paired(Rank rank) => new((sbyte)rank, (sbyte)rank);
			public static Cell Suited(Rank high, Rank low) => new((sbyte)low, (sbyte)high);
			public static Cell Offsuit(Rank high, Rank low) => new((sbyte)high, (sbyte)low);






		}
		
		
		
		
		
		public readonly partial struct Cell
		{
			public PocketIterator Pockets => new(this);

			public readonly struct PocketIterator : IGrouping<Cell, Pocket>
			{
				private readonly Cell _owner;

				public PocketIterator(Cell owner) => _owner = owner;

				Cell IGrouping<Cell, Pocket>.Key => _owner;

				// TODO: replace with custom enumerator
				/// <summary>
				/// 
				/// </summary>
				/// <returns></returns>
				public IEnumerator<Pocket> GetEnumerator()
				{
					if (_owner.IsPaired)
					{
						var ρ = _owner.Hi;
						yield return (ρ * σ1, ρ * σ2);
						yield return (ρ * σ1, ρ * σ3);
						yield return (ρ * σ1, ρ * σ4);
						yield return (ρ * σ2, ρ * σ3);
						yield return (ρ * σ2, ρ * σ4);
						yield return (ρ * σ3, ρ * σ4);
					}
					else if (_owner.IsSuited)
					{
						var (ρ1, ρ2) = (_owner.Hi, _owner.Lo);
						yield return (ρ1 * σ1, ρ2 * σ1);
						yield return (ρ1 * σ2, ρ2 * σ2);
						yield return (ρ1 * σ3, ρ2 * σ3);
						yield return (ρ1 * σ4, ρ2 * σ4);
					}
					else
					{
						var (ρ1, ρ2) = (_owner.Hi, _owner.Lo);
						yield return (ρ1 * σ1, ρ2 * σ2);
						yield return (ρ1 * σ1, ρ2 * σ3);
						yield return (ρ1 * σ1, ρ2 * σ4);
						yield return (ρ1 * σ2, ρ2 * σ1);
						yield return (ρ1 * σ2, ρ2 * σ3);
						yield return (ρ1 * σ2, ρ2 * σ4);
						yield return (ρ1 * σ3, ρ2 * σ1);
						yield return (ρ1 * σ3, ρ2 * σ2);
						yield return (ρ1 * σ3, ρ2 * σ4);
						yield return (ρ1 * σ4, ρ2 * σ1);
						yield return (ρ1 * σ4, ρ2 * σ2);
						yield return (ρ1 * σ4, ρ2 * σ3);
					}
				}

				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
			}

			
			
		}
	}
}
