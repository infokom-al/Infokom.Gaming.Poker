namespace Infokom.Numerics
{
	public interface IMatrixial<TMatrix, T> : IMatrix<T> where TMatrix : IMatrixial<TMatrix, T>
	{

		(int Ni, int Nj) IMatrix<T>.Size => TMatrix.Size;

		public static new abstract (int M, int N) Size { get; }
	}



}
