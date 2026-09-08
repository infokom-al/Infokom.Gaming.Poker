using System.Collections.Generic;

namespace MemoryPools;

public static class EmptyHashSet<T>
{
	public static readonly HashSet<T> Value = new HashSet<T>();
}
