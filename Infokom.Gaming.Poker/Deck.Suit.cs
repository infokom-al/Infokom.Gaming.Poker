using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using System.Collections;
using System.Collections.Immutable;
using System.Numerics;

namespace Infokom.Gaming.Poker
{


	public partial struct Deck : IIndexing<Suit>
	{
		public static readonly ImmutableArray<Suit> Suits = [Suit.Club, Suit.Diamond, Suit.Heart, Suit.Spade];

		static int IIndexing<Suit>.LowerBound => 0;
		static int IIndexing<Suit>.UpperBound => 3;
		public static int IndexOf(Suit source) => source switch
		{
			Suit.Club		=> 0,
			Suit.Diamond	=> 1,
			Suit.Heart	=> 2,
			Suit.Spade	=> 3,
			_ => throw new NotSupportedException()
		};
		static Suit IIndexing<Suit>.ValueOf(int index) => Suits[index];
	}


}
