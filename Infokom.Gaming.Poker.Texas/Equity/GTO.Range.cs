using Infokom.Numerics.Atomics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		private const int T = 10, J = 11, Q = 12, K = 13, A = 14;

		private const ushort ROW_Φ = 0b0111111111111100;
		private const ushort ROW_Ω = 0b0111111111111100;



		/// <summary>
		/// Represents the empty set of cells.
		/// </summary>
		public static readonly BitMatrix16x16 RANGE_Φ = new(
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000);

		/// <summary>
		/// Represents the universal set of all possible cells.
		/// </summary>
		public static BitMatrix16x16 RANGE_Ω => new(
			0b0000000000000000,
			0b0000000000000000,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0111111111111100,
			0b0000000000000000);

		private static readonly BitMatrix16x16 RANGE_A = new(
			0b0000000000000000,
			0b0000000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0111111111111100,
			0b0000000000000000);

		private static readonly BitMatrix16x16 RANGE_K = new(
			0b0000000000000000,
			0b0000000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b0100000000000000,
			0b1111111111111100,
			0b0100000000000000,
			0b0000000000000000);

		private static readonly BitMatrix16x16 RANGE_Q = new([0,0,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b0010000000000000,
			0b1111111111111100,
			0b0010000000000000,
			0b0010000000000000,
		0]);

		private static readonly BitMatrix16x16 RANGE_J = new([0,0,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
			0b1111111111111100,
			0b0001000000000000,
			0b0001000000000000,
			0b0001000000000000,
		0]);

		private static readonly BitMatrix16x16 RANGE_T = new([0,0,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
			0b1111111111111100,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
			0b0000100000000000,
		0]);

		private static readonly BitMatrix16x16 BRODWAYS = RANGE_A | RANGE_K | RANGE_Q | RANGE_J | RANGE_T;
		private static readonly BitMatrix16x16 FACIALS = BRODWAYS & ~RANGE_A;

		/// <summary>
		/// All paired cells: 22, 33, 44, 55, 66, 77, 88, 99, TT, JJ, QQ, KK, AA
		/// </summary>
		private static readonly BitMatrix16x16 RANGE_XX = new([0,0,
			0b0000000000000100,//2
			0b0000000000001000,//3
			0b0000000000010000,//4
			0b0000000000100000,//5
			0b0000000001000000,//6
			0b0000000010000000,//7
			0b0000000100000000,//8
			0b0000001000000000,//9
			0b0000010000000000,//T
			0b0000100000000000,//J
			0b0001000000000000,//Q
			0b010000000000000,//K
			0b100000000000000,//A
		//               A K Q J T 9 8 7 6 5 4 3 2
		0]);

		private static readonly BitMatrix16x16 RANGE_XYs = new(
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000000,
			0b0000000000000100,
			0b0000000000001100,
			0b0000000000011100,
			0b0000000000111100,
			0b0000000001111100,
			0b0000000011111100,
			0b0000000111111100,
			0b0000001111111100,
			0b0000011111111100,
			0b0000111111111100,
			0b0001111111111100,
			0b0011111111111100,
			0b0000000000000000
		);

		private static readonly BitMatrix16x16 RANGE_XYo = new(
			0b0000000000000000,
			0b0000000000000000,
			0b0111111111111000,//32o+
			0b0111111111110000,//43o+
			0b0111111111100000,//54o+
			0b0111111111000000,//65o+
			0b0111111110000000,
			0b0111111100000000,
			0b0111111000000000,
			0b0111110000000000,
			0b0111100000000000,
			0b0111000000000000,
			0b0110000000000000,//AQo+
			0b0100000000000000,//AKo
			0b0000000000000000,
			0b0000000000000000
		);

		/// <inheritdoc cref="Range.XX"/>
		public static readonly Range Coranked = Unsafe.As<BitMatrix16x16, Range>(ref RANGE_XX);


		/// <summary>
		/// <![CDATA[
		/// ┌                                                    ┐
		/// |  . AKs AQs AJs ATs A9s A8s A7s A6s A5s A4s A3s A2s |
		/// |  .  .  KQs KJs KTs K9s K8s K7s K6s K5s K4s K3s K2s |
		/// |  .  .   .  QJs QTs Q9s Q8s Q7s Q6s Q5s Q4s Q3s Q2s |
		/// |  .  .   .   .  JTs J9s J8s J7s J6s J5s J4s J3s J2s |
		/// |  .  .   .   .   .  T9s T8s T7s T6s T5s T4s T3s T2s |
		/// |  .  .   .   .   .   .  98s 97s 96s 95s 94s 93s 92s |
		/// |  .  .   .   .   .   .   .  87s 86s 85s 84s 83s 82s |
		/// |  .  .   .   .   .   .   .   .  76s 75s 74s 73s 72s |
		/// |  .  .   .   .   .   .   .   .   .  65s 64s 63s 62s |
		/// |  .  .   .   .   .   .   .   .   .   .  54s 53s 52s |
		/// |  .  .   .   .   .   .   .   .   .   .   .  43s 42s |
		/// |  .  .   .   .   .   .   .   .   .   .   .   .  32s |
		/// |  .  .   .   .   .   .   .   .   .   .   .   .   .  |
		/// └                                                    ┘
		///	]]>
		/// </summary>
		public static readonly Range Cosuited = Unsafe.BitCast<BitMatrix16x16, Range>(RANGE_XYs);


		/// <summary>
		/// <![CDATA[ 
		/// ┌                                                     ┐
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// | AKo  .   .   .   .   .   .   .   .   .   .   .   .  |
		/// | AQo KQo  .   .   .   .   .   .   .   .   .   .   .  |
		/// | AJo KJo QJo  .   .   .   .   .   .   .   .   .   .  |
		/// | ATo KTo QTo JTo  .   .   .   .   .   .   .   .   .  |
		/// | A9o K9o Q9o J9o T9o  .   .   .   .   .   .   .   .  |
		/// | A8o K8o Q8o J8o T8o 98o  .   .   .   .   .   .   .  |
		/// | A7o K7o Q7o J7o T7o 97o 87o  .   .   .   .   .   .  |
		/// | A6o K6o Q6o J6o T6o 96o 86o 76o  .   .   .   .   .  |
		/// | A5o K5o Q5o J5o T5o 95o 85o 75o 65o  .   .   .   .  |
		/// | A4o K4o Q4o J4o T4o 94o 84o 74o 64o 54o  .   .   .  |
		/// | A3o K3o Q3o J3o T3o 93o 83o 73o 63o 53o 43o  .   .  |
		/// | A2o K2o Q2o J2o T2o 92o 82o 72o 62o 52o 42o 32o  .  |
		/// └                                                     ┘
		///	]]>
		/// </summary>
		public static readonly Range Unsuited = Unsafe.As<BitMatrix16x16, Range>(ref RANGE_XYo);



		//NOTE: [A-Z0-9][0-9][os ]
		/// <summary>
		/// <![CDATA[
		/// ┌                                                     ┐
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .  KK  KQs KJs  .   .   .   .   .   .   .   .   .  |
		/// |  .  KQo QQ  QJs  .   .   .   .   .   .   .   .   .  |
		/// |  .  KJo QJo JJ   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// └                                                     ┘
		/// ]]>
		/// </summary>
		public static Range Facials => GTO.Brodways.Below(Rank.King).Above(Rank.Jack);

		//NOTE: [A-Z0-9][0-9][os ]
		/// <summary>
		/// <![CDATA[
		/// ┌                                                     ┐
		/// | AA  AKs AQs AJs ATs  .   .   .   .   .   .   .   .  |
		/// | AKo KK  KQs KJs KTs  .   .   .   .   .   .   .   .  |
		/// | AQo KQo QQ  QJs QTs  .   .   .   .   .   .   .   .  |
		/// | AJo KJo QJo JJ  JTs  .   .   .   .   .   .   .   .  |
		/// | ATo KTo QTo JTo TT   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// └                                                     ┘
		/// ]]>
		/// </summary>
		public static readonly Range Brodways = Range.Ω.Above(Rank.Ten);

		//NOTE: [AKQJ][A-Z0-9][os ]
		/// <summary>
		/// Universal <see cref="Range"/>.
		/// <![CDATA[
		/// ┌                                                     ┐
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .  T8o 98o 88  87s 86s 85s 84s 83s 82s |
		/// |  .   .   .   .  T7o 97o 87o 77  76s 75s 74s 73s 72s |
		/// |  .   .   .   .  T6o 96o 86o 76o 66  65s 64s 63s 62s |
		/// |  .   .   .   .  T5o 95o 85o 75o 65o 55  54s 53s 52s |
		/// |  .   .   .   .  T4o 94o 84o 74o 64o 54o 44  43s 42s |
		/// |  .   .   .   .  T3o 93o 83o 73o 63o 53o 43o 33  32s |
		/// |  .   .   .   .  T2o 92o 82o 72o 62o 52o 42o 32o 22  |
		/// └                                                     ┘
		/// ]]>
		/// </summary>
		public static readonly Range Numerals = Range.Ω.Below(Rank.Ten);


		//NOTE: [A-Z6-9][A-Z0-9][os ]
		/// <summary>
		/// Universal <see cref="Range"/>.
		/// <![CDATA[
		/// ┌                                                     ┐
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
		/// |  .   .   .   .   .   .   .   .   .  55  54s 53s 52s |
		/// |  .   .   .   .   .   .   .   .   .  54o 44  43s 42s |
		/// |  .   .   .   .   .   .   .   .   .  53o 43o 33  32s |
		/// |  .   .   .   .   .   .   .   .   .  52o 42o 32o 22  |
		/// └                                                     ┘
		/// ]]>
		/// </summary>
		public static readonly Range Babies = Range.Ω.Below(Rank.Five);





		/// <summary>
		/// Represents a range of possible pocket hands in Texas Hold'em poker.
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		public readonly partial struct Range : IEquatable<Range>
		{
			private const int R = 13, C = 13;

			/// <summary>
			/// Returns the size of the preflop matrix.
			/// </summary>
			public static readonly Size<int, int> Size = (C, R);


			[FieldOffset(0)] private readonly BitMatrix16x16 _data;








			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private Range(BitMatrix16x16 data) => _data = data;

			public readonly bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _data == default;
			}

			public readonly bool IsComplete
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _data == ~(default(BitMatrix16x16));
			}









			#region EQUALITY
			public override readonly int GetHashCode() => _data.GetHashCode();
			public readonly bool Equals(Range other) => this._data.Equals(other._data);
			public override readonly bool Equals(object obj) => obj is Range other && Equals(other);
			public static bool operator ==(Range r1, Range r2) => r1._data == r2._data;
			public static bool operator !=(Range r1, Range r2) => r1._data != r2._data;
			#endregion

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly bool Matches(Pocket pocket)
			{
				var ((r1, s1), (r2, s2)) = pocket;
				var (x, y) = (0, 0);


				if (r1 == r2)//paired
				{
					(x, y) = (+r1, +r2);
				}
				else if (s1 == s2)//suited
				{
					(x, y) = (+Rank.Min(r1, r2), +Rank.Max(r1, r2));
				}
				else//offsuit
				{
					(x, y) = (+Rank.Max(r1, r2), +Rank.Min(r1, r2));
				}

				return _data[y, x];
			}



			private const ushort ROW_Φ = 0b0111111111111100;
			private const ushort ROW_Ω = 0b0111111111111100;

			/// <summary>
			/// Empty <see cref="Range"/>.
			/// <![CDATA[
			/// ┌                                         ┐
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// └                                         ┘
			/// ]]>
			/// </summary>
			public static readonly Range Φ = new(RANGE_Φ);

			/// <summary>
			/// Universal <see cref="Range"/>.
			/// <![CDATA[
			/// ┌                                                     ┐
			/// | AA  AKs AQs AJs ATs A9s A8s A7s A6s A5s A4s A3s A2s |
			/// | AKo KK  KQs KJs KTs K9s K8s K7s K6s K5s K4s K3s K2s |
			/// | AQo KQo QQ  QJs QTs Q9s Q8s Q7s Q6s Q5s Q4s Q3s Q2s |
			/// | AJo KJo QJo JJ  JTs J9s J8s J7s J6s J5s J4s J3s J2s |
			/// | ATo KTo QTo JTo TT  T9s T8s T7s T6s T5s T4s T3s T2s |
			/// | A9o K9o Q9o J9o T9o 99  98s 97s 96s 95s 94s 93s 92s |
			/// | A8o K8o Q8o J8o T8o 98o 88  87s 86s 85s 84s 83s 82s |
			/// | A7o K7o Q7o J7o T7o 97o 87o 77  76s 75s 74s 73s 72s |
			/// | A6o K6o Q6o J6o T6o 96o 86o 76o 66  65s 64s 63s 62s |
			/// | A5o K5o Q5o J5o T5o 95o 85o 75o 65o 55  54s 53s 52s |
			/// | A4o K4o Q4o J4o T4o 94o 84o 74o 64o 54o 44  43s 42s |
			/// | A3o K3o Q3o J3o T3o 93o 83o 73o 63o 53o 43o 33  32s |
			/// | A2o K2o Q2o J2o T2o 92o 82o 72o 62o 52o 42o 32o 22  |
			/// └                                                     ┘
			/// ]]>
			/// </summary>
			public static Range Ω => new(RANGE_Ω);


			/// <summary>
			/// <![CDATA[
			/// ┌                                         ┐
			/// | AA  .  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  . KK  .  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  . QQ  .  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  . JJ  .  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  . TT  .  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  . 99  .  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  . 88  .  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  . 77  .  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  . 66  .  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  . 55  .  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  . 44  .  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  . 33  .  |
			/// |  .  .  .  .  .  .  .  .  .  .  .  . 22  |
			/// └                                         ┘
			/// ]]>
			/// </summary>
			public static readonly Range XX = new(RANGE_XX);

			/// <summary>
			/// All unsuited cells
			/// <![CDATA[
			/// ┌                                                    ┐
			/// |  . AKs AQs AJs ATs A9s A8s A7s A6s A5s A4s A3s A2s |
			/// |  .  .  KQs KJs KTs K9s K8s K7s K6s K5s K4s K3s K2s |
			/// |  .  .   .  QJs QTs Q9s Q8s Q7s Q6s Q5s Q4s Q3s Q2s |
			/// |  .  .   .   .  JTs J9s J8s J7s J6s J5s J4s J3s J2s |
			/// |  .  .   .   .   .  T9s T8s T7s T6s T5s T4s T3s T2s |
			/// |  .  .   .   .   .   .  98s 97s 96s 95s 94s 93s 92s |
			/// |  .  .   .   .   .   .   .  87s 86s 85s 84s 83s 82s |
			/// |  .  .   .   .   .   .   .   .  76s 75s 74s 73s 72s |
			/// |  .  .   .   .   .   .   .   .   .  65s 64s 63s 62s |
			/// |  .  .   .   .   .   .   .   .   .   .  54s 53s 52s |
			/// |  .  .   .   .   .   .   .   .   .   .   .  43s 42s |
			/// |  .  .   .   .   .   .   .   .   .   .   .   .  32s |
			/// |  .  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// └                                                    ┘
			///	]]>
			/// </summary>
			public static readonly Range XYs = new(RANGE_XYs);

			/// <summary>
			/// All unsuited cells
			/// <![CDATA[ 
			/// ┌                                                     ┐
			/// |  .   .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | AKo  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | AQo KQo  .   .   .   .   .   .   .   .   .   .   .  |
			/// | AJo KJo QJo  .   .   .   .   .   .   .   .   .   .  |
			/// | ATo KTo QTo JTo  .   .   .   .   .   .   .   .   .  |
			/// | A9o K9o Q9o J9o T9o  .   .   .   .   .   .   .   .  |
			/// | A8o K8o Q8o J8o T8o 98o  .   .   .   .   .   .   .  |
			/// | A7o K7o Q7o J7o T7o 97o 87o  .   .   .   .   .   .  |
			/// | A6o K6o Q6o J6o T6o 96o 86o 76o  .   .   .   .   .  |
			/// | A5o K5o Q5o J5o T5o 95o 85o 75o 65o  .   .   .   .  |
			/// | A4o K4o Q4o J4o T4o 94o 84o 74o 64o 54o  .   .   .  |
			/// | A3o K3o Q3o J3o T3o 93o 83o 73o 63o 53o 43o  .   .  |
			/// | A2o K2o Q2o J2o T2o 92o 82o 72o 62o 52o 42o 32o  .  |
			/// └                                                     ┘
			///	]]>
			/// </summary>
			public static readonly Range XYo = new(RANGE_XYo);



			/// <summary>
			/// All unsuited cells
			/// <![CDATA[ 
			/// ┌                                                     ┐
			/// | AA  AKs AQs AJs ATs A9s A8s A7s A6s A5s A4s A3s A2s |
			/// | AKo  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | AQo  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | AJo  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | ATo  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A9o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A8o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A7o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A6o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A5o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A4o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A3o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// | A2o  .   .   .   .   .   .   .   .   .   .   .   .  |
			/// └                                                     ┘
			///	]]>
			/// </summary>


			/// <summary>
			/// Returns a <see cref="Range"/> that includes all cells in the specified <paramref name="rank"/> (row and column).
			/// </summary>
			/// <param name="rank">The rank for which to create the range. </param>
			/// <returns>A <see cref="Range"/> that includes all cells in the specified <paramref name="rank"/> (row and column).</returns>
			/// <remarks>
			/// <![CDATA[
			/// 
			/// ]]>
			/// </remarks>
			public static Range Of(Rank rank)
			{
				var row = BitMatrix16x16.Row((int)rank);
				var col = row.Transpose();

				return new Range(row | col);
			}


			public static Range Create(Func<Cell, bool> filter)
			{
				var data = new BitMatrix16x16();
				for (Rank y = Rank.Two; y <= Rank.Ace; y++)
				{
					for (Rank x = Rank.Two; x <= Rank.Ace; x++)
					{
						var p = Point.X(x).Y(y);
						var cell = Unsafe.As<Point<sbyte, sbyte>, Cell>(ref p);

						if (filter(cell))
						{
							data[p.Y, p.X] = true;
						}
					}
				}

				return new Range(data);
			}


			public Range Pairs() => this & Range.XX;

			public Range Suited() => this & Range.XYs;

			public Range Unsuited() => this & Range.XYo;

			public Range Above(Rank rank) => this & Range.Create(cell => cell.Y >= rank && cell.X >= rank);

			public Range Below(Rank rank) => this & Range.Create(cell => cell.Y <= rank && cell.X <= rank);





			/// <summary>
			/// Intersection operator: returns a new range containing only the cells that are present in both the left and right ranges.
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <returns></returns>
			public static Range operator &(Range a, Range b) => new(a._data & b._data);


			/// <summary>
			/// Union operator: returns a new range containing all the cells that are present in either the <paramref name="a"/> or <paramref name="b"/> range.
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <returns></returns>
			public static Range operator |(Range a, Range b) => new(a._data | b._data);

			/// <summary>
			/// Symmetric difference operator: returns a new range containing only the cells that are present in either the <paramref name="a"/> or <paramref name="b"/> range, but not both.
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <returns></returns>
			public static Range operator ^(Range a, Range b) => new(a._data ^ b._data);


			/// <summary>
			/// Complement operator: returns a new range containing only the cells that are not present in <paramref name="range"/>.
			/// </summary>
			/// <param name="range"></param>
			/// <returns></returns>
			public static Range operator ~(Range range) => new Range(~range._data) & Range.Ω;
		}





	}
}