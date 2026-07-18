namespace Infokom.Numerics
{
	public interface IIndexing<TElement>
	{
		public static abstract int LowerBound { get; }
		public static abstract int UpperBound { get; }
		public static abstract int IndexOf(TElement source);
		public static abstract TElement ValueOf(int index);
	}
}
