using Infokom.Numerics.Atomics;

using System.Collections;
using System.Drawing;

namespace Infokom.Numerics
{
	public interface IMap<Tx, T>
	{
		public T this[Tx x] { get; }
	}

	public interface IMap<Tx, Ty, T>
	{
		public T this[Ty y, Tx x] { get; }
	}








	public interface IMatrix<T>
	{
		public T this[int y, int x] { get; }
		public T this[Point<int, int> p] { get; }

		public Size<int, int> Size { get; }
	}

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


		public Size<int, int> Size => unchecked((Size<int, int>)(_nj, _ni));


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

		public T this[Point<int, int> index]
		{
			get
			{
				return _data[index.Y, index.X];
			}

			set
			{
				_data[index.Y, index.X] = value;
			}
		}


		public IEnumerator<T> GetEnumerator() => this._data.Cast<T>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this._data.GetEnumerator();



		public static Matrix<T> Create(int ni, int nj) => (uint)ni > 0 && (uint)nj > 0 ? new Matrix<T>(new T[ni, nj]) : throw new ArgumentOutOfRangeException("(ni, nj)", (ni, nj), "Size must be composed by positive integer.");

		public static Matrix<T> Create(int ni, int nj, T value)
		{
			var v = Matrix<T>.Create(ni, nj);

			_ = Parallel.For(0, ni, i =>
			{
				for (int j = 0; j < nj; j++)
					v._data[i, j] = value;
			});

			return v;
		}

		public static Matrix<T> Create(int ni, int nj, Func<int, int, T> source)
		{
			var v = Matrix<T>.Create(ni, nj);

			_ = Parallel.For(0, ni, i =>
			{
				for (int j = 0; j < nj; j++)
					v._data[i, j] = source(i, j);
			});

			return v;
		}


	}


	public static class Matrix
	{
		public readonly record struct Size(int R, int C);
		public readonly record struct Index(int R, int C);




		extension<T>(T[,] source)
		{
			public (int i, int j) LowerBound => (source.GetLowerBound(0), source.GetLowerBound(1));
			public (int i, int j) UpperBound => (source.GetUpperBound(0), source.GetUpperBound(1));
			public (int m, int n) Size => (source.GetLength(0), source.GetLength(1));
		}

		public static Matrix<T> Create<T>(int ni, int nj) => Matrix<T>.Create(ni, nj);
		public static Matrix<T> Create<T>(int ni, int nj, T value) => Matrix<T>.Create(ni, nj, value);
		public static Matrix<T> Create<T>(int ni, int nj, Func<int, int, T> source) => Matrix<T>.Create(ni, nj, source);




		public static T[,] ToArray<T>(this IMatrix<T> source)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			var (ni, nj) = source.Size;

			var result = new T[ni, nj];

			_ = Parallel.For(0, ni, i =>
			{
				for (int j = 0; j < nj; j++)
				{
					result[i, j] = source[i, j];
				}
			});

			return result;
		}


		public static IEnumerable<(int i, int j, T value)> Cells<T>(this IMatrix<T> source)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));
			var (ni, nj) = source.Size;
			for (int i = 0; i < ni; i++)
			{
				for (int j = 0; j < nj; j++)
				{
					yield return (i, j, source[i, j]);
				}
			}
		}
	}
}
