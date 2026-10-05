using Infokom.Gaming.Poker.Texas.Estimators;

using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace Infokom.Gaming.Poker.Texas.WPF.Models
{
	public class GTORangeMatrixModel : IEnumerable<GTORangeMatrixCellModel>
	{
		private readonly GTORangeMatrixCellModel[,] _cells = new GTORangeMatrixCellModel[13, 13];

		public GTORangeMatrixModel()
		{
			int i = 0;
			foreach (var cell in GTO.Range.Ω.Cells.Reverse())
			{
				_cells[i / 13, i % 13] = new GTORangeMatrixCellModel()
				{
					Label = cell.ToString(),
				};

				i++;
			}
		}


		public GTORangeMatrixCellModel this[int row, int column] => _cells[row, column];


		public IEnumerator<GTORangeMatrixCellModel> GetEnumerator()
		{
			for (int i = 0; i < 13; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					yield return _cells[i, j];
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
