using BenchmarkDotNet.Attributes;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	[ThreadingDiagnoser]
	public class EquityBenchmarks
	{
		private Equity.Estimation.Request _request2, _request3, _request4;



		[GlobalSetup]
		public void Setup()
		{
			var h1 = GTO.Range.Ω;
			var h2 = GTO.Range.Ω;
			var h3 = GTO.Range.Ω;
			var h4 = GTO.Range.Ω;
			_request2 = Equity.Estimation.Request.Create(h1, h2);
			_request3 = Equity.Estimation.Request.Create(h1, h2, h3);
			_request4 = Equity.Estimation.Request.Create(h1, h2, h3, h4);
		}



		[Benchmark]
		public Equity.Estimation EstimateMonteCarlo2()
		{
			return Equity.Estimation.EstimateMonteCarlo(_request2);
		}

		[Benchmark]
		public Equity.Estimation EstimateMonteCarlo3()
		{
			return Equity.Estimation.EstimateMonteCarlo(_request3);
		}

		[Benchmark]
		public Equity.Estimation EstimateMonteCarlo4()
		{
			return Equity.Estimation.EstimateMonteCarlo(_request4);
		}
	}
}
