using Infokom.Gaming.Poker.Texas;

using Spectre.Console;

namespace TexasRangeCalc
{
	internal static class Program
	{
		static Program()
		{
			AnsiConsole.Clear();
		}

		public static void Main(string[] args)
		{
			PrintIntro();

			if (args.Length > 0)
			{
				EstimateCli(args);
				return;
			}

			EstimateInteractive();
		}

		private static void EstimateCli(string[] args)
		{
			int i = 0;

			if (args.Length > i && string.Equals(args[i], "poker-equity", StringComparison.OrdinalIgnoreCase))
				i++;

			var rangeArgs = new List<string>();

			while (i < args.Length && !IsOption(args[i]))
				rangeArgs.Add(args[i++]);

			if (rangeArgs.Count < 2)
			{
				PrintUsage();
				return;
			}


			while (i < args.Length)
			{
				string arg = args[i++];

				switch (arg)
				{
					case "--sim":
					case "--help":
					case "-h":
					case "/?":
						PrintUsage();
						return;

					default:
						AnsiConsole.MarkupLine($"[red]Unknown argument:[/] {Markup.Escape(arg)}");
						PrintUsage();
						return;
				}
			}

			RunEstimate(rangeArgs);
		}

		private static void EstimateInteractive()
		{
			AnsiConsole.MarkupLine("[yellow]Interactive mode[/]");
			AnsiConsole.MarkupLine("[grey]Type 'exit' at any prompt to quit.[/]");
			AnsiConsole.WriteLine();

			int playerCount = ReadInt($"Players {Markup.Escape("(2..10)")}: ", 2, 10, null, out bool cancelled);
			
			if (cancelled) return;

			var rangeArgs = new List<string>(playerCount);

			for (int p = 0; p < playerCount; p++)
			{
				string range = ReadText($"Range {p}: ", allowEmpty: false, out cancelled);
				
				if (cancelled) return;

				rangeArgs.Add(range);
			}

			if (cancelled) return;

			RunEstimate(rangeArgs);
		}

		private static void RunEstimate(IReadOnlyList<string> rangeArgs)
		{
			try
			{
				var ranges = new GTO.Range[rangeArgs.Count];

				for (int p = 0; p < rangeArgs.Count; p++)
					ranges[p] = GTO.Range.Parse(rangeArgs[p]);

				var request = Equity.Estimation.Request.Create(ranges);

				var sw = System.Diagnostics.Stopwatch.StartNew();
				var result = Equity.Estimation.EstimateMonteCarlo(request);
				sw.Stop();

				RenderResult(result, rangeArgs, sw.Elapsed);
			}
			catch (FormatException ex)
			{
				AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
			}
		}

		private static void RenderResult(Equity.Estimation result, IReadOnlyList<string> rangeArgs, TimeSpan elapsed)
		{
			AnsiConsole.Write(new Rule("[yellow]poker-equity[/]").RuleStyle("grey").LeftJustified());
			

			var summary = new Grid();
			_ = summary.AddColumn(new GridColumn().NoWrap());
			_ = summary.AddColumn();

			_ = summary.AddRow("[grey]Players[/]", $"[white]{result.PlayerCount}[/]");
			_ = summary.AddRow("[grey]Mode[/]", "[white]MonteCarlo[/]");
			_ = summary.AddRow("[grey]Games[/]", $"[white]{result.Size:N0}[/]");
			_ = summary.AddRow("[grey]Elapsed[/]", $"[white]{elapsed.TotalMilliseconds:N0} ms[/]");

			AnsiConsole.Write(
				new Panel(summary)
					.Header("Run")
					.Border(BoxBorder.Rounded)
					.Expand());

			var rangesTable = new Table().RoundedBorder().Title("Ranges");
			_ = rangesTable.AddColumn(new TableColumn("Player").Centered());
			_ = rangesTable.AddColumn(new TableColumn("Range"));

			for (int p = 0; p < rangeArgs.Count; p++)
				_ = rangesTable.AddRow(new Markup($"[aqua]{p}[/]"), new Text(rangeArgs[p]));

			AnsiConsole.Write(rangesTable);

			var resultTable = new Table().RoundedBorder().Title("Equity");
			_ = resultTable.AddColumn(new TableColumn("Player").Centered());
			_ = resultTable.AddColumn(new TableColumn("Wins").RightAligned());
			_ = resultTable.AddColumn(new TableColumn("Win Rate").RightAligned());

			for (int p = 0; p < result.PlayerCount; p++)
			{
				double equity = result.WinRateOf(p);

				_ = resultTable.AddRow(
					new Markup($"[aqua]{p}[/]"),
					new Text($"{result.Wins[p]:N0}"),
					new Markup($"[bold green]{equity:P4}[/]"));
			}

			AnsiConsole.Write(resultTable);

			var colors = new[]
			{
				Color.Aqua,
				Color.Green,
				Color.Yellow,
				Color.Fuchsia,
				Color.Orange1,
				Color.DeepSkyBlue1,
				Color.SpringGreen2,
				Color.HotPink,
				Color.Gold1,
				Color.Plum1
			};

			var chart = new BarChart()
				.Width(72)
				.Label("[bold]Equity[/]")
				.CenterLabel();

			for (int p = 0; p < result.PlayerCount; p++)
				_ = chart.AddItem($"P{p}", Math.Round(result.WinRateOf(p) * 100.0), colors[p % colors.Length]);
			
			chart.ValueFormatter = (value, _) => $"{value:N1}%";

			AnsiConsole.Write(chart);
		}

		private static void PrintIntro()
		{
			AnsiConsole.Write(new Rule("[yellow]poker-equity[/]").RuleStyle("grey").LeftJustified());
			PrintUsage();

			AnsiConsole.Write(
				new Panel(new Rows(
					new Markup($"[grey]- enter player count {Markup.Escape("(2..10)")}[/]"),
					new Markup("[grey]- then enter one range per player[/]"),
					new Markup("[grey]- examples: AKs,QQ or A2s+ KTs+ QJs[/]"),
					new Markup("[grey]- then optionally choose simulations, seed, and parallel mode[/]")))
				.Header("Interactive input")
				.Border(BoxBorder.Rounded)
				.Expand());
		}

		private static void PrintUsage()
		{
			var table = new Table().RoundedBorder().Title("Command-line usage");
			_ = table.AddColumn("Command");

			_ = table.AddRow(new Text(@"poker-equity ""AKs"" ""QQ+"""));
			_ = table.AddRow(new Text(@"poker-equity ""A2s+,KTs+,QJs"" ""TT+,AQo+"" --sim 1000000"));
			_ = table.AddRow(new Text(@"poker-equity ""AKs QQ"" ""TT+ AQo+"" ""22+,A2s+,K9s+"" --sim 1000000"));

			AnsiConsole.Write(table);

			var notes = new Rows(
				new Markup("[grey]- pass each player's range as one quoted argument[/]"),
				new Markup("[grey]- inside a range, tokens may be separated by comma, semicolon, dot, or whitespace[/]"),
				new Markup("[grey]- minimum 2 ranges, maximum 10 ranges[/]"),
				new Markup("[grey]- options: --sim <n>, --seed <n>, --no-parallel[/]"));

			AnsiConsole.Write(
				new Panel(notes)
					.Header("Notes")
					.Border(BoxBorder.Rounded)
					.Expand());
		}

		private static string ReadText(string prompt, bool allowEmpty, out bool cancelled)
		{
			while (true)
			{
				AnsiConsole.Markup(prompt);
				string value = Console.ReadLine()?.Trim();

				if (string.Equals(value, "exit", StringComparison.OrdinalIgnoreCase))
				{
					cancelled = true;
					return null;
				}

				if (allowEmpty || !string.IsNullOrWhiteSpace(value))
				{
					cancelled = false;
					return value;
				}

				AnsiConsole.MarkupLine("[red]Input cannot be empty.[/]");
			}
		}

		private static int ReadInt(string prompt, int min, int max, int? defaultValue, out bool cancelled)
		{
			while (true)
			{
				AnsiConsole.Markup(prompt);
				string? value = Console.ReadLine()?.Trim();

				if (string.Equals(value, "exit", StringComparison.OrdinalIgnoreCase))
				{
					cancelled = true;
					return default;
				}

				if (string.IsNullOrWhiteSpace(value) && defaultValue.HasValue)
				{
					cancelled = false;
					return defaultValue.Value;
				}

				if (int.TryParse(value, out int result) && result >= min && result <= max)
				{
					cancelled = false;
					return result;
				}

				AnsiConsole.MarkupLine($"[red]Please enter an integer between {min} and {max}.[/]");
			}
		}

		private static bool IsOption(string arg) => arg.StartsWith('-') || arg.StartsWith('/');
	}
}
