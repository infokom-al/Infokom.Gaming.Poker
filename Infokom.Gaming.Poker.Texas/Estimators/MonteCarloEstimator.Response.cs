using System.Collections;

namespace Infokom.Gaming.Poker.Texas.Estimators
{

	public partial class MonteCarloEstimator
	{
		public class Response
		{
			


			private readonly int[] _soleWins;
			private readonly int[] _sharedWins;
			private readonly int[] _topFinishes;
			private readonly double[] _equityShares;
			private readonly int _trials;
			private readonly int _litigants;

			public Response(ulong id, int litingants, int[] soleWins, int[] sharedWins, int[] topFinishes, double[] equityShares, int trials)
			{
				Id = id;
				_litigants = litingants;
				_soleWins = soleWins;
				_sharedWins = sharedWins;
				_topFinishes = topFinishes;
				_equityShares = equityShares;
				_trials = trials;
			}

			public ulong Id { get; set; }


			public int TrialCount => _trials;

			public int PlayerCount => _litigants;

			public int LitigantCount => _litigants;

			public int WinCountOf(int litigantIndex) => _soleWins[litigantIndex];
			public double WinRateOf(int litigantIndex) => (double)_soleWins[litigantIndex] / _trials;
			public double EquityOf(int litigantIndex) => _topFinishes[litigantIndex] * _equityShares[litigantIndex] / _trials;

			public readonly record struct Score(int SoleWins, int SharedWins, int TopFinishes, double EquityShare);


			

			public readonly struct ScoreList : IReadOnlyList<Score>
			{

				private readonly Response _owner;

				public ScoreList(Response owner)
				{
					_owner = owner;
				}

				public Score this[int index] => new(_owner._soleWins[index], _owner._sharedWins[index], _owner._topFinishes[index], _owner._equityShares[index] / _owner._trials);

				public int Count => _owner._soleWins.Length;



				public struct Enumerator : IEnumerator<Score>
				{
					private readonly ScoreList _span;

					private int _index;

					public Enumerator(ScoreList span)
					{
						_span = span;
						_index = -1;
					}

					public readonly Score Current => _span[_index];

					readonly object IEnumerator.Current => ((IEnumerator<Score>)this).Current;

					public bool MoveNext()
					{
						if (_index < _span.Count - 1)
						{
							_index++;
							return true;
						}
						return false;
					}

					void IEnumerator.Reset() => _index = -1;

					readonly void IDisposable.Dispose() { }
				}

				public Enumerator GetEnumerator() => new(this);

				IEnumerator<Score> IEnumerable<Score>.GetEnumerator() => this.GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
			}


			public ScoreList Scores => new(this);
		}
	}
}