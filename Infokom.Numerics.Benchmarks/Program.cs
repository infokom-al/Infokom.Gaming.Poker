namespace Infokom.Numerics.Benchmarks
{
	internal static class Program
	{
		public static void Main()
		{
			_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<Helpers.CombinatoricsBenckmarks>();
		}
	}
}
