namespace Infokom.Numerics
{
	public interface ISelection<TSelection> where TSelection : ISelection<TSelection>
	{
		public static abstract TSelection operator ~(TSelection s1);
		public static abstract TSelection operator &(TSelection s1, TSelection s2);
		public static abstract TSelection operator |(TSelection s1, TSelection s2);
		public static abstract TSelection operator ^(TSelection s1, TSelection s2);
	}
}
