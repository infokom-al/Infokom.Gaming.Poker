#pragma warning disable CA1822 // Mark members as static
using BenchmarkDotNet.Attributes;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	[ThreadingDiagnoser]
	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	[InvocationCount(100)]
	[IterationCount(500)]
	public class TableBenchmarks
	{
		private Table[] _tables;
		private Hand.Ranking[] _handRankBuffer;

		[IterationSetup]
		public void Setup()
		{
			_tables = new Table[9];
			for (int n = 2; n <= 10; n++)
				_tables[n - 2] = CreateRandomTable(n);
			_handRankBuffer = new Hand.Ranking[Table.MaxSeats];
		}

		[Benchmark]
		[Arguments(2)]
		[Arguments(3)]
		[Arguments(4)]
		[Arguments(5)]
		[Arguments(6)]
		[Arguments(7)]
		[Arguments(8)]
		[Arguments(9)]
		[Arguments(10)]
		public Hand.Ranking[] Evaluate(int seatCount)
		{
			return _tables[seatCount - 2].Evaluate();
		}

		private static Table CreateRandomTable(int players)
		{
			var table = new Table();

			for (int seat = 0; seat < players; seat++)
				table.OccupySeat(seat);

			var deck = Deck.Factory;

			while (!table.IsCompleted)
				table.OnNext(deck.Pop());

			return table;
		}
	}
}
#pragma warning restore CA1822 // Mark members as static