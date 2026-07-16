namespace Holdem.Core.Extensions
{
	public static class Lists
	{
		public static void ForEach<TList, TItem>(this TList source, Action<TItem> action) where TList : IList<TItem>
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.Count; i++)
				action.Invoke(source[i]);
		}

		public static void ForEach<TList, TItem>(this TList source, Action<int, TItem> action) where TList : IList<TItem>
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));

			for (int i = 0; i < source.Count; i++)
				action.Invoke(i, source[i]);
		}

		public static int ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));


			int i = 0;

			foreach (var item in source)
			{
				action.Invoke(item);
				i++;
			}

			return i;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">Collection of elements</param>
		/// <param name="action">The action to invoke over each elements</param>
		/// <returns>Number of elements where over wich the action was invoked.</returns>
		public static int ForEach<T>(this IEnumerable<T> source, Action<int, T> action) 
		{
			ArgumentNullException.ThrowIfNull(source, nameof(source));


			int i = 0;

			foreach(var item in source)
			{
				action.Invoke(i++, item);
			}

			return i;
		}
	}
}
