#pragma warning disable CA1822

using BenchmarkDotNet.Attributes;

using Infokom.Gaming.Poker;
using Infokom.Gaming.Poker.Texas;
using Infokom.Gaming.Poker.Texas.Internal;
using Infokom.Numerics.Extensions;

namespace Infokom.Benchmarks.H2NvsINF
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	[ThreadingDiagnoser]
	public class HandEvaluationBenchmarks
	{
		// ================================================================
		// ONE REPRESENTATIVE HAND PER CATEGORY
		//
		// Encoding:
		// [ H ][ C ][ D ][ S ]
		//
		// bit 0,1,15 unused
		// bit 2..14 = ranks
		// ================================================================

		// 0: High Card
		private readonly ulong HIGH_CARD =
		    Cards(
			   S: "AKJ84",
			   D: "",
			   C: "",
			   H: "");

		// 1: Pair
		private readonly ulong PAIR =
		    Cards(
			   S: "AAK84",
			   D: "",
			   C: "",
			   H: "");

		// 2: Two Pair
		private readonly ulong TWO_PAIR =
		    Cards(
			   S: "AAKK8",
			   D: "",
			   C: "",
			   H: "");

		// 3: Trips
		private readonly ulong TRIPS =
		    Cards(
			   S: "AAAK8",
			   D: "",
			   C: "",
			   H: "");

		// 4: Straight
		private readonly ushort STRAIGHT = 0b0111110000000000;

		// 5: Flush
		private readonly ulong FLUSH =
		    Cards(
			   S: "AKJ84",
			   D: "",
			   C: "",
			   H: "");

		// 6: Full House
		private readonly ulong FULL_HOUSE =
		    Cards(
			   S: "AAAKK",
			   D: "",
			   C: "",
			   H: "");

		// 7: Quads
		private readonly ulong QUADS =
		    Cards(
			   S: "AAAAK",
			   D: "",
			   C: "",
			   H: "");

		// 8: Straight Flush
		private readonly ulong STRAIGHT_FLUSH = Cards(S: "AKQJT", D: "", C: "", H: "");




		private ushort result;


		[Benchmark()]
		public ulong StraightClassic() => (ulong)HAND.EVAL.CLASSIC.Evaluate(STRAIGHT);

		[Benchmark]
		public ulong StraightControl() => HAND.EVAL.PIPELINED.Evaluate(STRAIGHT);

		//[Benchmark]
		//public ulong StraightSimd()
		//{
		//	if(HAND.STRAIGHT.TryIsolate_SIMD_256(STRAIGHT, out result))
		//	{
		//		result = (ushort)(result / 2);
		//		return result;
		//	}
		//	return 0;
		//}




		[Benchmark()]
		public ulong HighCard_Classic() => (ulong)HAND.EVAL.CLASSIC.Evaluate(HIGH_CARD);

		[Benchmark]
		public ulong HighCard_Control() => HAND.EVAL.PIPELINED.Evaluate(HIGH_CARD);


		// ================================================================
		// PAIR
		// ================================================================

		[Benchmark()]
		public ulong Pair_Classic() => (ulong)HAND.EVAL.CLASSIC.Evaluate(PAIR);

		[Benchmark]
		public ulong Pair_Control() => HAND.EVAL.PIPELINED.Evaluate(PAIR);


		// ================================================================
		// TWO PAIR
		// ================================================================

		[Benchmark()]
		public ulong TwoPair_Classic() => (ulong)HAND.EVAL.CLASSIC.Evaluate(TWO_PAIR);

		[Benchmark]
		public ulong TwoPair_Control() => HAND.EVAL.PIPELINED.Evaluate(TWO_PAIR);


		// ================================================================
		// TRIPS
		// ================================================================

		[Benchmark()]
		public ulong Trips_Classic() => (ulong)HAND.EVAL.CLASSIC.Evaluate(TRIPS);

		//[Benchmark]
		//public ulong Trips_Control()
		//{
		//	HAND.EVAL.PIPELINED.Evaluate(
		//	    TRIPS,

		// );

		//	return value;
		//}


		// ================================================================
		// STRAIGHT
		// ================================================================

		//[Benchmark()]
		//public ulong Straight_Classic()
		//{
		//	HAND.EVAL.CLASSIC.Evaluate(
		//	    STRAIGHT,

		//	    );

		//	return value;
		//}

		//[Benchmark]
		//public ulong Straight_Control()
		//{
		//	HAND.EVAL.PIPELINED.Evaluate(
		//	    STRAIGHT,

		//	    );

		//	return value;
		//}


		// ================================================================
		// FLUSH
		// ================================================================

		[Benchmark()]
		public ulong Flush_Classic()
		{
			return
			(ulong)HAND.EVAL.CLASSIC.Evaluate(
			    FLUSH

			    );
		}

		[Benchmark]
		public ulong Flush_Control()
		{
			return HAND.EVAL.PIPELINED.Evaluate(
			    FLUSH

			    );
		}


		// ================================================================
		// FULL HOUSE
		// ================================================================

		//[Benchmark()]
		//public ulong FullHouse_Classic()
		//{
		//	HAND.EVAL.CLASSIC.Evaluate(
		//	    FULL_HOUSE,

		//	    );

		//	return value;
		//}

		//[Benchmark]
		//public ulong FullHouse_Control()
		//{
		//	HAND.EVAL.PIPELINED.Evaluate(
		//	    FULL_HOUSE,

		//	    );

		//	return value;
		//}


		// ================================================================
		// QUADS
		// ================================================================

		//[Benchmark()]
		//public ulong Quads_Classic()
		//{
		//	HAND.EVAL.CLASSIC.Evaluate(
		//	    QUADS,

		//	    );

		//	return value;
		//}

		//[Benchmark]
		//public ulong Quads_Control()
		//{
		//	HAND.EVAL.PIPELINED.Evaluate(
		//	    QUADS,

		//	    );

		//	return value;
		//}



		[Benchmark()]
		public ulong StraightFlush_Classic() => (ulong)HAND.EVAL.CLASSIC.Evaluate(STRAIGHT_FLUSH);

		[Benchmark]
		public ulong StraightFlush_Control() => HAND.EVAL.PIPELINED.Evaluate(STRAIGHT_FLUSH);




		// ================================================================
		// CARD ENCODER
		// ================================================================

		private static ulong Cards(string S, string D, string C, string H)
		{
			return Lane(S) | ((ulong)Lane(D) << 16) | ((ulong)Lane(C) << 32) | ((ulong)Lane(H) << 48);
		}


		private static ushort Lane(string ranks)
		{
			ushort result = 0;

			foreach (char rank in ranks)
			{
				int bit = rank switch
				{
					'A' => 2,
					'2' => 3,
					'3' => 4,
					'4' => 5,
					'5' => 6,
					'6' => 7,
					'7' => 8,
					'8' => 9,
					'9' => 10,
					'T' => 11,
					'J' => 12,
					'Q' => 13,
					'K' => 14,
					_ => throw new ArgumentOutOfRangeException()
				};

				result |= (ushort)(1 << bit);
			}

			return result;
		}
	}
}
