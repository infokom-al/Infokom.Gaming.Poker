using Infokom.Gaming.Poker.Texas.Estimators;
using Infokom.Numerics;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

using static Infokom.Gaming.Poker.Texas.GTO;

namespace Infokom.Gaming.Poker.Texas
{


	
	

	public static partial class Equity
	{
		/// <summary>
		/// Estimates the equity matrix against the opponent's range. Each cell in the matrix corresponds to a starting hand, and the value 
		/// represents the estimated equity against the specified range.
		/// </summary>
		/// <param name="reus">The range of hands to estimate equity against.</param>
		/// <returns>A 13x13 matrix of cells containing the estimated equity for each starting hand.</returns>
		public static async Task<Matrix<Cell<double>>> EstimateEquityMatrix(GTO.Range villainRange)
		{
			var matrix = Matrix<Cell<double>>.Create(13, 13);



			foreach (var cell in GTO.Range.Ω.Cells)//iterate over all possible starting hands for hero
			{
				var heroRange = GTO.Range.Create(cell);

				var estimationRequest = MonteCarloEstimator.Request.Create(heroRange, villainRange);
				var estimation = await MonteCarloEstimator.Handler.Handle(estimationRequest, default(CancellationToken));


				//matrix[cell.Row, cell.Col] = cell.To(estimation.WinRateOf(0));
			}

			return matrix;
		}

	}
}