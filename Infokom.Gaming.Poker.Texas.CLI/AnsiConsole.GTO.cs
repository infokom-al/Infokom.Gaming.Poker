using Spectre.Console;
using Spectre.Console.Rendering;

namespace Infokom.Gaming.Poker.Texas.CLI
{

	public class AnsiConsoleGTO
	{
		public AnsiConsoleGTO()
		{
		}

		public AnsiConsoleGTOHeatmap Heatmap(int opponents)
		{
			return new AnsiConsoleGTOHeatmap(opponents);
		}
	}

	public class AnsiConsoleGTOHeatmap
	{
		public AnsiConsoleGTOHeatmap(int opponents)
		{
			this.Opponents = opponents;
		}

		public int Opponents { get; }




		public AnsiConsoleGTOHeatmapGenerator Generator()
		{
			return new AnsiConsoleGTOHeatmapGenerator(this.Opponents);
		}
	}

	public class AnsiConsoleGTOHeatmapGenerator
	{
		public AnsiConsoleGTOHeatmapGenerator(int opponents)
		{
			this.Opponents = opponents;
		}

		public int Opponents { get; }


		public AnsiConsoleGTOHeatmapGeneratorResult Generate()
		{
			var result = new AnsiConsoleGTOHeatmapGeneratorResult(this.Opponents);

			GTO.Range[] ranges = new GTO.Range[this.Opponents + 1];

			AnsiConsole.Progress().Start(ctx =>
			{
				var progressTask = ctx.AddTask("Computing heatmap", maxValue: 13 * 13);

				GTO.Heatmap.Generate(result.Heatmap, this.Opponents).Start(_ => progressTask.Increment(1));

				progressTask.StopTask();
			});

			result.NotifyHeeatMapChanged();
			return result;
		}
	}


	public class AnsiConsoleGTOHeatmapGeneratorResult
	{
		private readonly int _n;

		private double _min = 1.0;
		private double _max = 0.0;

		public AnsiConsoleGTOHeatmapGeneratorResult(int opponents)
		{
			_n = opponents + 1;
			this.Heatmap = GTO.Heatmap.Create();
		}

		public GTO.Heatmap Heatmap { get; }

		public void Print()
		{
			var table = new Table()
				.Border(TableBorder.None);

			for (int c = 0; c < 13; c++)
				_ = table.AddColumn(new TableColumn(string.Empty).Centered().NoWrap());

			for (int r = 0; r < 13; r++)
			{
				var row = new IRenderable[13];

				for (int c = 0; c < 13; c++)
				{
					row[c] = new Markup(BuildCellMarkup(this.Heatmap.Weights[r, c]));
				}

				_ = table.AddRow(row);
			}

			AnsiConsole.Write(table);
		}


		public void NotifyHeeatMapChanged()
		{
			for (int r = 0; r < 13; r++)
			{
				for (int c = 0; c < 13; c++)
				{
					var cell = this.Heatmap.Weights[r, c];
					if (cell < _min)
						_min = cell;
					if (cell > _max)
						_max = cell;
				}
			}
		}

		/// <summary>
		/// Kthen një vlerë intensiteti (0.0 - 1.0) në një ngjyrë RGB për Heatmap.
		/// Kaltër (0.0) -> Cjan (0.25) -> Gjelbër (0.5) -> Verdhë (0.75) -> Kuq (1.0)
		/// </summary>
		public static Color IntensityToColor(double intensity, double min, double max)
		{
			// Mbrojtje nëse min dhe max janë të barabarta (për të shmangur pjestimin me zero)
			if (Math.Abs(max - min) < 0.000001)
			{
				return new(0, 0, 255); // Kthen kaltër si default
			}

			// 1. Normalizimi
			intensity = (intensity - min) / (max - min);
			intensity = Math.Max(0.0, Math.Min(1.0, intensity));

			// RREGULLIMI: Nëse është saktësisht 1.0, e ulim fare pak që të qëndrojë brenda segmentit 3
			if (intensity >= 1.0)
			{
				return new(255, 0, 0); // Kthe menjëherë të Kuqe të pastër
			}

			// 2. Logjika e Heatmap (Tani funksionon perfekt për çdo vlerë tjetër < 1.0)
			double scaled = intensity * 4.0;
			int segment = (int)Math.Floor(scaled);
			double fraction = scaled - segment;

			double r = 0, g = 0, b = 0;

			switch (segment)
			{
				case 0: // Kaltër -> Cjan
					r = 0;
					g = fraction * 255;
					b = 255;
					break;
				case 1: // Cjan -> Gjelbër
					r = 0;
					g = 255;
					b = (1.0 - fraction) * 255;
					break;
				case 2: // Gjelbër -> Verdhë
					r = fraction * 255;
					g = 255;
					b = 0;
					break;
				case 3: // Verdhë -> Kuq
				case 4: // Trajton rastin specifik kur intensiteti është fiks 1.0
					r = 255;
					g = (1.0 - fraction) * 255;
					b = 0;
					break;
			}

			return new Color((byte)r, (byte)g, (byte)b);
		}

		private string BuildCellMarkup(double intensity)
		{
			var x = intensity;

			var (r, g, b) = IntensityToColor(x, _min, _max);

			return $"[#FFFFFF on #{r:X2}{g:X2}{b:X2}] {Markup.Escape(intensity.ToString("00.00%"))} [/]";
		}
	}

	public static class AnsiConsoleExtensions
	{
		extension(AnsiConsole)
		{
			public static AnsiConsoleGTO GTO()
			{
				return new AnsiConsoleGTO();
			}
		}


		extension(Color source)
		{
			public void Deconstruct(out byte r, out byte g, out byte b)
			{
				r = source.R;
				g = source.G;
				b = source.B;
			}
		}
	}
}
