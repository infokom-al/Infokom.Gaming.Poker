namespace Infokom.Numerics
{
	public interface ISelection<TSelection, TElement> : ISelection<TSelection>, IReadOnlyCollection<TElement> where TSelection : ISelection<TSelection, TElement>
	{	


		
		public static abstract TSelection Select(TElement element);
	}
}
