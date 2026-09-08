namespace MemoryPools;

public static class EmptyArray<T>
{
	public static readonly T[] Value = new T[0];

	public static readonly T[] SingleItemArray = new T[1];
}
