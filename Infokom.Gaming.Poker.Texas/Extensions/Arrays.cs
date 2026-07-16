using System.Runtime.CompilerServices;

namespace Holdem.Core.Extensions
{

	public static class Arrays
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="ni"></param>
		/// <param name="nj"></param>
		/// <returns>1D index from 2D index or -1 if any of the 2D index component is out of range</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LinearIndex(int i, int j, int ni, int nj) => (uint)i >= ni || (uint)j >= nj ? -1 : i * nj + j;



		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="k"></param>
		/// <param name="ni"></param>
		/// <param name="nj"></param>
		/// <param name="nk"></param>
		/// <returns>1D index from 3D index or -1 if any of the 3D index component is out of range</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LinearIndex(int i, int j, int k, int ni, int nj, int nk) => (uint)i >= ni || (uint)j >= nj || k >= nk ? -1 : i * nj * nk + j * nk + k;




		public static void ForEach<T>(this T[] source, Action<T> action)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.Length; i++)
				action.Invoke(source[i]);
		}

		public static void ForEach<T>(this T[] source, Action<int, T> action)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.Length; i++)
				action.Invoke(i, source[i]);
		}




		public static void ForEach<T>(this T[,] source, Action<T> action)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.GetLength(0); i++)
				for (int j = 0; j < source.GetLength(1); j++)
					action.Invoke(source[i, j]);
		}

		public static void ForEach<T>(this T[,] source, Action<int, int, T> action)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.GetLength(0); i++)
				for (int j = 0; j < source.GetLength(1); j++)
					action.Invoke(i, j, source[i, j]);
		}




		public static void ForEach<T>(this T[,,] source, Action<T> action)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.GetLength(0); i++)
				for (int j = 0; j < source.GetLength(1); j++)
					for (int k = 0; k < source.GetLength(2); k++)
						action.Invoke(source[i, j, k]);
		}

		public static void ForEach<T>(this T[,,] source, Action<int, int, int, T> action)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.GetLength(0); i++)
				for (int j = 0; j < source.GetLength(1); j++)
					for (int k = 0; k < source.GetLength(2); k++)
						action.Invoke(i, j, k, source[i, j, k]);
		}
	}
}
