using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Infokom.Gaming.Poker.Texas
{
	public partial class Equity
	{
		private const Suit σ1 = Suit.Spade, σ2 = Suit.Diamond, σ3 = Suit.Club, σ4 = Suit.Heart;

		public const int CellCount = 169;




		public static EstimationResult.Entry Estimate(Cell.Range hero, Cell.Range vill)
		{
			int wins = 0;
			int ties = 0;
			int losses = 0;

			// ------------------------------------------------------------
			// Environment at t0
			// ------------------------------------------------------------

			Deck deck = Deck.Factory;

			// Immutable snapshot of the available environment.
			Cards available = deck.State;

			// ------------------------------------------------------------
			// Hero preference × environment
			// ------------------------------------------------------------

			var heroSampler = hero.Sample(available);

			foreach (Pocket heroPocket in heroSampler)
			{
				// --------------------------------------------------------
				// Environment after Hero's selection
				// --------------------------------------------------------

				Cards afterHero = available & ~heroPocket.Cards;

				// --------------------------------------------------------
				// Villain preference × new environment
				// --------------------------------------------------------

				var villainSampler = vill.Sample(afterHero);

				foreach (Pocket villainPocket in villainSampler)
				{
					// ----------------------------------------------------
					// Environment after both selections
					// ----------------------------------------------------

					Cards afterVillain = afterHero & ~villainPocket.Cards;

					// ----------------------------------------------------
					// Enumerate every possible 5-card board
					// ----------------------------------------------------

					foreach (var board in afterVillain.Combinations(5))
					{
						// ------------------------------------------------
						// Evaluate
						// ------------------------------------------------

						var heroValue =
						    Hand.Evaluate(heroPocket.Cards | board);

						var villainValue =
						    Hand.Evaluate(villainPocket.Cards | board);

						// ------------------------------------------------
						// Comparison
						// ------------------------------------------------

						if (heroValue > villainValue)
							wins++;
						else if (heroValue < villainValue)
							losses++;
						else
							ties++;
					}
				}
			}

			// ------------------------------------------------------------
			// Aggregate
			// ------------------------------------------------------------

			int size = wins + ties + losses;

			return new EstimationResult(
			    wins,
			    ties,
			    losses,
			    size == 0
				   ? 0.0
				   : (wins + ties * 0.5) / size);
		}



		[InlineArray(10)]
		public struct EstimationResult
		{
			private Entry _0;

			[StructLayout(LayoutKind.Sequential)]
			public struct Entry
			{
				public int W, T, L;

				/// <summary>
				/// Initializes a new instance of the <see cref="Entry"/> struct.
				/// </summary>
				/// <param name="id">The unique identifier used for relating this entry to the actor</param>
				/// <param name="w">The number of wins.</param>
				/// <param name="t">The number of ties.</param>
				/// <param name="l">The number of losses.</param>
				public Entry(int w, int t, int l)
				{
					this.W = w;
					this.T = t;
					this.L = l;
				}

				public int this[int i]
				{
					readonly get => i switch
					{
						0 => W,
						1 => T,
						2 => L,
						_ => default
					};
					set
					{
						switch (i)
						{
							case 0: W = value; break;
							case 1: T = value; break;
							case 2: L = value; break;
						}
					}
				}

			}

		}
	}
}
