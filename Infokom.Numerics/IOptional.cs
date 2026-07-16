namespace Infokom.Numerics
{
	public interface IOptional<TOption, T> where TOption : IOptional<TOption, T>
	{
		public static abstract bool IsSelected(TOption option);

		public static abstract T ValueOf(TOption option);
	}

	public interface IOption<T>
	{
		public T Value { get; }

		public bool IsSelected { get; }
	}

	public readonly record struct Option<T>(T Value, bool IsSelected) : IOption<T>;
}
