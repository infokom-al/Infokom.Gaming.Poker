using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Infokom.Gaming.Poker.Texas.WPF.Controls
{
	/// <summary>
	/// Interaction logic for GTOCell.xaml
	/// </summary>
	public partial class GTOCell : UserControl
	{
		public GTOCell()
		{
			this.InitializeComponent();
		}
	}

	public class GTOCellModel
	{

		public GTO.Cell? Value { get; set; }
	}

	public class GTOCellModelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(value is string str)
			{
				return new GTOCellModel { Value = GTO.Cell.Parse(str) };
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is GTOCellModel model)
			{
				return model.Value.ToString();
			}

			return null;
		}
	}
}
