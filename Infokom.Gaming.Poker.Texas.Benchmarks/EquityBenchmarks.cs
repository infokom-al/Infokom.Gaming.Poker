using BenchmarkDotNet.Attributes;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	[ThreadingDiagnoser]
	public class EquityBenchmarks
	{



		[GlobalSetup]
		public void Setup()
		{
			
		}



	}
}
