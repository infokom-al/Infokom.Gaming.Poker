using System.Collections;

namespace Infokom.Numerics
{
	public interface IMatrix<T> 	{
		public T this[int i, int j] { get; }

		public (int Ni, int Nj) Size { get; }
	}
}
