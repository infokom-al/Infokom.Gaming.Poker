using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics;

using System.Collections.Immutable;

namespace Infokom.Gaming.Poker
{

	public partial struct Deck : IIndexing<Rank>
	{
		public static readonly ImmutableArray<Rank> Ranks = [Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace];

		static int IIndexing<Rank>.LowerBound => 0;
		static int IIndexing<Rank>.UpperBound => 12;
		public static int IndexOf(Rank source) => source switch
		{
			Rank.Two		=> 0x0,
			Rank.Three	=> 0x1,
			Rank.Four		=> 0x2,
			Rank.Five		=> 0x3,
			Rank.Six		=> 0x4,
			Rank.Seven	=> 0x5,
			Rank.Eight	=> 0x6,
			Rank.Nine		=> 0x7,
			Rank.Ten		=> 0x8,
			Rank.Jack		=> 0x9,
			Rank.Queen	=> 0xA,
			Rank.King		=> 0xB,
			Rank.Ace		=> 0xC,
			_ => throw new NotSupportedException()
		};
		static Rank IIndexing<Rank>.ValueOf(int index) => Ranks[index];
	}


}
