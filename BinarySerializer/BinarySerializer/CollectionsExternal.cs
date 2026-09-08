using System.Collections.Generic;

namespace BinarySerializer;

public static class CollectionsExternal
{
	public static SortedSet<T> ToSortedSet<T>(this List<T> list)
	{
		return new SortedSet<T>(list);
	}
}
