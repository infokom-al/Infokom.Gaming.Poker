#pragma warning disable CA1822 // Mark members as static
using BenchmarkDotNet.Attributes;

using static Infokom.Gaming.Poker.Cards;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{


	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	public class HandBenchmarks
	{
		private const Cards HIGH = SA | DK | C9 | H7 | S4 | C3 | H2;
		private const Cards STRA = SA | DK | CQ | HJ | ST | C7 | H2;
		private const Cards FLUS = SA | SK | S9 | S7 | S4 | DK | C2;
		private const Cards QUAD = SA | DA | CA | HA | SK | C7 | H2;
		private const Cards STRF = SA | SK | SQ | SJ | ST | D2 | C3;

		[Benchmark]
		[Arguments(HIGH)]
		[Arguments(STRA)]
		[Arguments(FLUS)]
		[Arguments(QUAD)]
		[Arguments(STRF)]
		public Hand.Ranking EvaluateBenchmark(Cards cards) => Hand.Evaluate(cards);
	}
}
#pragma warning restore CA1822 // Mark members as static