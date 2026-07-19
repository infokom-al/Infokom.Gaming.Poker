using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

using Holdem.Core;

using Infokom.Gaming.Poker;
using Infokom.Gaming.Poker.Atomics;
using Infokom.Gaming.Poker.Texas;

using TexasHandEvaluator.Benchmarks;

namespace TexasHandEvaluator
{
	internal class Program
	{
		static void Main()
		{
			RunBoardDemo();

			//RunBenchmarks();
		}

		private static void RunBenchmarks()
		{
			_ = BenchmarkRunner.Run<BoardBenchmarks>();
		}

		private static void RunBoardDemo()
		{
			var deck = new Deck();
			var rand = Random.Shared;

			var hand = Hand.Create(2, i => deck.Draw(rand));

			var commons = hand.Board.ToString();
			var pockets = hand.Pockets.Select(p => p.ToString()).ToArray();


			foreach(var p in pockets)
			{
				Console.Write("{0} ", p);
			}

			Console.Write(commons);

		}

	}
}
