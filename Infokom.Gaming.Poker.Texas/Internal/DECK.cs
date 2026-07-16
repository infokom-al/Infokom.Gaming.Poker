using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Holdem.Core.Internal
{

	internal static partial class DECK
	{

		



		


		extension(ValueTuple<DECK.CARD.RANK.ID, DECK.CARD.SUIT.ID> source)
		{
			public DECK.CARD.ID ID => (CARD.ID)((uint)source.Item1 + (uint)source.Item2 * CARD.RANK.COUNT);
		}

		extension(ValueTuple<DECK.CARD.RANK.FLAG, DECK.CARD.SUIT.FLAG> source)
		{
			public DECK.CARD.MASK MASK => (CARD.MASK)((uint)source.Item1 | ((uint)source.Item2 << CARD.RANK.COUNT));
		}

		extension(ValueTuple<DECK.CARD.RANK.SYMBOL, DECK.CARD.SUIT.SYMBOL> source)
		{
			public string SYMBOL => $"{(char)source.Item1}{(char)source.Item2}";
		}



		extension(DECK.CARD.ID source)
		{


			public CARD.RANK.ID R => (CARD.RANK.ID)((int)source % CARD.RANK.COUNT);
			public CARD.SUIT.ID S => (CARD.SUIT.ID)((int)source / CARD.RANK.COUNT);
			public void Deconstruct(out CARD.RANK.ID r, out CARD.SUIT.ID s) => (r, s) = (source.R, source.S);



			public static CARD.ID Of(CARD.RANK.ID r, CARD.SUIT.ID s) => (r, s).ID;
		}
	}
}
