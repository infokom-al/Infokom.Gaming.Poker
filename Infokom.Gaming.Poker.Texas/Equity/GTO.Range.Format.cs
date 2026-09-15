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
				var sb = new System.Text.StringBuilder();
				foreach (var cell in this.Cells)
				{
					_ = sb.Append(cell.Symbol + ",");
				}
				return sb.ToString();
			}

			public string ToString(string format)
			{
				if (format == "grid")
				{
					return Format().ToString();
				}

				return this.ToString();
			}
		}
	}
}
