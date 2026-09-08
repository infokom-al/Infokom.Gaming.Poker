#if DEBUG
using BenchmarkDotNet.Running;

using BinarySerializer;

using Poker.Calc.Tools;

namespace Poker.Calc.Benchmarks
{
	internal static class Program
	{
		static void Main()
		{
			var eq = GenerateEquityTable.GetEquityTableCellStartIndex;
		}
	}
}
#else
using BenchmarkDotNet.Running;

namespace Poker.Calc.Benchmarks
{
	internal static class Program
	{
		static void Main()
		{
			_ = BenchmarkRunner.Run<BitHelper_Benchmarks>();
		}
	}
}
#endif