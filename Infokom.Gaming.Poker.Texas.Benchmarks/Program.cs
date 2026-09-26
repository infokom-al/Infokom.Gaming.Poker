#if DEBUG
using Infokom.Gaming.Poker.Texas.Estimators;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using Microsoft.Diagnostics.Runtime;

using Spectre.Console;
using Spectre.Console.Rendering;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Infokom.Numerics.Extensions.Constant;
using static Infokom.Numerics.Extensions.Functions;

namespace Infokom.Gaming.Poker.Texas.Benchmarks
{
	internal static class Program
	{
		


		public static async Task Main()
		{
			var request = MonteCarloEstimator.Request.Create([GTO.Coranked.Above(Rank.Queen), GTO.Unsuited.Below(Rank.Four), GTO.Range.Ω], 1000000);

			var response = await MonteCarloEstimator.Handler.Handle(request, default(CancellationToken));
			var scores = response.Scores;


			var totalEquity = scores.Sum(s => s.EquityShare);

			for (int i = 0; i < response.LitigantCount; i++)
			{
				Console.Write($"{scores[i].EquityShare,10:P2}");
			}
			Console.WriteLine($"{totalEquity,10:P2}");
		}
	}
}
#else
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
#endif