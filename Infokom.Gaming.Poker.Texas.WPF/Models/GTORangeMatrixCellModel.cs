using System.ComponentModel;
using System.Windows;

namespace Infokom.Gaming.Poker.Texas.WPF.Models
{
	public class GTORangeMatrixCellModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		
		private string _label;
		private double _weight;
		private bool _isSelected;

		public GTORangeMatrixCellModel()
		{
			_label = string.Empty;
			_weight = 0.0;
			_isSelected = false;
		}

		
		public string Label { get => _label; set { _label = value; OnPropertyChanged(nameof(Label)); } }
		public double Weight { get => _weight; set { _weight = value; OnPropertyChanged(nameof(Weight)); } }
		public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }
	}
}
