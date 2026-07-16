using System.Collections;

namespace Infokom.Numerics
{

	public class Matrix<T> : IMatrix<T>, IEnumerable<T>
	{

		private readonly int _ni, _nj;
		private readonly T[,] _data;


		private Matrix(T[,] data)
		{
			ArgumentNullException.ThrowIfNull(data, nameof(data));

			this._data = data;
			(_ni, _nj) = (data.GetLength(0), data.GetLength(1));
		}


		public (int Ni, int Nj) Size => (_ni, _nj);


		public T this[int i, int j] 
		{
			get
			{
				return _data[i, j];
			}

			set
			{
				_data[i, j] = value;
			}
		}


		public IEnumerator<T> GetEnumerator() => this._data.Cast<T>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this._data.GetEnumerator();



		public static Matrix<T> Create(int ni, int nj) => (uint)ni > 0 && (uint)nj > 0 ? new Matrix<T>(new T[ni, nj]) : throw new ArgumentOutOfRangeException("(ni, nj)", (ni, nj), "Size must be composed by positive integer.");

		public static Matrix<T> Create(int ni, int nj, T value)
		{
			var v = Matrix<T>.Create(ni, nj);

			Parallel.For(0, ni, i =>
			{
				for (int j = 0; j < nj; j++)
					v._data[i, j] = value;
			});

			return v;
		}

		public static Matrix<T> Create(int ni, int nj, Func<int, int, T> source)
		{
			var v = Matrix<T>.Create(ni, nj);

			Parallel.For(0, ni, i => 
			{
				for (int j = 0; j < nj; j++)
					v._data[i, j] = source(i, j);
			});

			return v;
		}

		
	}
}
