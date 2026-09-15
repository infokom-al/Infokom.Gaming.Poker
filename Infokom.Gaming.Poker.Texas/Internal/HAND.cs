using Infokom.Numerics.Atomics;

using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Xml.Linq;



namespace Infokom.Gaming.Poker.Texas.Internal
{

	internal static partial class HAND
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ₙCₖ(int n, int k) => n switch
		{
			52 => k switch
			{
				5 => 2598960,
				7 => 133784560,
				_ => -1
			},

			_ => -1
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Cₖ(int k) => k switch { 0 => 0, 1 => 52, 2 => 1326, 3 => 22100, 4 => 270725, 5 => 2598960, 6 => 20358520, 7 => 133784560, _ => -1 };


		public const int N = 52;















		public const uint
				//		g		r1	r2	r3	r4	r5
				ROYF = 0xA000_0000,
				STRF = 0x9000_0000,
				QUAD = 0x8000_0000,
				FULL = 0x7000_0000,
				FLUS = 0x6000_0000,
				STRA = 0x5000_0000,
				TRIP = 0x4000_0000,
				TWOP = 0x3000_0000,
				PAIR = 0x2000_0000,
				HIGH = 0x1000_0000;

		private static uint Value(uint value, int rank) => value | ((uint)rank & 0xFu);
		private static uint Value(uint value, int rank1, int rank2) => value | (((uint)rank1 & 0xFu) << 4) | ((uint)rank2 & 0xFu);
		private static uint Value(uint value, int rank1, int rank2, int rank3) => value | (((uint)rank1 & 0xFu) << 8) | ((uint)rank2 & 0xFu) | ((uint)rank2 & 0xFu);
		private static uint Value(uint value, int rank1, int rank2, int rank3, int rank4) => value | (((uint)rank1 & 0xFu) << 8) | ((uint)rank2 & 0xFu) | ((uint)rank2 & 0xFu);
		private static uint Value(uint value, int rank1, int rank2, int rank3, int rank4, int rank5) => value | (((uint)rank1 & 0xFu) << 8) | ((uint)rank2 & 0xFu) | ((uint)rank2 & 0xFu);


	}


}
