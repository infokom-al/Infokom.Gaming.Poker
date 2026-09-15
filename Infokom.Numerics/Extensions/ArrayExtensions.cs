using System.Collections;

namespace Infokom.Numerics.Extensions
{
	public static class ArrayExtensions
	{
		public static string Stringify<T>(this T[] source, string elementSeparator = ", ", string elementFormat = null, IFormatProvider elementFormatProvider = null) where T : IFormattable
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			var cells = Array.ConvertAll(source, x => x.ToString(elementFormat, elementFormatProvider));

			var row = string.Join(elementSeparator, cells);

			return row;
		}

		public static string Stringify<T>(this ReadOnlySpan<T> source, string elementSeparator = ", ", string elementFormat = null, IFormatProvider elementFormatProvider = null) where T : IFormattable
		{
			Span<string> tokens = new string[source.Length];

			int i = 0;
			foreach (var element in source)
				tokens[i++] = element.ToString(elementFormat, elementFormatProvider);

			return string.Join(elementSeparator, tokens);
		}

		public static void Foo(int[,] source)
		{
			
		}
	}


	public static class ReadOnlyCollectionExtensions
	{
		/// <summary>
		/// Copies the elements of the source collection to the target span.
		/// </summary>
		/// <typeparam name="TSource">The type of the source collection.</typeparam>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="source">The source collection. </param>
		/// <param name="target">The target span.</param>
		/// <returns>The number of elements copied. </returns>
		public static int CopyTo<TSource, T>(this TSource source, Span<T> target) where TSource : IReadOnlyCollection<T>
		{
			int n = 0;

			if (source is not null && target.Length >= source.Count)
			{
				foreach(var x in source)
				{
					target[n++] = x;
				}
			}

			return n;
		}
	}
}
