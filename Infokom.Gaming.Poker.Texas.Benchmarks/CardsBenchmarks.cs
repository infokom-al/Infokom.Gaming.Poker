#pragma warning disable CA1822 // Mark members as static
using BenchmarkDotNet.Attributes;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	public class CardsBenchmarks
	{
		[Benchmark]
		[Arguments(Cards.Ω, 5)]
		[Arguments(Cards.Ω, 7)]
		public Cards[] CardsCombinationsBenchmark(Cards X, int k) => [.. X.Combinations(k)];
	}
}
#pragma warning restore CA1822 // Mark members as static