using System.Collections.Immutable;

namespace Infokom.Numerics
{
	public interface IIndexable<TSelf> where TSelf : IIndexable<TSelf>
	{
		public static abstract int LowerBound { get; }
		public static abstract int UpperBound { get; }
		public static abstract int IndexOf(TSelf source);
		public static abstract TSelf ValueOf(int index);
	}


}
