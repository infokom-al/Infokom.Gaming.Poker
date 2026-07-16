namespace Infokom.Numerics
{

	public interface IVectorial<TVector, T> : IVector<T> where TVector : IVectorial<TVector, T>
	{
		int IVector<T>.Size => TVector.Size;

		public static new abstract int Size { get; }
	}
}
