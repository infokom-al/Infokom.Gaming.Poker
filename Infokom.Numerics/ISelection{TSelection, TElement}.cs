namespace Infokom.Numerics
{
	public interface ISelection<TSelection, TElement> : ISelection<TSelection>, IReadOnlyCollection<TElement> where TSelection : ISelection<TSelection, TElement>
	{
		public bool this[TElement element] { get; }
		public bool IsIncluded(TElement element);
		public TSelection Include(TElement element);
		public TSelection Exclude(TElement element);


		public static abstract TSelection operator &(TSelection s, TElement e);
		public static abstract TSelection operator |(TSelection s, TElement e);
		public static abstract TSelection operator ^(TSelection s, TElement e);
	}
}
