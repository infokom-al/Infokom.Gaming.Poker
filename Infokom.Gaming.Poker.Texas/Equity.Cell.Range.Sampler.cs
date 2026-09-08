using System.Collections;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public partial class Equity
	{



		public readonly partial struct Cell
		{
			public readonly partial struct Range
			{

				// TODO: A sampler formally does not contain hands but it predicates aout feasibility of hands given a set of available cards;
				// i.e. the IReadOnlyCollection<Pocket> impl must not be visible to the caller. we can hide by using implicit interface implementation.
				public readonly struct Sampler : IReadOnlyCollection<Pocket>
				{
					private readonly Range _range;
					private readonly Cards _available;

					public Sampler(Range range, Cards available)
					{
						_range = range;
						_available = available;
					}

					public Range Range => _range;

					public Cards Available => _available;





					public readonly bool IsFeasible(Pocket pocket) => (pocket & _available) == pocket;


					public bool IsEmpty => Size == 0;


					//TODO inspect improvement
					public int Size
					{
						get
						{
							int count = 0;

							foreach (var cell in _range.Cells)
							{
								foreach (var pocket in cell.Pockets)
								{
									if (IsFeasible(pocket))
										count++;
								}
							}

							return count;
						}
					}

					int IReadOnlyCollection<Pocket>.Count => this.Size;

					IEnumerator<Pocket> IEnumerable<Pocket>.GetEnumerator()
					{
						foreach (var cell in _range.Cells)
						{
							foreach (var pocket in cell.Pockets)
							{
								if ((_available & pocket.Cards) == pocket.Cards)
									yield return pocket;
							}
						}
					}

					IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Pocket>)this).GetEnumerator();

					
				}



				/// <summary>
				/// Creates a new sampler with the specified available cards.
				/// </summary>
				/// <param name="source">The available cards.</param>
				/// <returns>A new sampler instance.</returns>
				public Sampler Sample(Cards source) => new(this, source);
			}






}


	}
}
