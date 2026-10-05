using Infokom.Gaming.Poker.Texas.Estimators;

using MediatR;

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Infokom.Gaming.Poker.Texas.WPF.Models
{
	public class GTORangePickerModel : INotifyPropertyChanging, INotifyPropertyChanged
	{
		public event PropertyChangingEventHandler PropertyChanging;
		private void OnPropertyChanging(string propertyName) => this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));




		private GTORangeMatrixModel _matrix;
		private readonly Estimator _estimator;

		private int _estimatorRivalsCount;
		private int _estimatorTrialsCount;
		private bool _estimatorIsRunning;
		private int _estimatorProgress;

		private int _selectedCellsCount;

		public GTORangePickerModel()
		{
			_matrix = new GTORangeMatrixModel();
			_estimator = new Estimator(this);
			_estimatorRivalsCount = 1;
			_estimatorTrialsCount = 10000;
			_selectedCellsCount = 0;
		}

		public ImmutableArray<int> EstimatorRivalsCountOptions { get; } = [.. Enumerable.Range(1, 8)];

		public int EstimatorRivalsCount
		{
			get => _estimatorRivalsCount;
			set
			{
				if (value is < 1 or > 9)
				{
					throw new ArgumentOutOfRangeException(nameof(EstimatorRivalsCount), "The number of opponents must be between 1 and 8.");
				}
				_estimatorRivalsCount = value;
				this.OnPropertyChanged(nameof(EstimatorRivalsCount));
			}
		}

		public int EstimatorTrialsCount
		{
			get => _estimatorTrialsCount;
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException(nameof(EstimatorTrialsCount), "The number of trials must be greater than 0.");
				}
				_estimatorTrialsCount = value;
				this.OnPropertyChanged(nameof(EstimatorTrialsCount));
			}
		}

		public bool EstimatorIsRunning
		{
			get => _estimatorIsRunning;
			set
			{
				_estimatorIsRunning = value;
				this.OnPropertyChanged(nameof(EstimatorIsRunning));
			}
		}

		public int EstimatorProgress
		{
			get => _estimatorProgress;
			set 
			{
				if (value is < 0 or > 169)
				{
					throw new ArgumentOutOfRangeException(nameof(EstimatorProgress), "The progress must be between 0 and 169.");
				}
				_estimatorProgress = value;
				this.OnPropertyChanged(nameof(EstimatorProgress));
			}
		}



		/// <summary>
		/// 169 cells matrix representing all possible starting hands in Texas Hold'em.
		/// </summary>
		public GTORangeMatrixModel Matrix
		{
			get => this._matrix;
			private set
			{
				_matrix = value;
				this.OnPropertyChanged(nameof(Matrix));
			}
		}






		/// <summary>
		/// Number of the top selected cells in the matrix. This is used to determine the percentage of hands selected by the user.
		/// </summary>
		public int SelectedCellsCount
		{
			get => _selectedCellsCount;
			set
			{
				if (_selectedCellsCount != value)
				{
					_selectedCellsCount = value;
					this.OnPropertyChanging(nameof(SelectedCellsCount));
					this.OnPropertyChanging(nameof(SelectedCellsFraction));
					this.OnPropertyChanging(nameof(SelectedCells));
					this.OnPropertyChanging(nameof(Matrix));

					_ = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
					{
						try
						{
							this.UpdateSelectedCells();
						}
						catch (Exception ex)
						{
							Debug.WriteLine($"Error updating selected cells: {ex.Message}");
						}

						this.OnPropertyChanged(nameof(SelectedCellsCount));
						this.OnPropertyChanged(nameof(SelectedCellsFraction));
						this.OnPropertyChanged(nameof(SelectedCells));
						this.OnPropertyChanged(nameof(Matrix));
					}));

					
				}
			}
		}

		public double SelectedCellsFraction => (double)this.SelectedCellsCount / 169;

		public IEnumerable<GTORangeMatrixCellModel> SelectedCells => _matrix.Where(c => c.IsSelected).OrderByDescending(c => c.Weight);

		private void UpdateSelectedCells()
		{
			var n = _selectedCellsCount;

			foreach (var cell in _matrix.OrderByDescending(c => c.Weight))
			{
				if (n-- > 0)
				{
					cell.IsSelected = true;
				}
				else
				{
					cell.IsSelected = false;
				}
			}
		}




		private class Estimator : ICommand
		{
			private bool _busy = false;
			private int _progress = 0;

			private readonly GTORangePickerModel _model;

			public Estimator(GTORangePickerModel model) => _model = model;

			public event EventHandler CanExecuteChanged;

			public bool CanExecute(object parameter) => true;
			
			public void Execute(object parameter)
			{
				_model.EstimatorIsRunning = true;

				_model.OnPropertyChanging(nameof(GTORangePickerModel.Matrix));

				var ranges = new GTO.Range[_model.EstimatorRivalsCount + 1];
				ranges.AsSpan()[1..].Fill(GTO.Range.Ω);


				int i = 0;
				foreach (var cell in GTO.Range.Ω.Cells)
				{
					ranges[0] = GTO.Range.Create(cell);

					var request = MonteCarloEstimator.Request.Create(ranges.AsSpan(), _model.EstimatorTrialsCount);
					var response = MonteCarloEstimator.Handle(request);

					_model._matrix[cell.Row, cell.Col].Weight = response.WinRateOf(0);

					_model.EstimatorProgress = ++i;
				}

				_model.UpdateSelectedCells();

				_model.OnPropertyChanged(nameof(GTORangePickerModel.Matrix));

				_model.EstimatorIsRunning = false;
			}
		}

		public ICommand EstimateCommand => new Estimator(this);
	}
}
