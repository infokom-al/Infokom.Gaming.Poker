#if DEBUG
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	internal static class Program
	{
		private static readonly string[,] CellLabels = new string[13, 13]
		{
			{ "AA ", "AKs", "AQs", "AJs", "ATs", "A9s", "A8s", "A7s", "A6s", "A5s", "A4s", "A3s", "A2s" },
			{ "AKo", "KK ", "KQs", "KJs", "KTs", "K9s", "K8s", "K7s", "K6s", "K5s", "K4s", "K3s", "K2s" },
			{ "AQo", "KQo", "QQ ", "QJs", "QTs", "Q9s", "Q8s", "Q7s", "Q6s", "Q5s", "Q4s", "Q3s", "Q2s" },
			{ "AJo", "KJo", "QJo", "JJ ", "JTs", "J9s", "J8s", "J7s", "J6s", "J5s", "J4s", "J3s", "J2s" },
			{ "ATo", "KTo", "QTo", "JTo", "TT ", "T9s", "T8s", "T7s", "T6s", "T5s", "T4s", "T3s", "T2s" },
			{ "A9o", "K9o", "Q9o", "J9o", "T9o", "99 ", "98s", "97s", "96s", "95s", "94s", "93s", "92s" },
			{ "A8o", "K8o", "Q8o", "J8o", "T8o", "98o", "88 ", "87s", "86s", "85s", "84s", "83s", "82s" },
			{ "A7o", "K7o", "Q7o", "J7o", "T7o", "97o", "87o", "77 ", "76s", "75s", "74s", "73s", "72s" },
			{ "A6o", "K6o", "Q6o", "J6o", "T6o", "96o", "86o", "76o", "66 ", "65s", "64s", "63s", "62s" },
			{ "A5o", "K5o", "Q5o", "J5o", "T5o", "95o", "85o", "75o", "65o", "55 ", "54s", "53s", "52s" },
			{ "A4o", "K4o", "Q4o", "J4o", "T4o", "94o", "84o", "74o", "64o", "54o", "44 ", "43s", "42s" },
			{ "A3o", "K3o", "Q3o", "J3o", "T3o", "93o", "83o", "73o", "63o", "53o", "43o", "33 ", "32s" },
			{ "A2o", "K2o", "Q2o", "J2o", "T2o", "92o", "82o", "72o", "62o", "52o", "42o", "32o", "22 " }
		};

		private readonly record struct HeatmapCell(string Label, float Weight);

		public static void Main()
		{
			var heatmap = new HeatmapCell[13, 13];

			for (int r = 0; r < 13; r++)
			{
				for (int c = 0; c < 13; c++)
				{
					var hero = GTO.Range.Parse(CellLabels[r, c]);
					var vill = GTO.Range.Ω & ~hero;

					var request = Equity.Estimation.Request.Create(hero, vill);
					var result = Equity.Estimation.Estimate(request);

					var winsum = result.Wins.Sum();
					var weight = result.Wins[0] / (float)winsum;

					heatmap[r, c] = new HeatmapCell(CellLabels[r, c], weight);

					AnsiConsole.WriteLine($"{CellLabels[r, c]} vs Any: {weight:F4}");
				}
			}

			if (AnsiConsole.Confirm("Do you want to print the heatmap?", true))
			{
				AnsiConsole.Clear();
				PrintHeatmap(heatmap);
			}
		}

		private static void PrintHeatmap(HeatmapCell[,] heatmap)
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

		private static string BuildCellMarkup(HeatmapCell cell)
		{
			if (cell.Weight >= 0.5f)
			{
				var (r, g, b) = InterpolateHeat(cell.Weight);
				string background = $"{r:X2}{g:X2}{b:X2}";
				string foreground = UseDarkForeground(r, g, b) ? "000000" : "FFFFFF";

				return $"[#{foreground} on #{background}] {Markup.Escape(cell.Label)} [/]";
			}
			else
			{
				return cell.Label;
			}
		}

		private static (byte R, byte G, byte B) InterpolateHeat(float weight)
		{
			weight = Math.Clamp(weight, 0f, 1f);

			if (weight < 0.5f)
				return (96, 96, 96); // gray

			float t = (weight - 0.5f) / 0.5f;

			var stops = new (float Position, int R, int G, int B)[]
			{
				(0.000f,  36,  92, 181), // blue
				(0.125f,  32, 140, 160), // blue-green
				(0.250f,  33, 140,  77), // green
				(0.375f, 120, 170,  60), // yellow-green
				(0.500f, 232, 197,  71), // yellow
				(0.625f, 240, 160,  60), // yellow-orange
				(0.750f, 230, 120,  40), // orange
				(0.875f, 214,  80,  40), // red-orange
				(1.000f, 198,  40,  40), // red
			};

			for (int i = 1; i < stops.Length; i++)
			{
				if (t <= stops[i].Position)
				{
					var a = stops[i - 1];
					var b = stops[i];

					float u = (t - a.Position) / (b.Position - a.Position);

					byte r = (byte)(a.R + (b.R - a.R) * u);
					byte g = (byte)(a.G + (b.G - a.G) * u);
					byte b2 = (byte)(a.B + (b.B - a.B) * u);

					return (r, g, b2);
				}
			}

			var last = stops[^1];
			return ((byte)last.R, (byte)last.G, (byte)last.B);
		}

		private static bool UseDarkForeground(byte r, byte g, byte b)
		{
			double luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;
			return luminance >= 140;
		}
	}
}
#else
namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	internal static class Program
	{
		public static void Main()
		{
			_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<EquityBenchmarks>();
		}
	}
}
#endif