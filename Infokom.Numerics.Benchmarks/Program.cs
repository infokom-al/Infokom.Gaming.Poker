#if DEBUG
using Infokom.Numerics;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;
namespace Infokom.Numerics.Benchmarks
{
	internal static class Program
	{
		public static void Main()
		{
			var data = 0ul;
			Console.WriteLine($"{data:B64}");

			data.Bits.Upper.Upper.Upper[1] = 1;
			Console.WriteLine($"{data:B64}");

			data.Bits[1] = 1;
			Console.WriteLine($"{data:B64}");

		}
	}
}
#else
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
#endif