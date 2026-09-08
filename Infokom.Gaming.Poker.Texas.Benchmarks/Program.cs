#if DEBUG

using CommandLine;

using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;
using Infokom.Numerics.Helpers;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	internal static class Program
	{
		public static void Main()
		{
			var cards7 = Cards.Select([
				Card.SA,
				Card.SK,
				Card.SQ,
				Card.SJ,
				Card.ST,
				Card.C3,
				Card.C2]);

			Console.WriteLine($"Cards7: {cards7.Stringify()}");

			foreach (var cards5 in cards7.Combinations(5))
			{
				Console.WriteLine($"{cards5.Ranks} => {cards5.Count}");
			}
		}

		private static void DemoShowdown()
		{
			var table = new Table();

			foreach (var seat in new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })
				table.OccupySeat(seat);

			var deck = Deck.Factory;
			while (!table.IsCompleted)
			{
				table.OnNext(deck.Pop());
			}
			var buffer = new Hand.Ranking[10];
			table.Evaluate(buffer);
			var winner = buffer.Max();

			Console.WriteLine($"Board: {table.Board}");
			for (int seat = 0; seat < Table.MaxSeats; seat++)
			{
				if (!table.IsOccupied(seat))
					continue;

				Console.Write($"Seat {seat}: {table.GetPocket(seat)} => {buffer[seat]}");
				if (buffer[seat] == winner)
				{
					Console.Write(" <- Winner");
				}
				Console.WriteLine();
			}
		}

		private static void PrintEquityMatrix()
		{
			for (int y = 14; y >= 2; y--)
			{
				for (int x = 14; x >= 2; x--)
				{
					Equity.Cell cell;

					if (x == y)
						cell = Equity.Cell.Paired((Rank)x);
					else if (x < y)
						cell = Equity.Cell.Suited((Rank)y, (Rank)x);
					else
						cell = Equity.Cell.Offsuit((Rank)x, (Rank)y);

					Console.Write($"│ {cell} ");
				}

				Console.WriteLine("│");
			}
		}


		private static void EstimationDemo()
		{
			var hero = Equity.Range().Suited(Rank.Ace, Rank.King);
			var villain = Equity.Pairs(Rank.Queen, Rank.Queen);

			var result = Equity.Estimate(hero, villain);

			Console.WriteLine($"Hero:    {hero}");
			Console.WriteLine($"Villain: {villain}");
			Console.WriteLine();
			Console.WriteLine($"Wins:    {result.Wins:N0}");
			Console.WriteLine($"Ties:    {result.T:N0}");
			Console.WriteLine($"Losses:  {result.L:N0}");
			Console.WriteLine($"Size:    {result.Size:N0}");
			Console.WriteLine($"Equity:  {result.Equity:P4}");

			Debug.Assert(
			    result.Wins + result.T + result.L == result.Size);
		}
		private static string Stringify(this Equity.Cell.Range range)
		{
			Span<ushort> span = stackalloc ushort[16];
			range.Write(span);
			span = span.Slice(2, 13);
			span.Reverse();

			var rank = Rank.MAX;
			var sb = new System.Text.StringBuilder()
				.Append("    A  K  Q  J  T  9  8  7  6  5  4  3  2" + Environment.NewLine);
			foreach (var data in span)
			{
				_ = sb.Append($"{(rank--).Symbol}: " + Convert.ToString(data >> 2, 2).PadLeft(13, '0')
					.Replace("0", " · ")
					.Replace("1", "███") + Environment.NewLine);
			}
			return sb.ToString();
		}


		private static string Stringify(this Cards value)
		{

			int n = value.Count;
			string s = string.Empty;
			foreach (var card in value.OrderByRankDescending())
			{
				s += card.Symbol;
			}
			return s;
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
