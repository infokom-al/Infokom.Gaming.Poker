using BenchmarkDotNet.Running;

using Infokom.Gaming.Poker;
using Infokom.Gaming.Poker.Extensions;
using Infokom.Gaming.Poker.Internal;
using Infokom.Gaming.Poker.Texas;
using Infokom.Gaming.Poker.Texas.Internal;
using Infokom.Numerics;
using Infokom.Numerics.Atomics;

using System.Collections.Immutable;
using System.Runtime.Intrinsics;

namespace Infokom.Benchmarks.H2NvsINF
{
	internal class Program
	{
		static unsafe void Main()
		{
#if DEBUG
			if (HAND.STRAIGHT.TryIsolate(0b0111110000000000, out var result))
			{
				Console.WriteLine("{0:x8}", result);
			}
#else
			BenchmarkDotNet.Running.BenchmarkRunner.Run<HandEvaluationBenchmarks>();
#endif
		}

























	}
}
