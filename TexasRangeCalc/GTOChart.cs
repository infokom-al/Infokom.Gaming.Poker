using Infokom.Gaming.Poker.Texas;

using Spectre.Console;
using Spectre.Console.Rendering;

using static Infokom.Numerics.Extensions.Functions;

namespace TexasRangeCalc
{
	public static class GTOChart
	{
		private static float min = float.MaxValue;
		private static float max = float.MinValue;


		public static GTO.Heatmap GenerateHeatmap(int opponents)
		{
			var heatmap = GTO.Heatmap.Create();

			GTO.Range[] ranges = new GTO.Range[opponents];

			AnsiConsole.Progress().Start(ctx =>
			{
				var progressTask = ctx.AddTask("Computing heatmap", maxValue: 13 * 13);

				for (int r = 0; r < 13; r++)
				{
					for (int c = 0; c < 13; c++)
					{
						ref var cell = ref heatmap[r, c];
						var (x, y) = (cell.Key.X, cell.Key.Y);
						ranges[0] = GTO.Range.Create(cell.Key);
						ranges.AsSpan(1).Fill(GTO.Range.Ω/* & ~ranges[0]*/);

						var request = Equity.Estimation.Request.Create(ranges);
						var result = Equity.Estimation.EstimateMonteCarlo(request, 10000/* * opponents*/);

						var weight = (float)result.WinRateOf(0);
						cell.Value = weight;

						if (weight < min)
							_ = Interlocked.Exchange(ref min, weight);

						if (weight > max)
							_ = Interlocked.Exchange(ref max, weight);


						progressTask.Increment(1);
					}
				}

				progressTask.StopTask();
			});


			return heatmap;
		}

		public static void Print(this GTO.Heatmap heatmap)
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
					var cell = heatmap[r, c];
					row[c] = new Markup(BuildCellMarkup(cell));
				}

				_ = table.AddRow(row);
			}

			AnsiConsole.Write(table);
		}


		private const double MID = 0.7;
		private static string BuildCellMarkup(GTO.Cell<float> cell)
		{
			var t = (cell.Value - min) / (max - min);

			var b = t < MID ? (MID - t) / MID : 0;
			var g = 0.0;/*1 - abs(t - MID) / (1 - MID)*/;
			var r = t > MID ? (t - MID) / (1 - MID) : 0;
			g = 1 - b - r;


			r *= 255;
			g *= 255;
			b *= 255;

			return $"[#FFFFFF on #{(byte)r:X2}{(byte)g:X2}{(byte)b:X2}] {Markup.Escape(cell.Value.ToString("F2"))} [/]";
		}
	}
}

