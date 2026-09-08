using Infokom.Numerics.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;


namespace Infokom.Numerics.Benchmarks.Helpers
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	public  class CombinatoricsBenckmarks
	{
		private int n, k;


		[GlobalSetup]
		public void Setup()
		{
			n = 52;
			k = 7;
		}

		[Benchmark]
		public int ChooseCountBenchmark() => Func.Choose(n, k);

		[Benchmark]
		public int ChooseIterateBenchmark() => Func.Choose(n, k);
	}
}
