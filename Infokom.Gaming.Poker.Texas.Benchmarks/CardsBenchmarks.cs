#pragma warning disable CA1822 // Mark members as static
using BenchmarkDotNet.Attributes;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	public class CardsBenchmarks
	{
		[Benchmark]
		[Arguments(0x7FFC7FFC7FFC7FFCul, 5)]
		[Arguments(0x7FFC7FFC7FFC7FFCul, 7)]
		public CardSet[] CardsCombinationsBenchmark(ulong X, int k) => [.. ((CardSet)X).Choose(k)];
	}
}
#pragma warning restore CA1822 // Mark members as static