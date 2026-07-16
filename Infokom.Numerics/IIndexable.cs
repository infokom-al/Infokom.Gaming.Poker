using System.Collections.Immutable;

namespace Infokom.Numerics
{
	public interface IIndexable<TSelf> where TSelf : IIndexable<TSelf>
	{
		public static abstract int Count { get; }
		public static abstract int LowerBound { get; }
		public static abstract int UpperBound { get; }
		public static abstract int Index(TSelf source);
	}


}
