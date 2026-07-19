using System.Collections.Immutable;
using System.Numerics;

namespace Infokom.Numerics
{
	public interface ISpectrum<TSpectrum, TElement> : ISelection<TSpectrum, TElement> where TSpectrum : ISpectrum<TSpectrum, TElement> where TElement : IIndexable<TElement>
	{
		
	}
}
