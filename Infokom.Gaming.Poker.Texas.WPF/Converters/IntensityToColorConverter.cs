using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using Infokom.Gaming.Poker.Texas.WPF.Models;
using Infokom.Numerics.Extensions;

using static Infokom.Numerics.Extensions.Functions;

namespace Infokom.Gaming.Poker.Texas.WPF.Converters
{
	public class IntensityToColorConverter : IValueConverter
	{

		public static HeaderedContentControl Default { get; } = new HeaderedContentControl()
		{
			Content = "Default",
			Background = Brushes.Gray
		};



		private static Color GetColor(float x, float min, float max)
		{
			x = (float)((x - min) / (max - min));

			var (b, g, r) = (x - 0.0f, x - 0.5f, x - 1.0f);

			if (x is >= 0 and <= 1)
			{
				b = exp(-b * b / 0.2f);
				g = exp(-g * g / 0.2f);
				r = exp(-r * r / 0.2f);

				(r, g, b) = (r, g, b) / (r + g + b);
			}

			return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double x)
			{
				if (x is >= 0.0 and <= 1.0)
				{
					return new SolidColorBrush(GetColor((float)x, 0, 0.86f));
				}
				else
				{
					return Brushes.DarkGray;
				}
			}
			return Brushes.Transparent;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
