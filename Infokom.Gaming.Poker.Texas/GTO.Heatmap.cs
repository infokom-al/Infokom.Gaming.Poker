using Infokom.Gaming.Poker.Texas.Estimators;
using Infokom.Numerics;

using System.Collections;
using System.Runtime.CompilerServices;

using static Infokom.Gaming.Poker.Texas.GTO;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		public sealed class Heatmap : IReadOnlyList<Cell<double>>
		{
			private static readonly string[,] CELL_LABELS = new string[13, 13]
			{
				{ "AA ", "AKs", "AQs", "AJs", "ATs", "A9s", "A8s", "A7s", "A6s", "A5s", "A4s", "A3s", "A2s" },
				{ "AKo", "KK ", "KQs", "KJs", "KTs", "K9s", "K8s", "K7s", "K6s", "K5s", "K4s", "K3s", "K2s" },
				{ "AQo", "KQo", "QQ ", "QJs", "QTs", "Q9s", "Q8s", "Q7s", "Q6s", "Q5s", "Q4s", "Q3s", "Q2s" },
				{ "AJo", "KJo", "QJo", "JJ ", "JTs", "J9s", "J8s", "J7s", "J6s", "J5s", "J4s", "J3s", "J2s" },
				{ "ATo", "KTo", "QTo", "JTo", "TT ", "T9s", "T8s", "T7s", "T6s", "T5s", "T4s", "T3s", "T2s" },
				{ "A9o", "K9o", "Q9o", "J9o", "T9o", "99 ", "98s", "97s", "96s", "95s", "94s", "93s", "92s" },
				{ "A8o", "K8o", "Q8o", "J8o", "T8o", "98o", "88 ", "87s", "86s", "85s", "84s", "83s", "82s" },
				{ "A7o", "K7o", "Q7o", "J7o", "T7o", "97o", "87o", "77 ", "76s", "75s", "74s", "73s", "72s" },
				{ "A6o", "K6o", "Q6o", "J6o", "T6o", "96o", "86o", "76o", "66 ", "65s", "64s", "63s", "62s" },
				{ "A5o", "K5o", "Q5o", "J5o", "T5o", "95o", "85o", "75o", "65o", "55 ", "54s", "53s", "52s" },
				{ "A4o", "K4o", "Q4o", "J4o", "T4o", "94o", "84o", "74o", "64o", "54o", "44 ", "43s", "42s" },
				{ "A3o", "K3o", "Q3o", "J3o", "T3o", "93o", "83o", "73o", "63o", "53o", "43o", "33 ", "32s" },
				{ "A2o", "K2o", "Q2o", "J2o", "T2o", "92o", "82o", "72o", "62o", "52o", "42o", "32o", "22 " }
			};



			private Heatmap()
			{
				Cells = new Cell[13, 13];
				Weights = new double[13, 13];
			}


			public Cell[,] Cells { get; }

			public double[,] Weights { get; }

			public int Count => 169;

			public Cell<double> this[int index] => (Cells[index / 13, index % 13], Weights[index / 13, index % 13]);

			public struct Enumerator : IEnumerator<Cell<double>>
			{
				private readonly Heatmap _owner;
				private int _offset = -1;
				public Enumerator(Heatmap owner) => this._owner = owner;
				public readonly Cell<double> Current => (_owner.Cells[_offset / 13, _offset % 13], _owner.Weights[_offset / 13, _offset % 13]);
				readonly object IEnumerator.Current => ((IEnumerator<Cell<double>>)this).Current;
				public bool MoveNext() => ++_offset < 169;
				void IEnumerator.Reset() => _offset = -1;
				readonly void IDisposable.Dispose() { }
			}

			public Enumerator GetEnumerator() => new(this);
			IEnumerator<Cell<double>> IEnumerable<Cell<double>>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();












			public static readonly Heatmap Cold = Create(0.0);

			public static Heatmap Create(double weight = 0.0)
			{
				var heatmap = new Heatmap();

				_ = Parallel.For(0, 13, r =>
				{
					for (int c = 0; c < 13; c++)
					{
						heatmap.Cells[r, c] = Cell.Parse(CELL_LABELS[r, c]);
						heatmap.Weights[r, c] = weight;
					}
				});

				return heatmap;
			}




			public class Generator
			{

				public readonly struct State
				{
					public readonly int Foregone;
					public readonly int Upcoming;
					public readonly GTO.Cell Cell;
					public readonly double Weight;
					
					public State(int foregone, int upcoming, GTO.Cell cell, double weight)
					{
						Foregone = foregone;
						Upcoming = upcoming;
						Cell = cell;
						Weight = weight;
					}

					public int Total => Foregone + Upcoming + 1;
				}

				public readonly struct OnProgressEventArgs
				{
					public readonly long Timestamp;
					public readonly int Foregone;
					public readonly int Upcoming;
					public readonly GTO.Cell CurrentCell;
					public readonly double CurrentWeight;

					public OnProgressEventArgs(long timestamp, int foregone, int upcoming, GTO.Cell current, double currentWeight)
					{
						Timestamp = timestamp;
						Foregone = foregone;
						Upcoming = upcoming;
						CurrentCell = current;
						CurrentWeight = currentWeight;
					}

					public int Total => Foregone + Upcoming + 1;
				}

				public delegate void OnProgressEventHandler(object sender, OnProgressEventArgs e);

				public event OnProgressEventHandler OnProgress;




				public readonly struct OnCompletedEventArgs
				{
					public readonly GTO.Heatmap Heatmap;
					public readonly double MinWeight;
					public readonly double MaxWeight;
					public OnCompletedEventArgs(GTO.Heatmap heatmap, double minWeight, double maxWeight)
					{
						Heatmap = heatmap;
						MinWeight = minWeight;
						MaxWeight = maxWeight;
					}
				}

				public delegate void OnCompletedEventHandler(object sender, OnCompletedEventArgs e);

				public event OnCompletedEventHandler OnCompleted;





				private readonly GTO.Range[] _opponents;
				private readonly GTO.Heatmap _heatmap;
				private int _offset;



				/// <summary>
				/// The lowest weight value encountered during heatmap generation.
				/// </summary>
				private double _min = 1.0;

				/// <summary>
				/// The highest weight value encountered during heatmap generation.
				/// </summary>
				private double _max = 0.0;

				public Generator(GTO.Heatmap heatmap, int opponents)
				{
					_opponents = new GTO.Range[opponents + 1];
					_opponents.AsSpan(1).Fill(GTO.Range.Ω/* & ~_opponents[0]*/);

					_heatmap =heatmap;// Initialize flat heatmap (all weights set to 0.0)
					_offset = -1; // Start before the first cell
				}


				private int Foregone => _offset;
				public (int Row, int Column, double Weight) Current => (_offset / 13, _offset % 13, _heatmap.Weights[_offset / 13, _offset % 13]);
				private int Upcoming => 13 * 13 - _offset - 1;


				private void Update(int row, int col)
				{
					_opponents[0] = GTO.Range.Create(Cell.Parse(CELL_LABELS[row, col]));
					var request = MonteCarloEstimator.Request.Create(_opponents);
					var result = MonteCarloEstimator.Handle(request);
					var weight = result.WinRateOf(0);
					_heatmap.Weights[row, col] = weight;
					if (weight < _min)
						_min = weight;
					if (weight > _max)
						_max = weight;
				}

				/// <summary>
				/// Computes the weight for a specific cell in the heatmap based on its linear index.
				/// </summary>
				/// <param name="index">Linear index of the cell in the heatmap.</param>
				/// <remarks>
				/// The linear index is the row-major order index of the cell in the 13x13 heatmap. The method calculates the corresponding row 
				/// and column from the index and then computes the weight for that cell.
				/// </remarks>	
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private bool Next()
				{
					if (++_offset < 13 * 13)
					{

						Update(_offset / 13, _offset % 13);

						OnProgress?.Invoke(this, new OnProgressEventArgs(
							DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
							Foregone,
							Upcoming, 
							Cell.Parse(CELL_LABELS[_offset / 13, _offset % 13]), 
							_heatmap.Weights[_offset / 13, _offset % 13]));

						return true;
					}
					else
					{
						OnCompleted?.Invoke(this, new OnCompletedEventArgs(_heatmap, _min, _max));

						return false;
					}
				}


				public void Start(Action<State> action)
				{
					while(Next())
					{
						action?.Invoke(new State(Foregone, Upcoming, Cell.Parse(CELL_LABELS[_offset / 13, _offset % 13]), _heatmap.Weights[_offset / 13, _offset % 13]));
					}
				}
			}

			public static Generator Generate(GTO.Heatmap heatmap, int opponents) => new(heatmap, opponents);
			public static void Generate(int opponents, Action<int, int, double> action) 
			{
				if (action is null)
					return;


				Span<GTO.Range> players = new GTO.Range[opponents + 1];
				players.Slice(1).Fill(GTO.Range.Ω);

				for (int row = 0; row < 13; row++)
				{
					for (int col = 0; col < 13; col++)
					{
						players[0] = GTO.Range.Create(Cell.Parse(CELL_LABELS[row, col]));
						var request = MonteCarloEstimator.Request.Create(players);
						var result = MonteCarloEstimator.Handle(request);
						var weight = result.WinRateOf(0);
						action.Invoke(row, col, weight);
					}
				}
			}
		}
	}
}