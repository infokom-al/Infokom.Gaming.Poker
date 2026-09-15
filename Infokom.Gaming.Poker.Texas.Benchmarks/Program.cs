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