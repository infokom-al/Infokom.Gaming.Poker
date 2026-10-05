using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;

using Infokom.Numerics.Extensions;
using static Infokom.Numerics.Extensions.Constants;
using static Infokom.Numerics.Extensions.Functions;
using WpfMath.Parsers;
using WpfMath.Rendering;

using XamlMath;
using System.Numerics.Colors;

namespace Infokom.Numerics.WPF.Demo.Models
{
	public class NormalDistributionHeatmapModel : INotifyPropertyChanged
	{
		private static Argb<byte> ValueToColor(float x, float min, float max)
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

			return new(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}


		private static readonly Argb<byte>[] Spectrum = [.. Enumerable.Range(0, 255).Select(i => ValueToColor(i/255f, 0, 1))];



		
		private double μ = 0.0;
		private double σ = 1.0;
		private readonly TexFormula _texFormula;
		private readonly TexEnvironment _texEnvironment;
		private BitmapSource _bmpFormula;
		private BitmapSource _bmpHeatmap;


		public event PropertyChangedEventHandler PropertyChanged;


		public NormalDistributionHeatmapModel()
		{
			_texFormula = WpfTeXFormulaParser.Instance.Parse(@"\mathcal{N_{\mu, \sigma}}\frac{1}{\sigma \sqrt{2 \pi}} e^{-\frac{(x - \mu)^2}{2 \sigma^2}}");
			_texEnvironment = WpfTeXEnvironment.Create(TexStyle.Display, 20.0, "Arial");

			_bmpFormula = _texFormula.RenderToBitmap(_texEnvironment, 96.0, 96.0);
			
			this.Refresh();
		}

		public BitmapSource Formula
		{
			get => _bmpFormula;
			set
			{
				if (_bmpFormula != value)
				{
					_bmpFormula = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Formula)));
				}
			}
		}

		public BitmapSource Heatmap
		{
			get => _bmpHeatmap;
			set
			{
				if (_bmpHeatmap != value)
				{
					_bmpHeatmap = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Heatmap)));
				}
			}
		}


		private void Refresh()
		{
			using(var bitmap = new Bitmap(256, 256, PixelFormat.Format32bppArgb))
			{
				var (m, n) = (bitmap.Height, bitmap.Width);
				for (int i = 0; i < m; i++)
				{
					for (int j = 0; j < n; j++)
					{
						var (x, y) = ((double)j/n - 0.5, (double)i/m - 0.5);
						var z = 2 * π * σ * gauss(x, μ, σ) * gauss(y, μ, σ);

						int colorIndex = (int)(z * (Spectrum.Length - 1));
						bitmap.SetPixel(j, i, Spectrum[colorIndex]);
					}
				}

				Heatmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					bitmap.GetHbitmap(),
					IntPtr.Zero,
					System.Windows.Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			}
		}
	}
}
