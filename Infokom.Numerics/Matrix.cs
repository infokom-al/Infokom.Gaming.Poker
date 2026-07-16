namespace Infokom.Numerics
{
	public static class Matrix
	{
		extension<T>(T[,] source)
		{
			public (int X, int Y) LowerBound => (source.GetLowerBound(0), source.GetLowerBound(1));
			public (int X, int Y) UpperBound => (source.GetUpperBound(0), source.GetUpperBound(1));
			public (int X, int Y) Size => (source.GetLength(0), source.GetLength(1));
		}

		public static Matrix<T> Create<T>(int ni, int nj) => Matrix<T>.Create(ni, nj);
		public static Matrix<T> Create<T>(int ni, int nj, T value) => Matrix<T>.Create(ni, nj, value);
		public static Matrix<T> Create<T>(int ni, int nj, Func<int, int, T> source) => Matrix<T>.Create(ni, nj, source);




		public static T[,] ToArray<T>(this IMatrix<T> source)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			var (ni, nj) = source.Size;

			var result = new T[ni, nj];

			Parallel.For(0, ni, i =>
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
