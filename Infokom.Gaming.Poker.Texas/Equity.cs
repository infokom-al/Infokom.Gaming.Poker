using Holdem.Core.Internal;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Holdem.Core
{
	public readonly record struct Equity(double W, double T, double L)
	{
		public static readonly Equity UnitW = new (1, 0, 0);
		public static readonly Equity UnitT = new (0, 1, 0);
		public static readonly Equity UnitL = new (0, 0, 1);

		public static Equity operator +(Equity e1, Equity e2) => new(e1.W + e2.W, e1.T + e2.T, e1.L + e2.L);
		public static Equity operator -(Equity e1, Equity e2) => new(e1.W - e2.W, e1.T - e2.T, e1.L - e2.L);
		public static Equity operator *(Equity e, double k) => new(e.W * k, e.T * k, e.L * k);
		public static Equity operator /(Equity e, double k) => new(e.W / k, e.T / k, e.L / k);


		
		public static Vector512<float> Estimate(GTO.Range[] ranges, int trials = 100000) => Estimate(Array.ConvertAll(ranges, r => r.ToMask()).AsSpan(), trials);

		internal static Vector512<float> Estimate(ReadOnlySpan<GTO.Range.Mask> ranges, int trials)
		{
			var n = ranges.Length;// Number of players

			var winCount = EQUITY.Estimate(ranges, trials);// Estimate the raw win counts for each player

			var winCountSum = Vector512.Sum(winCount);

			return Vector512.AsSingle(winCount) / Vector512.Sum(winCount);
		}


	}


	
}
