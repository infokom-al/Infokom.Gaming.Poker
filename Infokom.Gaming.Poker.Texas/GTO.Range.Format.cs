using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		public readonly partial struct Range
		{
			public string Format(Func<Cell, string> formatter = null)
			{
				formatter ??= (cell => cell.IsValid ? cell.Symbol : "░░░");

				string topsep = "┌" + string.Join("┬", Enumerable.Repeat("─────", 13)) + "┐";
				string rowsep = "├" + string.Join("┼", Enumerable.Repeat("─────", 13)) + "┤";
				string botsep = "└" + string.Join("┴", Enumerable.Repeat("─────", 13)) + "┘";
				var sb = new System.Text.StringBuilder();

				_ = sb.AppendLine(topsep);

				var range = this;
				var rows = Enumerable.Range(0, 13).Select(y => Enumerable.Range(0, 13).Select(x => formatter(range[y, x])));
				foreach (var row in rows)
				{
					_ = sb.AppendLine("| " + string.Join(" | ", row) + " |").AppendLine(rowsep);
				}

				_ = sb.Remove(sb.Length - rowsep.Length - Environment.NewLine.Length, rowsep.Length + Environment.NewLine.Length)
					.AppendLine(botsep);

				return sb.ToString();
			}

			public override string ToString()
			{
				if (IsEmpty)
					return string.Empty;

				var tokens = new List<string>();

				AppendPairRuns(tokens);
				AppendSuitedRuns(tokens);
				AppendOffsuitRuns(tokens);

				return string.Join(", ", tokens);
			}

			public string ToString(string format)
			{
				if (string.IsNullOrWhiteSpace(format) || string.Equals(format, "classic", StringComparison.OrdinalIgnoreCase))
					return ToString();

				if (string.Equals(format, "grid", StringComparison.OrdinalIgnoreCase))
					return Format();

				throw new NotSupportedException($"Unsupported format '{format}'.");
			}

			private void AppendPairRuns(List<string> tokens)
			{
				for (int high = (int)Rank.Ace; high >= (int)Rank.Two;)
				{
					if (!_data[high, high])
					{
						high--;
						continue;
					}

					int low = high;
					while (low - 1 >= (int)Rank.Two && _data[low - 1, low - 1])
						low--;

					tokens.Add(FormatPairRun(low, high));
					high = low - 1;
				}
			}

			private void AppendSuitedRuns(List<string> tokens)
			{
				for (int hi = (int)Rank.Ace; hi >= (int)Rank.Three; hi--)
				{
					int ceiling = hi - 1;

					for (int highLo = ceiling; highLo >= (int)Rank.Two;)
					{
						if (!_data[hi, highLo])
						{
							highLo--;
							continue;
						}

						int lowLo = highLo;
						while (lowLo - 1 >= (int)Rank.Two && _data[hi, lowLo - 1])
							lowLo--;

						tokens.Add(FormatNonPairRun((Rank)hi, lowLo, highLo, ceiling, 's'));
						highLo = lowLo - 1;
					}
				}
			}

			private void AppendOffsuitRuns(List<string> tokens)
			{
				for (int hi = (int)Rank.Ace; hi >= (int)Rank.Three; hi--)
				{
					int ceiling = hi - 1;

					for (int highLo = ceiling; highLo >= (int)Rank.Two;)
					{
						if (!_data[highLo, hi])
						{
							highLo--;
							continue;
						}

						int lowLo = highLo;
						while (lowLo - 1 >= (int)Rank.Two && _data[lowLo - 1, hi])
							lowLo--;

						tokens.Add(FormatNonPairRun((Rank)hi, lowLo, highLo, ceiling, 'o'));
						highLo = lowLo - 1;
					}
				}
			}

			private static string FormatPairRun(int low, int high)
			{
				char lowChar = RankChar((Rank)low);
				char highChar = RankChar((Rank)high);

				if (low == high)
					return $"{lowChar}{lowChar}";

				if (high == (int)Rank.Ace)
					return $"{lowChar}{lowChar}+";

				return $"{lowChar}{lowChar}-{highChar}{highChar}";
			}

			private static string FormatNonPairRun(Rank hi, int lowLo, int highLo, int ceiling, char suffix)
			{
				char hiChar = RankChar(hi);
				char lowChar = RankChar((Rank)lowLo);
				char highChar = RankChar((Rank)highLo);

				if (lowLo == highLo)
					return $"{hiChar}{lowChar}{suffix}";

				if (highLo == ceiling)
					return $"{hiChar}{lowChar}{suffix}+";

				return $"{hiChar}{lowChar}{suffix}-{hiChar}{highChar}{suffix}";
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static char RankChar(Rank rank) => rank.Symbol;
		}
	}
}