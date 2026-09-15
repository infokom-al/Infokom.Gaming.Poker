#pragma warning disable CA1822 // Mark members as static
using BenchmarkDotNet.Attributes;

using static Infokom.Gaming.Poker.CardSet;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{


	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	public class HandBenchmarks
	{
		private static readonly CardSet HIGH = SA | (D_ | _K) | C_ | _9 | H_ | _7 | S_ | _4 | C_ | _3 | H_ | _2;
		private static readonly CardSet STRA = SA | D_ | _K | C_ | _Q | H_ | _J | S_ | _T | C_ | _7 | H2;
		private static readonly CardSet FLUS = SA | S_ | _K | S_ | _9 | S_ | _7 | S_ | _4 | D_ | _K | C2;
		private static readonly CardSet QUAD = SA | DA | CA | H_ | _A | S_ | _K | C_ | _7 | H2;
		private static readonly CardSet STRF = SA | S_ | _K | S_ | _Q | S_ | _J | S_ | _T | D2 | C_ | _3;

		[Benchmark]
		public Hand.Ranking EvaluateHIGHBenchmark() => Hand.Evaluate(HIGH);
	}
}
#pragma warning restore CA1822 // Mark members as static