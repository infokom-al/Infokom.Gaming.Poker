namespace Infokom.Numerics
{
	public interface IVector<T>
	{
		public int Size { get; }

		public T this[int index] { get; }
	}
}
