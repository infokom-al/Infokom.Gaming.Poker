using Infokom.Gaming.Poker.Texas;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace TexasRangeGUI
{
	/// <summary>
	/// Interaction logic for GTORangePicker.xaml
	/// </summary>
	public partial class GTORangePicker : Window
	{
		private readonly ObservableCollection<GTO.Cell> _selectedCells = [];

		public GTORangePicker()
		{

			this.InitializeComponent();

			_selectedCells.CollectionChanged += (s, e) => UpdateSelectedCellsDisplay();
		}

		private void Cell_Checked(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton control)
			{
				if (control.Content is string label)
				{
					if (Regex.IsMatch(label, @"^[2-9TJQKA]{2}[so ]$"))
					{
						Debug.WriteLine($"Cell checked: {label}");
						if(GTO.Cell.TryParse(label, out var cell))
						{
							_selectedCells.Add(cell);
						}
					}
				}
			}
		}

		private void Cell_Unchecked(object sender, RoutedEventArgs e)
		{
			if(sender is ToggleButton control)
			{
				if (control.Content is string label)
				{
					Debug.WriteLine($"Cell unchecked: {label}");
					if(GTO.Cell.TryParse(label, out var cell))
					{
						_ = _selectedCells.Remove(cell);
					}
				}
			}
		}

		private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach(var item in this.CellsGrid.Children)
			{
				if (item is ToggleButton control)
				{
					if(control.Content is string label)
					{
						if(Regex.IsMatch(label, @"^[2-9TJQKA]{2}[so ]$"))
						{
							control.IsChecked = true;
						}
					}
				}
			}
		}

		private void ButtonSelectAnySuited_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in this.CellsGrid.Children)
			{
				if (item is ToggleButton control)
				{
					if (control.Content is string label)
					{
						if (Regex.IsMatch(label, @"^[2-9TJQKA]{2}[s]$"))
						{
							control.IsChecked = true;
						}
					}
				}
			}
		}

		private void ButtonSelectAnyBrodway_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in this.CellsGrid.Children)
			{
				if (item is ToggleButton control)
				{
					if (control.Content is string label)
					{
						if (Regex.IsMatch(label, @"^[TJQKA]{2}[so ]$"))
						{
							control.IsChecked = true;
						}
					}
				}
			}
		}

		private void ButtonSelectAnyPair_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in this.CellsGrid.Children)
			{
				if (item is ToggleButton control)
				{
					if (control.Content is string label)
					{
						if (Regex.IsMatch(label, @"^[2-9TJQKA]{2}[ ]$"))
						{
							control.IsChecked = true;
						}
					}
				}
			}
		}

		private void ButtonClearSelection_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in this.CellsGrid.Children)
			{
				if (item is ToggleButton control)
				{
					if (control.Content is string label)
					{
						if (Regex.IsMatch(label, @"^[2-9TJQKA]{2}[so ]$"))
						{
							control.IsChecked = false;
						}
					}
				}
			}
		}


		private void UpdateSelectedCellsDisplay()
		{
			var range = GTO.Range.Create(_selectedCells.ToArray());

			this.SelectedCellsTextBox.Text =range.ToString();
		}
	}

	/// <summary>
	/// Simple converter that classifies a cell by its Content text:
	/// returns "Suited" if content ends with 's', "Pair" if it's a pair label (e.g. "AA", "22", often with trailing space),
	/// otherwise "Other".
	/// </summary>
	public class CellTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "Other";
			var s = value.ToString().Trim();
			if (s.Length == 0) return "Other";

			// Suited cells end with 's'
			if (s.EndsWith("s", StringComparison.OrdinalIgnoreCase))
				return "Suited";

			if(s.EndsWith("o", StringComparison.OrdinalIgnoreCase))
				return "Offsuit";

			// Pairs often are like "AA", "KK" or have a trailing space in this XAML ("AA ")
			if (s.Length >= 2 && s[0] == s[1])
				return "Pair";

			// Also handle explicit trailing space variants
			if (s.EndsWith(' ')) return "Pair";

			return "Other";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
